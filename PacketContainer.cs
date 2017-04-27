using GanglionReader.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader
{
    /// <summary>
    /// Handles retrival and storage of packets
    /// </summary>
    class PacketContainer
    {
        /// <summary>
        /// Stores known packets
        /// </summary>
        public static List<IncomingPacket> PacketList = new List<IncomingPacket>();
        

        /// <summary>
        /// Get Packet by opCode
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        public static IncomingPacket GetPacket(String opCode)
        {
            foreach(IncomingPacket packet in PacketList)
            {
                if(packet.GetOpcode().Equals(opCode))
                {
                    return packet;
                }
            }
            return null;
        }

        /// <summary>
        /// Start Packet Container
        /// </summary>
        public static void Initialize()
        {
            PacketList.Add(new StatusPacket());
            PacketList.Add(new ScanPacket());
            PacketList.Add(new RetriveDataPacket());
            PacketList.Add(new ConnectedCompletePacket());
            PacketList.Add(new LogPacket());
            PacketList.Add(new CommandStatus());
        }       

    }
}
