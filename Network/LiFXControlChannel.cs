using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase
{
    public class LiFxControlChannel
    {

        Header header;
        //public LiFxControlChannel(byte[] macAddress, bool res_required = false)
        //{
        //    header = new Header();
        //    header.Res_required = res_required;
        //}

        public LiFxControlChannel( LiFXNetService udpService)
        {
            udpService.SendBroadCastMessage(GetServicePacket());
        }

        public void SetColor(double hue, double saturation, double lightness, double kelvin)
        {

        }

        public byte[] GetServicePacket()
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

    }
}
