using System.Diagnostics;
using System.Runtime.InteropServices;
using SDL3;

namespace AUSECOURS;

internal static class Program
{
    static nint renderer;
    static nint window;
    static nint tray;
    static Stopwatch sw = new();
    static nint mixer;
    
    static nint gifAnim; 
    static Image.Animation animData;
    static bool isPlayingGif = false;
    static int currentFrame = 0;
    static ulong frameStartTime = 0;
    
    static nint[] frameTextures;
    static int frameCount;
    static int gifW, gifH;
    static int[] frameDelays;
    
    //SOUND
    private static float volume = 0.45f;
    private static float chance = 0.5f;
    private static float time = 1f;

    static string mp3Path;
    static string gifPath;

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

    private static void InitializePaths()
    {
        string baseDir = "ListOfScreamers\\Foxy";

        gifPath = Directory.GetFiles(baseDir, "*.gif").FirstOrDefault();

        mp3Path = Directory.GetFiles(baseDir, "*.*")
            .FirstOrDefault(f => f.EndsWith(".mp3"));
        
        // Petite vérification de sécurité
        if (string.IsNullOrEmpty(gifPath) || string.IsNullOrEmpty(mp3Path))
        {
            Console.WriteLine("Erreur : Fichier GIF ou Audio manquant dans le dossier !");
            Mixer.DestroyMixer(mixer);
            Mixer.Quit();
            SDL.Quit();
            return;
        }
    }
    
    private static void callback_quit(nint test1, nint test2)
    {
        SDL.Event liberezmoi = new();
        liberezmoi.Type = (uint)SDL.EventType.Quit;
        SDL.PushEvent(ref liberezmoi);
    }

    private static bool Random(float chance, float time)
    {
        if (!sw.IsRunning)
            sw = Stopwatch.StartNew();

        if (!(sw.Elapsed.Seconds >= time))
            return false;

        sw.Restart();
        return System.Random.Shared.NextSingle() < chance / 100;
    }

    private static void PlaySound()
    {
        Mixer.SetMixerGain(mixer, volume);
        nint audio = Mixer.LoadAudio(mixer, mp3Path, predecode: true);
        if (audio == nint.Zero)
        {
            Console.WriteLine($"Failed loading audio: {SDL.GetError()}");
            Quit();
            return;
        }
        
        // Create track and attach loaded audio
        nint track = Mixer.CreateTrack(mixer);
        if (track == nint.Zero || !Mixer.SetTrackAudio(track, audio))
        {
            Console.WriteLine($"Failed creating track: {SDL.GetError()}");
            Quit();
            return;
        }

        // Play
        Mixer.PlayTrack(track, 0);
    }
    
    private static void LoadGif(nint renderer)
    {
        nint gifAnimPtr = Image.LoadAnimation(gifPath);
        if (gifAnimPtr == nint.Zero)
        {
            Console.WriteLine($"Erreur GIF: {SDL.GetError()}");
            return;
        }

        var anim = Marshal.PtrToStructure<Image.Animation>(gifAnimPtr);
        frameCount = anim.Count;
        gifW = anim.W;
        gifH = anim.H;
        frameTextures = new nint[frameCount];
        frameDelays = new int[frameCount];

        for (int i = 0; i < frameCount; i++)
        {
            // 1. Extraire le pointeur de la Surface
            nint surfacePtr = Marshal.ReadIntPtr(anim.Frames, i * nint.Size);
            // 2. Convertir la Surface en Texture pour le GPU
            frameTextures[i] = SDL.CreateTextureFromSurface(renderer, surfacePtr);
            // 3. Stocker le délai
            frameDelays[i] = Marshal.ReadInt32(anim.Delays, i * sizeof(int));
        }
    
        // On peut libérer l'animation originale, on a nos textures
        Image.FreeAnimation(gifAnimPtr);
    }

