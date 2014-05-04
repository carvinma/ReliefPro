
using System;
using System.Windows;
using System.Windows.Threading;
using ReliefProMain.View;
namespace ReliefProMain
{
    public partial class App : Application
    {
        public App()
        {
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //ShowInfo showinfo = new ShowInfo();
           //showinfo.Show();
            MainWindow mainF = new MainWindow();
            mainF.WindowState = WindowState.Maximized;
            mainF.Show();

            //注册Application_Error
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

        }

        //异常处理逻辑
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //处理完后，我们需要将Handler=true表示已此异常已处理过
            e.Handled = true;
        }
    }
}
