using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MediCare.NetworkLibrary;

namespace MediCare.DataHandling
{
    public class ObjectIOv2
    {
        private const string _dir = @"Measurements\";
        private const string _testDir = @"Inspanningstesten\";
        private const string _fileExt = ".dat";
        public string Status { get; private set; }
        private Dictionary<string, string> _dirDictionary;
        private const string EncryptionKey = "X10j6CZgLK24OESeXAoq";
        private byte[] salt = { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        public ObjectIOv2()
        {
            _dirDictionary = new Dictionary<string, string>();
        }

        /// <summary>
        /// This method creates a file from a packet with a timestamp
        /// </summary>
        /// <param name="p">Packet</param>
        /// <returns>Full Directory of created file/existing file</returns>
        public string Create_file(Packet p, bool test)
        {
            Console.WriteLine("\nTimestamp: " + p._message);

            string _fulldir = "";
            Status = "busy";
            Console.WriteLine(Path.Combine(_dir, p._id));

            try
            {
                if (!Directory.Exists(_dir))
                {
                    Directory.CreateDirectory(_dir);
                    Status = "Creating main directory: " + _dir;
                }
                if (!Directory.Exists(Path.Combine(_dir, p._id)))
                {
                    Directory.CreateDirectory(Path.Combine(_dir, p._id));
                    Status = "creating subdirectory: " + _dir + p._id;
                    Console.WriteLine(Status);
                }
                if (test)
                {
                    if (!Directory.Exists(Path.Combine(_dir, p._id, _testDir)))
                    {
                        Directory.CreateDirectory(Path.Combine(_dir, p._id, _testDir));
                        Status = "creating subdirectory: " + _dir + p._id + _testDir;
                        Console.WriteLine(Status);
                    }
                    if (File.Exists(Path.Combine(_dir, p._id, _testDir, p._message + _fileExt)))
                    {
                        Status = "File Already exists: " + Path.Combine(_dir, p._id, _testDir, p._message + _fileExt);
                        Console.WriteLine(Status);
                        _fulldir = Path.Combine(_dir, p._id, _testDir, p._message + _fileExt);
                    }
                    if (!File.Exists(Path.Combine(_dir, p._id, _testDir, p._message + _fileExt)))
                    {
                        Status = "Creating file.." + Path.Combine(_dir, p._id, _testDir, p._message + _fileExt);
                        File.Create(Path.Combine(_dir, p._id, _testDir, p._message + _fileExt)).Close();
                        Status = "file Created: " + Path.Combine(_dir, p._id, _testDir, p._message + _fileExt);
                        _fulldir = Path.Combine(_dir, p._id, _testDir, p._message + _fileExt);
                        Console.WriteLine(Status);
                        _dirDictionary.Add(p._id, p._message);
                    }
                    Add_Measurement(p, test);
                }
                else
                {
                    if (File.Exists(Path.Combine(_dir, p._id, p._message + _fileExt)))
                    {
                        Status = "File Already exists: " + Path.Combine(_dir, p._id, p._message + _fileExt);
                        Console.WriteLine(Status);
                        _fulldir = Path.Combine(_dir, p._id, p._message + _fileExt);
                    }
                    if (!File.Exists(Path.Combine(_dir, p._id, p._message + _fileExt)))
                    {
                        Status = "Creating file.." + Path.Combine(_dir, p._id, p._message + _fileExt);
                        File.Create(Path.Combine(_dir, p._id, p._message + _fileExt)).Close();
                        Status = "file Created: " + Path.Combine(_dir, p._id, p._message + _fileExt);
                        _fulldir = Path.Combine(_dir, p._id, p._message + _fileExt);
                        Console.WriteLine(Status);
                        _dirDictionary.Add(p._id, p._message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("A FileIO Measurement error occured: " + e.Message);
            }
            return _fulldir;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void Add_Measurement(Packet p, bool test)
        {
            string message = p._message;
            string filename = _dirDictionary[p._id];
            StreamWriter sw;
            if(test)
            {
                Console.WriteLine("Writing in file: " + _dir + p._id + _testDir);
                sw = File.AppendText(Path.Combine(_dir, p._id, _testDir, filename + _fileExt));
            }
            else
            {
                Console.WriteLine("Writing in file: " + _dir + p._id);
                sw = File.AppendText(Path.Combine(_dir, p._id, filename + _fileExt));
            }
            Console.WriteLine(p._message);
            using (sw)
            {

                byte[] clearBytes = Encoding.Unicode.GetBytes(p._message);
                string encryptedData = "";
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, salt);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        encryptedData = Convert.ToBase64String(ms.ToArray());
                    }
                }
                sw.WriteLine(encryptedData);
            }
        }

        public void Remove_client(Packet p)
        {
            if (_dirDictionary.ContainsKey(p._id))
            {
                _dirDictionary.Remove(p._id);
            }
        }
        public string Get_File(string fileName)
        {
            String[] files = fileName.Split('-');
            if (Directory.Exists(Path.Combine(_dir)))
            {
                DirectoryInfo di = new DirectoryInfo(_dir);
                foreach (DirectoryInfo map in di.GetDirectories())
                {
                    if (map.Name.Equals(files[0]))
                    {
                        foreach (FileInfo file in map.GetFiles())
                        {
                            if (file.Name.Equals(files[1]))
                            {
                                return file.FullName;
                            }
                        }
                    }
                }
            }
            return "no file found";
        }

        /// <summary>
        /// Find all files associated with the ID sent with the packet.
        /// </summary>
        /// <param name="p">Packet with an ID and message, where message should be the id of requested patient</param>
        /// <returns>Returns a packet with the correct destination, packettype and a messsage that sums up all files separated with a dash ("-")</returns>
        public Packet Get_Files(Packet p)
        {
            if (Directory.Exists(Path.Combine(_dir)))
            {
                DirectoryInfo di = new DirectoryInfo(_dir);
                Console.WriteLine("checking : " + _dir);
                foreach (DirectoryInfo map in di.GetDirectories())
                {
                    Console.WriteLine(map.ToString());
                    if (map.Name.Equals(p._message))
                    {
                        string message = "";
                        foreach (FileInfo file in map.GetFiles())
                        {
                            message += (file.ToString() + "-");
                        }
                        message = message.Remove(message.Length - 1);
                        Packet responsePacket = new Packet("server", "Filelist", "98765432", message);
                        return responsePacket;
                    }
                }
            }
            return new Packet("server", "Filelist", p._id, "No files found");
        }

        public ArrayList Read_file(Packet p)
        {
            string[] lines = File.ReadAllLines(Path.Combine(_dir, p._id, _dirDictionary[p._id] + _fileExt));
            ArrayList resultList = new ArrayList();
            foreach (var line in lines)
            {
                Console.WriteLine(Decrypt(line));
                resultList.Add(Decrypt(line));
            }
            return resultList;
        }

        private string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        /// <summary>
        /// removes file by id and datetime
        /// </summary>
        /// <param name="id">the id of the person from the file which needs to be removed</param>
        /// <param name="dateTime">datetime of the file which needs to be removed format: "yyyy_MM_dd HH_mm_ss"</param>
        /// <returns> when file exists: "File deleted" (when deleted), when not exists: "the file does not exist</returns>
        public string Remove_file(string id, string dateTime)
        {
            if (File.Exists(Path.Combine(_dir, id, dateTime + _fileExt)))
            {
                File.Delete(Path.Combine(_dir, id, dateTime + _fileExt));
            }
            return !File.Exists(Path.Combine(_dir, id, dateTime + _fileExt)) ? "the file does not exist" : "File Deleted";
        }
    }
}
