using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

namespace MediCare.Controller
{
    class SerialController : ComController
    {
        private SerialPort _comPort;

        public SerialController(string port)
        {
            _comPort = new SerialPort(port, 9600);
        }

        public SerialController()
        {
            List<string> ports = GetCorrectPort();
            string port = ports[0];
            Console.WriteLine("chosen port is: " + ports[0]);
            _comPort = new SerialPort(port, 9600);
        }

        /**
         * AutoPort Detection.
         * 
         * Note: In the SerialController is a TRY > CATCH clausule in order for this to work you have to comment out / remove that.
         * 
         * It may be necessary to do some cleanup i dont know if all SerialControllers are left over in the memory or not.
         * 
         * tested method, it works!
         * 
         * @Author: Frank van Veen
         * @corrector: Collin Baden
         * @Version: 1.0 
         * @Return: The correct port as string
         */
        override public List<string> GetCorrectPort()
        {
            string[] ports = getAvailablePorts();
            List<string> correctport = new List<string>();
            for (int i = 0; i < ports.Length -1; i++)
            {
                if (ports[i].StartsWith("COM"))
                {
                    try
                    {
                        SerialController sc = new SerialController(ports[i]);
                        sc.openConnection(); // breaks on this line
                        sc.send(Enums.GetValue(Enums.BikeCommands.RESET)); // send reset to port
                        string result = sc.read();
                        if (result != null && result != "" && !result.ToLower().Contains("err"))
                        {
                            if (result.ToLower().Contains("ac"))
                            {
                                correctport.Add(ports[i]); //add the correct port to string
                                Console.WriteLine("Port added: " + ports[i]);
                            }
                        }
                        Console.WriteLine("Checked: " + ports[i]);
                        sc.closeConnection();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(ports[i] + " Failed to open. Trying next port");
                    }
                }
            }
            if (correctport == null || correctport.Count == 0)
            {
                //MessageBox.Show("No physical device found.\nUsing Simulator.");
                Console.WriteLine("No physical device found.\nUsing Simulator.");
                correctport.Add("SIM");
                return correctport;
                //return null;
            }
            Console.WriteLine(correctport);
            return correctport;
        }


        /**
         * Remove Try Catch Clausule When switching to auto detection Mode for COM port detection (located in BikeController)
         */ 
        override public void openConnection()
        {
            try
            {
                _comPort.Open();
            }
            catch (System.IO.IOException)
            {
                closeConnection();
            }
            
        }

        override public void closeConnection()
        {
            _comPort.Close();
        }

        override public void send(string command)
        {
            _comPort.WriteLine(command);
        }

        override public string[] getAvailablePorts()
        {
            string[] temp = SerialPort.GetPortNames();
            List<string> tempList = temp.ToList();
            tempList.Add("SIM");
            return tempList.ToArray<string>();
        }

        override public string read()
        {
            return _comPort.ReadLine();
        }

        override public string getPort()
        {
            return _comPort.PortName;
        }

        /*public bool isOpen()
        {
            return _comPort.IsOpen();
        }*/
    }
}
