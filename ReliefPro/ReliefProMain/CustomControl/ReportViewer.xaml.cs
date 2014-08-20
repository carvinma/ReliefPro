using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;
using ReliefProMain.Models.Reports;
using ReliefProModel.Reports;

namespace ReliefProMain.CustomControl
{
    /// <summary>
    /// ReportViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ReportViewer : UserControl
    {
        public ReportViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ReportNameProperty =
           DependencyProperty.Register("ReportName", typeof(string), typeof(ReportViewer),
                                       new PropertyMetadata(""));

        public static readonly DependencyProperty bRefreshProperty =
         DependencyProperty.Register("bRefresh", typeof(bool), typeof(ReportViewer),
                                     new PropertyMetadata(false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ctrl = dependencyObject as ReportViewer;
            if (ctrl != null)
            {
                if (dependencyPropertyChangedEventArgs.NewValue != null)
                    ctrl.ReportViewerLoad();
            }
        }
        public bool bRefresh
        {
            get { return (bool)GetValue(bRefreshProperty); }
            set { SetValue(bRefreshProperty, value); }
        }
        public string ReportName
        {
            get { return (string)GetValue(ReportNameProperty); }
            set { SetValue(ReportNameProperty, value); }
        }

        public void ReportViewerLoad()
        {
            if (!string.IsNullOrEmpty(ReportName) && RptDataSouce.ReportDS != null)
            {
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportEmbeddedResource = string.Format("ReliefProMain.View.Reports.{0}", ReportName);
                rptViewer.LocalReport.DataSources.Clear();
                RptDataSouce.ReportDS.ForEach(p => { rptViewer.LocalReport.DataSources.Add(p); });
                rptViewer.LocalReport.Refresh();
                rptViewer.RefreshReport();
            }
        }
    }
}
