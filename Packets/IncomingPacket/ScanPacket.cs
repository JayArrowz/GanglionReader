using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GanglionReader.Packets
{
    /// <summary>
    /// Packet handles adding devices to the list
    /// </summary>
    class ScanPacket : IncomingPacket
    {

        public void Execute(RawPacket packet)
        {
            String[] data = packet.GetData();
            int code = int.Parse(data[1]);
            if (MainWindow.Instance.ProgramState == ProgramState.REFRESH_STATE)
            {
                App.Current.Dispatcher.Invoke((Action)(delegate
                {
                    MainWindow.Instance.DeviceList.Clear();
                }));
                MainWindow.Log("Refresh Completed.");
                MainWindow.Instance.ProgramState = ProgramState.NOT_CONNECTED;
            }

            switch (code)
            {
                case Constants.RESP_GANGLION_FOUND:
                    App.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        MainWindow.Instance.DeviceList.Add(data[2]);
                    }));
                    break;
                case Constants.RESP_SUCCESS:
                    //Success
                    break;
            }
        }

        public string GetOpcode()
        {
            return Constants.TCP_CMD_SCAN;
        }
    }
}
