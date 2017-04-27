using GanglionReader.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GanglionReader
{
    public class EegSocket
    {
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent ConnectWaitEvent =
            null;
        private static ManualResetEvent SendWaitEvent =
            null;
        private static ManualResetEvent RecieveWaitEvent =
            null;

        //TCP socket
        private Socket client;

        //Is searching 
        public bool initalizedSearch = false;

        //Flag to see if packets were dropped
        public bool PacketsDropped { get; set; }

        //The current data sample
        public DataPacket DataPacket { get; set; }

        //The previous sample index
        public int PreviousSampleIndex { get; set; }

        //Number of errors while sampling
        public int BleErrorCounter { get; set; }

        //Number of packets dropped, reset when PacketsDropped = false
        public int NumPacketsDropped { get; set; }

        //The TCP thread
        public Thread ConnectionThread { get; set; }

        //The current sample index
        public int CurrentDataPacketIndex { get; set; }

        //Reconnect flag on startup if NodeJS server initialization fails
        private static bool reconnect = false;

        //Data sample buffer
        public DataPacket[] DataPacketBuffer { get; set; }
        


        public EegSocket()
        {
            BleErrorCounter = 0;
            PreviousSampleIndex = 0;
            NumPacketsDropped = 0;
            CurrentDataPacketIndex = 0;
            PacketsDropped = false;
            DataPacket = new DataPacket(Constants.NCHAN_GANGLION, Constants.NUM_ACCEL_DIMS); //Constant number of channels
            DataPacketBuffer = new DataPacket[3 * (int)Constants.FS_HZ]; 
            for(int i = 0; i < DataPacketBuffer.Length; i++)
            {
                DataPacketBuffer[i] = new DataPacket(Constants.NCHAN_GANGLION, Constants.NUM_ACCEL_DIMS);
            }
            DataPacket.Initialize(); //Set values to 0
            StartConnectionThread();
        }

        /// <summary>
        /// Starts TCP connection with NodeJS Server
        /// </summary>
        void StartConnectionThread()
        {
            ConnectionThread = new Thread(new ThreadStart(() =>
            {
                //Set wait events
                ConnectWaitEvent = new ManualResetEvent(false);
                SendWaitEvent = new ManualResetEvent(false);
                RecieveWaitEvent = new ManualResetEvent(false);

                //Get IP Address
                IPHostEntry ipHostInfo = Dns.Resolve(Constants.IP);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Constants.PORT);

                // Create a TCP/IP socket.  
                client = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.NoDelay = true;

                //Attempts to reconnect continuously until connection to NodeJS server is established
                while (true)
                {
                    try
                    {
                        client.BeginConnect(remoteEP,  new AsyncCallback(ConnectCallback), client);
                        ConnectWaitEvent.WaitOne(Constants.TIMEOUT_MS_SERVER);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }

                    //Recconect if failed
                    if (reconnect)
                    {
                        reconnect = false;
                        MainWindow.Log("Error connecting to NodeJS Server.");
                        Thread.Sleep(Constants.TIMEOUT_MS_SERVER); //Wait for reconnect
                        continue;
                    }

                    //Connection established 
                    break;
                }

                //Keep recieving messages
                while (true) 
                {
                    Receive(client, this);
                    RecieveWaitEvent.WaitOne();
                    Thread.Sleep(1); //Prevent CPU overload
                }
            }));
            ConnectionThread.Name = "Connection Thread";
            ConnectionThread.Start();
        }

        /// <summary>
        /// Invokes a callback if data is available
        /// </summary>
        /// <param name="client">NodeJS Connection</param>
        /// <param name="eegSocket"></param>
        private static void Receive(Socket client, EegSocket eegSocket)
        {
            try
            {
                //If no data return
                if (client.Available <= 0) return;
                // Create the Packet object.  
                RawPacket state = new RawPacket();
                state.EegSocket = eegSocket;
                state.WorkSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.Buffer, 0, RawPacket.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                RawPacket state = (RawPacket)ar.AsyncState;
                Socket client = state.WorkSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.StrBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    //Ignore all packets without a ",;"
                    if (state.StrBuilder.ToString().Contains(",;"))
                    {
                        //Split packets with delimiter ','
                        string[] data = state.StrBuilder.ToString().Split(',');
                        IncomingPacket packet = PacketContainer.GetPacket(data[0]); //data[0] is packet id
                        if (packet != null)
                        {
                            try
                            {
                                //Process packet
                                packet.Execute(state);
                            }
                            catch (Exception e)
                            {
                                //Error processing packet
                                Console.WriteLine(e.ToString());
                            }
                        }
                        else
                        {
                            //Unrecognised packet
                            Console.WriteLine("Unrecognised opcode: " + state.StrBuilder.ToString());
                        }

                        //Search device
                        if (!state.EegSocket.initalizedSearch)
                        {
                            state.EegSocket.SearchDeviceStart();
                            state.EegSocket.initalizedSearch = true;
                        }
                        RecieveWaitEvent.Set(); //Unblock thread
                    }
                }
                else
                {
                    //Free memory if no message from NodeJS server
                    state.Buffer = null;
                    RecieveWaitEvent.Set(); //Unblock thread
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /// <summary>
        /// Callback on NodeJS Server connected
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                MainWindow.Log("Socket connected to: " + client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                ConnectWaitEvent.Set();
            }
            catch (Exception e)
            {
                //Error completeing connection
                reconnect = true;
            }
        }

        /// <summary>
        /// Send Packet to link to a BLE device
        /// </summary>
        /// <param name="id">Device Id</param>
        public void ConnectBLE(String id)
        {
            Write(Constants.TCP_CMD_CONNECT + "," + id + Constants.TCP_STOP);
            MainWindow.Instance.DeviceId = id;
        }

        /// <summary>
        /// Send packet to set device to start sampling data
        /// </summary>
        public void StartDataTransfer()
        {
            Write(Constants.TCP_CMD_COMMAND + "," + Constants.COMMAND_START_BINARY + Constants.TCP_STOP);
            MainWindow.Instance.ProgramState = ProgramState.SAMPLING;
        }

        /// <summary>
        /// Stop data transfer packet
        /// </summary>
        public void StopDataTransfer()
        {
            Write(Constants.TCP_CMD_COMMAND + "," + Constants.COMMAND_STOP + Constants.TCP_STOP);
        }

        /// <summary>
        /// Packet to disconnect BLE device.
        /// </summary>
        public void DisconnectBLE()
        {
            Write(Constants.TCP_CMD_DISCONNECT + Constants.TCP_STOP);
        }

        /// <summary>
        /// Packet to request new device list
        /// </summary>
        public void SearchDeviceStart()
        {
            Write(Constants.TCP_CMD_SCAN + ',' + Constants.TCP_ACTION_START + Constants.TCP_STOP);
        }

        // Channel setting
        //activate or deactivate an EEG channel...channel counting is zero through nchan-1
        public void ChangeChannelState(int Ichan, bool activate)
        {
            if ((Ichan >= 0))
            {
                if (activate)
                {
                    Write(Constants.TCP_CMD_COMMAND + "," + Constants.COMMAND_ACTIVATE_CHANNEL[Ichan] + Constants.TCP_STOP);
                }
                else
                {
                    Write(Constants.TCP_CMD_COMMAND + "," + Constants.COMMAND_DEACTIVATE_CHANNEL[Ichan] + Constants.TCP_STOP);
                }
            }
        }



        /// <summary>
        /// Write message to NodeJS server
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Write(String msg)
        {
            try
            {
                client.Send(Encoding.ASCII.GetBytes(msg));
                return true;
            }
            catch (Exception e)
            {
                MainWindow.Log("Error with TCP Write");
                Console.WriteLine(e.ToString());
            }
            return false;
        }
    }

}
