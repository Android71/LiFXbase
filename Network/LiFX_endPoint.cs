using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase.Network
{
    public class LiFX_EndPoint
    {
        public LiFX_EndPoint(IPAddress ip, byte[] macAddress)
        {

        }

        public IPAddress IPAddress { get; set; }

        public byte[] MacAddress { get; set; } 

        public UInt64 Target
        {
            get
            {
                return 0;
            }
        }
    }
}
