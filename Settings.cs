using System.IO;
using System.Text.Json;

namespace AutoPowerTimeOut;

public class Settings
{
    public TimeOutLevel PluggedInScreen { get; set; } = TimeOutLevel.FiveMinutes;
    public TimeOutLevel PluggedInSleep { get; set; } = TimeOutLevel.FiveMinutes;

    public TimeOutLevel OnBatteryScreen { get; set; } = TimeOutLevel.ThreeMinutes;
    public TimeOutLevel OnBatterySleep { get; set; } = TimeOutLevel.ThreeMinutes;

    private static readonly string SettingsPath = Path.Combine(AppContext.BaseDirectory, "Settings.json");

    public static Settings Load()
    {
        if (!File.Exists(SettingsPath))
        {
            return new Settings();
        }
        var json = File.ReadAllText(SettingsPath);
        return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(SettingsPath, json);
    }
}
