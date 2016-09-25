using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase
{
    public class LiFxControlChannel
    {

        Header header;
        UdpClient udpClient;
        IPEndPoint bulbEP;
        

        public LiFxControlChannel( UdpClient udpClient, LiFX_EndPoint endPoint)
        {
            this.udpClient = udpClient;
            LE_EndPoint = endPoint;
            header = new Header();
            header.Addressable = true;
            header.Target = endPoint.MacAddressEx;
            bulbEP = new IPEndPoint(LE_EndPoint.IPAddress, LE_EndPoint.Port);
            //udpService.SendBroadCastMessage(GetServicePacket());
        }

        public void SetColor(double hue, double saturation, double lightness, double kelvin)
        {

        }

        public void Get()
        {
            header.MessageType = MsgTypeEnum.Get;
            Send();
        }

        void Send()
        {
            byte[] packet = header.GetBytes();
            udpClient.Send(packet, 36, bulbEP);
        }

        public LiFX_EndPoint LE_EndPoint { get; set; }

        public State State { get; set; }

        //public byte[] GetServicePacket()
        //{
        //    header = new Header();
        //    header.Tagged = true;
        //    header.Addressable = true;
        //    header.Target = 0;
        //    header.Res_required = false;
        //    //header.Ack_required = true;
        //    header.Source = 24;
        //    header.Sequence = 55;
        //    header.MessageType = MsgTypeEnum.GetService;

        //    byte[] packet = header.GetBytes();
        //    return packet;
        //    //Header back = Header.FromBytes(packet);
        //}

    }
}
