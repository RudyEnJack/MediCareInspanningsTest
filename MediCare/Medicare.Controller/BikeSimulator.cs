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

        private long initialTime;
        
        private Boolean isCM = false;

        private int heartrate = 150;
        private int rpm = 60;
        private int speed = 33;
        private int distance = 14;
        private int power = 25;
        private int energy = 1200;
        private long timePassed = 0; //11minutes 11 seconds???? verify
        private int currentPower = 150;

        public BikeSimulator(string port)
        {
            Console.WriteLine("PortName: " + port);
            initialTime = DateTime.Now.Ticks;
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
                    timePassed = _value;
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
            string timeMin;
            string timeSec;
            if ((timePassed / 10000000) / 60 < 10)
            {
                timeMin = "0" + ((timePassed / 10000000) / 60);
            }
            else
            {
                timeMin = "" + ((timePassed / 10000000) / 60);
            }
            if ((timePassed / 10000000) % 60 < 10)
            {
                timeSec = "0" + ((timePassed / 10000000) % 60);
            }
            else
            {
                timeSec = "" + ((timePassed / 10000000) % 60);
            }
            return heartrate + " " + rpm + " " + speed + " " + distance + " " + power + " " + energy + " " + timeMin + ":" + timeSec + " " + currentPower; // Heartrate, Rpm, Speed, Distance, Power, Energy, Time, Current Power
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
            power = 25;
            energy = 1200;
            timePassed = 0; //11minutes 11 seconds???? verify
            currentPower = 150;
        }

        private void UpdateData()
        {
            timePassed = DateTime.Now.Ticks - initialTime;
            int secondsPassed = (int)(timePassed / TimeSpan.TicksPerSecond);

            Random rnd = new Random();
            double random = rnd.NextDouble() - 0.5;

            Console.WriteLine("time passed in seconds: " + secondsPassed + " random nubmer: " + random);

            //int values to tune settings for use with vo2max test // to enter these values exercise.cs should be checked
            int powerUpInterval = 30; // the interval in which the test increases the power level. in case of testing this could be 30 seconds. in release this should be 120 seconds for test results to be accurate
            int fewSeconds = 6; // extra seconds after 2 powerups for the simulator to keep cycling without too much heartrate increase
            int threshHoldHeartRate = 120; // value could be below 120 when testing. for end use should be 120.
            int lastStage = 0;
            int mintesForMeasurement = 1; //The ammount of minutes we are taking to do the actual test. for testing can be between 1 and 6. for end use should be 6 for the test result to make sense

            if(secondsPassed < 30) //if in first 10 seconds. the patient is connected to the hearthrate sensor but it not cyclign yet.
            {
                heartrate = 80 + (int)(random * 4);
                rpm = 0;
                speed = 0;
                //do not change currentpower on updates! its stupid!
                timePassed = DateTime.Now.Ticks - initialTime;
                distance = 0;
                energy = 0;
            }
            else if(secondsPassed < 30) // first 30 seconds of cycling
            {
                heartrate = 90 + (int)(random * 4);
                rpm = 55 + (int)(random * 6);
                speed = rpm * (power / 13);
                Console.WriteLine("Speed: " + speed);
                //do not change currentpower on updates! its stupid!
                timePassed = DateTime.Now.Ticks - initialTime;
                distance += speed * ((secondsPassed - 10) / 60);
            }
            else if(secondsPassed < ((2 * powerUpInterval) + fewSeconds + 30 ) ) // +30 is the time passed before this evaluation so 2 powerups and a few seconds after starting cycling
            {
                lastStage = ((2 * powerUpInterval) + fewSeconds + 30 );
                heartrate = 90 + 15 + (int)(random * 4); //stay under value of threshhold heartrate because we don't want the vo2max test to start just yet // +15 is just some madeup value
                rpm = 55 + (int)(random * 6);
                speed = rpm * (power / 13);
                Console.WriteLine("Speed: " + speed);
                //do not change currentpower on updates! its stupid!
                timePassed = DateTime.Now.Ticks - initialTime;
                distance += speed * ((secondsPassed - 10) / 60);
            }
            //else if(secondsPassed < lastStage + (mintesForMeasurement * 60) + fewSeconds) //before the lastStage stage plus 6 minutes and some fewSeconds
            else
            {
                if (power > 25) //if the power is above 25 (we are not cycling for cooling down. but we are measuring until the cooldown) these values
                {
                    heartrate = 90 + 39 + (int)(random * 4); //go over the threshhold value to trigger the exercise test start heartrate level
                    rpm = 55 + (int)(random * 6);
                    speed = rpm * (power / 13);
                    Console.WriteLine("Speed: " + speed);
                    //do not change currentpower on updates! its stupid!
                    timePassed = DateTime.Now.Ticks - initialTime;
                    distance += speed * ((secondsPassed - 10) / 60);
                }
                else //when the power is not over 25 its set to 25 by the exercise. this means cooldown
                {
                    heartrate = 90 + 15 + (int)(random * 4); //go over the threshhold value to trigger the exercise test start heartrate level
                    rpm = 55 + (int)(random * 6);
                    speed = rpm * (power / 13);
                    Console.WriteLine("Speed: " + speed);
                    //do not change currentpower on updates! its stupid!
                    timePassed = DateTime.Now.Ticks - initialTime;
                    distance += speed * ((secondsPassed - 10) / 60);
                }
            }
        }
/*
        private void UpdateData()
        {
            Random rnd = new Random();
            double random = rnd.NextDouble() * (1.2 - 0.8) + 0.8;

            heartrate = (int)(heartrate * random);
            rpm = (int)(rpm * random);
            speed = (int)(speed * random);
            currentPower = (int)(currentPower * random);
            if (currentPower > 400)
            {
                currentPower = 400;
            }

            
            timePassed = DateTime.Now.Ticks - initialTime;
            distance += (int)((speed * 3.6) * ((timePassed / 10000000)));
            energy += (int)(timePassed / 10000000); //60 Kjoules per minuut, dus een per seconde (als je 30km/u gaat en 66Kg weegt).
            //Console.WriteLine("timepassed: " + (timePassed / 10000000) / 60 + ":" + (timePassed / 10000000) % 60);

            // TODO Add Time
        }
        */
        /*public bool isOpen()
        {
            return _comPort.IsOpen();
        }*/
    }
}
