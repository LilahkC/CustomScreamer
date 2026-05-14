namespace CustomScreamer.MediaPlayer;

public class GifPlayer
{
    private string gifPath = "";
    
    public void InitializePath()
    {
        string baseDir = Path.Combine("ListOfScreamers", "Foxy");

        gifPath = Directory.GetFiles(baseDir, "*.*").FirstOrDefault(f => f.EndsWith(".gif"))!;

        if (!string.IsNullOrEmpty(gifPath))
            return;
        
        Console.WriteLine(".gif file not found in the directory : " + baseDir);
    }
}
