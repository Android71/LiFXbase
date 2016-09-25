using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase
{
    public class LE_Entry
    {
        public LE_Entry(LiFxControlChannel channel, UInt64 macUInt64)
        {
            Channel = channel;
            MacUInt64 = macUInt64;
        }

        public LiFxControlChannel Channel { get; set; }

        public LiFX_EndPoint EndPoint { get; set; }

        public UInt64 MacUInt64 { get; set; }
    }
}
