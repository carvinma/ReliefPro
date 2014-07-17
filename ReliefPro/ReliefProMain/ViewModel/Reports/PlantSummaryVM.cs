using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using Microsoft.Reporting.WinForms;
using ReliefProLL;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using ReliefProModel.Reports;

namespace ReliefProMain.ViewModel.Reports
{
    public class PlantSummaryVM : ViewModelBase
    {
        private List<PUsummaryGridDS> listPUReportDS;
        private List<PlantSummaryGridDS> listPlantReportDS;
        public StackPanel StackpanelReport
        {
            get;
            set;
        }
        public PlantSummaryVM(List<Tuple<string, List<string>>> UnitPath)
        {
            listPlantReportDS = new List<PlantSummaryGridDS>();
            ReportBLL report = new ReportBLL();
            List<FlareSystem> listDischargeTo = report.GetDisChargeTo();
            if (listDischargeTo.Count > 0)
            {
                string firstDischargeTo = listDischargeTo.First().FlareName;
                UnitPath.ForEach(p =>
                {
                    InitPUnitReportDS(firstDischargeTo, p.Item2);
                });
            }
        }
        private void InitPUnitReportDS(string ReportDischargeTo, List<string> UnitPath)
        {
            ReportBLL reportBLL = new ReportBLL(UnitPath);
            List<PSV> listPSV = reportBLL.PSVBag.ToList();
            listPSV = listPSV.Where(p => p.DischargeTo == ReportDischargeTo).ToList();
            listPUReportDS = reportBLL.GetPuReprotDS(listPSV);
            listPlantReportDS.Add(reportBLL.GetPlantReprotDS(listPUReportDS, 0));
        }
        private void CreateReport()
        {
            WindowsFormsHost host = new WindowsFormsHost();
            host.Width = 1340;
            host.Height = 500;
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local;
            reportViewer.LocalReport.ReportEmbeddedResource = "ReliefProMain.View.Reports.PlantSummaryRpt.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("PlantDS", CreateReportDataSource()));
            reportViewer.RefreshReport();
            host.Child = reportViewer;

            StackpanelReport.Children.Add(host);
        }
        private List<PUsummaryReportSource> CreateReportDataSource()
        {
            List<PUsummaryReportSource> listRS = new List<PUsummaryReportSource>();
            listRS = listPlantReportDS.Select(p => new PUsummaryReportSource
            {
                //  Device = p.psv.PSVName,
                ScenarioReliefRate = GetDouble(p.ControllingDS.ReliefLoad),
                ScenarioVolumeRate = GetDouble(p.ControllingDS.ReliefVolumeRate),
                ScenarioMWorSpGr = GetDouble(p.ControllingDS.ReliefMW),
                ScenarioT = GetDouble(p.ControllingDS.ReliefTemperature),
                ScenarioZ = GetDouble(p.ControllingDS.ReliefZ),
                ScenarioName = p.ControllingDS.ScenarioName,

                PowerReliefRate = GetDouble(p.PowerDS.ReliefLoad),
                PowerVolumeRate = GetDouble(p.PowerDS.ReliefVolumeRate),
                PowerMWorSpGr = GetDouble(p.PowerDS.ReliefMW),
                PowerT = GetDouble(p.PowerDS.ReliefTemperature),
                PowerZ = GetDouble(p.PowerDS.ReliefZ),

                WaterReliefRate = GetDouble(p.WaterDS.ReliefLoad),
                WaterVolumeRate = GetDouble(p.WaterDS.ReliefVolumeRate),
                WaterMWorSpGr = GetDouble(p.WaterDS.ReliefMW),
                WaterT = GetDouble(p.WaterDS.ReliefTemperature),
                WaterZ = GetDouble(p.WaterDS.ReliefZ),

                AirReliefRate = GetDouble(p.AirDS.ReliefLoad),
                AirVolumeRate = GetDouble(p.AirDS.ReliefVolumeRate),
                AirMWorSpGr = GetDouble(p.AirDS.ReliefMW),
                AirT = GetDouble(p.AirDS.ReliefTemperature),
                AirZ = GetDouble(p.AirDS.ReliefZ)
            }).ToList();
            return listRS;
        }
        private double? GetDouble(string value)
        {
            double reslut = 0;
            if (double.TryParse(value, out reslut))
                return reslut;
            return null;
        }
    }
}
