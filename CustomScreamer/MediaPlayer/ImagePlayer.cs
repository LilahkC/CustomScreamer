using CustomScreamer.Renderer;
using SDL3;

namespace CustomScreamer.MediaPlayer;

public class ImagePlayer
{
    private string imagePath = "";
    public readonly uint TimeToShowImage = 500;

    public void InitializePath()
    {
        string baseDir = Path.Combine("ListOfScreamers", "Screamer");

        imagePath = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))!;

        if (!string.IsNullOrEmpty(imagePath))
            return;
        
        Console.WriteLine(".jpg, .png or .jpeg file not found in the directory : " + baseDir);
    }

    public void Initialize()
    {
     //   window = Window;
     //   InitializePath();
     //   window.Texture = Image.LoadTexture(window.Renderer, imagePath);
     //   if(window.Texture != nint.Zero)
     //       Console.WriteLine("Loaded Texture with image : " + imagePath);
     //   else
     //       Console.WriteLine("Could not load image: " + imagePath);
    }

    public void ShowImage()
    {
    //    SDL.WindowPosCentered();
    //    SDL.ShowWindow(window.GetWindow());
//
    //    SDL.RenderTexture(window.Renderer, window.Texture, nint.Zero, nint.Zero);
    //    if (window.Texture == nint.Zero)
    //    {
    //        Console.WriteLine("Could not load image: " + imagePath);
    //        return;
    //    }
//
    //    SDL.RenderPresent(window.GetWindow());
    //    
    //    SDL.Delay(TimeToShowImage);
    //    
    //    SDL.RenderClear(window.GetWindow());
    //    SDL.HideWindow(window.GetWindow());
    }
    
    public void Quit()
    {
    }
}
