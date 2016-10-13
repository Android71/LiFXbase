using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//https://github.com/LIFX/lifx-protocol-docs/blob/master/messages/device.md#enumerated-types

namespace LiFXbase
{
    public enum MsgTypeEnum : UInt16
    {
        GetService = 2,
            StateService = 3,
        GetHostInfo = 12,
            StateHostInfo = 13,
        GetHostFirmware = 14,
            StateHostFirmware = 15,
        GetWifiInfo = 16,
            StateWifiInfo = 17,
        GetWifiFirmware = 18,
            StateWifiFirmware = 19,

        GetPower = 20,
        // Set device power level.
        // Zero implies standby and non-zero sets a corresponding power draw level. 
        // Currently only 0 and 65535 are supported (PowerOn/PowerOff).
        // payload - unsigned 16-bit integer
        SetPower = 21,
            StatePower = 22,

        GetLabel = 23,
        //Set the device label text.
        // payload - string, size: 32 bytes
        // string is a fixed-length field, which is not null-terminated 
        SetLabel = 24,
            StateLabel = 25,

        GetVersion = 32,
            StateVersion = 33,
        GetInfo = 34,
            StateInfo = 35,
        Acknowledgement = 45,
        GetLocation = 48,
            StateLocation = 50,
        GetGroup = 51,
            StateGroup = 53,
        EchoRequest = 58,
            EchoResponse = 59,

        Get = 101,
        SetColor = 102,
            State = 107,

        GetBulbPower = 116,
        SetBulbPower = 117,
            StateBulbPower = 118

    }

    public abstract class PayloadPart
    {
        public virtual byte[] GetBytes() { return null; }
        //public virtual PayloadPart FromBytes(byte[] payload) { return null; }
    }

    #region Device request payloads

    public class SetPower : PayloadPart
    {
        public SetPower(short level)
        {
            Level = level;
        }
        /// <summary>
        /// unsigned 16-bit integer
        /// Set device power level
        /// Zero implies standby and non-zero sets a corresponding power draw level. 
        /// Currently only 0 and 65535 are supported.
        /// </summary>
        public short Level { get; set; }

        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(Level);
        }

