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

namespace MediCare.Server
{
    class Server
    {
        private IPAddress _localIP = IPAddress.Parse("127.0.0.1");
        private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private Dictionary<string, SslStream> clientsStreams = new Dictionary<string, SslStream>();
        private ObjectIOv2 mIOv2; // do not remove, do not move and do not edit!

        private LoginIO loginIO = new LoginIO();

        private string _toAllDoctors = "Dokter";
        //README!!! - SSL certificate needs to be coppied from MediCare.Server\ssl_cert.pfx to C:\Windows\Temp\
        private X509Certificate certificate = new X509Certificate(@"C:\Windows\Temp\ssl_cert.pfx", "medicare");


        static void Main(string[] args)
        {
            new Server();
        }

        public Server()
        {
            loginIO.LoadLogins();
            Console.WriteLine(loginIO.getSize());
            mIOv2 = new ObjectIOv2(); // do not remove, do not move and do not edit!

            TcpListener server = new TcpListener(_localIP, 11000);
            server.Start();

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
                                HandleDisconnectPacket(packet);
                                break;
                                case "Data":
                                HandleDataPacket(packet);
                                break;
                                case "Registration":
                                HandleRegistrationPacket(packet);
                                break;
                                case "Broadcast":
                                HandleBroadcastMessagePacket(packet);
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
                                default: //nothing
                                break;
                            }
                        }
                    } // end While
                }).Start();
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
            foreach (KeyValuePair<string, SslStream> entry in clientsStreams)
            {
                if (IsDoctor(entry.Key))
                {
                    SslStream sslStream = entry.Value;
                    SendPacket(sslStream, packet);
                    //Console.WriteLine("packetMessage: " + packet._message);
                }
            }
        }

        private void SendToDestination(Packet packet)
        {
            SslStream sslStream;
            clientsStreams.TryGetValue(packet._destination, out sslStream);
            SendPacket(sslStream, packet);
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

            if (loginIsValid(p._message))
            {
                Packet response = new Packet("Server", "FirstConnect", p._id, "VERIFIED");
                SendPacket(stream, response);
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
                Packet response = new Packet("Server", "FirstConnect", p._id, "DENIED");
                SendPacket(stream, response);

#if DEBUG
                Console.WriteLine("Login credentials are invalid");
#endif
            }
        }

        private void addNewClient(Packet p, TcpClient incomingClient, SslStream stream)
        {
            clients.Add(p.GetID(), incomingClient);
            clientsStreams.Add(p.GetID(), stream);

            #region DEBUG
#if DEBUG
            Console.WriteLine("ID: " + p.GetID() + "incomingClient: " + incomingClient.ToString());
            printClientList();
#endif
            #endregion
        }

        private bool clientIsKnown(String id)
        {
            return clients.ContainsKey(id);
        }

        private bool loginIsValid(string credentials)
        {
            loginIO.add("12345678:dsa"); //TODO Remove
            loginIO.add("98765432:asd"); //TODO Remove
            loginIO.add("87654321:asd"); //TODO Remove

            // This part is for the Doctors signup tool.
            // The signup tool connects with "DoctorID" + "r"
            // this code checks if the doctor is connected if yes. its fine for the signuptool to connect without password
            if (credentials.Split(':')[0].EndsWith("r"))
            {
                return clients.ContainsKey(credentials.Split(':')[0].Substring(0, 8));
            }

            return loginIO.login(credentials);
        }

        /**
         * Stuur het sluit bericht terug naar de client en sluit de connectie. 
         * Client doet hetzelfde na het ontvangen van het sluit bericht.
         */
        private void HandleDisconnectPacket(Packet p)
        {
            mIOv2.Remove_client(p); // do not remove, do not move and do not edit!
            Packet response = new Packet("server", "Disconnect", p.GetID(), "LOGGED OFF");
            SendToDestination(response);
            Console.WriteLine(p.GetID() + " has disconnected");
            TcpClient sender;
            clients.TryGetValue(p._id, out sender);
            sender.Close();
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
                foreach (var s in clientsStreams)
                {
                    if (s.Key.StartsWith("9"))
                    {
                        //Console.WriteLine("Destination: " + s.Key);// + sslStream.ToString());
                        try
                        {
                            SendPacket(s.Value, packet);
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
                clientsStreams.TryGetValue(packet._destination, out sslStream);
                //   Console.WriteLine("Destination: " + packet._destination + " packet id destination: ");// + sslStream.ToString());
                try
                {
                    SendPacket(sslStream, packet);
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
            loginIO.add(p.GetMessage());
            Packet response = new Packet("server", "Registration", p.GetID(), "Registration attempt succeeded");
            SendToDestination(response);
        }

        /**
         * Handel een broadcast message af van de Doktor.
         * 
         */
        private void HandleBroadcastMessagePacket(Packet p)
        {
            string id = p._id + " [Broadcast]";
            foreach (string key in clients.Keys)
            {
                if (!key.StartsWith("9"))
                {
                    Packet response = new Packet(id, "Chat", key, p._message);
                    SendToDestination(response);
                }
            }
        }
        /*
         * Geeft het aantal actieve clients
         * 
         */
        private void HandleActiveClients(Packet p)
        {
            string ids = "";
            foreach (string key in clients.Keys)
            {
                if (!key.StartsWith("9"))
                {
                    ids += key + " ";
                }
            }
            //Console.WriteLine("Active clients: " + clients.Count.ToString());
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
            clientsStreams.TryGetValue(packet._destination, out sslStream);
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
            //client.Client.SendFile(mIOv2.Get_File(FileRequested));
            //TODO return file data!
            Console.WriteLine(mIOv2.Get_File(FileRequested));
        }

        private void HandleCommandPacket(Packet packet)
        {
            SendToDestination(packet);
        }

        private void SendPacket(SslStream stream, Packet p)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, Utils.GetPacketString(p));
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
                mIOv2.Create_file(p);
            }
            else
            {
                //Console.WriteLine("\nHeartbeat: " + data[0] + "\nRPM 1: " + data[1] + "\nSpeed 2: " + data[2] + "\nDistance 3: " + data[3] +
                //    "\nPower 4: " + data[4] + "\nEnergy 5: " + data[5] + "\nTimeRunning 6: " + data[6] + "\nBrake 7: " + data[7]);
                mIOv2.Add_Measurement(p);
            }
        }

        private void printClientList()
        {
            Console.WriteLine("\n############################################# ");
            Console.WriteLine("content for clients dictionary: ");
            foreach (KeyValuePair<string, TcpClient> entry in clients)
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
