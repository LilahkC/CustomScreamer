using CustomScreamer.MediaPlayer;

namespace CustomScreamer.Utils;

public static class Folder
{
    private static readonly string BaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ListOfScreamers", "Screamer");
    private static readonly Random Random = new Random();
    private static string randomFolder;
    private static string userFolder;
    
    public static void SetRandomFolder()
    {
        string[] directories = Directory.GetDirectories(string.IsNullOrEmpty(userFolder) ? BaseFolder : userFolder);
        int directoriesNumber = directories.Length;
        
        randomFolder = directoriesNumber > 0 ? directories[Random.Next(directoriesNumber)] : string.IsNullOrEmpty(userFolder) ? BaseFolder : userFolder;
    }

    public static string GetUserFolder()
    {
        return userFolder;
    }
    
    public static void SetUserFolder(string folder)
    {
        userFolder = folder;
    }
    
    public static string[] GetSubDirectories()
    {
        string[] subDirectories;
        string folder = GetFolder();

        if (Directory.GetDirectories(folder).Length > 0)
        {
            subDirectories = Directory.GetDirectories(folder);
        }
        else
        {
            subDirectories = new string[1];
            subDirectories[0] = BaseFolder;
        }
        
        for(int i = 0; i < subDirectories.Length; i++)
            subDirectories[i] = new DirectoryInfo(subDirectories[i]).Name;
        
        return subDirectories;
    }
    
    public static string GetSubDirectoriesText()
    {
        string[] subDirectories = GetSubDirectories();
        string subDirectoriesText = string.Join(", !", subDirectories);
        
        return subDirectoriesText;
    }

    public static string GetFolder()
    {
        return string.IsNullOrEmpty(randomFolder) ? string.IsNullOrEmpty(userFolder) ? BaseFolder : userFolder : randomFolder;
    }
}
