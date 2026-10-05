using CustomScreamer.MediaPlayer;
using CustomScreamer.Renderer;

namespace CustomScreamer.Game;

public class Game
{
    public const float Chance = 40f;
    public const float Time = 1f;
    public bool CanPlay { get; set; } = false;

    private readonly SoundPlayer soundPlayer = new();
    private readonly ImagePlayer imagePlayer = new();
    private readonly GifPlayer gifPlayer = new();
    private readonly Utils.Utils utils = new();

    public void Initialize()
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
        if (!CanPlay || !utils.Random(Chance, Time)) 
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
