using CustomScreamer.MediaPlayer;
using CustomScreamer.Renderer;

namespace CustomScreamer.Game;

public class Game
{
    public const float Chance = 40f;
    public const float Time = 1f;

    private static readonly SoundPlayer soundPlayer = new();
    private static readonly ImagePlayer imagePlayer = new();
    private readonly GifPlayer gifPlayer = new();
    private readonly Utils.Utils utils = new();

    public static void Initialize()
    {
        soundPlayer.Initialize();
        imagePlayer.Initialize();
        //gifPlayer.Initialize(window);
    }
    
    private void PlayScreamer()
    {
        soundPlayer.PlaySound();
        imagePlayer.ShowImage();
        //gifPlayer.ShowGif();
    }
    
    public void Update()
    {
        if (!Window.InGame || !utils.Random(Chance, Time)) 
            return;
        
        PlayScreamer();
        Console.WriteLine("Screamer is screaming");
    }
    
    public void Quit()
    {
        soundPlayer.Quit();
        imagePlayer.Quit();
        //gifPlayer.Quit();
    }
}
