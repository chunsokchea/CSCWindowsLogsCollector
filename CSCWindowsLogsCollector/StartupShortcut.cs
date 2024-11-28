using System;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace CSCWindowsLogsCollector
{
    public static class StartupShortcut
    {
        private static string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "CSCWindowsLogsCollector.lnk");

        public static void AddApplicationToStartup()
        {
            if (!System.IO.File.Exists(shortcutPath))
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Application.StartupPath;
                shortcut.Description = "CSC WindowsLogs Collector";
                shortcut.Save();
            }
        }

        public static void RemoveApplicationFromStartup()
        {
            if (System.IO.File.Exists(shortcutPath))
            {
                System.IO.File.Delete(shortcutPath);
            }
        }

        public static bool IsApplicationInStartup()
        {
            return System.IO.File.Exists(shortcutPath);
        }
    }
}
