using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase
{
    public class LiFX_EndPoint
    {
        public LiFX_EndPoint(IPAddress ip, UInt64 macAddressEx)
        {
            IPAddress = ip;
            Port = 56700;
            MacAddressEx = macAddressEx;
        }

        public IPAddress IPAddress { get; set; }

        public int Port { get; set; }

        public UInt64 MacAddressEx { get; set; } 

        public string MacAddress
        {
            
            get
            {
                byte[] mac = new byte[6];
                byte[] macEx = BitConverter.GetBytes(MacAddressEx);
                Array.Copy(macEx, mac, 6);
                return  string.Join(":", (from z in mac select z.ToString("X2")).ToArray());
            }
        }
    }
}
