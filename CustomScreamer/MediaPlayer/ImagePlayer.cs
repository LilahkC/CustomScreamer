using CustomScreamer.Renderer;
using SDL3;

namespace CustomScreamer.MediaPlayer;

public class ImagePlayer
{
    private string imagePath = "";
    public const uint TimeToShowImage = 500;

    public void InitializePath()
    {
        string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ListOfScreamers", "Screamer");

        imagePath = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))!;

        if (!string.IsNullOrEmpty(imagePath))
            return;
        
        Console.WriteLine(".jpg, .png or .jpeg file not found in the directory : " + baseDir);
    }

    public void Initialize()
    {
        InitializePath();
        Window.Texture = Image.LoadTexture(Window.Renderer, imagePath);
        if(Window.Texture != nint.Zero)
            Console.WriteLine("Loaded Image : " + imagePath[imagePath.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
        else 
            Console.WriteLine("Could not load image: " + imagePath);
    }

    public void ShowImage()
    {
        SDL.WindowPosCentered();
        SDL.ShowWindow(Window.SDLWindowHandle);

        SDL.RenderTexture(Window.Renderer, Window.Texture, nint.Zero, nint.Zero);
        if (Window.Texture == nint.Zero)
        {
            Console.WriteLine("Could not load image: " + imagePath);
            return;
        }

        SDL.RenderPresent(Window.Renderer);
        
        SDL.Delay(TimeToShowImage);
        
        SDL.RenderClear(Window.Renderer);
        SDL.HideWindow(Window.SDLWindowHandle);
    }
    
    public void Quit()
    {
    }
}
