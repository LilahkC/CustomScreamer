using CustomScreamer.Renderer;
using SDL3;

namespace CustomScreamer.MediaPlayer;

public class ImagePlayer
{
    private string imagePath = "";
    private readonly nint texture = nint.Zero;

    public void InitializePath()
    {
        const string BaseDir = "ListOfScreamers\\Foxy";

        imagePath = Directory.GetFiles(BaseDir, "*.*").FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))!;

        if (!string.IsNullOrEmpty(imagePath))
            return;
        
        Console.WriteLine(".jpg, .png or .jpeg file not found in the directory : " + BaseDir);
    }

    public void Initialize()
    {
        InitializePath();
    }

    public void ShowImage()
    {
       // Image.LoadTexture(window.Renderer, imagePath);
       // 
       // if (texture == nint.Zero)
       //     Console.WriteLine("Could not load image: " + imagePath);
    }
}
