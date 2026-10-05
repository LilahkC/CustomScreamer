using System.Numerics;
using System.Runtime.Versioning;
using CustomScreamer.Utils;
using ImGuiNET;
using SDL3ImGui;
using static SDL3.SDL;

namespace CustomScreamer.Renderer
{
    public class Window
    {
        public static nint SDLWindowHandle { get; private set; } = nint.Zero;
        public nint Device;
        
        public bool Loop = true;
        private readonly TrayMenu trayMenu = new();
        public static nint Texture { get; set; } = nint.Zero;
        public static nint Renderer { get; set; } = nint.Zero;
        
        public readonly ImGuiSDL3 Platform;
        public static ImGuiSDL3Renderer ImGUIRenderer;
        
        private Rect DisplayBounds;
        private FRect _srcRect;
        private Rect _screenClipRect;
        
        public Window()
        {
            if (!Init(InitFlags.Video | InitFlags.Audio))
            {
                LogError(LogCategory.System, $"could not initialize: {GetError()}");
                return;
            }
            
            nint context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            
            const WindowFlags Flags = WindowFlags.Maximized | WindowFlags.Hidden;
            
            // get primary display and set w and h to the size
            GetDisplayBounds(GetPrimaryDisplay(), out DisplayBounds);

            SDLWindowHandle = CreateWindow("CustomScreamer", DisplayBounds.W / 2, DisplayBounds.H / 2, Flags);
            Renderer = CreateRenderer(SDLWindowHandle, "");

            Device = Renderer;
            Platform = new (SDLWindowHandle, Device);
            ImGUIRenderer = new(Device);
        }

        public void Initialize()
        {
            SetRenderVSync(Renderer, 1);
                
            if (SDLWindowHandle == nint.Zero)
            {
                LogError(LogCategory.Application, $"Error creating window and rendering: {GetError()}");
                return;
            }
            
            ShowWindow(SDLWindowHandle);
            RaiseWindow(SDLWindowHandle);
            RenderClear(Renderer);
            RenderPresent(Renderer);
            trayMenu.CreateTray();
        }

        public void Update()
        {
            PollEvents();
            
            Platform.NewFrame();
            ImGUIRenderer.NewFrame();
            ImGui.NewFrame();

            if(ImGui.Begin("ImGUI"))
            {
                ImGui.Text("Hello from SDL3 & ImGui!");

                // Draw our texture in ImGui
                ImGui.Image(Texture, new(_srcRect.W, _srcRect.H));
            }
            
            ImGui.End();
            ImGui.EndFrame();
            
            Render();
        }

        public void Destroy()
        {
            DestroyTray(trayMenu.Tray);
            ImGui.DestroyContext();
            
            if (SDLWindowHandle != nint.Zero)
               DestroyWindow(SDLWindowHandle);
            
            if (Renderer != nint.Zero)
                DestroyRenderer(Renderer);
            
            Quit();
        }

        public static void SetShowWindow(bool show)
        {
            if (show)
                ShowWindow(SDLWindowHandle);
            else
                HideWindow(SDLWindowHandle);
        }
        
        public static void SetFullscreen(bool fullscreen)
        {
            SetWindowFullscreen(SDLWindowHandle, fullscreen);
        }

        public void PollEvents()
        {
            while (PollEvent(out Event e))
            {
                if(ImGui.GetIO().WantTextInput && !TextInputActive(SDLWindowHandle))
                    StartTextInput(SDLWindowHandle);
                else if(!ImGui.GetIO().WantTextInput && TextInputActive(SDLWindowHandle))
                    StopTextInput(SDLWindowHandle);

                Platform.ProcessEvent(e);
                
                switch ((EventType) e.Type)
                {
                    case EventType.Quit:
                    case EventType.WindowCloseRequested:
                        Loop = false;
                        break;
                }
            }
        }
        private void Render()
        {
            RenderClear(Device);

            // Reset the clip rect to the screen size
            SetRenderClipRect(Device, _screenClipRect);

            // Render ImGui
            ImGui.Render();
            ImGUIRenderer.RenderDrawData(ImGui.GetDrawData());

            RenderPresent(Device);
        }
    }
}
