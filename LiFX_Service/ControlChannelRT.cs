using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_RT
{
    public class ControlChannelRT
    {
        //public static ControlChannelRT Create(ControlChannel channel)
        //{
        //    ControlChannelRT ccRT = (ControlChannelRT)Activator.CreateInstance(Type.GetType(channel.DotNetType));
        //    ccRT.Id = channel.Id;
        //    ccRT.Name = channel.Name;
        //    ccRT.CSChannel_Id = channel.CSChannel_Id;
        //    ccRT.PointType = channel.PointType;
        //    ccRT.Profile = channel.Profile;
        //    ccRT.DotNetType = channel.DotNetType;
        //    ccRT.HaveTransition = channel.HaveTransition;
        //    ccRT.DefaultTransitionTime = channel.DefaultTransitionTime;
        //    ccRT.CanChangeTransition = channel.CanChangeTransition;
        //    ccRT.ThrottleTime = channel.ThrottleTime;
        //    ccRT.ControlSpace_Id = channel.ControlSpace_Id;
        //    ccRT.ControlDevice_Id = channel.Device_Id;
        //    return ccRT;
        //}

        public int Id { get; set; }
        public virtual string Name { get; set; }
        public bool IsFake { get; set; }
        public string CSChannel_Id { get; set; }
        //public GlobalEnums.PointTypeEnum PointType { get; set; }
        public bool HaveTransition { get; set; }
        public int DefaultTransitionTime { get; set; }
        public bool CanChangeTransition { get; set; }
        public int ThrottleTime { get; set; }
        public virtual string Profile { get; set; }
        public string DotNetType { get; set; }
        public int ControlSpace_Id { get; set; }

        //public ControlSpace ControlsSpace
        //{
        //    get { return RTDB.ControlSpaces[ControlSpace_Id]; }
        //}

        public int ControlDevice_Id { get; set; }
        //public DeviceRT Device { get { return RTDB.RTDevices[ControlDevice_Id]; } }
        public int LinkCount { get; set; }
        //public object CSService { get { return Device.CSService; } }

        public object CSService { get; set; }

        //#region Set from LightPoint

        //public void SetWhite(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{

        //}

        //public void SetWhiteAN(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{

        //}

        //public void SetWT(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{

        //}

        //public void SetWT_AN(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{

        //}

        //public void SetCW(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //public void SetCW_AN(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //public void SetRGB(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //    SetRGB(lp.H, lp.S, lp.L);
        //}

        //public void SetRGB_AN(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //    lp.SequenceChanger = sequenceChanger;
        //    SetRGB(lp.StartDMX_Ix, lp.RGB);
        //}

        //public void SetRGBW(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //public void SetRGBW_AN(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //public void SetRGBWT(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //public void SetRGBWT_AN(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //public void SetRGBCW(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //public void SetRGBCW_AN(LightPoint lp, Func<byte[], byte[]> sequenceChanger)
        //{
        //}

        //#endregion

        public virtual void SetWhite(double white) { }

        public virtual void SetWT(double white, double kelvin) { }

        public virtual void SetCW(double cold, double warm) { }

        public virtual void SetRGB(double h, double s, double l) { }

        public virtual void SetRGB(int dmx, byte[] rgb) { }

        public virtual void SetRGBW(int dmx, byte[] rgbw) { }

        public virtual void SetRGBW(double h, double s, double l, double white) { }

        public virtual void SetRGBWT(double h, double s, double l, double white, double kelvin) { }



        public virtual object InitService(ControlChannelRT channel)
        {
            return null;
        }
    }

}
