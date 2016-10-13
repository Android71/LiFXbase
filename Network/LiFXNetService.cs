using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/************ udp client timeout ****************

http://stackoverflow.com/questions/2281441/can-i-set-the-timeout-for-udpclient-in-c


var timeToWait = TimeSpan.FromSeconds(10);

var udpClient = new UdpClient( portNumber );
var asyncResult = udpClient.BeginReceive( null, null );
asyncResult.AsyncWaitHandle.WaitOne( timeToWait );
if (asyncResult.IsCompleted)
{
    try
    {
        IPEndPoint remoteEP = null;
        byte[] receivedData = udpClient.EndReceive( asyncResult, ref remoteEP );
        // EndReceive worked and we have received data and remote endpoint
    }
    catch (Exception ex)
    {
        // EndReceive failed and we ended up here
    }
} 
else
{
    // The operation wasn't completed before the timeout and we're off the hook
}
*/

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
            //var udpClient = new UdpClient(new IPEndPoint(ip, hostPort));
            udpClient.Send(messagePacket, messagePacket.Length, new IPEndPoint(ip, 56700));
        }

        public void SetColor(double hue, double saturation, double lightness, int lightIx, uint source)
        {
            double[] hsb = Utils.HSL2HSB(hue, saturation, lightness);
            // нормирование значений
            // Hue: range 0 to 65535
            // Saturation: range 0 to 65535
            // Brightness: range 0 to 65535
            // Kelvin: range 2500° (warm) to 9000° (cool)
            UInt16 Hue = (UInt16)((hsb[0] * 65535)/360.0);
            //Console.WriteLine($"HueUint16 {Hue}");
            UInt16 Sat = (UInt16)(hsb[1] * 65535);
            UInt16 Bri = (UInt16)(hsb[2] * 65535);
            SetColor payload = new SetColor(Hue, Sat, Bri, 2500, 0);
            Message msg = new Message();
            msg.Header.Target = LE_EntryList[lightIx].MacUInt64;
            msg.Header.Tagged = false;
            msg.Header.Addressable = true;
            msg.Header.Res_required = false;
            msg.Header.Source = source;
            msg.Header.MessageType = MsgTypeEnum.SetColor;
            msg.Payload = payload;
            SendMessage(LE_EntryList[lightIx].Channel.LE_EndPoint.IPAddress, msg.RawMessage);

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
