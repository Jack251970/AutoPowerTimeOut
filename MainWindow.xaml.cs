using System.Windows;

namespace AutoPowerTimeOut;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Application.Current.MainWindow = this;
        InitializeComponent();
    }
}
