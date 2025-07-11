using System.Runtime.InteropServices;

namespace AutoPowerTimeOut;

public static class TimeOutManager
{
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
    public static void SetDisplayAndSleepTimeout(uint acDisplayMinutes, uint dcDisplayMinutes,
                                                 uint acSleepMinutes, uint dcSleepMinutes)
    {
        var pActiveScheme = IntPtr.Zero;
        try
        {
            var res = Win32Helper.PowerGetActiveScheme(IntPtr.Zero, out pActiveScheme);
            if (res != 0)
            {
                throw new Exception($"PowerGetActiveScheme failed: {res}");
            }

            var activeScheme = Marshal.PtrToStructure<Guid>(pActiveScheme);

            // Set AC values
            _ = Win32Helper.PowerWriteACValueIndex(IntPtr.Zero, activeScheme, Win32Helper.GUID_VIDEO_SUBGROUP, Win32Helper.GUID_VIDEO_TIMEOUT, acDisplayMinutes * 60);
            _ = Win32Helper.PowerWriteACValueIndex(IntPtr.Zero, activeScheme, Win32Helper.GUID_SLEEP_SUBGROUP, Win32Helper.GUID_SLEEP_IDLE, acSleepMinutes * 60);

            // Set DC values
            _ = Win32Helper.PowerWriteDCValueIndex(IntPtr.Zero, activeScheme, Win32Helper.GUID_VIDEO_SUBGROUP, Win32Helper.GUID_VIDEO_TIMEOUT, dcDisplayMinutes * 60);
            _ = Win32Helper.PowerWriteDCValueIndex(IntPtr.Zero, activeScheme, Win32Helper.GUID_SLEEP_SUBGROUP, Win32Helper.GUID_SLEEP_IDLE, dcSleepMinutes * 60);

            // Commit changes
            _ = Win32Helper.PowerSetActiveScheme(IntPtr.Zero, activeScheme);
        }
        finally
        {
            if (pActiveScheme != IntPtr.Zero)
            {
                Win32Helper.LocalFree(pActiveScheme);
            }
        }
    }
}
