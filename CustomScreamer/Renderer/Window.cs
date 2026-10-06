using System.Numerics;
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
        
        private readonly TrayMenu trayMenu = new();

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

            if (OpenSettingsAtLaunch)
                CreateSettingsWindow();
        }
        
        public static void CreateSettingsWindow()
        {
            nint context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            
            HideWindow(SDLWindowHandle);
            
            ShowWindow(SettingsWindowHandle);
            RaiseWindow(SettingsWindowHandle);
            
            const WindowFlags Flags = WindowFlags.Resizable;
            SettingsWindowHandle = CreateWindow("Settings", displayBounds.W / 2, displayBounds.H / 2, Flags);
            SettingsRenderer = CreateRenderer(SettingsWindowHandle, "");
            
            SetRenderVSync(SettingsRenderer, 1);
            
            Platform = new (SettingsWindowHandle, SettingsRenderer);
            ImGUIRenderer = new(SettingsRenderer);
            
            InGame = false;
        }
        
        public void Initialize()
        {
            if (SDLWindowHandle == nint.Zero || SDLRenderer == nint.Zero)
            {
                LogError(LogCategory.Application, $"Error creating window and rendering: {GetError()}");
                return;
            }
            
            SetRenderVSync(SDLRenderer, 1);
            
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
            
            if (!InGame)
                ImGui.DestroyContext();
            
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
            RenderClear(SettingsRenderer);
            
            if (!InGame)
            {
                ImGui.Render();
                ImGUIRenderer.RenderDrawData(ImGui.GetDrawData());
            }
            
            RenderPresent(SettingsRenderer);
        }
        
        public static void CreateGameWindow()
        {
            InGame = true;
            ImGui.DestroyContext();
            DestroyWindow(SettingsWindowHandle);
            StopTextInput(SDLWindowHandle);
            
            SetShowWindow(false);
            
            SetWindowFocusable(SDLWindowHandle, false);
            SetWindowHitTest(SDLWindowHandle, null, nint.Zero);
            SetWindowAlwaysOnTop(SDLWindowHandle, true);
            SetWindowBordered(SDLWindowHandle, false);
            SetWindowFullscreenMode(SDLWindowHandle, nint.Zero);
            SetWindowFullscreen(SDLWindowHandle, true);
            SetWindowSize(SDLWindowHandle, displayBounds.W, displayBounds.H);
            
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
    }
}