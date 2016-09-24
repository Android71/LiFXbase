using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase
{
    public class LiFXNetService
    {
        int hostPort = 56700;
        IPAddress broadCastIP;
        UdpClient udpClient;


        public event EventHandler<UdpReceiveResult> ReceivedPacket;

        public LiFXNetService()
        {
            Init();
        }

        public void Init()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (/*ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||*/ ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) 
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {

                            if (ip.Address.ToString().StartsWith("192"))
                                HostIP = ip.Address;
                            IPAddress mask = ip.IPv4Mask;
                            broadCastIP = GetBroadCastIP(HostIP, mask);
                        }
                    }
                }
            }

            Task.Run(() => UDPListener());
            
        }

        IPAddress HostIP { get; set; }

        public void SendMessage(IPAddress ip, byte[] messagePacket)
        {
            var udpClient = new UdpClient(new IPEndPoint(ip, hostPort));
            udpClient.Send(messagePacket, messagePacket.Length);
        }

        public void SendBroadCastMessage(byte[] messagePacket)
        {
            IPAddress ip = IPAddress.Parse("192.168.1.255");
            //var udpClient = new UdpClient(new IPEndPoint(HostIP, hostPort));
            //var udpClient = new UdpClient(new IPEndPoint(broadCastIP, hostPort));
            udpClient.EnableBroadcast = true;
            udpClient.Send(messagePacket, messagePacket.Length, new IPEndPoint(ip, 56700));
        }


        IPAddress GetBroadCastIP(IPAddress host, IPAddress mask)
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

        public void DiscoverDevices()
        {
            Header discoverHeader;
        }

        async void UDPListener()
        {
            byte[] headerPacket = new byte[36];
            Header hdr;
            byte[] mac;
            Console.WriteLine("UDP Listener started;");
            udpClient = new UdpClient(new IPEndPoint(HostIP, 56700));

            while (true)
            {
                UdpReceiveResult udpResult = await udpClient.ReceiveAsync();
                if (udpResult.Buffer.Length > 0)
                {
                    if (udpResult.RemoteEndPoint.Port == 56700)
                    {
                        
                        Console.WriteLine($"Packet received Length {udpResult.Buffer.Length} IP{udpResult.RemoteEndPoint.Address} Port{udpResult.RemoteEndPoint.Port}");
                        if (udpResult.Buffer.Length == 41)
                        {
                            Array.Copy(udpResult.Buffer, headerPacket, 36);
                            hdr = Header.FromBytes(headerPacket);
                            mac = BitConverter.GetBytes(hdr.Target);
                        }
                        //OnReceivedPacket(udpResult);
                    }
                }
            }
        }

        void OnReceivedPacket(UdpReceiveResult udpResult)
        {
            ReceivedPacket?.Invoke(null, udpResult);
        }
    }
}
