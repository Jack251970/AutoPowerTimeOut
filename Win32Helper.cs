using System.Runtime.InteropServices;
using Windows.Win32;

namespace AutoPowerTimeOut;

internal class Win32Helper
{
    // Import required power profile functions
    [DllImport("powrprof.dll", SetLastError = true)]
    public static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, out IntPtr ActivePolicyGuid);

    [DllImport("powrprof.dll", SetLastError = true)]
    public static extern uint PowerSetActiveScheme(IntPtr UserRootPowerKey, [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid);

    [DllImport("powrprof.dll", SetLastError = true)]
    public static extern uint PowerWriteACValueIndex(IntPtr RootPowerKey, [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfTimeOutSettingsGuid, [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
        uint AcValueIndex);

    [DllImport("powrprof.dll", SetLastError = true)]
    public static extern uint PowerWriteDCValueIndex(IntPtr RootPowerKey, [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
        [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfTimeOutSettingsGuid, [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
        uint DcValueIndex);

    // Release the pointer returned by PowerGetActiveScheme
    [DllImport("kernel32.dll")]
    public static extern void LocalFree(IntPtr hMem);

    // GUIDs
    public static Guid GUID_VIDEO_SUBGROUP = new("7516b95f-f776-4464-8c53-06167f40cc99");
    public static Guid GUID_VIDEO_TIMEOUT = new("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e");

    public static Guid GUID_SLEEP_SUBGROUP = new("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");
    public static Guid GUID_SLEEP_IDLE = new("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

    public static bool SetForegroundWindow(nint handle)
    {
        return PInvoke.SetForegroundWindow(new(handle));
    }
}
