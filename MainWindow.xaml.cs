using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AutoPowerTimeOut;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public List<TimeOutModel> PluggedInItemSource { get; set; } = TimeOutModel.GetValues(true);
    public List<TimeOutModel> OnBatteryItemSource { get; set; } = TimeOutModel.GetValues(false);

    private TimeOutLevel _pluggedInScreen;
    public TimeOutLevel PluggedInScreen
    {
        get => _pluggedInScreen;
        set
        {
            if (_pluggedInScreen != value)
            {
                _pluggedInScreen = value;
                OnPropertyChanged();
            }
        }
    }

    private TimeOutLevel _pluggedInSleep;
    public TimeOutLevel PluggedInSleep
    {
        get => _pluggedInSleep;
        set
        {
            if (_pluggedInSleep != value)
            {
                _pluggedInSleep = value;
                OnPropertyChanged();
            }
        }
    }

    private TimeOutLevel _onBatteryScreen;
    public TimeOutLevel OnBatteryScreen
    {
        get => _onBatteryScreen;
        set
        {
            if (_onBatteryScreen != value)
            {
                _onBatteryScreen = value;
                OnPropertyChanged();
            }
        }
    }

    private TimeOutLevel _onBatterySleep;
    public TimeOutLevel OnBatterySleep
    {
        get => _onBatterySleep;
        set
        {
            if (_onBatterySleep != value)
            {
                _onBatterySleep = value;
                OnPropertyChanged();
            }
        }
    }

    public string FileVersion { get; }

    private bool _showNotifications;
    public bool ShowNotifications
    {
        get => _showNotifications;
        set
        {
            if (_showNotifications != value)
            {
                _showNotifications = value;
                OnPropertyChanged();
            }
        }
    }

    public MainWindow()
    {
        PluggedInScreen = App.Settings.PluggedInScreen;
        PluggedInSleep = App.Settings.PluggedInSleep;
        OnBatteryScreen = App.Settings.OnBatteryScreen;
        OnBatterySleep = App.Settings.OnBatterySleep;
        ShowNotifications = App.Settings.ShowNotifications;
        FileVersion = GetVersion();
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
