using CustomScreamer.Utils;
using ImGuiNET;
using SDL3ImGui;
using static SDL3.SDL;

namespace CustomScreamer.Renderer
{
    public class Window
    {
        public static nint SDLWindowHandle { get; private set; } = nint.Zero;
        public static nint SDLRenderer { get; set; } = nint.Zero;
        
        public static nint SettingsWindowHandle { get; private set; } = nint.Zero;
        public static nint SettingsRenderer { get; set; } = nint.Zero;
        
        public static nint Texture { get; set; } = nint.Zero;
        public static ImGuiSDL3Renderer ImGUIRenderer;
        
        private readonly EventFilter EventWatcher;
        
        public bool Loop = true;
        public static bool InGame;
        private static bool startGame;
        public static ImGuiSDL3 Platform;
        private static Rect displayBounds;
        private static bool OpenSettingsAtLaunch = true;
        
        private static TrayMenu trayMenu = new();

        public Window()
        {
            if (!Init(InitFlags.Video | InitFlags.Audio))
            {
                LogError(LogCategory.System, $"could not initialize: {GetError()}");
                return;
            }
            
            GetDisplayBounds(GetPrimaryDisplay(), out displayBounds);
            
            EventWatcher = EventWatch;
            AddEventWatch(EventWatcher, nint.Zero);
            
            const WindowFlags Flags = WindowFlags.Borderless | WindowFlags.AlwaysOnTop | WindowFlags.Hidden;
            SDLWindowHandle = CreateWindow("CustomScreamer", displayBounds.W, displayBounds.H, Flags);
            SDLRenderer = CreateRenderer(SDLWindowHandle, "");
            
            SetRenderVSync(SDLRenderer, 1);
            
            StopTextInput(SDLWindowHandle);
            
            if (SDLWindowHandle == nint.Zero || SDLRenderer == nint.Zero)
            {
                LogError(LogCategory.Application, $"Error creating window and rendering: {GetError()}");
                return;
            }

            if (OpenSettingsAtLaunch)
                CreateSettingsWindow();
            else
                CreateGameWindow();
        }
        
        public static void CreateSettingsWindow()
        {
            DestroyMainWindow();

            ShowWindow(SettingsWindowHandle);
            RaiseWindow(SettingsWindowHandle);
            
            const WindowFlags Flags = WindowFlags.Resizable;
            
            SettingsWindowHandle = CreateWindow("Settings", displayBounds.W / 2, displayBounds.H / 2, Flags);
            SettingsRenderer = CreateRenderer(SettingsWindowHandle, "");
            
            SetRenderVSync(SettingsRenderer, 1);
            
            if (ImGui.GetCurrentContext() == nint.Zero)
            {
                nint context = ImGui.CreateContext();
                ImGui.SetCurrentContext(context);
            }
            
            Platform = new (SettingsWindowHandle, SettingsRenderer);
            ImGUIRenderer = new(SettingsRenderer);
            
            InGame = false;
            DestroyTray(trayMenu.Tray);
        }
        
        public void Update()
        {
            PollEvents();

            if (!InGame)
            {
                BuildUI();
                Render();
            }

            if (!startGame) 
                return;
            
            startGame = false;
            CreateGameWindow();
        }
        
        public void Destroy()
        {
            DestroyTray(trayMenu.Tray);
            
            DestroyMainWindow();
            DestroySettingsWindow();
            
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
                StartTextInput(SettingsWindowHandle);
            else
                StopTextInput(SettingsWindowHandle);
        }
        
        private void BuildUI()
        {
            Platform.NewFrame();
            ImGUIRenderer.NewFrame();
            ImGui.NewFrame();
            
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.WorkPos);
            ImGui.SetNextWindowSize(viewport.WorkSize);

            const ImGuiWindowFlags Flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove;
            
            if (ImGui.Begin("ImGUI", Flags))
            {
                if (ImGui.Button("Play"))
                    startGame = true;

                //if (ImGui.Button("Connect"));

                if (ImGui.Button("Quit"))
                    Loop = false;
            }
            
            ImGui.End();
            ImGui.EndFrame();
        }
        
        private void Render()
        {
            RenderClear(SettingsRenderer);
            
            ImGui.Render();
            ImGUIRenderer.RenderDrawData(ImGui.GetDrawData());
            
            RenderPresent(SettingsRenderer);
        }
        
        public static void CreateGameWindow()
        {
            InGame = true;
            DestroySettingsWindow();
            
            SetShowWindow(false);
            
            SetWindowFocusable(SDLWindowHandle, false);
            SetWindowHitTest(SDLWindowHandle, null, nint.Zero);
            SetWindowAlwaysOnTop(SDLWindowHandle, true);
            SetWindowSize(SDLWindowHandle, displayBounds.W, displayBounds.H);
            SetWindowBordered(SDLWindowHandle, false);
            SetWindowFullscreenMode(SDLWindowHandle, nint.Zero);
            SetWindowFullscreen(SDLWindowHandle, true);
            
            trayMenu.CreateTray();
            
            Game.Game.Initialize();
        }
        
        public static void SetShowWindow(bool show)
        {
            if (show)
            {
                ShowWindow(SDLWindowHandle);
                SetWindowFullscreen(SDLWindowHandle, true);
            }
            else
            {
                HideWindow(SDLWindowHandle);
            }
        }
        
        private bool EventWatch(nint userdata, ref Event e)
        {
            if ((EventType)e.Type == EventType.WindowExposed && !InGame && RuntimeInfo.IsWindows)
            {
                BuildUI();
                Render();
            }
            return true;
        }

        private static void DestroyMainWindow()
        {
            if (SDLRenderer != nint.Zero)
            {
                DestroyRenderer(SDLRenderer);
                SDLRenderer = nint.Zero;
            }
            
            if (SDLWindowHandle != nint.Zero)
            {
                DestroyWindow(SDLWindowHandle);
                SDLWindowHandle = nint.Zero;
            }
        }

        private static void DestroySettingsWindow()
        {
            if (SettingsRenderer != nint.Zero)
            {
                DestroyRenderer(SettingsRenderer);
                SettingsRenderer = nint.Zero;
            }
            
            if (SettingsWindowHandle != nint.Zero)
            {
                DestroyWindow(SettingsWindowHandle);
                SettingsWindowHandle = nint.Zero;
            }
            
            Platform = null;
            ImGUIRenderer = null;
            
            if(ImGui.GetCurrentContext() != nint.Zero)
                ImGui.DestroyContext();
        }
    }
}