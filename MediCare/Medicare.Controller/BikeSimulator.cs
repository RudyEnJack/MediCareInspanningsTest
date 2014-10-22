using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MediCare.Controller
{
    class BikeSimulator : ComController
    {
        private String _status = ""; //command that was sent in through the send method. excluding the value that has been added to the end.
        private int _value = 0; //value that was passed in with the command via send.

        private int initialTimeInSecs;
        private int initialTimeInMins;
        
        private Boolean isCM = false;

        private int heartrate = 110;
        private int rpm = 40;
        private int speed = 33;
        private int distance = 14;
        private int power = 170;
        private int energy = 1200;
        private int timeSec = 0;
        private int timeMin = 0;
        private long time = 0; //11minutes 11 seconds???? verify
        private int currentPower = 150;

        public BikeSimulator(string port)
        {
            Console.WriteLine("PortName: " + port);
            initialTimeInSecs = DateTime.Now.Second;
            initialTimeInMins = DateTime.Now.Minute;
        }

        override public void openConnection()
        {
            _status = "open";
        }

        override public void closeConnection()
        {
            _status = "closed";
        }

        override public void send(string command)
        {

            if(Enums.ContainsCommand(command.Substring(0, 2)))
	        {
                _status = command.Substring(0, 2);
                if (command.Length > 2)
                {
                   _value = int.Parse(command.Substring(2, command.Length - 2));
                }

                if (command == "cm" || command == "cd")
                {
                    isCM = true;
                }
                UpdateData();
	        }
        }

        override public List<string> GetCorrectPort()
        {
            return new List<string> { "SIM" };
        }

        override public string[] getAvailablePorts()
        {
            return new string[] {"SIM"};
        }

        override public string read()
        {
            Thread.Sleep(400);

            //Console.WriteLine("set status was: " + _status);

            switch (_status)
            {
                case "cd":
                case "cm":
                    return "ACK";
                case "id":
                    return "AA1A1337";
                case "ki":
                    return "X7";
                case "lb":
                case "rg":
                    return GetCMStatus();
                case "pd":
                    distance = _value;
                    return GetStatus();
                case "pe":
                    energy = _value;
                    return GetStatus();
                case "pt":
                    time = _value;
                    return GetStatus();
                case "pw":
                    power = _value;
                    return GetStatus();
                case "rf":
                case "ee":
                case "es":
                case "rd":
                case "rm":
                    return "This method is not supported by the simulator.";
                case "rs":
                    Reset();
                    return "ACK";        
                case "st":
                    return GetStatus();
                case "tr":
                    return "";
                case "ve":
                    return "111";
                case "vs":
                case "vz":
                    return "" + rpm;
                case "ca":
                    return "999";
                case "op":
                    return "ACK";
                default:
                    return "ERROR"; //TODO change
            }            
        }

        public override string getPort()
        {
            return "SIM";
        }

        private string GetStatus()
        {
            string time = "";
            if (timeMin < 10)
            {
                time += "0" + timeMin + ":";
            }
            else
            {
                time += timeMin + ":";
            }
            if (timeSec < 10)
            {
                time += "0" + timeSec;
            }
            else
            {
                time += timeSec;
            }
            return heartrate + " " + rpm + " " + speed + " " + distance + " " + power + " " + energy + " " + time + " " + currentPower; // Heartrate, Rpm, Speed, Distance, Power, Energy, Time, Current Power
        }

        private String GetCMStatus()
        {
            
            if (isCM)
            {
                return "ACK";
            }
            else
            {
                return "ERROR";
            }
        }

        private void Reset()
        {
            _value = 0;
            isCM = false;

            heartrate = 110;
            rpm = 40;
            speed = 33;
            distance = 0;
            power = 100;
            energy = 1200;
            time = 0; //11minutes 11 seconds???? verify
            currentPower = 150;
        }

        private void UpdateData()
        {
            Random rnd = new Random();
            double random = rnd.NextDouble() * (1.2 - 0.8) + 0.8;

            heartrate = (int)(heartrate * random);
            rpm = (int)(rpm * random);
            speed = (int)(speed * random);
            power = (int) (170 * ((random + 1) * 1));
            currentPower = (int)(currentPower * random);
            if (currentPower > 400)
            {
                currentPower = 400;
            }

            timeSec = DateTime.Now.Second - initialTimeInSecs;
            timeMin = timeSec / 60;
            if (timeSec < 0)
            {
                if (timeSec <= 0)
                {
                    timeMin++;
                }
                timeSec = 60 + timeSec;
            }
            distance += (int)((speed * 3.6) * (timeSec + (60*timeMin)));
            energy += (timeSec + (60*timeMin)); //60 Kjoules per minuut, dus een per seconde (als je 30km/u gaat en 66Kg weegt).
            Console.WriteLine("timepassed: " + timeMin + ":" + timeSec);

            // TODO Add Time
        }

        /*public bool isOpen()
        {
            return _comPort.IsOpen();
        }*/
    }
}
