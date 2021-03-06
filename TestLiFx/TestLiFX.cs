﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiFXbase;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace TestLiFx
{
    class TestLiFX
    {
        //static IPAddress HostIP;

        static void Main(string[] args)
        {
            LiFXService service = new LiFXService();
            Thread.Sleep(500);
            Console.WriteLine($"Обнаружено {service.LE_EntryList.Count} лампы");
            //service.StopParse = true;
            double hue = 0.0;
            service.StopParse = true;

            int i = 0;
            while (true)
            {
                
                service.SetColor(hue, 1.0, 0.1, 0, 2000, (byte)i);
                ////Thread.Sleep(10);
                service.SetColor(hue, 1.0, 0.1, 1, 2010, 0);
                //Thread.Sleep(25);
                //service.SetColor(hue, 1.0, 0.2, 0, 2000);
                ////Thread.Sleep(10);
                //service.SetColor(hue, 1.0, 0.2, 1, 2010);

                //Console.WriteLine($"Hue {hue}");
                //Console.ReadLine();
                //Console.WriteLine("GetState");
                //service.LE_EntryList[0].Channel.Get();
                //service.LE_EntryList[1].Channel.Get();
                Thread.Sleep(30);
                //Console.ReadLine();
                hue += 2.0;
                if (hue >= 360.0)
                    hue = 0.0;
                i++;
            }
        }

        //static public void Init()
        //{
        //    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        //    {
        //        if (/*ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||*/ ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
        //        {
        //            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
        //            {
        //                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        //                {

        //                    if (ip.Address.ToString().StartsWith("192"))
        //                    HostIP = ip.Address;
        //                    IPAddress mask = ip.IPv4Mask;
        //                    IPAddress broadCastIp = GetBroadCastIP(HostIP, mask);
        //                }
        //            }
        //        }
        //    }
        //}

        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }
    }
}
