using CustomScreamer.MediaPlayer;
using CustomScreamer.Renderer;

namespace CustomScreamer.Game;

public class Game
{
    public const float Chance = 10f;
    public const float Time = 1f;

    private readonly SoundPlayer soundPlayer = new();
    private readonly ImagePlayer imagePlayer = new();
    private readonly GifPlayer gifPlayer = new();
    private readonly Utils utils = new();
    private Window window;
    
    public Game(Window Window)
    {
        window = Window;
    }

    public void Initialize()
    {
        soundPlayer.Initialize();
        imagePlayer.Initialize(window);
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
        if (utils.Random(Chance, Time))
        {
            PlayScreamer();
            Console.WriteLine("Screamer is screaming");
        }
    }
    
    public void Quit()
    {
        soundPlayer.Quit();
        imagePlayer.Quit();
        //gifPlayer.Quit();
    }
}
