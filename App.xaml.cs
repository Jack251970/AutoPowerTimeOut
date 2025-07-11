using System.Windows;
using System.Windows.Threading;

namespace AutoPowerTimeOut;

public partial class App : Application
{
    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(e.Exception.ToString());
        e.Handled = true;
    }
}
