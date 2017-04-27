using GanglionReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Action_Trainer
{
    /// <summary>
    /// Handles initailization of NodeJS Process GanglionHub.exe
    /// </summary>
    public class ServerProcess
    {
        /// <summary>
        /// Start GanglionHub.exe
        /// </summary>
        public static void InitializeNodeJsServer()
        {
            if(!IsProcessRunning())
            {
                Process.Start(Path.GetFullPath(Constants.NODE_JS_SERVER_DIR + "GanglionHub.exe"));
                System.Threading.Thread.Sleep(2000);
            } else
            {
                Console.WriteLine("GanglionHub.exe is already running.");
            }
        }

        /// <summary>
        /// Terminate GanglionHub
        /// </summary>
        public static void Dispose()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("GanglionHub");
                if (processes.Length == 0)
                {
                    return;
                }
                foreach (Process process in processes)
                    process.Close();
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Checks if GanglionHub process is running
        /// </summary>
        /// <returns></returns>
        static bool IsProcessRunning()
        {
            Process[] processes = Process.GetProcessesByName("GanglionHub");
            if (processes.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
       
    }
}
