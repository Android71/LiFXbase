using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiFX_Lib
{
    [Flags]
    public enum ProtocolEnum : UInt16
    {
        Tagged = 8192,
        Addressable = 4096,
        Protocol = 1024
    }

    public enum ReservEnum : Byte
    {
        Res_required = 1,
        Ack_required = 2
    }

    public class Header
    {
        public Header()
        {
            Array.Clear(Reserv1, 0, Reserv1.Length);
           
        }
        /*---------------------------------------------------------------------------*/

        #region Frame

        // Protocol Origin Tagged and Addressable share 2 byte
        //--------------------------------------------------------------------------------------------------------
        //| 15 | 14 | 13 | 12 | 11 | 10 | 9 | 8 | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        //--------------------------------------------------------------------------------------------------------
        //             |    |    |____________________________________________|
        //             |    |                         |----------------------------> Protocol (Always 1024)
        //  |    |     |    |------------------------------------------------------> Addressable
        //  |____|     |-----------------------------------------------------------> Tagged
        //     |-------------------------------------------------------------------> Origin ( Always 0 )
        //

        /// <summary>
        /// uint16_t Size of entire message in bytes including this field
        /// </summary>
        public UInt16 Size { get; set; } = 36;

        
        /// <summary>
        /// uint16_t Shared BitField Origin, Tagged, Addressable and Protocol must be 0x400 (1024 decimal)
        /// </summary>
        public ProtocolEnum Protocol { get; set; } = ProtocolEnum.Protocol;

        /// <summary>
        /// Determines usage of Frame Address target field
        /// Indicates whether the Frame Address Target field is being used to address an individual device or all devices.
        /// For device discovery using GetService message, 
        /// the Tagged field should be set to one (true) and the Target should be all zeroes. 
        /// In all other messages, the Tagged field should be set to zero (false) and the Target field should contain the device MAC address. 
        /// </summary>
        public bool Tagged
        {
            get { return (Protocol & ProtocolEnum.Tagged) == ProtocolEnum.Tagged; }
            set
            {
                if (value)
                    Protocol |= ProtocolEnum.Tagged;
                else
                    Protocol &= ~ProtocolEnum.Tagged;
            }
        }

        /// <summary>
        /// Message includes a target address: must be one(1)
        /// </summary>
        public bool Addressable
        {
            get { return (Protocol & ProtocolEnum.Addressable) == ProtocolEnum.Addressable; }
            set
            {
                if (value)
                    Protocol |= ProtocolEnum.Addressable;
                else
                    Protocol &= ~ProtocolEnum.Addressable;
            }
        }

        

        /// <summary>
        /// uint32_t Source identifier: unique value set by the client, used by responses
        /// If the source identifier is a zero value, 
        /// then the LIFX device may send a broadcast message that can be received by all clients on the same sub-net
        /// </summary>
        public UInt32 Source { get; set; }

        #endregion

        /*---------------------------------------------------------------------------*/

        #region Frame Address

        //target	64	uint64_t	6 byte device address(MAC address) or zero(0) means all devices
        //reserved	48	uint8_t[6] Must all be zero(0)
        //reserved	6		Reserved
        //ack_required	1	bool Acknowledgement message required
        //res_required	1	bool Response message required
        //sequence	8	uint8_t Wrap around message sequence number

        /// <summary>
        /// uint64_t	6 byte device address(MAC address) or zero(0) means all devices
        /// </summary>
        public UInt64 Target { get; set; } = 0;


        /// <summary>
        /// 48 bit	uint8_t[6] Must all be zero(0)
        /// </summary>
        public byte[] Reserv1 { get; set; } = new byte[6];

        public ReservEnum Reserv2 { get; set; } = 0;

        /// <summary>
        /// Acknowledgement message required
        /// </summary>
        public bool Ack_required
        {
            get { return (Reserv2 & ReservEnum.Ack_required) == ReservEnum.Ack_required; }
            set
            {
                if (value)
                    Reserv2 |= ReservEnum.Ack_required;
                else
                    Reserv2 &= ~ReservEnum.Ack_required;
            }
        }

        /// <summary>
        /// Response message required
        /// </summary>
        public bool Res_required
        {
            get { return (Reserv2 & ReservEnum.Res_required) == ReservEnum.Res_required; }
            set
            {
                if (value)
                    Reserv2 |= ReservEnum.Res_required;
                else
                    Reserv2 &= ~ReservEnum.Res_required;
            }
        }

        /// <summary>
        /// uint8_t Wrap around message sequence number
        /// </summary>
        public byte Sequence { get; set; }

        #endregion

        /*---------------------------------------------------------------------------*/

        #region Protocol Header

        //reerved	64	uint64_t Reserved
        //type	16	uint16_t Message type determines the payload being used
        //reserved	16		Reserved

        public UInt64 Reserv3 { get; set; }

        /// <summary>
        /// uint16_t Message type determines the payload being used
        /// </summary>
        public UInt16 MsgType { get; set; }

        public MsgTypeEnum MessageType
        {
            get { return (MsgTypeEnum)MsgType; }
            set { MsgType = (UInt16)value; }
        }

        public UInt16 Reserv4 { get; set; }

        #endregion

        public byte[] GetBytes()
        {
            List<byte> bl = new List<byte>();
            
            bl.AddRange(BitConverter.GetBytes(Size));
            bl.AddRange(BitConverter.GetBytes((UInt16)Protocol));
            bl.AddRange(BitConverter.GetBytes(Source));
            bl.AddRange(BitConverter.GetBytes(Target));
            bl.AddRange(Reserv1);
            bl.Add((byte)Reserv2);
            bl.Add(Sequence);
            bl.AddRange(BitConverter.GetBytes(Reserv3));
            bl.AddRange(BitConverter.GetBytes(MsgType));
            bl.AddRange(BitConverter.GetBytes(Reserv4));

            return bl.ToArray();
        }

        public static Header FromBytes(byte[] payload)
        {
            Header header = new Header();
            header.Size = (UInt16)BitConverter.ToInt16(payload, 0);             // 0 - 1
            header.Protocol = (ProtocolEnum)BitConverter.ToInt16(payload, 2);   // 2 - 3
            header.Source = (UInt32)BitConverter.ToInt32(payload, 4);           // 4 - 7
            header.Target = (UInt64)BitConverter.ToInt64(payload, 8);           // 8 - 15
            //Reserv1 6 bytes                                                   // 16 - 21
            header.Reserv2 = (ReservEnum)payload[22];                           // 22
            header.Sequence = payload[23];                                      // 23
            //Reserv3 8 bytes                                                   // 24 - 31
            header.MsgType = (UInt16)BitConverter.ToInt16(payload, 32);         // 32 - 33
            //Reserv4 2 bytes                                                   // 34 - 35
            return header;
        }
    }

    public class Message
    {
        public Message()
        {
            Header = new Header();
        }

        public Header Header { get; set; }

        public PayloadPart Payload { get; set; }

        public byte[] RawMessage
        {
            get
            {
                byte[] tmp = Payload.GetBytes();
                Header.Size = (ushort)(36 + tmp.Length);
                List<byte> bl = new List<byte>();
                bl.AddRange(Header.GetBytes());
                bl.AddRange(tmp);

                return bl.ToArray();
            }
        }

        public bool Tagged
        {
            get { return Header.Tagged; }
            set { Header.Tagged = value; }
        }

        public bool Adressable
        {
            get { return Header.Addressable; }
            set { Header.Addressable = value; }
        }

        public UInt32 Source
        {
            get { return Header.Source; }
            set { Header.Source = value; }
        }

        public UInt64 Target
        {
            get { return Header.Target; }
            set { Header.Target = value; }
        }

        public bool Ack_required
        {
            get { return Header.Ack_required; }
            set { Header.Ack_required = value; }
        }

        public bool Res_required
        {
            get { return Header.Res_required; }
            set { Header.Res_required = value; }
        }

        MsgTypeEnum MessageType
        {
            get { return (MsgTypeEnum)Header.MsgType; }
            set { Header.MsgType = (byte)value; }
        }

    }
}
