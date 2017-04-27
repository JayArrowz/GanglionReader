using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader.Packets
{
    class CommandStatus : IncomingPacket
    {
        public void Execute(RawPacket packet)
        {
            string[] data = packet.GetData();

            int status = int.Parse(data[1]);
            if(status == 200)
            {
                if(MainWindow.Instance.ProgramState == ProgramState.EXITING)
                {
                    Environment.Exit(0);
                }
            }

        }

        public string GetOpcode()
        {
            return "k";
        }
    }
}
