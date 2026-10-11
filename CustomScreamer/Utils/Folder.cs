using CustomScreamer.MediaPlayer;
using NativeFileDialogExtendedSharp;

namespace CustomScreamer.Utils;

public static class Folder
{
    private static string BaseDir;
    private static Random random = new Random();
    public static string RandomFolder;
    
    public static void SetRandomFolder()
    {
        string[] directories = Directory.GetDirectories(BaseDir);
        int directoriesNumber = directories.Length;
        
        RandomFolder = directoriesNumber > 0 ? directories[random.Next(directoriesNumber)] : BaseDir;
        
        ImagePlayer.Initialize();
        SoundPlayer.Initialize();
    }

    public static void SetFolder(string folder)
    {
        BaseDir = folder;
    }
    
    public static string GetBaseDir()
    {
        return BaseDir;
    }

    public static string GetRandomFolder()
    {
        return RandomFolder;
    }
}
