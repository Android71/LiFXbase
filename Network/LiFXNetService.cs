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
        UdpClient udpClient = null;
        Header header = null;
        public List<LE_Entry> LE_EntryList = new List<LE_Entry>();


        public event EventHandler<UdpReceiveResult> ReceivedPacket;

        public LiFXNetService()
        {
            Init();
            while (udpClient == null) { }
            SendGetService();
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
                            {
                                HostIP = ip.Address;
                                IPAddress mask = ip.IPv4Mask;
                                broadCastIP = GetBroadCastIP(HostIP, mask);
                            }
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

        void SendGetService()
        {
            //IPAddress ip = IPAddress.Parse("192.168.1.255");
            //var udpClient = new UdpClient(new IPEndPoint(HostIP, hostPort));
            //var udpClient = new UdpClient(new IPEndPoint(broadCastIP, hostPort));
            //udpClient.EnableBroadcast = true;
            udpClient.Send(GetServicePacket(), 36, new IPEndPoint(broadCastIP, 56700));
        }

        public bool StopParse = false;

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
            
            byte[] mac;
            Console.WriteLine("UDP Listener started;");
            udpClient = new UdpClient(new IPEndPoint(HostIP, 56700));

            while (true)
            {
                UdpReceiveResult udpResult = await udpClient.ReceiveAsync();

                if (!StopParse)
                {
                    if (udpResult.Buffer.Length > 0)
                    {
                        if (udpResult.RemoteEndPoint.Port == 56700)
                        {


                            if (udpResult.Buffer.Length >= 36)
                            {
                                MsgTypeEnum msgType = (MsgTypeEnum)BitConverter.ToInt16(udpResult.Buffer, 32);
                                if (msgType != MsgTypeEnum.GetService)
                                {
                                    Array.Copy(udpResult.Buffer, headerPacket, 36);
                                    header = Header.FromBytes(headerPacket);
                                    mac = BitConverter.GetBytes(header.Target);

                                    if (msgType == MsgTypeEnum.StateService)
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine(DateTime.Now.ToString());
                                        Console.WriteLine($"Payload Length: {udpResult.Buffer.Length - 36}");
                                        Console.WriteLine($"Type: {header.MessageType}");
                                        Console.WriteLine($"IP: {udpResult.RemoteEndPoint.Address}");
                                        Console.WriteLine($"Port: {udpResult.RemoteEndPoint.Port}");
                                        Console.WriteLine($"MAC: {MacStr(header.Target)}");
                                        if (LE_EntryList.FirstOrDefault(p => p.MacUInt64 == header.Target) == null)
                                            LE_EntryList.Add(new LE_Entry(new LiFxControlChannel(this.udpClient, new LiFX_EndPoint(udpResult.RemoteEndPoint.Address, header.Target)), header.Target));
                                        else
                                            Console.WriteLine("Повторный пакет");

                                    }
                                    else
                                    {
                                        LE_Entry le_Entry = LE_EntryList.FirstOrDefault(p => p.MacUInt64 == header.Target);

                                        if (le_Entry != null)
                                        {
                                            LiFxControlChannel targetChannel = le_Entry.Channel;
                                            byte[] payload = new byte[udpResult.Buffer.Length - 36];
                                            Array.Copy(udpResult.Buffer, 36, payload, 0, udpResult.Buffer.Length - 36);

                                            switch (msgType)
                                            {
                                                case MsgTypeEnum.State:
                                                    var x = State.FromBytes(payload);
                                                    targetChannel.State = x;
                                                    //targetChannel.State = State.FromBytes(payload);
                                                    Console.WriteLine();
                                                    Console.WriteLine($"Hue: {x.Hue}");
                                                    Console.WriteLine($"Saturation: {x.Saturation}");
                                                    Console.WriteLine($"Brightness: {x.Brightness}");
                                                    Console.WriteLine($"Kelvin: {x.Kelvin}");
                                                    Console.WriteLine($"Label {x.LabelStr}");
                                                    Console.WriteLine($"Power {x.Power}");
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            //OnReceivedPacket(udpResult);
                        }
                    }
                }
            }
        }

        byte[] GetServicePacket()
        {
            header = new Header();
            header.Tagged = true;
            header.Addressable = true;
            header.Target = 0;
            header.Res_required = false;
            //header.Ack_required = true;
            header.Source = 24;
            header.Sequence = 55;
            header.MessageType = MsgTypeEnum.GetService;

            byte[] packet = header.GetBytes();
            return packet;
            //Header back = Header.FromBytes(packet);
        }

        string MacStr(UInt64 macUint64)
        {
            byte[] mac = new byte[6];
            byte[] macEx = BitConverter.GetBytes(macUint64);
            Array.Copy(macEx, mac, 6);
            return string.Join(":", (from z in mac select z.ToString("X2")).ToArray());
        }

        void OnReceivedPacket(UdpReceiveResult udpResult)
        {
            ReceivedPacket?.Invoke(null, udpResult);
        }
    }
}
