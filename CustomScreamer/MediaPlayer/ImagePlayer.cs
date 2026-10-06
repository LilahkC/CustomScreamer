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
        Window.Texture = Image.LoadTexture(Window.SDLRenderer, imagePath);
        if(Window.Texture != nint.Zero)
            Console.WriteLine("Loaded Image : " + imagePath[imagePath.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
        else 
            Console.WriteLine("Could not load image: " + imagePath);
    }

    public void ShowImage()
    {
        Window.SetShowWindow(true);

        SDL.RenderTexture(Window.SDLRenderer, Window.Texture, nint.Zero, nint.Zero);
        if (Window.Texture == nint.Zero)
        {
            Console.WriteLine("Could not load image: " + imagePath);
            return;
        }

        SDL.RenderPresent(Window.SDLRenderer);
        
        SDL.Delay(TimeToShowImage);
        
        SDL.RenderClear(Window.SDLRenderer);
        Window.SetShowWindow(false);
    }
    
    public void Quit()
    {
        if (Window.Texture != nint.Zero)
        {
            SDL.DestroyTexture(Window.Texture);
            Window.Texture = nint.Zero;
            
            if(!string.IsNullOrEmpty(imagePath))
                Console.WriteLine("Unloading Image : " + imagePath[imagePath.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
        }
    }
}
