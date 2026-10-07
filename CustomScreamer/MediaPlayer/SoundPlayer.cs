namespace CustomScreamer.MediaPlayer;
using SDL3;

public class SoundPlayer
{
    private string mp3Path = "";
    private const float Volume = 0.45f;
    private nint mixer;
    private nint audio;
    private nint track;
    
    private static bool InitializeMixer()
    {
        if (Mixer.Init())
            return true;
        
        SDL.LogInfo(SDL.LogCategory.Error, $"Mixer Init Error: {SDL.GetError()}");
        return false;
    }

    private bool InitializeMixerDevice()
    {
        mixer = Mixer.CreateMixerDevice(SDL.AudioDeviceDefaultPlayback, nint.Zero);
        Mixer.SetMixerGain(mixer, Volume);
        
        if (mixer != nint.Zero)
            return true;
        
        SDL.LogInfo(SDL.LogCategory.Error, $"MixerDevice creation failed: {SDL.GetError()}");
        return false;
    }
    
    public void InitializePath()
    {
        string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ListOfScreamers", "Screamer");

        mp3Path = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".mp3"))!;

        if (string.IsNullOrEmpty(mp3Path))
        {
            SDL.LogInfo(SDL.LogCategory.Error, "MP3 file not found in the directory : " + baseDir);
            Mixer.DestroyMixer(mixer);
            Mixer.Quit();
            SDL.Quit();
        }
        else
        {
            SDL.LogInfo(SDL.LogCategory.Application, "Loaded Sound : " + mp3Path[mp3Path.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
        }
    }
    
    public void Initialize()
    {
        if (!InitializeMixer())
            return;
        
        if (!InitializeMixerDevice())
            return;
        
        InitializePath();
        audio = Mixer.LoadAudio(mixer, mp3Path, predecode: true);
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
    
    public void PlaySound()
    {
        Mixer.PlayTrack(track, 0);
    }

    public void Quit()
    {
        if(track != nint.Zero)
            Mixer.DestroyTrack(track);
        if(audio != nint.Zero)
            Mixer.DestroyAudio(audio);
        if(mixer != nint.Zero)
            Mixer.DestroyMixer(mixer);
        
        if(!string.IsNullOrEmpty(mp3Path))
            SDL.LogInfo(SDL.LogCategory.Application, "Unloading Sound : " + mp3Path[mp3Path.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
    
        Mixer.Quit();
    }
}
