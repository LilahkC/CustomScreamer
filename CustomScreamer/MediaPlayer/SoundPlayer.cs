namespace CustomScreamer.MediaPlayer;
using SDL3;

public class SoundPlayer
{
    private string mp3Path = "";
    private const float Volume = 0.45f;
    private nint mixer;

    private static bool InitializeMixer()
    {
        if (Mixer.Init())
            return true;
        
        Console.WriteLine($"Mixer Init Error: {SDL.GetError()}");
        return false;
    }

    private bool InitializeMixerDevice()
    {
        mixer = Mixer.CreateMixerDevice(SDL.AudioDeviceDefaultPlayback, nint.Zero);
        
        if (mixer != nint.Zero) 
            return true;
        
        Console.WriteLine($"MixerDevice creation failed: {SDL.GetError()}");
        return false;
    }
    
    public void Initialize()
    {
        if (!InitializeMixer())
            return;
        
        if (!InitializeMixerDevice())
            return;
        
        InitializePath();
    }
    
    public void InitializePath()
    {
        const string BaseDir = "ListOfScreamers\\Foxy";

        mp3Path = Directory.GetFiles(BaseDir, "*.*").FirstOrDefault(f => f.EndsWith(".mp3"))!;

        if (string.IsNullOrEmpty(mp3Path))
        {
            Console.WriteLine("MP3 file not found in the directory : " + BaseDir);
            Mixer.DestroyMixer(mixer);
            Mixer.Quit();
            SDL.Quit();
        }
    }
    
    public void PlaySound()
    {
        Mixer.SetMixerGain(mixer, Volume);
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

    public void Quit()
    {
        Mixer.DestroyMixer(mixer);
        Mixer.Quit();
    }
}
