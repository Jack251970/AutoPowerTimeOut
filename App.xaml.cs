using iNKORE.UI.WPF.Modern.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using MouseButtons = System.Windows.Forms.MouseButtons;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using Timer = System.Timers.Timer;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;

namespace AutoPowerTimeOut;

public partial class App : Application
{
    public static Settings Settings { get; private set; } = null!;

    private NotifyIcon _notifyIcon = null!;
    private readonly ContextMenu _contextMenu = new();

    private readonly Timer _timer = new();

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
                    Win32Helper.BringToForegroundEx(Current.MainWindow, Current.MainWindow.Topmost);

                    break;
                case MouseButtons.Right:

                    _contextMenu.IsOpen = true;
                    // Get context menu handle and bring it to the foreground at the topmost
                    if (PresentationSource.FromVisual(_contextMenu) is HwndSource hwndSource)
                    {
                        Win32Helper.BringToForegroundEx(hwndSource.Handle, true);
                    }
                    _contextMenu.Focus();

                    break;
            }
        };
        SetupPowerSettings(true);
        AutoStartup();
        Current.MainWindow = new MainWindow();
        _timer.Elapsed += Timer_Elapsed;
        _timer.Interval = 1000 * 60 * 3; // 3 minutes since it is the recommended time-out for battery mode
        _timer.AutoReset = true;
        _timer.Start();
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        SetupPowerSettings(false);
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

    private static void SetupPowerSettings(bool showFailedNotification)
    {
        try
        {
            // Get setted values from settings
            var preferredAcDisplayMinutes = (uint)Settings.PluggedInScreen;
            var preferredDcDisplayMinutes = (uint)Settings.OnBatteryScreen;
            var preferredAcSleepMinutes = (uint)Settings.PluggedInSleep;
            var preferredDcSleepMinutes = (uint)Settings.OnBatterySleep;

            // Check if the current settings match the preferred settings
            if (Win32Helper.GetDisplayAndSleepTimeout(
                out var acDisplayMinutes,
                out var dcDisplayMinutes,
                out var acSleepMinutes,
                out var dcSleepMinutes) &&
                preferredAcDisplayMinutes == acDisplayMinutes &&
                preferredDcDisplayMinutes == dcDisplayMinutes &&
                preferredAcSleepMinutes == acSleepMinutes &&
                preferredDcSleepMinutes == dcSleepMinutes)
            {
                if (Settings.ShowNotifications && showFailedNotification)
                {
                    Win32Helper.ShowNotification("Power settings have been updated successfully.");
                }
                return;   
            }

            // Set the power settings
            Win32Helper.SetDisplayAndSleepTimeout(
                preferredAcDisplayMinutes,
                preferredDcDisplayMinutes,
                preferredAcSleepMinutes,
                preferredDcSleepMinutes
            );
        }
        catch (Exception e)
        {
            if (showFailedNotification)
            {
                Win32Helper.ShowNotification($"Failed to update power settings: {e.Message}");
            }
            return;
        }

        if (Settings.ShowNotifications)
        {
            Win32Helper.ShowNotification("Power settings have been updated successfully.");
        }
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
        _timer.Dispose();
    }
}
