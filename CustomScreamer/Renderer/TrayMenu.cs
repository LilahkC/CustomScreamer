using System.Reflection;
using SDL3;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace CustomScreamer.Renderer;

public class TrayMenu
{
    public nint Tray;
    
    private string regKeyLocation = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    string? appName = Assembly.GetExecutingAssembly().FullName;
    
    public void CreateTray()
    {
        using Stream content = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomScreamer.Icon.sssdfg.png")!;
        using SDL.IOStreamOwner stream = SDL.IOFromStream(content);
        nint image = Image.LoadPNGIO(stream.Handle);
        
        Tray = SDL.CreateTray(image, null);
        nint trayMenu = SDL.CreateTrayMenu(Tray);
        nint trayChance = SDL.InsertTrayEntryAt(trayMenu, 0, $"Chance : {Game.Game.Chance} %", SDL.TrayEntryFlags.Disabled);
        nint trayTime = SDL.InsertTrayEntryAt(trayMenu, 1, $"Every {Game.Game.Time} sec", SDL.TrayEntryFlags.Disabled);
        
        nint trayBoot = SDL.InsertTrayEntryAt(trayMenu, 2, $"Launch at boot", SDL.TrayEntryFlags.CheckBox);
        SDL.SetTrayEntryChecked(trayBoot, IsOpenOnBootEnabled());
        SDL.SetTrayEntryCallback(trayBoot, SetStartup, nint.Zero);
        
        nint trayQuit = SDL.InsertTrayEntryAt(trayMenu, 3, "Quit", SDL.TrayEntryFlags.Button);
        SDL.SetTrayEntryCallback(trayQuit, callback_quit, nint.Zero);
    }
    
    private static void callback_quit(nint userdata, nint entry)
    {
        SDL.Event quit = new()
        {
            Type = (uint)SDL.EventType.Quit
        };
        SDL.PushEvent(ref quit);
    }
    
    void SetStartup(nint userdata, nint entry)
    {
        bool enable = SDL.GetTrayEntryChecked(entry);
        string? exePath = Environment.ProcessPath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            RegistryKey? regKey = Registry.CurrentUser.OpenSubKey(regKeyLocation, true);
            
            if (enable)
                regKey?.SetValue(appName, exePath!);
            else
                regKey?.DeleteValue(appName!, false);
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string desktopFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".config/autostart/{appName}.desktop");
            
            if (enable)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(desktopFile)!);
                File.WriteAllText(desktopFile, $"[Desktop Entry]\nType=Application\nName={appName}\nExec={exePath}");
            }
            else
            {
                File.Delete(desktopFile);
            }
        }
    }

    bool IsOpenOnBootEnabled()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using RegistryKey? regKey = Registry.CurrentUser.OpenSubKey(regKeyLocation, false);
            return regKey?.GetValue(appName) is not null;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string desktopFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".config/autostart/{appName}.desktop");
            return File.Exists(desktopFile);
        }

        return false;
    }
}
