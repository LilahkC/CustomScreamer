using CustomScreamer.MediaPlayer;
using CustomScreamer.Renderer;
using CustomScreamer.Utils;
using SDL3;

namespace CustomScreamer.Game;

public static class Game
{
    public static float Chance = 0.25f;
    public static float Time = 1f;
    public static bool OpenSettingsOnLaunch = true;
    
    private static ulong nextRoll;
    
    private static void ScheduleNextRoll() => nextRoll = SDL.GetTicks() + (ulong)(Time * 1000);
    
    public static void Initialize()
    {
        SoundPlayer.Initialize();
        ScheduleNextRoll();
    }
    
    private static void PlayScreamer()
    {
        Folder.SetRandomFolder();
        
        SoundPlayer.Load();
        SoundPlayer.PlaySound();
        
        ImagePlayer.Load();
        ImagePlayer.ShowImage();
        
        SDL.LogInfo(SDL.LogCategory.Application, "Screamer is screaming");
        ImagePlayer.Unload();
        
        //gifPlayer.ShowGif();
    }
    
    public static void Update()
    {
        if (!Window.InGame || SDL.GetTicks() < nextRoll)
            return;

        if (Random.Shared.NextSingle() < Chance / 100f)
            PlayScreamer();

        ScheduleNextRoll();
    }
    
    public static void Quit()
    {
        SoundPlayer.Quit();
        ImagePlayer.Quit();
        //gifPlayer.Quit();
    }
}
