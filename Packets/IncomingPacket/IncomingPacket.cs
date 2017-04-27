using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader.Packets
{
    /// <summary>
    /// IncomingPacket Interface defines a packet recieved from a NodeJS server.
    /// </summary>
    interface IncomingPacket
    {
        /// <summary>
        /// OpCode of packet
        /// </summary>
        /// <returns></returns>
        string GetOpcode();

        /// <summary>
        /// Execution once GetOpcode() is equal to the Packet OpCode recieved from the NodeJS Server
        /// </summary>
        /// <param name="packet"></param>
        void Execute(RawPacket packet);

    }
}
