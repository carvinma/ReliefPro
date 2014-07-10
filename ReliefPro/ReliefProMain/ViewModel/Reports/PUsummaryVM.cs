using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using ReliefProBLL;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model.Reports;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using ReliefProModel.Reports;

namespace ReliefProMain.ViewModel.Reports
{
    public class PUsummaryVM : ViewModelBase
    {
        private ReportBLL reportBLL;
        PUsummaryGridDS SumDs = new PUsummaryGridDS();
        StackPanel stackPanel;
        public ICommand OKCMD { get; set; }
        public ICommand BtnReportCMD { get; set; }
        public ICommand ExportExcelCMD { get; set; }
        public PUsummaryModel model { get; set; }
        List<string> listScenario = new List<string> { "PowerDS", "WaterDS", "AirDS", "SteamDS", "FireDS" };
        List<string> listProperty = new List<string> { "ReliefLoad", "ReliefMW", "ReliefTemperature", "ReliefZ" };
        public StackPanel Stackpanel
        {
            get { return stackPanel; }
            set { stackPanel = value; }
        }
        public PUsummaryVM(List<string> ReportPath)
        {
            BtnReportCMD = new DelegateCommand<object>(BtnReprotClick);
            ExportExcelCMD = new DelegateCommand<object>(BtnExportExcel);
            reportBLL = new ReportBLL(ReportPath);
            CreateControl(reportBLL.GetDisChargeTo());

            model = new PUsummaryModel();
            model.listGrid = new List<PUsummaryGridDS>();
            InitModel("ALL");
        }
        private void InitModel(string ReportDischargeTo)
        {
            List<PSV> listPSV = reportBLL.PSVBag.ToList();
            if (ReportDischargeTo != "ALL")
                listPSV = listPSV.Where(p => p.DischargeTo == ReportDischargeTo).ToList();
            model.listGrid = reportBLL.GetPuReprotDS(listPSV);
            model.listGrid = reportBLL.CalcMaxSum(model.listGrid);
        }

        private void BtnReprotClick(object obj)
        {
            if (obj != null)
            {
                InitModel(obj.ToString());
            }
        }
        private void BtnExportExcel(object obj)
        {
            ExportLib.ExportExcel export = new ExportLib.ExportExcel();
            export.ExportToExcelPUsummary(model.listGrid, "PUsummary.xlsx");
        }
        private void CreateControl(List<FlareSystem> lstFlareSystem)
        {
            Stackpanel = new StackPanel();
            Stackpanel.Orientation = Orientation.Horizontal;
            int i = 1;
            foreach (var flareSystem in lstFlareSystem)
            {
                Button btn = new Button();
                btn.Content = flareSystem.FlareName;
                btn.Command = BtnReportCMD;
                btn.Margin = new System.Windows.Thickness(20, 0, 0, 0); i++;
                btn.CommandParameter = flareSystem.FlareName;
                Stackpanel.Children.Add(btn);
            }
            Button btnALL = new Button();
            btnALL.Content = "ALL";
            btnALL.Command = BtnReportCMD;
            btnALL.CommandParameter = btnALL.Content;
            btnALL.Width = 50;
            btnALL.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            Stackpanel.Children.Add(btnALL);
        }
    }
}
