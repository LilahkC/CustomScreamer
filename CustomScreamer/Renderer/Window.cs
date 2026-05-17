using System.Security;

namespace CustomScreamer.Renderer;

using System.Runtime.InteropServices;
using SDL3;

public class Window
{
    public nint Renderer;
    private nint window;
    public bool Loop = true;
    private readonly TrayMenu trayMenu = new();
    public nint Texture;
    
    public void Initialize()
    {
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }
        
        const SDL.WindowFlags Flags = SDL.WindowFlags.AlwaysOnTop | SDL.WindowFlags.NotFocusable | SDL.WindowFlags.Fullscreen | SDL.WindowFlags.Hidden | SDL.WindowFlags.OpenGL;
        
        if (!SDL.CreateWindowAndRenderer("CustomScreamer", 0, 0, Flags, out window, out Renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window and rendering: {SDL.GetError()}");
            return;
        }
        
        SDL.SetWindowHitTest(window, null, nint.Zero);
        
        SDL.SetRenderDrawColor(Renderer, 0, 0, 0, 0);
        SDL.SetRenderDrawBlendMode(Renderer, SDL.BlendMode.Blend);

        SetShowWindow(false);
        trayMenu.CreateTray();
    }

    public void Update()
    {
        PoolEvents();
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

    public nint GetWindow()
    {
        return window;
    }
}
