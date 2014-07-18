using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using Microsoft.Reporting.WinForms;
using ReliefProLL;
using ReliefProMain.Model.Reports;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using ReliefProModel.Reports;

namespace ReliefProMain.ViewModel.Reports
{
    public class PlantSummaryVM : ViewModelBase
    {
        private List<PUsummaryGridDS> listPUReportDS;
        private List<PlantSummaryGridDS> listPlantReportDS;
        private string selectedCalcFun;
        public string SelectedCalcFun
        {
            get { return selectedCalcFun; }
            set
            {
                selectedCalcFun = value;
                this.OnPropertyChanged("SelectedCalcFun");
            }
        }

        public List<FlareSystem> listDischargeTo
        {
            get;
            set;
        }
        private ReportBLL report;
        public StackPanel StackpanelReport
        {
            get;
            set;
        }
        public PlantSummaryVM(List<Tuple<int, List<string>>> UnitPath)
        {
            listPlantReportDS = new List<PlantSummaryGridDS>();
            report = new ReportBLL();

            listDischargeTo = report.GetDisChargeTo();
            if (listDischargeTo.Count > 0)
            {
                string firstDischargeTo = listDischargeTo.First().FlareName;
                UnitPath.ForEach(p =>
                {
                    InitPUnitReportDS(firstDischargeTo, p.Item1, p.Item2);
                });
            }
        }
        private void InitPUnitReportDS(string ReportDischargeTo, int UnitID, List<string> UnitPath)
        {
            ReportBLL reportBLL = new ReportBLL(UnitID, UnitPath);
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
            List<EffectFactorModel> listEffect = new List<EffectFactorModel>();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("PlantDS", CreateReportDataSource(out listEffect)));
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("PlantEffectFactorDS", listEffect));
            reportViewer.RefreshReport();
            host.Child = reportViewer;

            StackpanelReport.Children.Add(host);
        }
        private List<PUsummaryReportSource> CreateReportDataSource(out List<EffectFactorModel> listEffect)
        {
            List<PUsummaryReportSource> listRS = new List<PUsummaryReportSource>();
            listRS = listPlantReportDS.Select(p => new PUsummaryReportSource
            {
                Device = p.ProcessUnit,
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

            var listEffectFactor = report.CalcPlantSummary(listPlantReportDS);
            listRS.AddRange(listEffectFactor);
            listEffect = CalcEffectFactor(listEffectFactor);
            return listRS;
        }

        private List<EffectFactorModel> CalcEffectFactor(List<PUsummaryReportSource> listEffectFactor)
        {
            double? W, T, MW, K;
            List<EffectFactorModel> lsitCalc = new List<EffectFactorModel>();
            PUsummaryReportSource RptDS = listEffectFactor.FirstOrDefault(p => p.Device == selectedCalcFun);
            W = RptDS.PowerReliefRate;
            T = RptDS.PowerT;
            MW = RptDS.PowerMWorSpGr;
            K = RptDS.PowerCpCv;

            EffectFactorModel effectPressure = new EffectFactorModel();
            EffectFactorModel effectMach = new EffectFactorModel();

            if (MW != null && MW != 0)
            {
                effectPressure.Power = (W * W * T) / MW;
                if (K != null && K != 0)
                    effectMach.Power = (W * W * T) / (MW * K);
            }
            W = RptDS.WaterReliefRate;
            T = RptDS.WaterT;
            MW = RptDS.WaterMWorSpGr;
            K = RptDS.WaterCpCv;
            if (MW != null && MW != 0)
            {
                effectPressure.Water = (W * W * T) / MW;
                if (K != null && K != 0)
                    effectMach.Water = (W * W * T) / (MW * K);
            }
            W = RptDS.AirReliefRate;
            T = RptDS.AirT;
            MW = RptDS.AirMWorSpGr;
            K = RptDS.AirCpCv;
            if (MW != null && MW != 0)
            {
                effectPressure.Air = (W * W * T) / MW;
                if (K != null && K != 0)
                    effectMach.Air = (W * W * T) / (MW * K);
            }
            lsitCalc.Add(effectPressure);
            lsitCalc.Add(effectMach);
            return lsitCalc;
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
