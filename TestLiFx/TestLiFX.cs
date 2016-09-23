using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiFXbase;
using System.Net;

namespace TestLiFx
{
    class TestLiFX
    {
        static void Main(string[] args)
        {
            //short val = 1024;
            //byte[] vbyte = BitConverter.GetBytes(val);
            //short val1 = IPAddress.HostToNetworkOrder(val);
            //byte[] vbyte2 = BitConverter.GetBytes(val1);

            //short val2 = BitConverter.ToInt16(vbyte, 0);
            //short val3 = BitConverter.ToInt16(vbyte2, 0);

            //var x = new LiFXNetService();
            //byte[] mac = new byte[8] { 1, 2, 3, 4, 5, 6, 0, 0};
            //byte[] macForRevers = new byte[8] { 1, 2, 3, 4, 5, 6, 0, 0 };
            //UInt64 macInt = (UInt64)BitConverter.ToInt64(mac, 0); // 6618611909121
            //byte[] macArr = BitConverter.GetBytes(macInt);
            //UInt64 macIntRev = ReverseBytes(macInt);
            //Array.Reverse(macForRevers);
            //UInt64 macIntRev = (UInt64)BitConverter.ToInt64(macForRevers, 0); // 72623859790381056


            LiFxControlChannel ch = new LiFxControlChannel();
            ch.GetService();
        }

        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }
    }
}
