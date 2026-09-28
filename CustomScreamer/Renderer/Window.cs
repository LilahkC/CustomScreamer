using System.Runtime.Versioning;
using CustomScreamer.Utils;
using SDL3;
using static SDL3.SDL;

namespace CustomScreamer.Renderer
{
    public class Window
    {
        public static nint SDLWindowHandle { get; private set; } = nint.Zero;
        public bool Loop = true;
        private readonly TrayMenu trayMenu = new();
        public static nint Texture { get; set; } = nint.Zero;
        public static nint Renderer { get; set; } = nint.Zero;
        
        internal static bool IsWayland;

        public IntPtr WindowHandle
        {
            get
            {
                if (SDLWindowHandle == nint.Zero)
                    return IntPtr.Zero;

                uint props = GetWindowProperties(SDLWindowHandle);
                if (props == 0)
                    return IntPtr.Zero;

                switch (RuntimeInfo.OS)
                {
                    case RuntimeInfo.Platform.Windows:
                        return GetPointerProperty(props, Props.WindowWin32HWNDPointer, IntPtr.Zero);

                    case RuntimeInfo.Platform.Linux:
                        if (IsWayland)
                            return GetPointerProperty(props, Props.WindowWaylandSurfacePointer, IntPtr.Zero);

                        if (GetCurrentVideoDriver() == "x11")
                            return new(GetNumberProperty(props, Props.WindowX11WindowNumber, 0));

                        return IntPtr.Zero;

                    case RuntimeInfo.Platform.macOS:
                        return GetPointerProperty(props, Props.WindowCocoaWindowPointer, IntPtr.Zero);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [SupportedOSPlatform("linux")]
        public IntPtr DisplayHandle
        {
            get
            {
                if (SDLWindowHandle == nint.Zero)
                    return IntPtr.Zero;

                uint props = GetWindowProperties(SDLWindowHandle);
                if (props == 0)
                    return IntPtr.Zero;

                if (IsWayland)
                    return GetPointerProperty(props, Props.WindowWaylandDisplayPointer, IntPtr.Zero);

                if (GetCurrentVideoDriver() == "x11")
                    return GetPointerProperty(props, Props.WindowX11DisplayPointer, IntPtr.Zero);

                return IntPtr.Zero;
            }
        }

        public Window()
        {
            if (!Init(InitFlags.Video | InitFlags.Audio))
            {
                LogError(LogCategory.System, $"could not initialize: {GetError()}");
                return;
            }
            
            IsWayland = GetCurrentVideoDriver() == "wayland";
        }

        public void Initialize()
        {
            const WindowFlags Flags = WindowFlags.AlwaysOnTop | WindowFlags.NotFocusable | WindowFlags.Borderless | 
                                      WindowFlags.Hidden;
            
            // get primary display and set w and h to the size
            uint primaryDisplay = GetPrimaryDisplay();
            GetDisplayBounds(primaryDisplay, out Rect display);

            SDLWindowHandle = CreateWindow("CustomScreamer", display.W, display.H, Flags);
            Renderer = CreateRenderer(SDLWindowHandle, "");

            SetRenderVSync(Renderer, 1);

            if (SDLWindowHandle == nint.Zero)
            {
                LogError(LogCategory.Application, $"Error creating window and rendering: {GetError()}");
                return;
            }
            
            // we want text input to only be active when SDL3DesktopWindowTextInput is active.
            // SDL activates it by default on some platforms: https://github.com/libsdl-org/SDL/blob/release-2.0.16/src/video/SDL_video.c#L573-L582
            // so we deactivate it on startup.
            StopTextInput(SDLWindowHandle);
            SetWindowFocusable(SDLWindowHandle, false);

            SetWindowHitTest(SDLWindowHandle, null, nint.Zero);

            SetShowWindow(false);
            trayMenu.CreateTray();
        }

        public void Update()
        {
            PoolEvents();
        }

        public void Destroy()
        {
            DestroyTray(trayMenu.Tray);
            
            if (SDLWindowHandle != nint.Zero)
               DestroyWindow(SDLWindowHandle);
            
            if (Renderer != nint.Zero)
                DestroyRenderer(Renderer);
            
            Quit();
        }

        public static void SetShowWindow(bool show)
        {
            if (show)
            {
                if (IsWayland)
                    SetWindowFullscreen(SDLWindowHandle, true);
                
                ShowWindow(SDLWindowHandle);
            }
            else
            {
                if (IsWayland)
                    SetWindowFullscreen(SDLWindowHandle, false);
                
                HideWindow(SDLWindowHandle);
            }
        }

        public void PoolEvents()
        {
            while (PollEvent(out Event e))
            {
                if ((EventType) e.Type == EventType.Quit)
                {
                    Loop = false;
                }
            }
        }
    }
}
