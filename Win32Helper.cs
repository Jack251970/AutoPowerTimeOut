using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace AutoPowerTimeOut;

internal class Win32Helper
{
    public static Guid GUID_VIDEO_TIMEOUT = new("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e");
    public static Guid GUID_SLEEP_IDLE = new("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

    /// <summary>
    /// Sets the display and sleep timeout values for both AC and DC power states.
    /// </summary>
    /// <param name="acDisplayMinutes">
    /// The number of minutes to turn off display when plugged in (AC).
    /// </param>
    /// <param name="dcDisplayMinutes">
    /// The number of minutes to turn off display when on battery (DC).
    /// </param>
    /// <param name="acSleepMinutes">
    /// The number of minutes to put the computer to sleep when plugged in (AC).
    /// </param>
    /// <param name="dcSleepMinutes">
    /// The number of minutes to put the computer to sleep when on battery (DC).
    /// </param>
    /// <exception cref="Exception"></exception>
    public unsafe static void SetDisplayAndSleepTimeout(
        uint acDisplayMinutes, uint dcDisplayMinutes, uint acSleepMinutes, uint dcSleepMinutes)
    {
        try
        {
            var res = PInvoke.PowerGetActiveScheme(null, out var pActiveScheme);
            if (res != WIN32_ERROR.ERROR_SUCCESS)
            {
                throw new Exception($"PowerGetActiveScheme failed: {res}");
            }

            var activeScheme = *pActiveScheme;

            // Set AC values
            _ = PInvoke.PowerWriteACValueIndex(null, activeScheme, PInvoke.GUID_VIDEO_SUBGROUP, GUID_VIDEO_TIMEOUT, acDisplayMinutes * 60);
            _ = PInvoke.PowerWriteACValueIndex(null, activeScheme, PInvoke.GUID_SLEEP_SUBGROUP, GUID_SLEEP_IDLE, acSleepMinutes * 60);

            // Set DC values
            _ = PInvoke.PowerWriteDCValueIndex(null, activeScheme, PInvoke.GUID_VIDEO_SUBGROUP, GUID_VIDEO_TIMEOUT, dcDisplayMinutes * 60);
            _ = PInvoke.PowerWriteDCValueIndex(null, activeScheme, PInvoke.GUID_SLEEP_SUBGROUP, GUID_SLEEP_IDLE, dcSleepMinutes * 60);

            // Commit changes
            _ = PInvoke.PowerSetActiveScheme(null, activeScheme);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to set power settings", ex);
        }
    }

    public static bool SetForegroundWindow(nint handle)
    {
        return PInvoke.SetForegroundWindow(new(handle));
    }

    public static bool SetForegroundWindow(Window window)
    {
        return SetForegroundWindow(new WindowInteropHelper(window).Handle);
    }

    private static bool IsNotificationSupported()
    {
        // Notifications only supported on Windows 10 19041+
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            Environment.OSVersion.Version.Build >= 19041;
    }

    public static bool ShowNotification(string message)
    {
        if (!IsNotificationSupported())
        {
            return false;
        }

        try
        {
            new ToastContentBuilder()
                .AddText("Auto Power Time-out", hintMaxLines: 1)
                .AddText(message)
                .AddAppLogoOverride(new Uri(Constants.IconPath))
                .Show();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to show notification: {ex.Message}");
            return false;
        }
    }
}
