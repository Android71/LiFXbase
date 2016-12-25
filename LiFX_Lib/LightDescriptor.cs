using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiFX_Lib
{
    public class LightDescriptor
    {
        public LightDescriptor(IPAddress ip, UInt64 mac)
        {
            Address = ip;
            MAC = mac;
        }
        public IPAddress Address { get; private set; }

        public UInt64 MAC { get; private set; }

        public string MacStr
        {
            get { return LiFXUtils.MacString(MAC); }
        }
    }
}
