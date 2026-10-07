using System.Text.Json;
using SDL3;

namespace CustomScreamer.Utils;

public static class Settings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CustomScreamer",
        "settings.json");
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
    
    private class Data
    {
        public float Chance { get; init; } = 0.25f;
        public float Time { get; init; } = 1f;
        public bool OpenSettingsOnLaunch { get; init; } = true;
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
        Game.Game.OpenSettingsOnLaunch = data.OpenSettingsOnLaunch;
            
        SDL.LogInfo(SDL.LogCategory.Application, "Loaded Settings");
    }
    
    public static void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        
        Data data = new()
        {
            Chance = Game.Game.Chance,
            Time = Game.Game.Time,
            OpenSettingsOnLaunch = Game.Game.OpenSettingsOnLaunch
        };
        
        File.WriteAllText(FilePath, JsonSerializer.Serialize(data, JsonOptions));
        
        SDL.LogInfo(SDL.LogCategory.Application, "Saved Settings");
    }
}