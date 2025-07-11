using System;
using System.IO;

namespace AutoPowerTimeOut;

public static class Constants
{
    public static readonly string SettingsPath = Path.Combine(AppContext.BaseDirectory, "Settings.json");

    public static readonly string IconPath = Path.Combine(AppContext.BaseDirectory, "icon.png");
}
