using CustomScreamer.Renderer;
using CustomScreamer.Utils;
using SDL3;

namespace CustomScreamer.MediaPlayer;

public static class ImagePlayer
{
    private static readonly string ImageBaseFolder = AppDomain.CurrentDomain.BaseDirectory + "ListOfScreamers" + "Screamer";
    private static string image;
    public const uint TimeToShowImage = 500;

    public static void InitializePath()
    {
        string baseDir = string.IsNullOrEmpty(Folder.GetRandomFolder()) ? ImageBaseFolder : Folder.GetRandomFolder();

        image = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))!;
        
        if (!string.IsNullOrEmpty(image))
            return;
        
        SDL.LogInfo(SDL.LogCategory.Error, ".jpg, .png or .jpeg file not found in the directory : " + baseDir);
    }

    public static void Initialize()
    {
        InitializePath();
        Window.Texture = Image.LoadTexture(Window.SDLRenderer, image);
        if(Window.Texture != nint.Zero)
            SDL.LogInfo(SDL.LogCategory.Application,"Loaded Image : " + image[image.LastIndexOf("Screamer", StringComparison.Ordinal)..]);
        else 
            SDL.LogInfo(SDL.LogCategory.Error,"Could not load image: " + image);
    }

    public static void ShowImage()
    {
        Window.SetShowWindow(true);

        SDL.RenderTexture(Window.SDLRenderer, Window.Texture, nint.Zero, nint.Zero);

        SDL.RenderPresent(Window.SDLRenderer);
        
        SDL.Delay(TimeToShowImage);
        
        SDL.RenderClear(Window.SDLRenderer);
        Window.SetShowWindow(false);
    }
    
    public static void Quit()
    {
        if (Window.Texture != nint.Zero)
        {
            SDL.DestroyTexture(Window.Texture);
            Window.Texture = nint.Zero;
            
        }

        if (!string.IsNullOrEmpty(image))
        {
            SDL.LogInfo(SDL.LogCategory.Application,"Unloading Image : " + image);
            image = string.Empty;
        }
    }
}
