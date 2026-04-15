using System.Reflection;
using System.Resources;
using SDL3;

namespace CustomScreamer.Renderer;
using System.Runtime.InteropServices;
using SDL3;

public class Window
{
    public nint Renderer;
    private nint window;
    private nint tray;
    public bool Loop = true;
    
    private readonly Game.Game game = new();
    
    // ── Win32 P/Invoke ──────────────────────────────────────────────────────
    const int GWL_EXSTYLE       = -20;
    const int WS_EX_LAYERED     = 0x00080000;
    const int WS_EX_TRANSPARENT = 0x00000020;

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowLong(nint hwnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int SetWindowLong(nint hwnd, int nIndex, int dwNewLong);

    static void EnableClickThrough(nint hwnd)
    {
        int style = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
    }
    // ──────────────────────────────────Merci Claude──────────────────────────────────────

    public void Initialize()
    {
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }

        const SDL.WindowFlags Flags = SDL.WindowFlags.AlwaysOnTop | SDL.WindowFlags.NotFocusable | SDL.WindowFlags.Fullscreen | SDL.WindowFlags.Transparent | SDL.WindowFlags.Hidden;

        if (!SDL.CreateWindowAndRenderer("SDL3 Create Window", 0, 0, Flags, out window, out Renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window and rendering: {SDL.GetError()}");
            return;
        }

        SDL.SetRenderDrawColor(Renderer, 0, 0, 0, 0);
        SDL.SetRenderDrawBlendMode(Renderer, SDL.BlendMode.Blend);

        uint props = SDL.GetWindowProperties(window);
        nint hwnd  = SDL.GetPointerProperty(props, "SDL.window.win32.hwnd", nint.Zero);

        if (hwnd != nint.Zero)
            EnableClickThrough(hwnd);
        else
            SDL.LogError(SDL.LogCategory.Application, "Cannot get the HWND (Windows only).");

        CreateTray();
    }

    public void CreateTray()
    {
        using Stream content = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomScreamer.Icon.sssdfg.png")!;
        using SDL.IOStreamOwner stream = SDL.IOFromStream(content);
        nint image = Image.LoadPNGIO(stream.Handle);
        
        tray = SDL.CreateTray(image, null);
        nint trayMenu = SDL.CreateTrayMenu(tray);
        nint trayChance = SDL.InsertTrayEntryAt(trayMenu, 0, $"Chance : {Game.Game.Chance} %", SDL.TrayEntryFlags.Disabled);
        nint trayTime = SDL.InsertTrayEntryAt(trayMenu, 1, $"Every {Game.Game.Time} sec", SDL.TrayEntryFlags.Disabled);
        nint trayQuit = SDL.InsertTrayEntryAt(trayMenu, 2, "Quit", SDL.TrayEntryFlags.Button);

        SDL.SetTrayEntryCallback(trayQuit, callback_quit, nint.Zero);
    }
    
    private static void callback_quit(nint test1, nint test2)
    {
        SDL.Event quit = new()
        {
            Type = (uint)SDL.EventType.Quit
        };
        SDL.PushEvent(ref quit);
    }
    
    public void Quit()
    {
        SDL.DestroyWindow(window);
        SDL.DestroyTray(tray);
        SDL.Quit();
    }
    
    public void SetShowWindow(bool show)
    {
        if(show)
            SDL.ShowWindow(window);
        else
            SDL.HideWindow(window);
    }

    public void ClearRenderer()
    {
        SDL.RenderClear(Renderer);
    }
    
    public void RenderPresent()
    {
        SDL.RenderPresent(Renderer);
    }

    public void PoolEvents()
    {
        while (SDL.PollEvent(out var e))
        {
            if ((SDL.EventType)e.Type == SDL.EventType.Quit)
            {
                Loop = false;
            }
        }
    }
}