        public static SetPower FromBytes(byte[] payload)
        {
            return new SetPower(BitConverter.ToInt16(payload, 0));
        }
    }

    public class SetLabel : PayloadPart
    {
        public SetLabel(string label)
        {
            Label = new byte[32];
            for (int i = 0; i < 32; i++)
                Label[i] = 0x32;    // Space
            byte[] tmp = Encoding.UTF8.GetBytes(label);
            Array.Copy(tmp, Label, tmp.Length);
        }

        public SetLabel(byte[] label)
        {
            Label = new byte[32];
            Array.Copy(label, Label, 32);
        }

        /// <summary>
        /// string, size: 32 bytes
        /// string is a fixed-length field, which is not null-terminated 
        /// Set device label
        /// </summary>
        public byte[] Label { get; set; }

        public string LabelStr
        { get { return Encoding.UTF8.GetString(Label); } }

        public override byte[] GetBytes()
        {
            return Label;
        }

        public static SetLabel FromBytes(byte[] payload)
        {
            return new SetLabel(payload);
        }
    }

    #endregion

    #region Device response payloads

    public class StateService : PayloadPart
    {
        public StateService(byte service, uint port)
        {
            Service = service;
            Port = port;
        }
        // Always 1 - UDP
        public byte Service { get; set; }

        /// <summary>
        /// unsigned 32-bit integer
        /// If the Service is temporarily unavailable, then the port value will be 0.
        /// </summary>
        public uint Port { get; set; }

        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StateService(payload[0], (uint)BitConverter.ToInt32(payload, 1));
        //}
    }

    public class StateHostInfo : PayloadPart
    {
        public StateHostInfo (float signal, uint tx, uint rx)
        {
            Signal = signal;
            Tx = tx;
            Rx = rx;
        }
        /// <summary>
        /// 32-bit float
        /// radio receive signal strength in milliWatts
        /// </summary>
        public float Signal { get; set; }

        /// <summary>
        /// unsigned 32-bit integer
        /// bytes transmitted since power on
        /// </summary>
        public uint Tx { get; set; }

        /// <summary>
        /// unsigned 32-bit integer
        /// bytes received since power on
        /// </summary>
        public uint Rx { get; set; }

        // signed 16-bit integer
        public short Reserv { get; set; }

        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StateHostInfo(BitConverter.ToSingle(payload, 0), 
        //                            (uint)BitConverter.ToInt32(payload, 4),
        //                            (uint)BitConverter.ToInt32(payload, 8));
        //}
    }

    public class StateHostFirmware : PayloadPart
    {
        public StateHostFirmware(ulong buildTime, uint version)
        {
            BuildTime = buildTime;
            Version = version;
        }
        /// <summary>
        /// unsigned 64-bit integer
        /// firmware build time (absolute time in nanoseconds since epoch)
        /// </summary>
        public ulong BuildTime { get; set; }

        //unsigned 64-bit integer
        public ulong Reserv { get; set; }

        /// <summary>
        /// unsigned 32-bit integer
        /// firmware version
        /// </summary>
        public uint Version { get; set; }
        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StateHostFirmware((ulong)BitConverter.ToInt32(payload, 0),
        //                                 (uint)BitConverter.ToInt32(payload, 15));
        //}
    }

    public class StateWifiInfo : StateHostInfo
    {
        public StateWifiInfo(float signal, uint tx, uint rx) : base(signal, tx, rx) { }
        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StateWifiInfo(BitConverter.ToSingle(payload, 0),
        //                            (uint)BitConverter.ToInt32(payload, 4),
        //                            (uint)BitConverter.ToInt32(payload, 8));
        //}
    }

    public class StateWifiFirmware : StateHostFirmware
    {
        public StateWifiFirmware(ulong buildTime, uint version) : base(buildTime, version) { }

        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StateWifiFirmware((ulong)BitConverter.ToInt32(payload, 0),
        //                                 (uint)BitConverter.ToInt32(payload, 15));
        //}
    }

    public class StatePower : PayloadPart
    {
        public StatePower(short level)
        {
            Level = level;
        }
        /// <summary>
        /// unsigned 16-bit integer
        /// Zero implies standby and non-zero sets a corresponding power draw level. 
        /// Currently only 0 and 65535 are supported.
        /// </summary>
        public short Level { get; set; }
        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StatePower(BitConverter.ToInt16(payload, 0));
                                   
        //}
    }

    public class StateLabel : PayloadPart
    {
        public StateLabel(byte[] label)
        {
            Label = new byte[32];
            Array.Copy(label, Label, 32);
        }
        /// <summary>
        /// string, size: 32 bytes
        /// Provides device label
        /// </summary>
        public byte[] Label { get; set; }

        public string LabelStr
        { get { return Encoding.UTF8.GetString(Label); } }

        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new SetLabel(payload);
        //}
    }

    public class StateVersion : PayloadPart
    {
        public StateVersion(uint vendor, uint product, uint version)
        {
            Vendor = vendor;
            Product = product;
            Version = version;
        }
        /// <summary>
        /// vendor ID
        /// </summary>
        public uint Vendor { get; set; }

        /// <summary>
        /// product ID
        /// </summary>
        public uint Product { get; set; }

        /// <summary>
        /// hardware version
        /// </summary>
        public uint Version { get; set; }

        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StateVersion((uint)BitConverter.ToInt32(payload, 0),
        //                            (uint)BitConverter.ToInt32(payload, 4),
        //                            (uint)BitConverter.ToInt32(payload, 8));
        //}
    }


    public class StateInfo : PayloadPart
    {
        public StateInfo(ulong timeEpoch, ulong upTime, ulong downTime)
        {
            TimeEpoch = timeEpoch;
            UpTime = upTime;
            DownTime = downTime;
        }
        /// <summary>
        /// current time(absolute time in nanoseconds since epoch)
        /// </summary>
        public ulong TimeEpoch { get; set; }

        /// <summary>
        /// time since last power on(relative time in nanoseconds)
        /// </summary>
        public ulong UpTime { get; set; }

        /// <summary>
        /// last power off period, 5 second accuracy(in nanoseconds)
        /// </summary>
        public ulong DownTime { get; set; }

        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    return new StateInfo((ulong)BitConverter.ToInt64(payload, 0),
        //                        (ulong)BitConverter.ToInt64(payload, 8),
        //                        (ulong)BitConverter.ToInt64(payload, 15));
        //}
    }

    public class StateLocation : PayloadPart
    {
        public StateLocation(byte[] location, byte[] label, ulong updateAt)
        {
            Location = new byte[16];
            Array.Copy(location, Location, 16);
            Label = new byte[32];
            Array.Copy(label, Label, 32);
            UpdatedAt = updateAt;
        }
        //byte array, size: 16 
        public byte[] Location { get; set; }

        //label string, size: 32 
        public byte[] Label { get; set; }

        /// <summary>
        /// unsigned 64-bit integer
        /// last updated time in nanoseconds since epoch
        /// </summary>
        public ulong UpdatedAt { get; set; }
        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    byte[] location = new byte[16];
        //    Array.Copy(payload, 0, location, 0, 16);
        //    byte[] label = new byte[32];
        //    Array.Copy(payload, 16, label, 0, 32);
        //    return new StateLocation(location,
        //                             label,
        //                             (ulong)BitConverter.ToInt64(payload, 48));
        //}
    }

    public class StateGroup : PayloadPart
    {
        public StateGroup(byte[] group, byte[] label, ulong updateAt)
        {
            Group = group;
            Label = label;
            UpdatedAt = updateAt;
        }
        //byte array, size: 16 
        public byte[] Group { get; set; }

        //label string, size: 32 
        public byte[] Label { get; set; }

        /// <summary>
        /// unsigned 64-bit integer
        /// last updated time in nanoseconds since epoch
        /// </summary>
        public ulong UpdatedAt { get; set; }

        //public override PayloadPart FromBytes(byte[] payload)
        //{
        //    byte[] group = new byte[16];
        //    Array.Copy(payload, 0, group, 0, 16);
        //    byte[] label = new byte[32];
        //    Array.Copy(payload, 16, label, 0, 32);
        //    return new StateGroup(group,
        //                             label,
        //                             (ulong)BitConverter.ToInt64(payload, 48));
        //}
    }

    public class EchoResponse
    {
        //byte array, size: 64 bytes
        public byte[] Payload { get; set; }
    }

    #endregion

    #region Bulb request payloads

    /// <summary>
    /// The color is represented as an HSB(Hue, Saturation, Brightness) value.
    /// The color temperature is represented in K(Kelvin) and is used to adjust the warmness / coolness of a white light,
    /// which is most obvious when saturation is close zero.
    /// Hue: range 0 to 65535
    /// Saturation: range 0 to 65535
    /// Brightness: range 0 to 65535
    /// Kelvin: range 2500° (warm) to 9000° (cool)
    /// </summary>
    public class SetColor : PayloadPart
    {
        public SetColor(UInt16 hue, UInt16 saturation, UInt16 brightness, UInt16 kelvin, UInt16 duration)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
            Kelvin = kelvin;
            Duration = duration;
        }
        public byte Reserv { get; set; }

        public UInt16 Hue { get; set; }

        public UInt16 Saturation { get; set; }

        public UInt16 Brightness { get; set; }

        public UInt16 Kelvin { get; set; }

        /// <summary>
        /// The color transition time in milliseconds
        /// </summary>
        public uint Duration { get; set; }

        public override byte[] GetBytes()
        {
            List<byte> bl = new List<byte>();
            bl.Add(Reserv);
            bl.AddRange(BitConverter.GetBytes(Hue));
            bl.AddRange(BitConverter.GetBytes(Saturation));
            bl.AddRange(BitConverter.GetBytes(Brightness));
            bl.AddRange(BitConverter.GetBytes(Kelvin));
            return bl.ToArray();
        }
    }

    public class SetBulbPower : PayloadPart
    {
        public SetBulbPower(short level, uint duration)
        {
            Level = level;
            Duration = duration;
        }
        /// <summary>
        /// unsigned 16-bit integer
        /// Set bulb power level
        /// Zero implies standby and non-zero sets a corresponding power draw level. 
        /// Currently only 0 and 65535 are supported.
        /// </summary>
        public short Level { get; set; }

        /// <summary>
        /// The power level  transition time in milliseconds
        /// </summary>
        public uint Duration { get; set; }

        public override byte[] GetBytes()
        {
            List<byte> bl = new List<byte>();
            bl.AddRange(BitConverter.GetBytes(Level));
            bl.AddRange(BitConverter.GetBytes(Duration));
            return bl.ToArray();
        }
    }

    #endregion

    #region Bulb response payloads

    /// <summary>
    /// Sent by a device to provide the current light state.
    /// </summary>
    public class State : PayloadPart
    {
        public State(ushort hue, ushort saturation, ushort brightness, ushort kelvin, ushort power, byte[] label)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
            Kelvin = kelvin;
            Power = power;
            Label = label;
        }
        //Sent by a device to provide the current light state.
        //color HSBK
        //reserved signed 16-bit integer
        //power unsigned 16-bit integer
        //label string, size: 32 bytes
        //reserved unsigned 64-bit integer
        

        public UInt16 Hue { get; set; }

        public UInt16 Saturation { get; set; }

        public UInt16 Brightness { get; set; }

        public UInt16 Kelvin { get; set; }

        public Int16 Reserv1 { get; set; }

        public UInt16 Power { get; set; }

        //label string, size: 32 bytes
        public byte[] Label { get; set; }

        public string LabelStr
        {
            get { return Encoding.UTF8.GetString(Label, 0, 32); }
        }

        //string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

        public UInt64 Reserv2 { get; set; }

        public static State FromBytes(byte[] payload)
        {
            byte[] label = new byte[32];
            Array.Copy(payload, 12, label, 0, 32);
            return new State((UInt16)BitConverter.ToInt16(payload, 0),
                             (UInt16)BitConverter.ToInt16(payload, 2),
                             (UInt16)BitConverter.ToInt16(payload, 4),
                             (UInt16)BitConverter.ToInt16(payload, 6),
                             (UInt16)BitConverter.ToInt16(payload, 10),
                            label
                            );
        }

    }

    public class StateBulbPower
    {
        public StateBulbPower(UInt16 level)
        {
            Level = level;
        }
        public UInt16 Level { get; set; }

        public static StateBulbPower FromBytes(byte[] payload)
        {
            return new StateBulbPower((ushort)BitConverter.ToInt16(payload, 0));
                             
                            
        }

    }

    #endregion

}
