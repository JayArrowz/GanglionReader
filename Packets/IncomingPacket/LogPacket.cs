using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader.Packets
{
    public class LogPacket : IncomingPacket
    {
        public void Execute(RawPacket packet)
        {
            string[] data = packet.GetData();

            string message = data[2];
           
            MainWindow.Log(message);
        }

        public string GetOpcode()
        {
            return "l";
        }

    }
}
