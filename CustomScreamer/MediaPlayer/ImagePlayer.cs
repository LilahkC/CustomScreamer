namespace CustomScreamer.MediaPlayer;

public class ImagePlayer
{
    private string imagePath = "";
    private nint texture = nint.Zero;
    //private readonly Window window = new();

    public void InitializePath()
    {
        string baseDir = Path.Combine("ListOfScreamers", "Foxy");

        imagePath = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))!;

        if (!string.IsNullOrEmpty(imagePath))
            return;
        
        Console.WriteLine(".jpg, .png or .jpeg file not found in the directory : " + baseDir);
    }

    public void Initialize()
    {
        InitializePath();
        
        //texture = Image.LoadTexture(Window.Renderer, imagePath);
        //
        //if (texture == nint.Zero)
        //    Console.WriteLine("Could not load image: " + imagePath);
    }

    public void ShowImage()
    { 
        
    }
}
