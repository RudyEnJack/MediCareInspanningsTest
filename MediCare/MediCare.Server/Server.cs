using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using MediCare.NetworkLibrary;
using MediCare.DataHandling;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace MediCare.Server
{
    public class Server
    {
        private IPAddress _localIP = IPAddress.Parse("127.0.0.1");
        private Dictionary<string, TcpClient> _clients = new Dictionary<string, TcpClient>();
        private Dictionary<string, SslStream> _clientsStreams = new Dictionary<string, SslStream>();
        private ObjectIOv2 mIOv2; // do not remove, do not move and do not edit!
        private bool _test = false;

        private LoginIO _loginIO = new LoginIO();

        private string _toAllDoctors = "Dokter";
        //README!!! - SSL certificate needs to be coppied from MediCare.Server\ssl_cert.pfx to C:\Windows\Temp\
        private X509Certificate certificate = new X509Certificate(@"C:\Windows\Temp\ssl_cert.pfx", "medicare");

        //sendqueue
        private BlockingCollection<Tuple<SslStream, Packet>> sendQueue = new BlockingCollection<Tuple<SslStream, Packet>>(new ConcurrentQueue<Tuple<SslStream, Packet>>());
        private bool _running;


        static void Main(string[] args)
        {
            new Server();
        }

        public Server()
        {
            _loginIO.LoadLogins();

            mIOv2 = new ObjectIOv2(); // do not remove, do not move and do not edit!

            TcpListener server = new TcpListener(/*_localIP,*/ 11000);
            server.Start();

            startServerHelper();
            _running = true;

            TcpClient incomingClient;
            Console.WriteLine("Waiting for connection...");
            while (true)
            {
                incomingClient = server.AcceptTcpClient();

                new Thread(() =>
                {
                    Console.WriteLine("Connection found!");
                    BinaryFormatter formatter = new BinaryFormatter();
                    TcpClient sender = incomingClient;
                    SslStream sslStream = new SslStream(incomingClient.GetStream());
                    sslStream.AuthenticateAsServer(certificate);

                    while (true)
                    {

                        String dataString = "";
                        Packet packet = null;
                        if (sender.Connected)
                        {
                            dataString = (String)formatter.Deserialize(sslStream);
                            //dataString = (String)formatter.Deserialize(sender.GetStream());
                            packet = Utils.GetPacket(dataString);
                            //Console.WriteLine("Packet received: " + packet.toString());
                            if (!Packet.hasValidId(packet))
                                Console.WriteLine("WARNING! PACKET HAS INVALID ID. ONE SHOULD NOT SEND PACKETS TO THE SERVER WHO HAVE AN ID THAT DOES NOT PASS THE hasValidId(p) METHOD! \nPACKET ID USED WHAS: " + packet._id);

                            //Console.WriteLine(dataString);

                            //Console.WriteLine("Incoming action" + packet._type);
                            switch (packet._type)
                            {
                                //sender = incoming client
                                //packet = data van de client
                                case "Chat":
                                    HandleChatPacket(packet);
                                    break;
                                case "FirstConnect":
                                    HandleFirstConnectPacket(packet, incomingClient, sslStream);
                                    break;
                                case "Disconnect":
                                    HandleDisconnectPacket(packet, sslStream);
                                    break;
                                case "Data":
                                    HandleDataPacket(packet);
                                    break;
                                case "Registration":
                                    HandleRegistrationPacket(packet);
                                    break;
                                case "ManageUsers":
                                    HandleManageUsersPacket(packet);
                                    break;
                                case "Timestamp":
                                    HandleTimestampPacket(packet);
                                    break;
                                case "ActiveClients":
                                    HandleActiveClients(packet);
                                    break;
                                case "Filelist":
                                    HandleFileList(packet);
                                    break;
                                case "FileRequest":
                                    HandleFileRequest(incomingClient, packet);
                                    break;
                                case "FileDelete":
                                    HandleFileDeleteRequest(incomingClient, packet);
                                    break;
                                case "Command":
                                    HandleCommandPacket(packet);
                                    break;
                                case "TestStart":
                                    HandleTestStartPacket(packet);
                                    break;
                                case "TestResults":
                                    HandleTestResultsPacket(packet);
                                    break;
                                case "TestEnd":
                                    HandleTestEndPacket(packet);
                                    break;
                                default: //nothing
                                    break;
                            }
                        }
                        Thread.Sleep(5);
                    } // end While
                }).Start();
            }

        }

        private void HandleTestEndPacket(Packet packet)
        {
            _test = false;
            mIOv2.Add_Measurement(packet, _test);
        }

        private void HandleTestResultsPacket(Packet packet)
        {
            if (IsDoctor(packet._id))
            {
                //Send Inspanningstest results of packet._message
            }
            else
            {
                //Send Inspanningstest results of packet._id
            }
        }

        private void HandleTestStartPacket(Packet packet)
        {
            if (IsDoctor(packet._id))
            {
                SendToDestination(packet);
            }
            else
            {
                string[] timestamp = DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss").Split();
                _test = true;
                mIOv2.Create_file(new Packet(packet._id, packet._type, timestamp[0] + " " + timestamp[1]), _test);
                mIOv2.Add_Measurement(packet, _test);
                if (packet._message.Split(' ')[2] == "man")
                {
                    Exercise ex = new Exercise(true);
                }
                else
                {
                    Exercise ex = new Exercise(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incomingClient"></param>
        /// <param name="packet"></param>
        /// <param name="ssl"></param>
        private void HandleFileDeleteRequest(TcpClient incomingClient, Packet packet)
        {
            //TODO test method
            //first string is ID [0], 
            //second & third is the datetime [1](date) [2](time)
            string[] split = packet._message.Split();
            string resultstring = mIOv2.Remove_file(split[0], split[1] + " " + split[2]);
            Packet result = new Packet("Server", "Result", packet._id, resultstring);
            try
            {
                SendToDestination(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /**
         * Versturen van een chat, package blijft hetzelfde.
         * Methode is er omdat het anders uit de toon valt met andere methodes.
         */
        private void HandleChatPacket(Packet packet)
        {
            if (IsDoctor(packet._id)) //if source is doctor send the message to the destination.
            {
                SendToDestination(packet);
            }
            else //else: source is client. send to all the connected doctors. (connections with an id who start with 9)
            {
                sendToDoctors(packet);
            }
        }

        /// <summary>
        /// chathelpers are used by the handleChatPacket method.
        /// contents: sendToDoctors(Packet packet):void; sendToDestination(packet):void and isDoctor(String id):bool
        /// </summary>
        #region chathelpers
        private void sendToDoctors(Packet packet)
        {
            foreach (KeyValuePair<string, SslStream> entry in _clientsStreams)
            {
                if (IsDoctor(entry.Key))
                {
                    SslStream sslStream = entry.Value;
                    EnqueuePacket(sslStream, packet);
                    //Console.WriteLine("packetMessage: " + packet._message);
                }
            }
        }

        private void SendToDestination(Packet packet)
        {
            SslStream sslStream;
            _clientsStreams.TryGetValue(packet._destination, out sslStream);
            EnqueuePacket(sslStream, packet);
        }

        private bool IsDoctor(String id)
        {
            return id.StartsWith("9");
        }

        #endregion

        /**
         * Zoeken in de *database* naar de juiste persoons gegevens en haal daar het correcte ID op
         * 
         */
        private void HandleFirstConnectPacket(Packet p, TcpClient incomingClient, SslStream stream)
        {
            if (_clients.ContainsKey(p._id))
            {
                Console.WriteLine("Client already logged in!");
                Packet response = new Packet("Server", "FirstConnect", p._id, "Login failed, this ID is already logged in.");
                EnqueuePacket(stream, response);
            }
            else
            {
                if (loginIsValid(p._message))
                {
                    Packet response = new Packet("Server", "FirstConnect", p._id, "VERIFIED");
                    EnqueuePacket(stream, response);
#if DEBUG
                    Console.WriteLine("Login succeeded");
#endif
                    //login is valid. Do add the client to the dictionaries.
                    if (!clientIsKnown(p._id))
                    {
                        addNewClient(p, incomingClient, stream);
                    }
                }
                else
                {
                    Packet response = new Packet("Server", "FirstConnect", p._id, "Login failed, login credentials are invalid");
                    EnqueuePacket(stream, response);
#if DEBUG
                    Console.WriteLine("Login credentials are invalid");
#endif
                }
            }
        }

        private void addNewClient(Packet p, TcpClient incomingClient, SslStream stream)
        {
            _clients.Add(p.GetID(), incomingClient);
            _clientsStreams.Add(p.GetID(), stream);

            #region DEBUG
#if DEBUG
            Console.WriteLine("ID: " + p.GetID() + "incomingClient: " + incomingClient.ToString());
            printClientList();
#endif
            #endregion
        }

        private bool clientIsKnown(String id)
        {
            return _clients.ContainsKey(id);
        }

        private bool loginIsValid(string credentials)
        {
            _loginIO.add("12345678:dsa"); //TODO Remove
            _loginIO.add("98765432:asd"); //TODO Remove
            _loginIO.add("87654321:asd"); //TODO Remove

            // This part is for the Doctors signup tool/manage users tool.
            // The signup tool connects with "DoctorID" + "r" or "DoctorID" + "m"
            // this code checks if the doctor is connected if yes. its fine for the signuptool to connect without password
            if (credentials.Split(':')[0].EndsWith("r") || credentials.Split(':')[0].EndsWith("m"))
            {
                return _clients.ContainsKey(credentials.Split(':')[0].Substring(0, 8));
            }

            return _loginIO.login(credentials);
        }

        /**
         * Client stuurt een Disconnect type packet, en wordt hier afgehandeld.
         * Stuur het sluit bericht terug naar de client en sluit de connectie. 
         */
        private void HandleDisconnectPacket(Packet p, SslStream sslStream)
        {
            if (p._id != null)
            {
                Console.WriteLine("Removing client from dictionary's: " + p._id);
                mIOv2.Remove_client(p); // do not remove, do not move and do not edit!
                if (_clients.ContainsKey(p._id))
                {
                    _clients.Remove(p._id);
                }
                if (_clientsStreams.ContainsKey(p._id))
                {
                    _clientsStreams.Remove(p._id);
                }
            }
            Packet response = new Packet("server", "Disconnect", p.GetID(), "LOGGED OFF");
            EnqueuePacket(sslStream, response);
            Console.WriteLine("A client has disconnected");
            sslStream.Close();
        }

        /**
         * Save de data die je binnen krijgt.
         * Stuur de data door naar de DoktorClient.
         */
        private void HandleDataPacket(Packet packet)
        {
            SaveMeasurement(packet);
            Packet response_Sender = new Packet("Server", "Data", packet._id, "Data Saved");
            SendToDestination(response_Sender);
            SslStream sslStream;
            if (packet._destination == _toAllDoctors)
            {
                foreach (var s in _clientsStreams)
                {
                    if (s.Key.StartsWith("9"))
                    {
                        //Console.WriteLine("Destination: " + s.Key);// + sslStream.ToString());
                        try
                        {
                            EnqueuePacket(s.Value, packet);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            else
            {
                _clientsStreams.TryGetValue(packet._destination, out sslStream);
                //   Console.WriteLine("Destination: " + packet._destination + " packet id destination: ");// + sslStream.ToString());
                try
                {
                    EnqueuePacket(sslStream, packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /**
        * Handel het registratie process af. genereer een uniek ID voeg toe aan bestand (zie LoginIO)
        * Stuur een bericht terug dat de data is aangekomen.
        */
        private void HandleTimestampPacket(Packet p)
        {
            SaveMeasurement(p);
        }

        /**
         * Handel het registratie process af. genereer een uniek ID voeg toe aan bestand (zie LoginIO)
         * Stuur een bericht terug dat de data is aangekomen.
         */
        private void HandleRegistrationPacket(Packet p)
        {
            string[] credentials = p.GetMessage().Split(':');
            string name = credentials[0];
            string pass = credentials[1];
            Console.WriteLine("Server: " + name);

            bool doesthepasswordcontainweirdcharsthatyoudontwantinapassword = Regex.IsMatch(pass, @"[^A-Za-z0-9]+");
            if (doesthepasswordcontainweirdcharsthatyoudontwantinapassword)
            {
                Packet response = new Packet("Server", "Registration", p.GetID(), "INVALID_PASS");
                SendToDestination(response);
            }
            else if (_loginIO.UserExist(name))
            {
                Packet response = new Packet("Server", "Registration", p.GetID(), "REGISTER_FAIL");
                SendToDestination(response);
            }
            else
            {
                _loginIO.add(p.GetMessage());
                _loginIO.SaveLogins();
                Packet response = new Packet("server", "Registration", p.GetID(), "REGISTER_SUCCESS");
                SendToDestination(response);
            }
        }
        /**
          * Handel het manage user proces af. Maak één lange string aan met alle ids en bijbehorende wachtwoorden
          * de ids en wachtwoorden worden gesplit dmv een '@', zelf worden de ids en pass gesplit op een spatie
          * Stuur een bericht terug 
          */
        private void HandleManageUsersPacket(Packet p)
        {
            if (p._message.Equals("GetLogins"))
            {
                string ids = string.Empty;
                string passwords = string.Empty;

                foreach (KeyValuePair<string, string> login in _loginIO._logins)
                {
                    // elke dokter kan van elke patient het ww wijzigen?
                    if (!login.Key.StartsWith("9"))
                    {
                        ids += login.Key + " ";
                        passwords += login.Value + " ";
                    }
                }
                Packet response = new Packet("Server", "GetLogins", p._id, ids.Trim() + "@" + passwords.Trim());
                SendToDestination(response);
            }
            else if (p._message.Substring(0, 7).Equals("NewPass"))
            {
                string[] credentials = p._message.Split('@');
                string id = credentials[1];
                string pass = credentials[2];
                if (_loginIO.UserExist(id))
                    _loginIO.del(id);
                _loginIO.add(id + ":" + pass);
                //_loginIO.SaveLogins(); // TODO: uncommenten
                Packet response = new Packet("Server", "NewPass", p._id, "Pass changed");
                SendToDestination(response);
            }
            else if (p._message.Substring(0, 10).Equals("DeleteUser"))
            {
                //Deleteuser@id:pass@id:pass@id:pass@...
                string[] data = p._message.Split('@');
                for (int i = 1; i < data.Length; i++)
                {
                    string[] credentials = data[i].Split(':');
                    string id = credentials[0];
                    string pass = credentials[1];
                    if (_loginIO.UserExist(id))
                        _loginIO.del(id);
                }
                //_loginIO.SaveLogins(); // TODO: uncommenten
                Packet response = new Packet("Server", "NewPass", p._id, "User(s) deleted");
                SendToDestination(response);
            }
        }
        /*
         * Geeft het aantal actieve clients
         * 
         */
        private void HandleActiveClients(Packet p)
        {
            string ids = "";
            foreach (string key in _clients.Keys)
            {
                if (!key.StartsWith("9"))
                {
                    ids += key + " ";
                }
            }
            Packet response = new Packet("Server", "ActiveClients", p._id, ids.Trim());
            SendToDestination(response);
        }

        /// <summary>
        /// Methode die aangeroepen wordt als de server een request voor de files binnenkrijgt
        /// </summary>
        /// <param name="packet">Packet waarin de message het id van de opgevraagde patient moet zijn</param>
        /// <param name="stream"></param>
        private void HandleFileList(Packet packet)
        {
            Packet response = mIOv2.Get_Files(packet);
            SslStream sslStream;
            _clientsStreams.TryGetValue(packet._destination, out sslStream);
            //Console.WriteLine("THIS IS THE RESPONSE PACKET " + response.toString());
            try
            {
                SendToDestination(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <param name="ssl"></param>
        private void HandleFileRequest(TcpClient client, Packet packet)
        {
            string FileRequested = packet._message;
            client.Client.SendFile(mIOv2.Get_File(FileRequested));
            //TODO return file data!
            Console.WriteLine(mIOv2.Get_File(FileRequested));
        }

        private void HandleCommandPacket(Packet packet)
        {
            Console.WriteLine("Received Command packet: " + packet._message);
            SendToDestination(packet);
        }

        private void EnqueuePacket(SslStream stream, Packet p)
        {
            sendQueue.Add(new Tuple<SslStream, Packet>(stream, p));
        }

        public void sendPacket(SslStream stream, Packet p)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, Utils.GetPacketString(p));
        }

        private void startServerHelper()
        {
            new Thread(() =>
            {
                while (_running)
                {
                    var t = sendQueue.Take();

                    if (t.Item1 != null && t.Item1.CanWrite && t.Item2 != null)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(t.Item1, Utils.GetPacketString(t.Item2));
                    }
                }
            }).Start();
        }

        private void SaveMeasurement(Packet p)
        {
            /*
             * eerste packet is altijd een timestamp
             * daarna:
             * 7 strings in de array:
                Heartbeat = data[0];
                RPM = data[1];
                Speed = data[2];
                Distance = data[3];
                Power = data[4];
                Energy = data[5];
                TimeRunning = data[6];
                Brake = data[7];
            */
            string[] data = (p.GetMessage().Split(' '));
            if (data.Length < 8)
            {
                mIOv2.Create_file(p, _test);
            }
            else
            {
                //Console.WriteLine("\nHeartbeat: " + data[0] + "\nRPM 1: " + data[1] + "\nSpeed 2: " + data[2] + "\nDistance 3: " + data[3] +
                //    "\nPower 4: " + data[4] + "\nEnergy 5: " + data[5] + "\nTimeRunning 6: " + data[6] + "\nBrake 7: " + data[7]);
                mIOv2.Add_Measurement(p, _test);
            }
        }

        private void printClientList()
        {
            Console.WriteLine("\n############################################# ");
            Console.WriteLine("content for clients dictionary: ");
            foreach (KeyValuePair<string, TcpClient> entry in _clients)
            {
                Console.WriteLine("ID: " + ((string)entry.Key) + " TcpClient: " + entry.Value.GetHashCode());
            }
            Console.WriteLine("#############################################\n");
        }

        private string ResolveID(string id)
        {
            string temp = id.Substring(0, 1);
            switch (temp)
            {
                case "9":
                    return "Doctor";
                default:
                    return "Client";
            }
        }
    }
}
