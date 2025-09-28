using System;
using System.IO;

namespace AutoPowerTimeOut;

public static class Constants
{
    public const string AppName = "Auto Power Time-out";

    public static readonly string ExecutablePath = Path.Combine(AppContext.BaseDirectory, "AutoPowerTimeOut.exe");

    public static readonly string SettingsPath = Path.Combine(AppContext.BaseDirectory, "Settings.json");

    public static readonly string IconPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
}
