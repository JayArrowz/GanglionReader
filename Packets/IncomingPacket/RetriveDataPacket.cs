using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader.Packets
{
    class RetriveDataPacket : IncomingPacket
    {
        public void Execute(RawPacket packet)
        {
            string[] data = packet.GetData();
            Console.WriteLine("Data Packet recieved: " + packet.StrBuilder.ToString());
            int code = int.Parse(data[1]);
            EegSocket socket = packet.EegSocket;

            DataPacket dataPacket = socket.DataPacket;
            if(code == Constants.RESP_SUCCESS_DATA_SAMPLE)
            {
                dataPacket.SampleIndex = int.Parse(data[2]);
                if ((dataPacket.SampleIndex - socket.PreviousSampleIndex) != 1)
                {
                    if (dataPacket.SampleIndex != 0)
                    { // if we rolled over, don't count as error
                        socket.BleErrorCounter++;
                        socket.PacketsDropped = true;
                        if(dataPacket.SampleIndex < socket.PreviousSampleIndex)
                        {
                            socket.NumPacketsDropped = (dataPacket.SampleIndex + 200) - socket.PreviousSampleIndex;
                        } else
                        {
                            socket.NumPacketsDropped = dataPacket.SampleIndex - socket.PreviousSampleIndex;
                        }
                    }
                }

                socket.PreviousSampleIndex = dataPacket.SampleIndex;
                for(int i = 0; i < Constants.NCHAN_GANGLION; i++)
                {
                    dataPacket.Values[i] = int.Parse(data[3 + i]);
                }

                GetRawValues(dataPacket);
                socket.CurrentDataPacketIndex = (socket.CurrentDataPacketIndex + 1) % socket.DataPacketBuffer.Length;
                CopyDataPacketTo(socket.DataPacket, socket.DataPacketBuffer[socket.CurrentDataPacketIndex]);

                if (socket.PacketsDropped)
                {
                    for(int i = socket.NumPacketsDropped; i > 0; i--)
                    {
                        int tempDataPacketInd = socket.CurrentDataPacketIndex - i;
                        if(tempDataPacketInd >= 0 && tempDataPacketInd < socket.DataPacketBuffer.Length)
                        {
                            CopyDataPacketTo(socket.DataPacket, socket.DataPacketBuffer[tempDataPacketInd]);
                        } else
                        {
                            CopyDataPacketTo(socket.DataPacket, socket.DataPacketBuffer[tempDataPacketInd + 200]);
                        }
                    }
                    socket.PacketsDropped = false;
                    socket.NumPacketsDropped = 0;
                }

                //socket.DataPacket.WriteDataPacket();
            } else
            {
                socket.BleErrorCounter++;
            }


        }

        public int CopyDataPacketTo(DataPacket packet, DataPacket target)
        {
            return packet.CopyTo(target);
        }
        private void GetRawValues(DataPacket packet)
        {
            for (int i = 0; i < Constants.NCHAN_GANGLION; i++)
            {
                int val = packet.Values[i];
                //println(binary(val, 24));
                byte[] rawValue = new byte[3];
                // Breakdown values into
                rawValue[2] = (byte) (val & 0xFF);
                //println("rawValue[2] " + binary(rawValue[2], 8));
                rawValue[1] = (byte)((val & (0xFF << 8)) >> 8);
                //println("rawValue[1] " + binary(rawValue[1], 8));
                rawValue[0] = (byte)((val & (0xFF << 16)) >> 16);
                //println("rawValue[0] " + binary(rawValue[0], 8));
                // Store to the target raw values
                packet.RawValues[i] = rawValue;
                //println();
            }
        }
        public string GetOpcode()
        {
            return "t";
        }
    }
}
