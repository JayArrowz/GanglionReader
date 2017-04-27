using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanglionReader
{
    class Constants
    {
        #region Program Constants
        public const String COMMAND_STOP = "s";
        public const String COMMAND_START_BINARY = "b";
        public const String COMMAND_START_BINARY_WAUX = "n"; // already doing this with 'b' now

        public static String[] COMMAND_DEACTIVATE_CHANNEL = { "1", "2", "3", "4", "5", "6", "7", "8", "q", "w", "e", "r", "t", "y", "u", "i" };
        public static String[] COMMAND_ACTIVATE_CHANNEL = { "!", "@", "#", "$", "%", "^", "&", "*", "Q", "W", "E", "R", "T", "Y", "U", "I" };
        public static string USER_HOME_DIR = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
        ? Environment.GetEnvironmentVariable("HOME")
        : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

        /// <summary>
        /// Path to write data
        /// </summary>
        public static string DATA_DIR = USER_HOME_DIR+Path.PathSeparator+"GanglionDataDir";

        /// <summary>
        /// NodeJS server DIR
        /// </summary>
        public const string NODE_JS_SERVER_DIR = "./Data/";
        #endregion


        #region Network Configuration
        public const int TIMEOUT_MS_SERVER = 1000;
        /// IP
        public const String IP = "127.0.0.1";
        //Port
        public const int PORT = 10996;
        #endregion

        #region TCP Commands
        /// <summary>
        /// Ganglion Commands
        /// </summary>
        public const String TCP_CMD_ACCEL = "a";
        public const String TCP_CMD_CONNECT = "c";
        public const String TCP_CMD_COMMAND = "k";
        public const String TCP_CMD_DISCONNECT = "d";
        public const String TCP_CMD_DATA = "t";
        public const String TCP_CMD_ERROR = "e"; //<>//
        public const String TCP_CMD_IMPEDANCE = "i";
        public const String TCP_CMD_LOG = "l";
        public const String TCP_CMD_SCAN = "s";
        public const String TCP_CMD_STATUS = "q";
        public const String TCP_STOP = ",;\n";
        public const String TCP_ACTION_START = "start";
        public const String TCP_ACTION_STATUS = "status";
        public const String TCP_ACTION_STOP = "stop";
        public const String GANGLION_BOOTLOADER_MODE = ">";
        #endregion

        public const byte BYTE_START = (byte)0xA0;
        public const byte BYTE_END = (byte)0xC0;


        #region States for setting hardware
        public const int STATE_NOCOM = 0;
        public const int STATE_COMINIT = 1;
        public const int STATE_SYNCWITHHARDWARE = 2;
        public const int STATE_NORMAL = 3;
        public const int STATE_STOPPED = 4;
        public const int COM_INIT_MSEC = 3000; //you may need to vary this for your computer or your Arduino
        public const int NCHAN_GANGLION = 4;
        public const int NUM_ACCEL_DIMS = 3;

        public const int RAW_ADS_SIZE = 3;
        public const int RAW_AUX_SIZE = 2;


        public const int RESP_ERROR_UNKNOWN = 499;
        public const int RESP_ERROR_BAD_PACKET = 500;
        public const int RESP_ERROR_BAD_NOBLE_START = 501;
        public const int RESP_ERROR_ALREADY_CONNECTED = 408;
        public const int RESP_ERROR_COMMAND_NOT_RECOGNIZED = 406;
        public const int RESP_ERROR_DEVICE_NOT_FOUND = 405;
        public const int RESP_ERROR_NO_OPEN_BLE_DEVICE = 400;
        public const int RESP_ERROR_UNABLE_TO_CONNECT = 402;
        public const int RESP_ERROR_UNABLE_TO_DISCONNECT = 401;
        public const int RESP_ERROR_SCAN_ALREADY_SCANNING = 409;
        public const int RESP_ERROR_SCAN_NONE_FOUND = 407;
        public const int RESP_ERROR_SCAN_NO_SCAN_TO_STOP = 410;
        public const int RESP_ERROR_SCAN_COULD_NOT_START = 412;
        public const int RESP_ERROR_SCAN_COULD_NOT_STOP = 411;
        public const int RESP_GANGLION_FOUND = 201;
        public const int RESP_SUCCESS = 200;
        public const int RESP_SUCCESS_DATA_ACCEL = 202;
        public const int RESP_SUCCESS_DATA_IMPEDANCE = 203;
        public const int RESP_SUCCESS_DATA_SAMPLE = 204;
        public const int RESP_STATUS_CONNECTED = 300;
        public const int RESP_STATUS_DISCONNECTED = 301;
        public const int RESP_STATUS_SCANNING = 302;
        public const int RESP_STATUS_NOT_SCANNING = 303;


        private const float MCP3912_Vref = 1.2f;  // reference voltage for ADC in MCP3912 set in hardware
        private const float MCP3912_gain = 1.0f;  //assumed gain setting for MCP3912.  NEEDS TO BE ADJUSTABLE JM
        public const float SCALE_FAC_UVOLTS_PER_COUNT = (MCP3912_Vref * 1000000F) / (8388607.0F * MCP3912_gain * 1.5F * 51.0F); //MCP3912 datasheet page 34. Gain of InAmp = 80
                                                                                                                                // private float scale_fac_accel_G_per_count = 0.032;
        public const float SCALE_FAC_ACCEL_G_PER_COUNT = 0.016f;

        public const float FS_HZ = 200.0f;
        #endregion
    }
}
