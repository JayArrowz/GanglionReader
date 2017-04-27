using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader.Packets
{
    /// <summary>
    /// Status Information of BLE connection state and Sampling state
    /// </summary>
    class ConnectedCompletePacket : IncomingPacket
    {
        public void Execute(RawPacket packet)
        {
            string[] data = packet.GetData();

            int code = int.Parse(data[1]);
            if(code == Constants.RESP_SUCCESS)
            {
                if (MainWindow.Instance.ProgramState == ProgramState.SAMPLING)
                {
                    packet.EegSocket.StartDataTransfer();
                }
                else
                {
                    MainWindow.Instance.ProgramState = ProgramState.CONNECTED;
                }
                MainWindow.Log("Connected to BLE device successfully");
            } else
            {
                MainWindow.Log("Error connecting to BLE device");
            }
        }

        public string GetOpcode()
        {
            return "c";
        }
    }
}
