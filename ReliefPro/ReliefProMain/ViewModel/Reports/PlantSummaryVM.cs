using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Reporting.WinForms;

using ReliefProMain.Models.Reports;
using ReliefProMain.View.Reports;
using ReliefProMain.ViewModel.Trees;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using ReliefProModel.Reports;
using UOMLib;
using ReliefProBLL;

namespace ReliefProMain.ViewModel.Reports
{
    public class PlantSummaryVM : ViewModelBase
    {
        private List<PUsummaryGridDS> listPUReportDS;
        private List<PlantSummaryGridDS> listPlantReportDS;
        private List<Tuple<int, List<string>>> ProcessUnitPath;
        private string selectedCalcFun = "100-30-30";
        public string SelectedCalcFun
        {
            get { return selectedCalcFun; }
            set
            {
                selectedCalcFun = value;
                this.OnPropertyChanged("SelectedCalcFun");
                ChangerDischargeTo(selectedDischargeTo);
            }
        }

        private string selectedDischargeTo;
        public string SelectedDischargeTo
        {
            get
            {
                return selectedDischargeTo;
            }
            set
            {
                selectedDischargeTo = value;
                this.OnPropertyChanged("SelectedDischargeTo");
                ChangerDischargeTo(value);
            }
        }
        public List<string> listPlantCalc { get; set; }
        public List<FlareSystem> listDischargeTo
        {
            get;
            set;
        }
        private ReportBLL report;

        private string reportName;
        public string ReportName
        {
            get { return reportName; }
            set
            {
                reportName = value;
                this.OnPropertyChanged("ReportName");
            }
        }

        private bool reportFresh;
        public bool ReportFresh
        {
            get { return reportFresh; }
            set
            {
                reportFresh = value;
                this.OnPropertyChanged("ReportFresh");
            }
        }
        public ICommand NextPlantCMD { get; set; }

