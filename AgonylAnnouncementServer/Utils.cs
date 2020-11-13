using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AgonylAnnouncementServer
{
    public static class Utils
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        public static bool IsAlreadyRunning()
        {
            // get all processes by Current Process name
            var processes =
                Process.GetProcessesByName(
                    Process.GetCurrentProcess().ProcessName);

            // if there is more than one process...
            if (processes.Length > 1)
            {
                // if other process id is OUR process ID...
                // then the other process is at index 1
                // otherwise other process is at index 0
                var n = (processes[0].Id == Process.GetCurrentProcess().Id) ? 1 : 0;

                // get the window handle
                var hWnd = processes[n].MainWindowHandle;

                // if iconic, we need to restore the window
                if (IsIconic(hWnd)) ShowWindowAsync(hWnd, 9);

                // Bring it to the foreground
                SetForegroundWindow(hWnd);
                return true;
            }
            return false;
        }

        public static string GetMyDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public static string ConfigFilePath()
        {
            return GetMyDirectory() + Path.DirectorySeparatorChar + "Config.json";
        }

        public static string AnnouncementsFilePath()
        {
            return GetMyDirectory() + Path.DirectorySeparatorChar + "Announcements.txt";
        }
    }
}
