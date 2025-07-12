using iNKORE.UI.WPF.Modern.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using MouseButtons = System.Windows.Forms.MouseButtons;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace AutoPowerTimeOut;

public partial class App : Application
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
        exitItem.Click += (o, e) =>
        {
            _contextMenu.IsOpen = false;
            Current.Shutdown();
        };
        _contextMenu.Items.Add(exitItem);
        _notifyIcon = new NotifyIcon
        {
            Text = "Auto Power Time-out",
            Icon = AutoPowerTimeOut.Properties.Resources.Icon,
            Visible = true
        };
        _notifyIcon.MouseClick += (o, e) =>
        {
            switch (e.Button)
            {
                case MouseButtons.Left:

                    // Show the main window and bring it to the foreground
                    Current.MainWindow.Show();
                    Win32Helper.BringToForegroundEx(Current.MainWindow);

                    break;
                case MouseButtons.Right:

                    _contextMenu.IsOpen = true;
                    // Get context menu handle and bring it to the foreground
                    if (PresentationSource.FromVisual(_contextMenu) is HwndSource hwndSource)
                    {
                        Win32Helper.BringToForegroundEx(hwndSource.Handle);
                    }
                    _contextMenu.Focus();

                    break;
            }
        };
        SetupPowerSettings();
        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        AutoStartup();
        Current.MainWindow = new MainWindow();
    }

    [Conditional("RELEASE")]
    private static void AutoStartup()
    {
        try
        {
            StartupHelper.CheckIsEnabled();
        }
        catch (Exception e)
        {
            Win32Helper.ShowNotification($"Failed to register logon task: {e.Message}");
        }
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
        try
        {
            Win32Helper.SetDisplayAndSleepTimeout(
                (uint)Settings.PluggedInScreen,
                (uint)Settings.OnBatteryScreen,
                (uint)Settings.PluggedInSleep,
                (uint)Settings.OnBatterySleep
            );
        }
        catch (Exception e)
        {
            Win32Helper.ShowNotification($"Failed to update power settings: {e.Message}");
            return;
        }
        Win32Helper.ShowNotification("Power settings have been updated successfully.");
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show(e.Exception.ToString());
        e.Handled = true;
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        ToastNotificationManagerCompat.Uninstall();
        _contextMenu.IsOpen = false;
        _notifyIcon?.Dispose();
        SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
    }
}
