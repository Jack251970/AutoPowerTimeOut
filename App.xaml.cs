using iNKORE.UI.WPF.Modern.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using Timer = System.Timers.Timer;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using System.Threading;

namespace AutoPowerTimeOut;

public partial class App : Application, IDisposable, ISingleInstanceApp
{
    public static Settings Settings { get; private set; } = null!;

    private SystemTrayIcon _notifyIcon = null!;
    private readonly ContextMenu _contextMenu = new();

    private readonly Timer _timer = new();

    private static bool _disposed;
    // To prevent two disposals running at the same time.
    private static readonly Lock _disposingLock = new();

    [STAThread]
    public static void Main()
    {
        // Start the application as a single instance
        if (SingleInstance<App>.InitializeAsFirstInstance())
        {
            using var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }

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
        _notifyIcon = new SystemTrayIcon
        {
            Tooltip = Constants.AppName,
            Icon = new(Constants.IconPath)
        };
        _notifyIcon.Show();
        _notifyIcon.LeftClick += (o, e) =>
        {
            // Show the main window and bring it to the foreground
            Current.MainWindow.Show();
            Win32Helper.BringToForegroundEx(Current.MainWindow, Current.MainWindow.Topmost);
        };
        _notifyIcon.RightClick += (o, e) =>
        {
            _contextMenu.IsOpen = true;
            // Get context menu handle and bring it to the foreground at the topmost
            if (PresentationSource.FromVisual(_contextMenu) is HwndSource hwndSource)
            {
                Win32Helper.BringToForegroundEx(hwndSource.Handle, true);
            }
            _contextMenu.Focus();
        };
        SetupPowerSettings(true);
        SetupLidPowerSleepButtonControlOptions(true);
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
        SetupLidPowerSleepButtonControlOptions(false);
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

    private static void SetupLidPowerSleepButtonControlOptions(bool showFailedNotification)
    {
        try
        {
            // Get setted values from settings
            var preferredAcPowerOption = Settings.PluggedInPowerButton;
            var preferredDcPowerOption = Settings.OnBatteryPowerButton;
            var preferredAcSleepOption = Settings.PluggedInSleepButton;
            var preferredDcSleepOption = Settings.OnBatterySleepButton;
            var preferredAcLidOption = Settings.PluggedInLidButton;
            var preferredDcLidOption = Settings.OnBatteryLidButton;

            // Check if the current settings match the preferred settings
            if (Win32Helper.GetLidPowerSleepButtonControlOptions(
                out var acPowerOption,
                out var dcPowerOption,
                out var acSleepOption,
                out var dcSleepOption,
                out var acLidOption,
                out var dcLidOption) &&
                preferredAcPowerOption == acPowerOption &&
                preferredDcPowerOption == dcPowerOption &&
                preferredAcSleepOption == acSleepOption &&
                preferredDcSleepOption == dcSleepOption &&
                preferredAcLidOption == acLidOption &&
                preferredDcLidOption == dcLidOption)
            {
                if (Settings.ShowNotifications && showFailedNotification)
                {
                    Win32Helper.ShowNotification("Lid, Power, and Sleep button control options have been updated successfully.");
                }
                return;
            }

            // Set the lid, power, and sleep button control options
            Win32Helper.SetLidPowerSleepButtonControlOptions(
                preferredAcPowerOption,
                preferredDcPowerOption,
                preferredAcSleepOption,
                preferredDcSleepOption,
                preferredAcLidOption,
                preferredDcLidOption
            );
        }
        catch (Exception e)
        {
            if (showFailedNotification)
            {
                Win32Helper.ShowNotification($"Failed to update Lid, Power, and Sleep button control options: {e.Message}");
            }
            return;
        }

        if (Settings.ShowNotifications)
        {
            Win32Helper.ShowNotification("Lid, Power, and Sleep button control options have been updated successfully.");
        }
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show(e.Exception.ToString());
        e.Handled = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        // Prevent two disposes at the same time.
        lock (_disposingLock)
        {
            if (!disposing)
            {
                return;
            }

            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        if (disposing)
        {
            ToastNotificationManagerCompat.Uninstall();
            _contextMenu.IsOpen = false;
            _notifyIcon?.Hide();
            _notifyIcon?.Dispose();
            _timer.Dispose();
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void OnSecondAppStarted()
    {
        // Show the main window and bring it to the foreground
        Current.MainWindow.Show();
        Win32Helper.BringToForegroundEx(Current.MainWindow, Current.MainWindow.Topmost);
    }
}
