using Action_Trainer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GanglionReader
{
    /// <summary>
    /// States
    /// </summary>
    public enum ProgramState
    {
        REFRESH_STATE,
        NOT_CONNECTED,
        CONNECTED,
        SAMPLING,
        EXITING
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Program State
        /// </summary>
        public ProgramState ProgramState {
            get; set;
        }

        /// <summary>
        /// Instance
        /// </summary>
        public static MainWindow Instance
        {
            get; set;
        }

        /// <summary>
        /// List of devices
        /// </summary>
        public ObservableCollection<String> DeviceList {
            get; set;
        }

        /// <summary>
        /// Log list
        /// </summary>
        public ObservableCollection<String> LogErrorList
        {
            get; set;
        }

        /// <summary>
        /// Socket Wrapper
        /// </summary>
        public EegSocket Socket
        {
            get; set;
        }

        /// <summary>
        /// Current Device Id
        /// </summary>
        public String DeviceId
        {
            get; set;
        }

        public MainWindow()
        {
            ServerProcess.InitializeNodeJsServer();
            Instance = this;
            DeviceList = new ObservableCollection<String>();
            LogErrorList = new ObservableCollection<String>();
            PacketContainer.Initialize();
            InitializeComponent();
            ProgramState = ProgramState.NOT_CONNECTED;
            this.DeviceListBox.ItemsSource = DeviceList;
            this.LogMessageListBox.ItemsSource = LogErrorList;
            this.DataContext = this;
            Socket = new EegSocket();
            this.Closed += MainWindow_Closed;
        }

        /// <summary>
        /// On Window Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (ProgramState == ProgramState.SAMPLING)
            {
                Socket.StopDataTransfer();
            }

            if(ProgramState == ProgramState.CONNECTED || ProgramState == ProgramState.SAMPLING)
            {
                Socket.DisconnectBLE();
            }
            ProgramState = ProgramState.EXITING;
            ServerProcess.Dispose();         
        }

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                try
                {
                    MainWindow.Instance.LogErrorList.Add("[" + DateTime.Now + "]: " + msg);
                } catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
        }

        /// <summary>
        /// Refresh button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Log("Starting Refresh.");

            //Refresh State to handle ScanPacket
            ProgramState = ProgramState.REFRESH_STATE;

            //Send Refresh packet
            Socket.SearchDeviceStart();

        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {

            if(DeviceListBox.SelectedItem == null)
            {
                return;
            }
            
            string deviceId = (String) DeviceListBox.SelectedItem;
            ProgramState = ProgramState.SAMPLING; //Set to sampling mode
            Socket.ConnectBLE(deviceId); //Send Connect Packet

        }
    }
}
