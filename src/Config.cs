[SupportedOSPlatform("windows")]
public static class Config
{
    private static readonly ConfigManager _manager = new ConfigManager("config.json");
    public static bool IsLoaded { get; private set; } = false;
    public static void Init()
    {
        _manager.Load();
        IsLoaded = true;
    }

    public static T Get<T>(string key, T defaultValue = default)
    {
        if (!IsLoaded)
        {
            Init(); 
        }
        return _manager.Get(key, defaultValue);
    }
    
    public static void Save()
    {
        _manager.Save();
    }
}