using System.IO;
using System.Text.Json;

namespace AutoPowerTimeOut;

public class Settings
{
    public TimeOutLevel PluggedInScreen { get; set; } = TimeOutLevel.FiveMinutes;
    public TimeOutLevel PluggedInSleep { get; set; } = TimeOutLevel.FiveMinutes;

    public TimeOutLevel OnBatteryScreen { get; set; } = TimeOutLevel.ThreeMinutes;
    public TimeOutLevel OnBatterySleep { get; set; } = TimeOutLevel.ThreeMinutes;

    public LidPowerSleepButtonOption PluggedInPowerButton { get; set; } = LidPowerSleepButtonOption.Sleep;
    public LidPowerSleepButtonOption PluggedInSleepButton { get; set; } = LidPowerSleepButtonOption.Sleep;
    public LidPowerSleepButtonOption PluggedInLidButton { get; set; } = LidPowerSleepButtonOption.Sleep;

    public LidPowerSleepButtonOption OnBatteryPowerButton { get; set; } = LidPowerSleepButtonOption.Sleep;
    public LidPowerSleepButtonOption OnBatterySleepButton { get; set; } = LidPowerSleepButtonOption.Sleep;
    public LidPowerSleepButtonOption OnBatteryLidButton { get; set; } = LidPowerSleepButtonOption.Sleep;

    public bool ShowNotifications { get; set; } = true;

    public static Settings Load()
    {
        if (!File.Exists(Constants.SettingsPath))
        {
            return new Settings();
        }
        var json = File.ReadAllText(Constants.SettingsPath);
        return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(Constants.SettingsPath, json);
    }
}
