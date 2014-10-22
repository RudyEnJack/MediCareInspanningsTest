using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCare.NetworkLibrary
{
    public interface ClientInterface
    {
        void sendPacket(Packet p);
    }
}
