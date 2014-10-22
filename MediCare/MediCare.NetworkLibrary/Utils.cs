using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MediCare.NetworkLibrary
{
    public class Utils
    {
        public static Packet GetPacket(String s)
        {
            return new JavaScriptSerializer().Deserialize<Packet>(s);
        }

        public static String GetPacketString(Packet p)
        {
            return new JavaScriptSerializer().Serialize(p);
        }
    }
}
