using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace ReliefProMain
{
    internal class SplashScreenManager
    {
        private static AutoResetEvent ManualRestEvent = new AutoResetEvent(false);
        private static SplashScreen splashScreen;
        private static object sslock = new object();
        /// <summary>
        /// 显示启动界面
        /// </summary>
        public static void Show()
        {
            if (splashScreen == null)
            {
                lock (sslock)
                {
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        CreateSplashScreen();
                    }));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                    ManualRestEvent.WaitOne();
                }
            }
        }

        /// <summary>
        ///实例化启动界面并显示
        /// </summary>
        private static void CreateSplashScreen()
        {
            splashScreen = new SplashScreen();
            splashScreen.Show();
            ManualRestEvent.Set();
            Dispatcher.Run();
        }

        /// <summary>
        /// 给启动界面发送加载信息
        /// </summary>
        /// <param name="msg"></param>
        public static void SentMsgToScreen(string msg)
        {
            if (splashScreen != null)
            {
                splashScreen.Dispatcher.BeginInvoke(new Action(() =>
                {
                    splashScreen.Message = msg;
                    splashScreen.ProgressValue += 1;
                }));
            }
        }

        /// <summary>
        /// 关闭启动界面
        /// </summary>
        public static void Close()
        {
            if (splashScreen != null)
            {
                splashScreen.Dispatcher.BeginInvoke(new Action(() =>
                {
                    splashScreen.Close();
                }));
            }
        }
    }
}
