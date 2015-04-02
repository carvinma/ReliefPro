using ReliefProMain.View.Common;
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
        public static double SplashValue;
        /// <summary>
        /// 显示启动界面
        /// </summary>
        public static void Show(double maxValue=5)
        {
            if (splashScreen == null)
            {
                lock (sslock)
                {
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        CreateSplashScreen(maxValue);
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
        private static void CreateSplashScreen(double maxValue)
        {
            splashScreen = new SplashScreen();
            splashScreen.SProgressValue = maxValue;
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
                    SplashValue = splashScreen.ProgressValue;
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
                    splashScreen = null;

                }));
            }
        }
    }


    public class LongTimeTaskAbc 
    {
        private Thread m_threadWorking;
        private PsvWaitingDlg m_dlgWaiting;

        public void Start(PsvWaitingDlg dlg)
        {
            m_dlgWaiting = dlg;
            m_threadWorking = new Thread(Calcing);
            m_threadWorking.Start();
        }

        private void Calcing()
        {
            //lll
            m_dlgWaiting.Show1(10);
            //222
            Thread.Sleep(2000);
            m_dlgWaiting.Show2(10);
            Thread.Sleep(2000);
            m_dlgWaiting.Show3(10);
            Thread.Sleep(2000);
        }


    }
}
