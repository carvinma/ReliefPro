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
        public PsvWaitingDlg()
        {
            InitializeComponent();
        }
        public PsvWaitingDlg(LongTimeTaskAbc task)
        {
            m_task = task;
            InitializeComponent();
        }
        public void Calc1(string str)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.tbPrompt1.Text = str;
            }));
        }
        public void Calc2(string str)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.tbPrompt2.Text = str;
            }));
        }
        public void Calc3(string str)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.tbPrompt3.Text = str;
            }));
        }
       

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            m_task.Start(this);
        }
    }
}
