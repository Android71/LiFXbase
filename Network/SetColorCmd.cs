using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase
{
    public class SetColorCmd
    {
        Header header;
        public SetColorCmd(byte[] macAddress, bool res_required = false)
        {
            header = new Header();
        }


    }
}
