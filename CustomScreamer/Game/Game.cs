using CustomScreamer.MediaPlayer;

namespace CustomScreamer.Game;

public class Game
{
    public const float Chance = 0.5f;
    public const float Time = 1f;

    private readonly SoundPlayer soundPlayer = new();
    private readonly ImagePlayer imagePlayer = new();
    private readonly Utils utils = new();

    public void Initialize()
    {
        soundPlayer.Initialize();
        imagePlayer.Initialize();
    }
    
    private void PlayScreamer()
    {
        soundPlayer.PlaySound();
    }
    
    public void Update()
    {
        if (utils.Random(Chance, Time))
        {
            PlayScreamer();
        }
    }
    
    public void Quit()
    {
        soundPlayer.Quit();
    }
}
