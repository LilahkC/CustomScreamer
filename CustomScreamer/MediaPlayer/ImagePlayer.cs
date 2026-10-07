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
        
        SDL.LogInfo(SDL.LogCategory.Error, ".jpg, .png or .jpeg file not found in the directory : " + baseDir);
    }

    public void Initialize()
    {
        InitializePath();
        Window.Texture = Image.LoadTexture(Window.SDLRenderer, imagePath);
        if(Window.Texture != nint.Zero)
            SDL.LogInfo(SDL.LogCategory.Application,"Loaded Image : " + imagePath[imagePath.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
        else 
            SDL.LogInfo(SDL.LogCategory.Error,"Could not load image: " + imagePath);
    }

    public void ShowImage()
    {
        Window.SetShowWindow(true);

        SDL.RenderTexture(Window.SDLRenderer, Window.Texture, nint.Zero, nint.Zero);

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
            
        }
        if(!string.IsNullOrEmpty(imagePath))
            SDL.LogInfo(SDL.LogCategory.Application,"Unloading Image : " + imagePath[imagePath.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
    }
}
