using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

/**
 * @Author: Frank
 * @version: 1.0
 * Every data will be stored here (for now) u can call the last 2 methods to save and load the file.
 * It maybe necessary to write something that will split it up in smaller files etc... just ask me to improve in that case.
 * 
 * Tested fully operational.
 *
 */

namespace MediCare.DataHandling
{
    [Serializable()]
    public class ObjectIO
    {
        private Dictionary<string, Session> sessions = new Dictionary<string, Session>();
        private ArrayList sessionIndex = new ArrayList();
        public string userID;
        private string path = "measurementData/Index.Dat";

        public ObjectIO()
        {
            
        }

        public void add(string userID)
        {
             ;
            //sessions.Add(userID, new Session(userID, timeStamp));
        }

        public void closeSession(int index)
        {
            // save Session to disk
            // remove from Live Sessions
            // add to Idex Sessions
        }

        public void startSession(string UserID)
        {
            // create new Session on disk
            DateTime date = DateTime.Now;
            string timeStamp = Regex.Replace(date.ToString(), @"[\/\:\\]", "-");
            Session session = new Session(getNewSessionID(), userID, timeStamp);
            Tuple<string, DateTime, string> userData = new Tuple<string, DateTime, string>(userID, date, "Path");
            // add to Live Sessions
            // add to Idex Sessions
        }

        //public int getSize()
        //{
            //return measurements.Capacity;
        //}

        // save the arraylist of measurements to a file
        public void SaveMeasurements()
        {
            foreach (var key in sessions.Keys)
            {
                Session session;
                sessions.TryGetValue(key, out session);
                session.Encrypt();
            } 
        }

        private int getNewSessionID() 
        {
            return 0;
        }
    }

    [Serializable()]
    class Session
    {
        public int sessionID {get; set;}
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        private byte[] key = Encoding.Unicode.GetBytes("IkHebEchtGeenFlauwIdeeWatIkNuDoe213312782@#");
        private byte[] iv = { 4, 3, 7, 4, 0, 1, 2, 8 };
        
        ArrayList measurements {get; set;} 
        string userID;
        string timeTramp;
        string path;

        public Session(int sessionID, string userID, string timeTramp)
        {
            measurements = new ArrayList();
            this.userID = userID;
            this.timeTramp = timeTramp;
            this.path = @"\MeasurementData\" + userID + "-" + timeTramp + ".edat";
            this.sessionID = sessionID;
        }

        public void add(Measurement m)
        {
            measurements.Add(m);
        }

        public void del(int index)
        {
            measurements.RemoveAt(index);
        }

        //Will be inserted AFTER the given index
        public void insert(int index, Measurement m)
        {
            measurements.Insert(index, m);
        }

        public void empty()
        {
            measurements.Clear();
        }

        // to save diskspace always trim before writing to disk
        public void trim(int size)
        {
            measurements.TrimToSize();
        }

        public int getSize()
        {
            return measurements.Capacity;
        }

        public ArrayList LoadMeasurements()
        {
            ArrayList result = null;

            if (File.Exists(path))
            {
                using (var fs = new FileStream(this.path, FileMode.Open, FileAccess.Read))
                {
                    var cryptoStream = new CryptoStream(fs, des.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                    BinaryFormatter formatter = new BinaryFormatter();

                    result = (ArrayList)formatter.Deserialize(cryptoStream);
                }
            }

            return result;
        }

        public void Encrypt()
        {
            using (var fs = new FileStream(this.path, FileMode.Create, FileAccess.Write))
            {
                var cryptoStream = new CryptoStream(fs, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(cryptoStream, this.measurements);
                cryptoStream.FlushFinalBlock();
            }
        }
    }
    }
