using System.ComponentModel;
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

    private readonly Settings _settings;

    public MainWindow()
    {
        _settings = Settings.Load();
        PluggedInScreen = _settings.PluggedInScreen;
        PluggedInSleep = _settings.PluggedInSleep;
        OnBatteryScreen = _settings.OnBatteryScreen;
        OnBatterySleep = _settings.OnBatterySleep;
        InitializeComponent();
        PropertyChanged += MainWindow_PropertyChanged;
    }

    private void MainWindow_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PluggedInScreen):
                _settings.PluggedInScreen = PluggedInScreen;
                break;
            case nameof(PluggedInSleep):
                _settings.PluggedInSleep = PluggedInSleep;
                break;
            case nameof(OnBatteryScreen):
                _settings.OnBatteryScreen = OnBatteryScreen;
                break;
            case nameof(OnBatterySleep):
                _settings.OnBatterySleep = OnBatterySleep;
                break;
        }
        _settings.Save();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
