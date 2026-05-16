using CustomScreamer.Renderer;
using SDL3;

namespace CustomScreamer.MediaPlayer;

public class ImagePlayer
{
    private string imagePath = "";
    public uint TimeToShowImage = 500;
    Window window;

    public void InitializePath()
    {
        string baseDir = Path.Combine("ListOfScreamers", "Screamer");

        imagePath = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))!;

        if (!string.IsNullOrEmpty(imagePath))
            return;
        
        Console.WriteLine(".jpg, .png or .jpeg file not found in the directory : " + baseDir);
    }

    public void Initialize(Window Window)
    {
        window = Window;
        InitializePath();
        window.Texture = Image.LoadTexture(window.Renderer, imagePath);
    }

    public void ShowImage()
    {
        SDL.ShowWindow(window.GetWindow());

        if (window.Texture == nint.Zero)
        {
            Console.WriteLine("Could not load image: " + imagePath);
            return;
        }
        Console.WriteLine("Loaded Texture with image : " + imagePath);
        SDL.RenderTexture(window.Renderer, window.Texture, nint.Zero, nint.Zero);
        window.RenderPresent();
        
        SDL.Delay(TimeToShowImage);
        
        window.ClearRenderer();
        SDL.HideWindow(window.GetWindow());
    }
    
    public void Quit()
    {
        SDL.DestroyTexture(window.Texture);
    }
}
