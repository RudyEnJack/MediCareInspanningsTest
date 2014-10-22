using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MediCare.Controller
{
    public class Enums
    {

        public enum BikeCommands
        { 
            [StringValue("rs")]
            RESET,
            [StringValue("lb")]
            LOCK,
            [StringValue("st")]
            STATUS,
            [StringValue("pw")]
            POWER,
            [StringValue("cm")]
            CONTROLMODE,
            [StringValue("cd")]
            CONTROLMODE2,
            [StringValue("id")]
            ID,
            [StringValue("ki")]
            TYPE,
            [StringValue("pt")]
            SETTIME,
            [StringValue("pd")]
            SETDISTANCE,
            [StringValue("pe")]
            SETENERGY,
            [StringValue("tr")]
            GETDATETIME
        };

        public static bool ContainsCommand(String cmd)
        {
            foreach (Enums.BikeCommands bc in (BikeCommands[])Enum.GetValues(typeof(BikeCommands)) )
            {
                if (Enums.GetValue(bc).Contains(cmd) )
                {
                    return true;
                }
            }

            return false;
        }

        public enum StatusInfo
        {
            HEARTRATE = 0,
            RPM = 1,
            SPEED = 2,
            DISTANCE = 3,
            POWER = 4,
            ENERGY = 5,
            TIME = 6,
            CURRENTPOWER = 7 
        };

        public class StringValue : System.Attribute
        {
            private string _value;

            public StringValue(string value)
            {
                _value = value;
            }

            public string Value
            {
                get { return _value; }
            }

        }
        
        public static string GetValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();
            
            //Check first in our cached results...

            //Look for our 'StringValueAttribute' 

            //in the field's custom attributes

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs =
                fi.GetCustomAttributes(typeof(StringValue),
                                           false) as StringValue[];
                if (attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }

                return output;
            }
        }
    }

