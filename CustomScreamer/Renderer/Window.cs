namespace CustomScreamer.Renderer;

using System.Runtime.InteropServices;
using SDL3;

public class Window
{
    public nint Renderer;
    private nint window;
    public bool Loop = true;
    private readonly TrayMenu trayMenu = new();
    
    // ── Win32 P/Invoke ──────────────────────────────────────────────────────
    const int GWL_EXSTYLE = -20;
    const int WS_EX_LAYERED = 0x00080000;
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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            SDL.SetHint("SDL_MOUSE_FOCUS_CLICKTHROUGH", "1");
        
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }
        
        const SDL.WindowFlags Flags = SDL.WindowFlags.AlwaysOnTop | SDL.WindowFlags.NotFocusable | SDL.WindowFlags.Fullscreen | SDL.WindowFlags.Transparent | SDL.WindowFlags.Hidden;
        
        if (!SDL.CreateWindowAndRenderer("CustomScreamer", 0, 0, Flags, out window, out Renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window and rendering: {SDL.GetError()}");
            return;
        }
        
        SDL.SetRenderDrawColor(Renderer, 0, 0, 0, 0);
        SDL.SetRenderDrawBlendMode(Renderer, SDL.BlendMode.Blend);
        
        uint props = SDL.GetWindowProperties(window);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            nint hwnd  = SDL.GetPointerProperty(props, "SDL.window.win32.hwnd", nint.Zero);
            
            if (hwnd != nint.Zero)
                EnableClickThrough(hwnd);
            else
                SDL.LogError(SDL.LogCategory.Application, "Cannot get the HWND (Windows only).");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            SDL.SetWindowFocusable(window, false);
            SDL.SetWindowKeyboardGrab(window, false);
        }
        SetShowWindow(true);
        SDL.SetWindowAlwaysOnTop(window, true);
        trayMenu.CreateTray();
    }

    public void Update()
    {
        PoolEvents();
        ClearRenderer();
        RenderPresent();
    }
    
    public void Quit()
    {
        SDL.DestroyRenderer(Renderer);
        SDL.DestroyTray(trayMenu.Tray);
        SDL.DestroyWindow(window);
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
