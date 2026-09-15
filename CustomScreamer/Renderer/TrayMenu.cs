using System.Reflection;
using SDL3;
using CustomScreamer.Utils;

namespace CustomScreamer.Renderer
{
    public class TrayMenu
    {
        public nint Tray;
        
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
            SDL.SetTrayEntryChecked(trayBoot, LaunchOnBoot.IsOpenOnBootEnabled());
            SDL.SetTrayEntryCallback(trayBoot, LaunchOnBoot.SetStartup, nint.Zero);
            
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
    }
}
