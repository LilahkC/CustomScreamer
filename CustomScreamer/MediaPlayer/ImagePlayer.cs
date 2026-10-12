using CustomScreamer.Renderer;
using CustomScreamer.Utils;
using SDL3;

namespace CustomScreamer.MediaPlayer;

public static class ImagePlayer
{
    public const uint TimeToShowImage = 500;
    private static readonly string[] SupportedExtensions = [".jpg", ".jpeg", ".png"];
    private static string imagePath;
    
    private static bool IsSupportedImage(string path) => SupportedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
    
    public static void InitializePath()
    {
        string baseDir = Folder.GetFolder();

        imagePath = Directory.EnumerateFiles(baseDir).FirstOrDefault(IsSupportedImage);

        if (imagePath is null)
            SDL.LogInfo(SDL.LogCategory.Application, $".jpg, .jpeg or .png file not found in the directory: {baseDir}");
    }

    public static void Load()
    {
        if (imagePath is null)
            InitializePath();

        Window.Texture = Image.LoadTexture(Window.SDLRenderer, imagePath!);

        if (Window.Texture == nint.Zero)
        {
            SDL.LogInfo(SDL.LogCategory.Application, $"Could not load image: {imagePath}");
            return;
        }

        SDL.LogInfo(SDL.LogCategory.Application, $"Loaded image: {imagePath}");
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

    public static void Unload()
    {
        SDL.LogInfo(SDL.LogCategory.Application, $"Unloaded Image: {imagePath}");
        imagePath = null;

        if (Window.Texture == nint.Zero)
            return;
 
        SDL.DestroyTexture(Window.Texture);
        Window.Texture = nint.Zero;
    }

    public static void Quit()
    {
        if (Window.Texture == nint.Zero)
            return;

        SDL.DestroyTexture(Window.Texture);
        Window.Texture = nint.Zero;
    }
}
