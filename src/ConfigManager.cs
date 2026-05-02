namespace EOCS.ConfigManager;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text.Json;

[SupportedOSPlatform("windows")]
public class ConfigManager
{
    private readonly Dictionary<string, object> _settings = new Dictionary<string, object>();
    
    private readonly string _filePath;

    public ConfigManager(string filePath = "config.json")
    {
        _filePath = filePath;
    }

    public void Load()
    {
        if (!File.Exists(_filePath))
        {
            Console.WriteLine($"[ConfigManager.cs:25] File '{_filePath}' not found. Creating default empty config.");
            Save();
            return;
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, options);

            if (data != null)
            {
                _settings.Clear();
                foreach (var kvp in data)
                {
                    _settings[kvp.Key] = ConvertJsonElementToObject(kvp.Value);
                }
            }
            
            Console.WriteLine("[ConfigManager.cs:51] Loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ConfigManager.cs:55] Error loading file: {ex.Message}");
        }
    }

    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        string json = JsonSerializer.Serialize(_settings, options);
        File.WriteAllText(_filePath, json);
    }

    public T Get<T>(string key, T defaultValue = default!)
    {
        if (_settings.TryGetValue(key, out object? value))
        {
            try
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                return (T)Convert.ChangeType(value, typeof(T))!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConfigManager.cs:84] Type mismatch for key '{key}'. Expected {typeof(T)}, got {value.GetType()}. Error: {ex.Message}");
                return defaultValue;
            }
        }
        else
        {
            return defaultValue;
        }
    }

    private object ConvertJsonElementToObject(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString() ?? string.Empty;
            case JsonValueKind.Number:
                if (element.TryGetInt32(out int i)) return i;
                if (element.TryGetInt64(out long l)) return l;
                if (element.TryGetDouble(out double d)) return d;
                return element.GetDouble();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
                return DBNull.Value;
            default:
                return element.GetRawText(); 
        }
    }
}