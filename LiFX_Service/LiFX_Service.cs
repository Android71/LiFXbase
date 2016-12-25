using LiFX_Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Model_RT
{
    public class MessageCmd
    {
        Message message;
        IPAddress ipAddress;

        public MessageCmd(IPAddress ipAddress, Message message)
        {
            this.ipAddress = ipAddress;
            this.message = message;
        }

        public IPEndPoint IPEndPoint { get { return new IPEndPoint(ipAddress, 56700); } }

        public byte[] Content { get { return message.RawMessage; } }
    }

    public class LiFX_Service
    {
        #region Fields

        UdpClient udpClient = null;
        ConcurrentQueue<MessageCmd> requestQueue = new ConcurrentQueue<MessageCmd>();

        //int lifxPort = 56700;
        //IPAddress broadCastIP;

        #endregion

        public LiFX_Service()
        {
            IPAddress hostIP = null;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (/*ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||*/ ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            if (ip.Address.ToString().StartsWith("192"))
                            {
                                hostIP = ip.Address;
                                //IPAddress mask = ip.IPv4Mask;
                                //broadCastIP = LiFXUtils.GetBroadCastIP(HostIP, mask);
                            }
            }
            //IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            udpClient = new UdpClient(new IPEndPoint(hostIP, 56700));
            Task.Run(() => ServiceTask());
        }

        public void SetColor(IPAddress ipAddress, UInt64 mac, double h, double s, double l, int transitiontime = 0)
        {
            double[] hsb = LiFXUtils.HSL2HSB(h, s, l);

            // нормирование значений
            // Hue: range 0 to 65535
            // Saturation: range 0 to 65535
            // Brightness: range 0 to 65535
            // Kelvin: range 2500° (warm) to 9000° (cool)

            UInt16 Hue = (UInt16)((hsb[0] * 65535) / 360.0);
            //Console.WriteLine($"HueUint16 {Hue}");
            UInt16 Sat = (UInt16)(hsb[1] * 65535);
            UInt16 Bri = (UInt16)(hsb[2] * 65535);
            SetColor payload = new SetColor(Hue, Sat, Bri, 2500, (ushort)transitiontime);
            Message msg = new Message();
            msg.Header.Target = mac;
            msg.Header.Tagged = false;
            msg.Header.Addressable = true;
            msg.Header.Res_required = false;
            msg.Ack_required = true;
            //msg.Header.Source = source;
            msg.Header.MessageType = MsgTypeEnum.SetColor;
            //msg.Header.Sequence = sequence;
            msg.Payload = payload;

            requestQueue.Enqueue(new MessageCmd(ipAddress, msg));

            //SendMessage(LE_EntryList[lightIx].Channel.LE_EndPoint.IPAddress, msg.RawMessage);
        }

        //udpClient.Send(messagePacket, messagePacket.Length, new IPEndPoint(ip, 56700));
        void ServiceTask()
        {
            //HttpResponseMessage response = null;
            //DateTime lastPing;
            MessageCmd messageCmd = null;
            //BridgeConnectionStatus pingStatus = BridgeConnectionStatus.Success;

            WriteLine("LiFX service started");
            //lastPing = DateTime.Now;
            //Stopwatch sw = new Stopwatch();

            while (true)
            {
                //client = new HttpClient();
                if (!requestQueue.IsEmpty)
                {
                    while (requestQueue.TryDequeue(out messageCmd))
                    {
                        udpClient.SendAsync(messageCmd.Content, messageCmd.Content.Length, messageCmd.IPEndPoint);
                    }
                }
                else

                    //WriteLine($"{(DateTime.Now - lastPing).Seconds}");
                    //TODO Отключеие Ping HueService
                    /*
                    if ((DateTime.Now - lastPing).Seconds > 10)
                    {

                        pingStatus = await PingAsync();
                        WriteLine(pingStatus);
                        System.GC.Collect();
                        lastPing = DateTime.Now;
                    }
                    */
                    Thread.Sleep(10);
            }
        }
    }
}
