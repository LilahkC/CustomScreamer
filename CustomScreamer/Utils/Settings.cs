using System.Text.Json;
using CustomScreamer.MediaPlayer;
using SDL3;

namespace CustomScreamer.Utils;

public static class Settings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CustomScreamer",
        "settings.json");
    
    private class Data
    {
        public float Chance { get; init; } = 0.25f;
        public float Time { get; init; } = 1f;
        public float Volume { get; init; } = 50f;
        public bool OpenSettingsOnLaunch { get; init; } = true;
        public string LastScreamerFolder { get; init; }
    }
    
    public static void Load()
    {
        if (!File.Exists(FilePath))
            return;
        
        Data data = JsonSerializer.Deserialize<Data>(File.ReadAllText(FilePath));
        if (data == null)
            return;
        
        Game.Game.Chance = data.Chance;
        Game.Game.Time = data.Time;
        SoundPlayer.Volume = data.Volume;
        Game.Game.OpenSettingsOnLaunch = data.OpenSettingsOnLaunch;
        
        if(data.LastScreamerFolder != null)
            Folder.SetUserFolder(data.LastScreamerFolder);
            
        SDL.LogInfo(SDL.LogCategory.Application, "Loaded Settings");
    }
    
    public static void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        
        Data data = new()
        {
            Chance = Game.Game.Chance,
            Time = Game.Game.Time,
            Volume = SoundPlayer.Volume,
            OpenSettingsOnLaunch = Game.Game.OpenSettingsOnLaunch,
            LastScreamerFolder = Folder.GetUserFolder()
        };
        
        File.WriteAllText(FilePath, JsonSerializer.Serialize(data));
        
        SDL.LogInfo(SDL.LogCategory.Application, "Saved Settings");
    }

    public static void DeleteSave()
    {
        Directory.Delete(Path.GetDirectoryName(FilePath)!);
    }
}