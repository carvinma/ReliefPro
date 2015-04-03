using NHibernate;
using ReliefProMain.Models;
using ReliefProModel;
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
        public ISession SessionPlant { set; get; }
        public ISession SessionProtectedSystem { set; get; }
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { set; get; }
        public string EqName { get; set; }
        public string EqType { get; set; }
        public PSVModel CurrentModel { get; set; }
        public PSV psv { get; set; }

        public double CriticalLatent { get; set; }
        public string FileFullPath { get; set; }
        public string tempdir { get; set; }

        public double ReliefOHVaporSph { get; set; }
        public Latent latent = new Latent();
        public List<LatentProduct> lstLatentProduct = new List<LatentProduct>();
        public List<TowerFlashProduct> lstTowerFlashProduct = new List<TowerFlashProduct>();
        public int ErrorType;
        private readonly LongTimeTaskAbc m_task;
        private readonly List<string> m_lstCalcInfo;


        public PsvWaitingDlg()
        {
            InitializeComponent();
        }
        public PsvWaitingDlg(LongTimeTaskAbc task, List<string> lstCalcInfo)
        {
            InitializeComponent();
            m_task = task;
            m_lstCalcInfo = lstCalcInfo;
            if (m_lstCalcInfo.Count==4)
            {
                this.progressInfo1.Content = m_lstCalcInfo[0];
                this.progressInfo2.Content = m_lstCalcInfo[1];
                this.progressInfo3.Content = m_lstCalcInfo[2];
                this.progressInfo4.Content = m_lstCalcInfo[3];
            }
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
