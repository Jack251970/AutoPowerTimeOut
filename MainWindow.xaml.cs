using System.Windows;

namespace AutoPowerTimeOut;

public partial class MainWindow : Window
{
    public List<TimeOutModel> PluggedInItemSource { get; set; } = TimeOutModel.GetValues(true);
    public List<TimeOutModel> OnBatteryItemSource { get; set; } = TimeOutModel.GetValues(false);

    public TimeOutLevel PluggedInScreen { get; set; } = TimeOutLevel.FiveMinutes;
    public TimeOutLevel PluggedInSleep { get; set; } = TimeOutLevel.FiveMinutes;

    public TimeOutLevel OnBatteryScreen { get; set; } = TimeOutLevel.ThreeMinutes;
    public TimeOutLevel OnBatterySleep { get; set; } = TimeOutLevel.ThreeMinutes;

    public MainWindow()
    {
        Application.Current.MainWindow = this;
        InitializeComponent();
    }
}
