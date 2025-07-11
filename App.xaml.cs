using System.Windows;
using System.Windows.Threading;

namespace AutoPowerTimeOut;

public partial class App : Application
{
    public static Settings Settings { get; private set; } = null!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Settings = Settings.Load();
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(e.Exception.ToString());
        e.Handled = true;
    }
}
