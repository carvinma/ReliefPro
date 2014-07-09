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

            foreach (var PSV in listPSV)
            {
                PUsummaryGridDS gridDs = new PUsummaryGridDS();
                gridDs.psv = PSV;
                gridDs.PowerDS = reportBLL.ScenarioBag.FirstOrDefault(p => p.ScenarioName == reportBLL.ScenarioName[1] && p.dbPath == PSV.dbPath);
                gridDs.WaterDS = reportBLL.ScenarioBag.FirstOrDefault(p => p.ScenarioName == reportBLL.ScenarioName[2] && p.dbPath == PSV.dbPath);
                gridDs.AirDS = reportBLL.ScenarioBag.FirstOrDefault(p => p.ScenarioName == reportBLL.ScenarioName[3] && p.dbPath == PSV.dbPath);
                gridDs.SteamDS = reportBLL.ScenarioBag.FirstOrDefault(p => p.ScenarioName == reportBLL.ScenarioName[4] && p.dbPath == PSV.dbPath);
                gridDs.FireDS = reportBLL.ScenarioBag.FirstOrDefault(p => p.ScenarioName == reportBLL.ScenarioName[5] && p.dbPath == PSV.dbPath);
                InitControllingSingleScenarioDS(ref gridDs);
                model.listGrid.Add(gridDs);
            }
            model.listGrid = reportBLL.CalcMaxSum(model.listGrid);
        }

        private void InitControllingSingleScenarioDS(ref PUsummaryGridDS gridDs)
        {
            double maxPowerDS = !string.IsNullOrEmpty(gridDs.PowerDS.ReliefLoad) ? double.Parse(gridDs.PowerDS.ReliefLoad) : 0;
            double maxWaterDS = !string.IsNullOrEmpty(gridDs.WaterDS.ReliefLoad) ? double.Parse(gridDs.WaterDS.ReliefLoad) : 0;
            double maxAirDS = !string.IsNullOrEmpty(gridDs.AirDS.ReliefLoad) ? double.Parse(gridDs.AirDS.ReliefLoad) : 0;
            double maxSteamDS = !string.IsNullOrEmpty(gridDs.SteamDS.ReliefLoad) ? double.Parse(gridDs.SteamDS.ReliefLoad) : 0;
            double maxFireDS = !string.IsNullOrEmpty(gridDs.FireDS.ReliefLoad) ? double.Parse(gridDs.FireDS.ReliefLoad) : 0;
            List<double> MaxList = new List<double> { maxPowerDS, maxWaterDS, maxAirDS, maxSteamDS, maxFireDS };
            var v = MaxList.Select((m, index) => new { index, m }).OrderByDescending(n => n.m).Take(1);
            int Index = 0;
            foreach (var t in v)
            {
                Index = t.index; break;
            }
            switch (Index)
            {
                case 0: gridDs.SingleDS = gridDs.PowerDS; break;
                case 1: gridDs.SingleDS = gridDs.WaterDS; break;
                case 2: gridDs.SingleDS = gridDs.AirDS; break;
                case 3: gridDs.SingleDS = gridDs.SteamDS; break;
                case 4: gridDs.SingleDS = gridDs.FireDS; break;
                default: break;
            }

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
            export.ExportToExcel(model.listGrid, "PUsummary.xlsx");
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
