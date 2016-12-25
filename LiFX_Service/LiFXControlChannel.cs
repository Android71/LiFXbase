using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model_RT
{
    public class LiFXControlChannel : ControlChannelRT
    {
        public LiFXControlChannel(string profile)
        {
            Profile = profile;
        }

        public override void SetRGB(double h, double s, double l)
        {
            LiFX_Service lx = CSService as LiFX_Service;
            lx.SetColor(IPAddress, Mac, h, s, l);
        }

        public IPAddress IPAddress { get; set; }

        public UInt64 Mac { get; set; }

        public override string Profile
        {
            get { return CreateProfile(); }
            set { ParseProfile(value); }
        }

        void ParseProfile(string profile)
        {
            IPAddress ip;
            XElement xdata = XElement.Parse(profile);
            IPAddress.TryParse(xdata.Attribute("IP").Value, out ip);
            IPAddress = ip;
            Mac = ulong.Parse(xdata.Attribute("MAC").Value);
        }

        string CreateProfile()
        {
            return string.Format($"<Params IP = \"{IPAddress}\"  MAC = \"{Mac}\"/>"); ;
        }
    }
}
