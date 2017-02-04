using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LiFX_Lib
{
    public class LiFXUtils
    {
        public static List<LightDescriptor> GetLights()
        {
            List<LightDescriptor> lightList = new List<LightDescriptor>();

            IPEndPoint myEndPoint;
            IPAddress HostIP = null;
            IPAddress broadCastIP = null;
            //UdpClient udpClient = null;

            //foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    if (/*ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||*/ ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            //        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            //            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
            //                if (ip.Address.ToString().StartsWith("192"))
            //                {
            //                    HostIP = ip.Address;
            //                    IPAddress mask = ip.IPv4Mask;
            //                    broadCastIP = LiFXUtils.GetBroadCastIP(HostIP, mask);
            //                }
            //}

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in interfaces)
            {
                var ipProps = adapter.GetIPProperties();

                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if ((adapter.OperationalStatus == OperationalStatus.Up)
                        && (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        /*&& (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)*/)
                    {
                        Console.WriteLine(ip.Address.ToString() + "|" + adapter.Description.ToString());
                        if (ip.Address.ToString().StartsWith("192"))
                        {
                            HostIP = ip.Address;
                            IPAddress mask = ip.IPv4Mask;
                            broadCastIP = LiFXUtils.GetBroadCastIP(HostIP, mask);
                        }
                    }
                }
            }

            myEndPoint = new IPEndPoint(HostIP, 56700);
            using (UdpClient udpClient = new UdpClient(myEndPoint))
            {
                bool endScan = false;
                IPEndPoint remoteEndPoint = null;
                byte[] headerPacket = new byte[36];
                byte[] mac = null;
                Header header = null;
                udpClient.Client.ReceiveTimeout = 2000;

                udpClient.Send(LiFXUtils.GetServicePacket, 36, new IPEndPoint(broadCastIP, 56700));

                while (!endScan)
                {
                    try
                    {
                        byte[] data = udpClient.Receive(ref remoteEndPoint);
                        if (!remoteEndPoint.Address.Equals(myEndPoint.Address))
                        {
                            MsgTypeEnum msgType = (MsgTypeEnum)BitConverter.ToInt16(data, 32);

                            if (msgType == MsgTypeEnum.StateService)
                            {
                                Array.Copy(data, headerPacket, 36);
                                header = Header.FromBytes(headerPacket);
                                mac = BitConverter.GetBytes(header.Target);

                                //Console.WriteLine();
                                //Console.WriteLine(DateTime.Now.Millisecond.ToString());
                                //Console.WriteLine($"Payload Length: {data.Length - 36}");
                                //Console.WriteLine($"Type: {header.MessageType}");
                                //Console.WriteLine($"IP: {remoteEndPoint.Address}");
                                //Console.WriteLine($"Port: {remoteEndPoint.Port}");
                                //Console.WriteLine($"MAC: {Utils.MacString(header.Target)}");
                                if (lightList.FirstOrDefault(p => p.MAC == header.Target) == null)
                                    lightList.Add(new LightDescriptor(remoteEndPoint.Address, header.Target));
                            }
                        }
                    }
                    catch/*(SocketException ex)*/
                    {
                        //Console.WriteLine($"{ex.SocketErrorCode}");
                        endScan = true;
                    }
                }
            }
            return lightList;
        }

        public static List<LightDescriptor> GetNewLights(List<LightDescriptor> currentLights)
        {
            List<LightDescriptor> newLights = new List<LightDescriptor>();
            List<LightDescriptor> lightList = GetLights();

            foreach(LightDescriptor ld in lightList)
            {
                if (currentLights.FirstOrDefault(p => p.MAC == ld.MAC) == null)
                    newLights.Add(ld);
            }

            return newLights;
        }

        public static byte[] GetServicePacket
        {
            get
            {
                Header header = new Header();
                header.Tagged = true;
                header.Addressable = true;
                header.Target = 0;
                header.Res_required = false;
                //header.Ack_required = true;
                header.Source = 24;
                header.Sequence = 55;
                header.MessageType = MsgTypeEnum.GetService;
                return header.GetBytes(); ;
            }
        }

        public static string MacString(UInt64 macUint64)
        {
            byte[] mac = new byte[6];
            byte[] macEx = BitConverter.GetBytes(macUint64);
            Array.Copy(macEx, mac, 6);
            return string.Join(":", (from z in mac select z.ToString("X2")).ToArray());
        }

        public static IPAddress GetBroadCastIP(IPAddress host, IPAddress mask)
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

        public static double[] HSL2HSB(double h, double s, double l)
        {
            double[] hsb = new double[3];

            // Hue
            hsb[0] = h;

            // Brightness
            //B = 2L + Shsl(1−| 2L−1 |) / 2
            hsb[2] = (2.0 * l + s * (1.0 - Math.Abs(2.0 * l - 1.0))) / 2.0;

            // Saturation
            //Shsb = 2(B−L)/B
            hsb[1] = 2.0 * (hsb[2] - l) / hsb[2];

            return hsb;
        }
    }
}
