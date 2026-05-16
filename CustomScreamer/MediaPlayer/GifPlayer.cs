using CustomScreamer.Renderer;
namespace CustomScreamer.MediaPlayer;
using SDL3;

public class GifPlayer
{
    private string gifPath = "";
    Window window;
    private bool Running = true;
    
    public void InitializePath()
    {
        string baseDir = Path.Combine("ListOfScreamers", "Foxy");

        gifPath = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".gif"))!;

        if (!string.IsNullOrEmpty(gifPath))
            return;
        
        Console.WriteLine(".gif file not found in the directory : " + baseDir);
    }

    public void Initialize(Window Window)
    {
        InitializePath();
        window = Window;
    }
    
    public void ShowGif()
    {
        SDL.ShowWindow(window.GetWindow());
        nint animation = Image.LoadAnimation(gifPath);

        if (animation == nint.Zero)
        {
            Console.WriteLine("Could not load gif: " + gifPath);
            return;
        }
    }
    
    public void Quit()
    {
        Running = false;
    }
}
