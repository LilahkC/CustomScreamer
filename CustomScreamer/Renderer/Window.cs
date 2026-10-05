using System.Numerics;
using ImGuiNET;
using SDL3ImGui;
using static SDL3.SDL;

namespace CustomScreamer.Renderer
{
    public class Window
    {
        private readonly EventFilter EventWatcher;
        
        public static nint SDLWindowHandle { get; private set; } = nint.Zero;
        public static nint Renderer { get; set; } = nint.Zero;
        public static nint Texture { get; set; } = nint.Zero;
        public static ImGuiSDL3Renderer ImGUIRenderer;
        
        public bool Loop = true;
        public static bool InGame;
        private static bool startGame;
        public readonly ImGuiSDL3 Platform;
        
        private readonly TrayMenu trayMenu = new();

        public Window()
        {
            if (!Init(InitFlags.Video | InitFlags.Audio))
            {
                LogError(LogCategory.System, $"could not initialize: {GetError()}");
                return;
            }

            EventWatcher = EventWatch;
                
            nint context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            
            GetDisplayBounds(GetPrimaryDisplay(), out Rect displayBounds);
            
            const WindowFlags Flags = WindowFlags.Resizable;
            SDLWindowHandle = CreateWindow("CustomScreamer", displayBounds.W / 2, displayBounds.H / 2, Flags);
            
            Renderer = CreateRenderer(SDLWindowHandle, "");
            
            Platform = new (SDLWindowHandle, Renderer);
            ImGUIRenderer = new(Renderer);
        }
        
        public void Initialize()
        {
            if (SDLWindowHandle == nint.Zero || Renderer == nint.Zero)
            {
                LogError(LogCategory.Application, $"Error creating window and rendering: {GetError()}");
                return;
            }
            
            AddEventWatch(EventWatcher, nint.Zero);
            
            SetRenderVSync(Renderer, 1);
            
            ShowWindow(SDLWindowHandle);
            RaiseWindow(SDLWindowHandle);
            
            RenderClear(Renderer);
            RenderPresent(Renderer);
            
            trayMenu.CreateTray();
        }
        
        public void Update()
        {
            PollEvents();
            
            if(!InGame)
                BuildUI();
            
            Render();
            
            if (!startGame)
                return;
            
            startGame = false;
            CreateGameWindow();
        }
        
        public void Destroy()
        {
            DestroyTray(trayMenu.Tray);
            
            RemoveEventWatch(EventWatcher, nint.Zero);
            
            if (!InGame)
                ImGui.DestroyContext();
            
            if (Renderer != nint.Zero)
            {
                DestroyRenderer(Renderer);
                Renderer = nint.Zero;
            }
            
            if (SDLWindowHandle != nint.Zero)
            {
                DestroyWindow(SDLWindowHandle);
                SDLWindowHandle = nint.Zero;
            }
            
            Quit();
        }
        
        private void PollEvents()
        {
            if (!InGame)
                UpdateTextInputState();
            
            while (PollEvent(out Event e))
            {
                if (!InGame)
                    Platform.ProcessEvent(e);

                switch ((EventType)e.Type)
                {
                    case EventType.Quit:
                    case EventType.WindowCloseRequested:
                        Loop = false;
                        break;
                }
            }
        }
        
        private static void UpdateTextInputState()
        {
            if (ImGui.GetIO().WantTextInput)
                StartTextInput(SDLWindowHandle);
            else
                StopTextInput(SDLWindowHandle);
        }
        
        private void BuildUI()
        {
            Platform.NewFrame();
            ImGUIRenderer.NewFrame();
            ImGui.NewFrame();
            
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.WorkPos);
            ImGui.SetNextWindowSize(viewport.WorkSize);

            const ImGuiWindowFlags Flags = ImGuiWindowFlags.NoDecoration;
            
            if (ImGui.Begin("ImGUI", Flags))
            {
                ImGui.SetCursorPos(new(viewport.WorkSize.X / 2 - 40f,  viewport.WorkSize.Y / 2 - 20f));
                if (ImGui.Button("Play", new(40f,20f)))
                    startGame = true;
            }
            
            ImGui.End();
        }
        
        private void Render()
        {
            RenderClear(Renderer);
            
            if (!InGame)
            {
                ImGui.Render();
                ImGUIRenderer.RenderDrawData(ImGui.GetDrawData());
            }
            
            RenderPresent(Renderer);
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
        
        public static void CreateGameWindow()
        {
            InGame = true;
            ImGui.DestroyContext();
            StopTextInput(SDLWindowHandle);
            
            RenderClear(Renderer);
            RenderPresent(Renderer);
            
            SetWindowFocusable(SDLWindowHandle, false);
            SetWindowHitTest(SDLWindowHandle, null, nint.Zero);
            SetWindowAlwaysOnTop(SDLWindowHandle, true);
            SetFullscreen(true);
            SetShowWindow(false);
            
            Game.Game.Initialize();
        }
        
        private bool EventWatch(nint userdata, ref Event e)
        {
            if ((EventType)e.Type == EventType.WindowExposed && !InGame)
            {
                BuildUI();
                Render();
            }
            return true;
        }
    }
}