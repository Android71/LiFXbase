using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiFXbase;
using System.Net;
using System.Net.NetworkInformation;

namespace TestLiFx
{
    class TestLiFX
    {
        static IPAddress HostIP;

        static void Main(string[] args)
        {
            IPAddress HostIp;
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
            //Init();

            LiFXNetService service = new LiFXNetService();
            Console.ReadLine();
            LiFxControlChannel ch = new LiFxControlChannel(service);
            Console.ReadLine();
            //ch.GetService();
        }

        static public void Init()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (/*ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||*/ ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //var x = ni.GetPhysicalAddress();
                    //string s = x.ToString();
                    //byte[] phaBytes = x.GetAddressBytes();
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {

                            if (ip.Address.ToString().StartsWith("192"))
                            HostIP = ip.Address;
                            IPAddress mask = ip.IPv4Mask;
                            IPAddress broadCastIp = GetBroadCastIP(HostIP, mask);
                        }
                    }
                }
            }
        }

        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        static IPAddress GetBroadCastIP(IPAddress host, IPAddress mask)
        {
            byte[] broadcastIPBytes = new byte[4];
            byte[] hostBytes = host.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();
            for (int i = 0; i < 4; i++)
            {
                broadcastIPBytes[i] = (byte)(hostBytes[i] | (byte)~maskBytes[i]);
            }
            return new IPAddress(broadcastIPBytes);
        }
    }
}
