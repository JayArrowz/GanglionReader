using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader
{
    // RawPacket object for receiving data from remote device.  
    public class RawPacket
    {
        // Client socket.  
        public Socket WorkSocket = null;
        
        //Main Socket
        public EegSocket EegSocket = null;

        // Size of receive buffer.  
        public const int BufferSize = 512;

        // Receive buffer.  
        public byte[] Buffer = new byte[BufferSize];

        // Received data string.  
        public StringBuilder StrBuilder = new StringBuilder();

        /// <summary>
        /// Split packets with ','
        /// </summary>
        /// <returns>Packet Data</returns>
        public String[] GetData()
        {
            return StrBuilder.ToString().Split(',');
        }
    }
}