    private static void PlayImageOrVideoOrGif()
    {
        PlaySound();
        
        if (isPlayingGif) 
            return;

        isPlayingGif = true;
        currentFrame = 0;
        frameStartTime = SDL.GetTicks();
    }
    
    private static void UpdateAndRenderGif(nint renderer)
    {
        if (!isPlayingGif || frameTextures == null) return;

        int delay = frameDelays[currentFrame];
        if (delay <= 0) delay = 100;

        if (SDL.GetTicks() - frameStartTime >= (uint)delay)
        {
            currentFrame++;
            frameStartTime = SDL.GetTicks();

            if (currentFrame >= frameCount)
            {
                isPlayingGif = false;
                currentFrame = 0;
                return;
            }
        }

        SDL.GetWindowSize(window, out int w, out int h);
        SDL.FRect destRect = new SDL.FRect { X = 0, Y = 0, W = w, H = h };
    
        // On utilise maintenant notre tableau de textures
        SDL.RenderTexture(renderer, frameTextures[currentFrame], nint.Zero, ref destRect);
    }

    private static void Main()
    {
        InitializePaths();
        
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }
        
        if (!Mixer.Init())
        {
            Console.WriteLine($"Mixer Init Error: {SDL.GetError()}");
            Quit();
            return;
        }
        
        // Open audio device from mixer
        // If a specification is required here, pass null for auto-fitting.
        mixer = Mixer.CreateMixerDevice(SDL.AudioDeviceDefaultPlayback, nint.Zero);
        if (mixer == nint.Zero)
        {
            Console.WriteLine($"MixerDevice creation failed: {SDL.GetError()}");
            Quit();
            return;
        }

        const SDL.WindowFlags Flags = SDL.WindowFlags.AlwaysOnTop | SDL.WindowFlags.NotFocusable | SDL.WindowFlags.Fullscreen | SDL.WindowFlags.Transparent | SDL.WindowFlags.Hidden;

        if (!SDL.CreateWindowAndRenderer("SDL3 Create Window", 0, 0, Flags, out window, out renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window and rendering: {SDL.GetError()}");
            return;
        }

        SDL.SetRenderDrawColor(renderer, 0, 0, 0, 0);
        SDL.SetRenderDrawBlendMode(renderer, SDL.BlendMode.Blend);

        uint props = SDL.GetWindowProperties(window);
        nint hwnd  = SDL.GetPointerProperty(props, "SDL.window.win32.hwnd", nint.Zero);

        if (hwnd != nint.Zero)
            EnableClickThrough(hwnd);
        else
            SDL.LogError(SDL.LogCategory.Application, "Cannot get the HWND (Windows only).");

        var loop = true;
        SDL.ShowWindow(window);

        tray = SDL.CreateTray(nint.Zero, null);
        nint traymenu = SDL.CreateTrayMenu(tray);
        nint trayQuit = SDL.InsertTrayEntryAt(traymenu, 0, "Quit", SDL.TrayEntryFlags.Button);
        SDL.SetTrayEntryCallback(trayQuit, callback_quit, nint.Zero);
        
        LoadGif(renderer);

        SDL.ShowWindow(window);
        
        while (loop)
        {
            if (Random(chance, time))
            {
                PlayImageOrVideoOrGif();
            }

            while (SDL.PollEvent(out var e))
            {
                if ((SDL.EventType)e.Type == SDL.EventType.Quit)
                {
                    loop = false;
                }
            }

            SDL.RenderClear(renderer);
            
            if (isPlayingGif)
            { 
                UpdateAndRenderGif(renderer);
            }
            
            SDL.RenderPresent(renderer);
        }

        Quit();
    }

    private static void Quit()
    {
        Mixer.DestroyMixer(mixer);
        Mixer.Quit();
        SDL.DestroyRenderer(renderer);
        SDL.DestroyWindow(window);
        SDL.DestroyTray(tray);
        SDL.Quit();
    }
}