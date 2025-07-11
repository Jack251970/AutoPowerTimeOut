using iNKORE.UI.WPF.Modern.Controls;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace AutoPowerTimeOut;

public partial class App : System.Windows.Application
{
    public static Settings Settings { get; private set; } = null!;

    private NotifyIcon _notifyIcon = null!;
    private readonly ContextMenu _contextMenu = new();

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Settings = Settings.Load();
        var exitItem = new MenuItem
        {
            Header = "Exit",
            Icon = new FontIcon { Glyph = "\ue7e8" }
        };
        _contextMenu.Items.Add(exitItem);
        _notifyIcon = new NotifyIcon
        {
            Text = "Auto Power Time-out",
            Visible = true
        };
        _notifyIcon.MouseClick += (o, e) =>
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Current.MainWindow.Show();
                    break;
                case MouseButtons.Right:

                    _contextMenu.IsOpen = true;
                    // Get context menu handle and bring it to the foreground
                    if (PresentationSource.FromVisual(_contextMenu) is HwndSource hwndSource)
                    {
                        Win32Helper.SetForegroundWindow(hwndSource.Handle);
                    }

                    _contextMenu.Focus();
                    break;
            }
        };
        SetupPowerSettings();
        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
    }

    private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode == PowerModes.Resume)
        {
            SetupPowerSettings();
        }
    }

    private static void SetupPowerSettings()
    {
        TimeOutManager.SetDisplayAndSleepTimeout(
            (uint)Settings.PluggedInScreen,
            (uint)Settings.OnBatteryScreen,
            (uint)Settings.PluggedInSleep,
            (uint)Settings.OnBatterySleep
        );
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show(e.Exception.ToString());
        e.Handled = true;
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _contextMenu.IsOpen = false;
        _notifyIcon?.Dispose();
        SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
    }
}
