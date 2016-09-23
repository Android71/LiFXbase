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
                    //var x = ni.GetPhysicalAddress();
                    //string s = x.ToString();
                    //byte[] phaBytes = x.GetAddressBytes();
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            
                            HostIP = ip.Address;
                        }
                    }
                }
            }
            
        }

        IPAddress HostIP { get; set; }

        public void SendMessage(IPAddress ip, byte[] messagePacket)
        {
            var udpClient = new UdpClient(new IPEndPoint(ip, hostPort));
            udpClient.Send(messagePacket, messagePacket.Length);
        }

        public void DiscoverDevices()
        {
            Header discoverHeader;
        }

        async void UDPListener()
        {
            var udpClient = new UdpClient(new IPEndPoint(HostIP, 56700));

            while (true)
            {
                UdpReceiveResult udpResult = await udpClient.ReceiveAsync();
                if (udpResult.Buffer.Length > 0)
                {
                    if (udpResult.RemoteEndPoint.Port == 56700)
                    {
                        OnReceivedPacket(udpResult);
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
