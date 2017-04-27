using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader.Packets
{
    /// <summary>
    /// ?
    /// </summary>
    class StatusPacket : IncomingPacket
    {
        public void Execute(RawPacket packet)
        {

        }

        public string GetOpcode()
        {
            return "q";
        }
    }
}
