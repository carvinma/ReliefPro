using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReliefProMain.View.Common
{
    /// <summary>
    /// PsvWaitingDlg.xaml 的交互逻辑
    /// </summary>
    public partial class PsvWaitingDlg : Window
    {
        private readonly LongTimeTaskAbc m_task;
        private readonly List<string> m_lstCalcInfo;
        public PsvWaitingDlg()
        {
            InitializeComponent();
        }
        public PsvWaitingDlg(LongTimeTaskAbc task, List<string> lstCalcInfo)
        {
            m_task = task;
            m_lstCalcInfo = lstCalcInfo;
            InitializeComponent();
        }
        public void Show1(double value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.progress1.Value = value;
            }));
        }
        public void Show2(double value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.progress2.Value = value;
            }));
        }
        public void Show3(double value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.progress3.Value = value;
            }));
        }
        public void Show4(double value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.progress4.Value = value;
            }));
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            m_task.Start(this);
        }
    }
}
