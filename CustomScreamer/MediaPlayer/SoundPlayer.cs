using CustomScreamer.Utils;

namespace CustomScreamer.MediaPlayer;
using SDL3;

public static class SoundPlayer
{
    private static string mp3;
    private const float Volume = 0.45f;
    private static nint mixer;
    private static nint audio;
    private static nint track;
    
    private static bool InitializeMixer()
    {
        if (Mixer.Init())
            return true;
        
        SDL.LogInfo(SDL.LogCategory.Error, $"Mixer Init Error: {SDL.GetError()}");
        return false;
    }

    private static bool InitializeMixerDevice()
    {
        mixer = Mixer.CreateMixerDevice(SDL.AudioDeviceDefaultPlayback, nint.Zero);
        Mixer.SetMixerGain(mixer, Volume);
        
        if (mixer != nint.Zero)
            return true;
        
        SDL.LogInfo(SDL.LogCategory.Error, $"MixerDevice creation failed: {SDL.GetError()}");
        return false;
    }
    
    public static void InitializePath()
    {
        string baseDir = Folder.GetRandomFolder();

        mp3 = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".mp3"))!;

        if (string.IsNullOrEmpty(mp3))
        {
            SDL.LogInfo(SDL.LogCategory.Error, "MP3 file not found in the directory : " + baseDir);
            Mixer.DestroyMixer(mixer);
            Mixer.Quit();
            SDL.Quit();
        }
        else
        {
            SDL.LogInfo(SDL.LogCategory.Application, "Loaded Sound : " + mp3[mp3.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
        }
    }
    
    public static void Initialize()
    {
        if (!InitializeMixer())
            return;
        
        if (!InitializeMixerDevice())
            return;
        
        InitializePath();
        audio = Mixer.LoadAudio(mixer, mp3, predecode: true);
        if (audio == nint.Zero)
        {
            SDL.LogInfo(SDL.LogCategory.Error, $"Failed loading audio: {SDL.GetError()}");
            Quit();
            return;
        }
        
        track = Mixer.CreateTrack(mixer);
        if (track == nint.Zero || !Mixer.SetTrackAudio(track, audio))
        {
            SDL.LogInfo(SDL.LogCategory.Error, $"Failed creating track: {SDL.GetError()}");
            Quit();
        }
    }
    
    public static void PlaySound()
    {
        Mixer.PlayTrack(track, 0);
    }

    public static void Quit()
    {
        if(track != nint.Zero)
            Mixer.DestroyTrack(track);
        if(audio != nint.Zero)
            Mixer.DestroyAudio(audio);
        if(mixer != nint.Zero)
            Mixer.DestroyMixer(mixer);

        if (!string.IsNullOrEmpty(mp3))
        {
            SDL.LogInfo(SDL.LogCategory.Application, "Unloading Sound : " + mp3);
            mp3 = "";
        }
    
        Mixer.Quit();
    }
}
