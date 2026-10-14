using CustomScreamer.Utils;
using SDL3;

namespace CustomScreamer.MediaPlayer;

public static class SoundPlayer
{
    public static float Volume = 50f;
    
    private static string mp3Path;
    private static nint mixer;
    private static nint audio;
    private static nint track;

    private static readonly Mixer.TrackStoppedCallback OnTrackStopped = (_, _) => Unload();
    private static bool IsMp3(string path) => string.Equals(Path.GetExtension(path), ".mp3", StringComparison.OrdinalIgnoreCase);
    public static void InitializePath() => TrySetSound(Folder.GetFolder());
    
    private static bool InitializeMixer()
    {
        if (Mixer.Init())
            return true;

        SDL.LogInfo(SDL.LogCategory.Application, $"Mixer Init Error: {SDL.GetError()}");
        return false;
    }

    private static bool InitializeMixerDevice()
    {
        mixer = Mixer.CreateMixerDevice(SDL.AudioDeviceDefaultPlayback, nint.Zero);

        if (mixer == nint.Zero)
        {
            SDL.LogInfo(SDL.LogCategory.Application, $"MixerDevice creation failed: {SDL.GetError()}");
            return false;
        }

        Mixer.SetMixerGain(mixer, Volume / 100);
        return true;
    }

    public static void UpdateVolume(float newVolume)
    {
        Mixer.SetMixerGain(mixer, newVolume);
    }
    
    private static bool TrySetSound(string folder)
    {
        mp3Path = Directory.EnumerateFiles(folder).FirstOrDefault(IsMp3);

        if (mp3Path is null)
        {
            SDL.LogInfo(SDL.LogCategory.Application, $"MP3 file not found in the directory: {folder}");
            Quit();
            return false;
        }

        SDL.LogInfo(SDL.LogCategory.Application, $"Loaded sound: {mp3Path}");
        return true;
    }
    
    public static void Initialize()
    {
        if (!InitializeMixer() || !InitializeMixerDevice())
            return;

        track = Mixer.CreateTrack(mixer);

        if (track == nint.Zero || !Mixer.SetTrackAudio(track, audio))
        {
            SDL.LogInfo(SDL.LogCategory.Application, $"Failed creating track: {SDL.GetError()}");
            Quit();
            return;
        }

        Mixer.SetTrackStoppedCallback(track, OnTrackStopped, nint.Zero);
    }

    private static bool LoadAudio()
    {
        if (!TrySetSound(Folder.GetFolder()))
            return false;
        
        StopSound();
        
        if (mp3Path is null)
            InitializePath();

        audio = Mixer.LoadAudio(mixer, mp3Path!, predecode: true);

        if (audio != nint.Zero)
            return true;

        SDL.LogInfo(SDL.LogCategory.Application, $"Failed loading audio: {SDL.GetError()}");
        return false;
    }

    public static void Load()
    {
        if (!LoadAudio())
            return;

        if (track != nint.Zero)
            Mixer.SetTrackAudio(track, audio);
    }

    public static void PlaySound()
    {
        if (audio == nint.Zero)
            Load();
        
        Mixer.PlayTrack(track, 0);
    }
    
    public static void StopSound()
    {
        if (track != nint.Zero)
            Mixer.StopTrack(track, 0);
    }

    public static void Unload()
    {
        if (mp3Path is not null)
        {
            SDL.LogInfo(SDL.LogCategory.Application, $"Unloaded Sound: {mp3Path}");
            mp3Path = null;
        }
     
        if (audio == nint.Zero)
            return;

        if (track != nint.Zero)
            Mixer.SetTrackAudio(track, nint.Zero);

        Mixer.DestroyAudio(audio);
        audio = nint.Zero;
    }

    public static void Quit()
    {
        Unload();

        if (track != nint.Zero)
        {
            Mixer.DestroyTrack(track);
            track = nint.Zero;
        }

        if (mixer != nint.Zero)
        {
            Mixer.DestroyMixer(mixer);
            mixer = nint.Zero;
            Mixer.Quit();
        }

        if (mp3Path is not null)
        {
            SDL.LogInfo(SDL.LogCategory.Application, $"Unloading sound: {Path.GetFileName(mp3Path)}");
            mp3Path = null;
        }
    }
}