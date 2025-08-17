using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace AutoPowerTimeOut;

[INotifyPropertyChanged]
public partial class MainWindow : Window
{
    public string FileVersion { get; }

    public List<TimeOutModel> PluggedInItemSource { get; set; } = TimeOutModel.GetValues(true);
    public List<TimeOutModel> OnBatteryItemSource { get; set; } = TimeOutModel.GetValues(false);

    [ObservableProperty]
    private TimeOutLevel _pluggedInScreen;

    [ObservableProperty]
    private TimeOutLevel _pluggedInSleep;

    [ObservableProperty]
    private TimeOutLevel _onBatteryScreen;

    [ObservableProperty]
    private TimeOutLevel _onBatterySleep;

    [ObservableProperty]
    private bool _showNotifications;

    public MainWindow()
    {
        FileVersion = GetVersion();
        PluggedInScreen = App.Settings.PluggedInScreen;
        PluggedInSleep = App.Settings.PluggedInSleep;
        OnBatteryScreen = App.Settings.OnBatteryScreen;
        OnBatterySleep = App.Settings.OnBatterySleep;
        ShowNotifications = App.Settings.ShowNotifications;
        InitializeComponent();
        PropertyChanged += MainWindow_PropertyChanged;
    }

    private static string GetVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return version == null ? "v0.0.0" : $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    private void MainWindow_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PluggedInScreen):
                App.Settings.PluggedInScreen = PluggedInScreen;
                break;
            case nameof(PluggedInSleep):
                App.Settings.PluggedInSleep = PluggedInSleep;
                break;
            case nameof(OnBatteryScreen):
                App.Settings.OnBatteryScreen = OnBatteryScreen;
                break;
            case nameof(OnBatterySleep):
                App.Settings.OnBatterySleep = OnBatterySleep;
                break;
            case nameof(ShowNotifications):
                App.Settings.ShowNotifications = ShowNotifications;
                break;
        }
        App.Settings.Save();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        Hide();
        e.Cancel = true; // Prevent the window from closing
    }
}
