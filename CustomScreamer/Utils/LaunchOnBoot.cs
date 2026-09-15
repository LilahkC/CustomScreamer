using System.Reflection;
using SDL3;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace CustomScreamer.Utils
{
    public static class LaunchOnBoot
    {
        private const string RegKeyLocation = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string AppName = Assembly.GetExecutingAssembly().FullName;
        
        public static void SetStartup(nint userdata, nint entry)
        {
            bool enable = SDL.GetTrayEntryChecked(entry);
            string exePath = AppDomain.CurrentDomain.BaseDirectory;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegKeyLocation, true);

                if (enable)
                    regKey?.SetValue(AppName, exePath!);
                else
                    regKey?.DeleteValue(AppName!, false);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string desktopFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    $".config/autostart/{AppName}.desktop");

                if (enable)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(desktopFile)!);
                    File.WriteAllText(desktopFile, $"[Desktop Entry]\nType=Application\nName={AppName}\nExec={exePath}");
                }
                else
                {
                    File.Delete(desktopFile);
                }
            }
        }

        public static bool IsOpenOnBootEnabled()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegKeyLocation, false);
                return regKey?.GetValue(AppName) is not null;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string desktopFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    $".config/autostart/{AppName}.desktop");
                return File.Exists(desktopFile);
            }

            return false;
        }
    }
}
