using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCare.Controller
{
    public class BikeController
    {
        private ComController cc;

        public BikeController(string comPort)
        {
            if (comPort.Equals(""))
            {
                cc = new SerialController();
                comPort = cc.getPort();
            }
            
            if (comPort.Contains("COM"))
            {
                cc = new SerialController(comPort);
                //Console.WriteLine(Enums.GetValue(Enums.BikeCommands.CONTROLMODE));
            }
            else
            {
                cc = new BikeSimulator(comPort); //Bikesimulator
            }

            cc.openConnection();
        }


        #region getters

        public string[] GetPorts()
        {
            try
            {
                return cc.getAvailablePorts();
            }
            catch (Exception e)
            {
                string[] rawArray = new string[1];
                rawArray[0] = e.Message.ToString();
                Console.WriteLine(e.Message.ToString());
                return rawArray;
            }
        }


        public string[] GetStatus()
        {
            try
            {
                cc.send(Enums.GetValue(Enums.BikeCommands.STATUS));
                string raw = cc.read();
                if (!raw.ToLower().Contains("err"))
                {
                    string[] rawArray = raw.Split();
            	    rawArray[2] = (float.Parse(rawArray[2]) / 10).ToString();
                    rawArray[3] = (float.Parse(rawArray[3]) / 10).ToString();
                    return rawArray;
                }
                else
                {
                    string[] rawArray = null;
                    rawArray[0] = "ERROR";
                    return rawArray; 
                }
            }
            catch (Exception e)
            {
                string[] rawArray = new string[1];
                rawArray[0] = e.Message.ToString();
                return rawArray;
            }
        }

        public string getTime()
        {
            try
            {
                cc.send(Enums.GetValue(Enums.BikeCommands.GETDATETIME));
                string raw = cc.read();
                if (!raw.ToLower().Contains("err"))
                {
                    return raw;
                }
                else return "ERROR";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        #endregion

        #region setters

        public string ResetBike() 
        {
            try
            {
                cc.send(Enums.GetValue(Enums.BikeCommands.RESET));
                return cc.read();
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        public string[] SetPower(int power)
        {
            try
            {
                cc.send(Enums.GetValue(Enums.BikeCommands.CONTROLMODE));
                string rawcm = cc.read();
                if (rawcm.ToLower().Contains("err"))
                {
                    string[] rawArrayCM = new string[1];
                    rawArrayCM[0] = "ERROR CM";
                    return rawArrayCM;
                }
                else
                {
                    cc.send(Enums.GetValue(Enums.BikeCommands.POWER) + /*" " +*/ power.ToString());
                    string raw = cc.read();
                    if (!raw.ToLower().Contains("err"))
                    {
                        string[] rawArray = raw.Split();
                        rawArray[3] = (float.Parse(rawArray[3]) / 10).ToString();
                        rawArray[4] = (float.Parse(rawArray[4])).ToString();
                        return rawArray;
                    }
                    else
                    {
                        string[] ErrorArray = new string[1];
                        ErrorArray[0] = "ERROR PW";
                        return ErrorArray;
                    }
                }
            }
            catch (Exception e)
            {
                string[] rawArray = new string[1];
                rawArray[0] = e.Message.ToString();
                return rawArray;
            }
        }

        public string[] SetTime(int time)
        {
            try
            {
                cc.send(Enums.GetValue(Enums.BikeCommands.CONTROLMODE));
                string rawcm = cc.read();
                if (rawcm.ToLower().Contains("err"))
                {
                    string[] rawArrayCM = new string[1];
                    rawArrayCM[0] = "ERROR CM";
                    return rawArrayCM;
                }
                cc.send(Enums.GetValue(Enums.BikeCommands.SETTIME) + /*" " +*/ time.ToString());
                string raw = cc.read();
                if (!raw.ToLower().Contains("err"))
                {
                    string[] rawArray = raw.Split();
                    rawArray[3] = (float.Parse(rawArray[3]) / 10).ToString();
                    rawArray[4] = (float.Parse(rawArray[4])).ToString();
                    return rawArray;
                }
                else
                {
                    string[] ErrorArray = new string[1];
                    ErrorArray[0] = "ERROR ST";
                    return ErrorArray;
                }
            }
            catch (Exception e)
            {
                string[] rawArray = new string[1];
                rawArray[0] = e.Message.ToString();
                return rawArray;
            }
        }

        public string[] SetEnergy(int energy)
        {
            try
            {
                cc.send(Enums.GetValue(Enums.BikeCommands.CONTROLMODE));
                string rawcm = cc.read();
                if (rawcm.ToLower().Contains("err"))
                {
                    string[] rawArrayCM = new string[1];
                    rawArrayCM[0] = "ERROR CM";
                    return rawArrayCM;
                }
                cc.send(Enums.GetValue(Enums.BikeCommands.SETENERGY) + /*" " +*/ energy.ToString());
                string raw = cc.read();
                if (!raw.ToLower().Contains("err"))
                {
                    string[] rawArray = raw.Split();
                    rawArray[3] = (float.Parse(rawArray[3]) / 10).ToString();
                    rawArray[4] = (float.Parse(rawArray[4])).ToString();
                    return rawArray;
                }
                else
                {
                    string[] ErrorArray = new string[1];
                    ErrorArray[0] = "ERROR PE";
                    return ErrorArray;
                }
            }
            catch (Exception e)
            {
                string[] rawArray = new string[1];
                rawArray[0] = e.Message.ToString();
                return rawArray;
            }
        }

        #endregion
    }
}