        private ReportBLL reportBLL;
        private PlantSummaryView view;
        private List<PlantVM> PlantList;
        private ObservableCollection<PlantVM> PlantCollection;
        private void LoadSingPlant(List<Tuple<int, List<string>>> UnitPath)
        {
            ProcessUnitPath = UnitPath;
            listPlantReportDS = new List<PlantSummaryGridDS>();
            NextPlantCMD = new DelegateCommand<object>(BtnNextPlant);
            report = new ReportBLL(UnitPath[0].Item1, UnitPath[0].Item2);

            listPlantCalc = report.listPlantCalc;
            listDischargeTo = report.GetDisChargeTo();
            if (listDischargeTo.Count > 0)
            {
                selectedDischargeTo = listDischargeTo.First().FlareName;
                ChangerDischargeTo(selectedDischargeTo);
            }
            if (report != null) report.ClearSession();
        }
        public PlantSummaryVM(ObservableCollection<PlantVM> plantCollection)
        {
            PlantList = new List<PlantVM>();
            List<Tuple<int, List<string>>> UnitPath = new List<Tuple<int, List<string>>>();
            if (plantCollection.Count > 0)
            {
                PlantCollection = plantCollection;
                string dbPlantFile = string.Empty;
                string unitPath = string.Empty;
                List<string> ReportPath = new List<string>();

                foreach (PlantVM plantvm in PlantCollection)
                {
                    dbPlantFile = plantvm.PlantDir + @"\plant.mdb";
                    if (!PlantList.Contains(plantvm))
                    {
                        PlantList.Add(plantvm);

                        foreach (UnitVM uvm in plantvm.UnitCollection)
                        {
                            ReportPath.Clear();
                            unitPath = plantvm.PlantDir + @"\" + uvm.UnitName;
                            ReportPath.Add(dbPlantFile);
                            foreach (PSVM p in uvm.PSCollection)
                            {
                                ReportPath.Add(unitPath + @"\" + p.PSName + @"\protectedsystem.mdb");
                            }
                            Tuple<int, List<string>> t = new Tuple<int, List<string>>(uvm.ID, ReportPath);
                            UnitPath.Add(t);
                        }
                        view = new PlantSummaryView();
                        this.LoadSingPlant(UnitPath);
                        view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        view.WindowState = WindowState.Maximized;
                        view.DataContext = this;
                        view.ShowDialog();
                        break;
                    }
                }
            }
        }
        public PlantSummaryVM(List<Tuple<int, List<string>>> UnitPath)
        {
            ProcessUnitPath = UnitPath;
            listPlantReportDS = new List<PlantSummaryGridDS>();
            NextPlantCMD = new DelegateCommand<object>(BtnNextPlant);
            report = new ReportBLL(UnitPath[0].Item1, UnitPath[0].Item2);

            listPlantCalc = report.listPlantCalc;
            listDischargeTo = report.GetDisChargeTo();
            if (listDischargeTo.Count > 0)
            {
                selectedDischargeTo = listDischargeTo.First().FlareName;
                ChangerDischargeTo(selectedDischargeTo);
            }
            if (report != null) report.ClearSession();

        }
        private void ChangerDischargeTo(string ReportDischargeTo)
        {
            if (listPlantReportDS != null) listPlantReportDS.Clear();
            if (listDischargeTo.Count > 0)
            {
                ProcessUnitPath.ForEach(p =>
                {
                    InitPUnitReportDS(ReportDischargeTo, p.Item1, p.Item2);
                });

                CreateReport();
            }
        }
        private void InitPUnitReportDS(string ReportDischargeTo, int UnitID, List<string> UnitPath)
        {
            report = new ReportBLL(UnitID, UnitPath);
            List<PSV> listPSV = report.PSVBag.ToList();
            listPSV = listPSV.Where(p => p.DischargeTo == ReportDischargeTo).ToList();
            listPUReportDS = report.GetPuReprotDS(listPSV);
            PlantSummaryGridDS psDS = report.GetPlantReprotDS(listPUReportDS, 0);
            if (psDS != null)
            {
                //  listPlantReportDS.Clear();
                listPlantReportDS.Add(psDS);
            }
            // if (report != null) report.ClearSession();
        }
        private void CreateReport()
        {
            reportName = "PlantSummaryRpt.rdlc";

            List<EffectFactorModel> listEffect = new List<EffectFactorModel>();
            List<PlantReprotHead> reportHeadDS = new List<PlantReprotHead>();
            RptDataSouce.ReportDS.Clear();
            RptDataSouce.ReportDS.Add(new ReportDataSource("PlantDS", CreateReportDataSource(out listEffect, out reportHeadDS)));
            RptDataSouce.ReportDS.Add(new ReportDataSource("PlantEffectFactorDS", listEffect));
            RptDataSouce.ReportDS.Add(new ReportDataSource("PlantHeadDS", reportHeadDS));

            ReportFresh = !ReportFresh;
        }
        private List<PUsummaryReportSource> CreateReportDataSource(out List<EffectFactorModel> listEffect, out List<PlantReprotHead> reportHeadDS)
        {
            List<PUsummaryReportSource> listRS = new List<PUsummaryReportSource>();
            listRS = listPlantReportDS.Select(p => new PUsummaryReportSource
            {
                Device = p.ProcessUnit,
                ScenarioReliefRate = p.ControllingDS.ReliefLoad,
                ScenarioVolumeRate = p.ControllingDS.ReliefVolumeRate,
                ScenarioMWorSpGr = p.ControllingDS.ReliefMW,
                ScenarioT = p.ControllingDS.ReliefTemperature,
                ScenarioZ = p.ControllingDS.ReliefZ,
                ScenarioName = p.ControllingDS.ScenarioName,

                PowerReliefRate = p.PowerDS.ReliefLoad,
                PowerVolumeRate = p.PowerDS.ReliefVolumeRate,
                PowerMWorSpGr = p.PowerDS.ReliefMW,
                PowerT = p.PowerDS.ReliefTemperature,
                PowerZ = p.PowerDS.ReliefZ,

                WaterReliefRate = p.WaterDS.ReliefLoad,
                WaterVolumeRate = p.WaterDS.ReliefVolumeRate,
                WaterMWorSpGr = p.WaterDS.ReliefMW,
                WaterT = p.WaterDS.ReliefTemperature,
                WaterZ = p.WaterDS.ReliefZ,

                AirReliefRate = p.AirDS.ReliefLoad,
                AirVolumeRate = p.AirDS.ReliefVolumeRate,
                AirMWorSpGr = p.AirDS.ReliefMW,
                AirT = p.AirDS.ReliefTemperature,
                AirZ = p.AirDS.ReliefZ
            }).ToList();
            if (listPlantReportDS.Count > 0)
            {
                var listEffectFactor = report.CalcPlantSummary(listPlantReportDS);
                listRS.AddRange(listEffectFactor);
                listEffect = CalcEffectFactor(listEffectFactor);
            }
            else listEffect = new List<EffectFactorModel>();

            reportHeadDS = new List<PlantReprotHead>();
            var plantReprotHead = new PlantReprotHead();
            plantReprotHead.SummationFun = selectedCalcFun;
            plantReprotHead.PlantFlare = string.Empty;
            plantReprotHead.DischargeTo = SelectedDischargeTo;
            if (listEffect.Count > 0)
            {
                if (listEffect[0].Power >= listEffect[0].Water && listEffect[0].Power >= listEffect[0].Air)
                {
                    plantReprotHead.PlantFlare = "General Electric Power Failure";
                }
                else if (listEffect[0].Water >= listEffect[0].Power && listEffect[0].Water >= listEffect[0].Air)
                {
                    plantReprotHead.PlantFlare = "General Cooling Water Failure";
                }
                else if (listEffect[0].Air >= listEffect[0].Power && listEffect[0].Air >= listEffect[0].Water)
                {
                    plantReprotHead.PlantFlare = "General Instrument Air Failure";
                }

            }
            reportHeadDS.Add(plantReprotHead);
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
            if (T != null)
            {
                T = UnitConvert.Convert(UOMEnum.Pressure, "kPag", T.Value);
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
                effectPressure.Power = effectPressure.Power ?? 0;
                effectPressure.Water = effectPressure.Water ?? 0;
                effectPressure.Air = effectPressure.Air ?? 0;
                lsitCalc.Add(effectPressure);
                lsitCalc.Add(effectMach);
            }
            return lsitCalc;
        }

        private void BtnNextPlant(object obj)
        {
            List<Tuple<int, List<string>>> UnitPath = new List<Tuple<int, List<string>>>();
            if (PlantCollection.Count > 0)
            {
                string dbPlantFile = string.Empty;
                string unitPath = string.Empty;
                List<string> ReportPath = new List<string>();

                foreach (PlantVM plantvm in PlantCollection)
                {
                    dbPlantFile = plantvm.PlantDir + @"\plant.mdb";
                    if (!PlantList.Contains(plantvm))
                    {
                        PlantList.Add(plantvm);

                        foreach (UnitVM uvm in plantvm.UnitCollection)
                        {
                            ReportPath.Clear();
                            unitPath = plantvm.PlantDir + @"\" + uvm.UnitName;
                            ReportPath.Add(dbPlantFile);
                            foreach (PSVM p in uvm.PSCollection)
                            {
                                ReportPath.Add(unitPath + @"\" + p.PSName + @"\protectedsystem.mdb");
                            }
                            Tuple<int, List<string>> t = new Tuple<int, List<string>>(uvm.ID, ReportPath);
                            UnitPath.Add(t);
                        }
                        this.LoadSingPlant(UnitPath);
                        break;
                    }
                }
            }
        }


    }
}
