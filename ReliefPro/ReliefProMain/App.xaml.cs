using System;
using System.Windows;
using System.Windows.Threading;
using ReliefProMain.View;

namespace ReliefProMain
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {
            
        }

        protected override void OnDeactivated(EventArgs e)
        {
            
        }
        protected  void OnStartup(object sender, StartupEventArgs e)
        {
            //TowerScenarioView mainF = new TowerScenarioView();
            
            MainWindow mainF = new MainWindow();
            mainF.WindowState = WindowState.Maximized;
            mainF.Show();
            
        }
        private void APP_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message,"Message Box");
            e.Handled = true;
        }
    }
}
