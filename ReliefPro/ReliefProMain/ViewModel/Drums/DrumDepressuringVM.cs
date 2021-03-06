﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

using NHibernate;

using ReliefProMain.Commands;
using ReliefProMain.Models;
using UOMLib;
using ReliefProMain.View.DrumDepressures;
using ReliefProBLL;
using ReliefProModel;
using ReliefProDAL;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumDepressuringVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public ICommand CalcCMD { get; set; }
        public ICommand DetailedCMD { get; set; }
        public ICommand DepressuringCurveCMD { get; set; }

        private KeyValuePair<int, double>[] Wt = new KeyValuePair<int, double>[16];
        private KeyValuePair<int, double>[] Pt = new KeyValuePair<int, double>[16];
        private string selectedShotCut = "Shortcut";
        public string SelectedShotCut
        {
            get { return selectedShotCut; }
            set
            {
                selectedShotCut = value;
                if (value == "Shortcut")
                {
                    isEnableFireHeatInput = false;
                }
                else
                {
                    isEnableFireHeatInput = true;
                }
                OnPropertyChanged("SelectedShotCut");
            }
        }
        private bool enableFireHeatInput;
        public bool isEnableFireHeatInput
        {
            get { return enableFireHeatInput; }
            set
            {
                enableFireHeatInput = value;
                OnPropertyChanged("isEnableFireHeatInput");
            }
        }

        public List<string> lstShortCut { get; set; }

        private string selectedDeprRqe = "21bar/min";
        public string SelectedDeprRqe
        {
            get { return selectedDeprRqe; }
            set
            {
                selectedDeprRqe = value;

                OnPropertyChanged("SelectedDeprRqe");
            }
        }
        public List<string> lstDeprRqe { get; set; }

        private string selectedHeatInput = "API 521";
        public string SelectedHeatInput
        {
            get { return selectedHeatInput; }
            set
            {
                selectedHeatInput = value;
                OnPropertyChanged("SelectedHeatInput");
            }
        }
        public List<string> lstHeatInput { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumDepressuringModel model { get; set; }
        public Scenario modelSC { get; set; }
        private DrumDepressuringBLL drumBLL;
        private bool isCalc = false;
        public DrumDepressuringVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(Calc);
            DetailedCMD = new DelegateCommand<object>(CalcDetailed);
            DepressuringCurveCMD = new DelegateCommand<object>(DepressuringCurve);

            lstShortCut = new List<string>(new[] { "Shortcut" });//, "PROII DEPR Unit"
            lstDeprRqe = new List<string>{
                "21bar/min","7bar/min","15min to 50% Design pressure","15min to 7barg"};   //,"Specify" 暂时去掉
            lstHeatInput = new List<string>(new[] { "API 521", "API 521 Scale", "API 2000", "API 2000 Scale" });

            drumBLL = new DrumDepressuringBLL(SessionPS, SessionPF);
            var drumModel = drumBLL.GetDrumPressuring(ScenarioID);
            drumModel = drumBLL.ReadConvertModel(drumModel);
            if (!string.IsNullOrEmpty(drumModel.HeatInputModel))
                selectedHeatInput = drumModel.HeatInputModel;
            if (!string.IsNullOrEmpty(drumModel.DepressuringRequirements))
                selectedDeprRqe = drumModel.DepressuringRequirements;
            if (!string.IsNullOrEmpty(drumModel.HeatInputModel))
                selectedShotCut = drumModel.ShortCut;
            
            ScenarioDAL scdal=new ScenarioDAL();
            modelSC = scdal.GetModel(ScenarioID, SessionPS);


            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model = new DrumDepressuringModel(drumModel);

            model.InitialPressureUnit = uomEnum.UserPressure;
            model.VaporDensityUnit = uomEnum.UserDensity;
            model.TotalVaporVolumeUnit = uomEnum.UserVolume;
            model.VesseldesignpressureUnit = uomEnum.UserPressure;
            model.TotalWettedAreaUnit = uomEnum.UserArea;
            model.InitialDepressuringRateUnit = uomEnum.UserMassRate;

            model.TimespecifyUnit = uomEnum.UserTime;
            model.CalculatedVesselPressureUnit = uomEnum.UserPressure;
            model.CalculatedDepressuringRateUnit = uomEnum.UserMassRate;

            model.DetailPUnit = uomEnum.UserPressure;
            model.DetailPTimeUnit = uomEnum.UserTime;
            model.TimeStepUnit = uomEnum.UserTime;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.InitialPressure = UnitConvert.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.InitialPressure);
            model.dbmodel.VaporDensity = UnitConvert.Convert(model.VaporDensityUnit, UOMLib.UOMEnum.Density.ToString(), model.VaporDensity);
            model.dbmodel.TotalVaporVolume = UnitConvert.Convert(model.TotalVaporVolumeUnit, UOMLib.UOMEnum.Volume.ToString(), model.TotalVaporVolume);
            model.dbmodel.Vesseldesignpressure = UnitConvert.Convert(model.VesseldesignpressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.Vesseldesignpressure);
            model.dbmodel.TotalWettedArea = UnitConvert.Convert(model.TotalWettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.TotalWettedArea);
            //model.dbmodel.ValveConstantforSonicFlow = uc.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.InitialDepressuringRate = UnitConvert.Convert(model.InitialDepressuringRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.InitialDepressuringRate);
            model.dbmodel.Timespecify = UnitConvert.Convert(model.TimespecifyUnit, UOMLib.UOMEnum.Time.ToString(), model.Timespecify);
            model.dbmodel.CalculatedVesselPressure = UnitConvert.Convert(model.CalculatedVesselPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.CalculatedVesselPressure);
            model.dbmodel.CalculatedDepressuringRate = UnitConvert.Convert(model.CalculatedDepressuringRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.CalculatedDepressuringRate);

            model.dbmodel.DeltaP = UnitConvert.Convert(model.DetailPUnit, UOMLib.UOMEnum.Pressure.ToString(), model.DetailP);
            model.dbmodel.DeltaPTime = UnitConvert.Convert(model.DetailPTimeUnit, UOMLib.UOMEnum.Time.ToString(), model.DetailPTime);
            model.dbmodel.TimeStep = UnitConvert.Convert(model.TimeStepUnit, UOMLib.UOMEnum.Time.ToString(), model.TimeStep);

            model.dbmodel.ShortCut = selectedShotCut;
            model.dbmodel.DepressuringRequirements = selectedDeprRqe;
            model.dbmodel.FireHeatInput = enableFireHeatInput;
            model.dbmodel.HeatInputModel = selectedHeatInput;
        }
        private void Calc(object obj)
        {
            isCalc = true;
            double tConstant = 0;
            double tRequire = 0;
            double pRequire = 0;
            double tempPress = 0;
            switch (selectedDeprRqe)
            {
                case "21bar/min":
                    tempPress = UnitConvert.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.InitialPressure);
                    pRequire = tempPress - 2.1;
                    tRequire = 1.0 / 60;

                    break;
                case "7bar/min":
                    tempPress = UnitConvert.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.InitialPressure);
                    pRequire = tempPress - 0.7;
                    tRequire = 1.0 / 60;
                    break;
                case "15min to 50% Design pressure":
                    tempPress = UnitConvert.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.Vesseldesignpressure);
                    pRequire = tempPress / 2;
                    tRequire = 15.0 / 60;
                    break;
                case "15min to 7barg":
                    pRequire = 0.7;
                    tRequire = 15.0 / 60;
                    break;
                default:
                    break;
            }
            if (pRequire <= 0)
            {
                MessageBox.Show("P t_require can't be lesser than zero,please change Initial Pressure.", "Message Box");
                return;
            }
            tConstant = tRequire / Math.Log(model.InitialPressure / pRequire);
            double wInitDepr = model.VaporDensity * model.TotalVaporVolume / tConstant;
            Wt[0] = new KeyValuePair<int, double>(0, wInitDepr);
            Pt[0] = new KeyValuePair<int, double>(0, model.InitialPressure);
            for (int i = 1; i <= 15; i++)
            {
                double j = i / 60.0;
                double exp = Math.Exp(-1.0 * j / tConstant);
                double tmpWt = wInitDepr * exp;
                double tmpPt = model.InitialPressure * exp;
                decimal dd = (decimal)tmpPt;
                Wt[i] = new KeyValuePair<int, double>(i, tmpWt);
                Pt[i] = new KeyValuePair<int, double>(i, tmpPt);
            }
            model.InitialDepressuringRate = wInitDepr;
            //model.Timespecify = (int)(tConstant * 60);
            //model.Timespecify = 15;//15分钟的数据

            double currentTime = model.Timespecify / 60;
            model.CalculatedDepressuringRate = wInitDepr * Math.Exp(-1*currentTime / tConstant);
            model.CalculatedVesselPressure = model.InitialPressure * Math.Exp(-1 * currentTime / tConstant);

            //下列值需要保存到
            modelSC.ReliefLoad = wInitDepr;
            modelSC.ReliefPressure = model.InitialPressure;
           
        }
        private void CalcDetailed(object obj)
        {
        }
        private void DepressuringCurve(object obj)
        {
            if (!isCalc)
                Calc(obj);
            RateCurveView v = new RateCurveView();

            v.WtSource = Wt;
            v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            v.Show();
            PressureCurveView v2 = new PressureCurveView();
            v2.PtSource = Pt;
            v2.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            v2.Show();

        }
        private void Save(object obj)
        {
            WriteConvertModel();
            drumBLL.SaveData(model.dbmodel, SessionPS);
            modelSC.ReliefMW = drumBLL.DrumVaporStream.BulkMwOfPhase;
            modelSC.ReliefTemperature = drumBLL.DrumVaporStream.Temperature;
            modelSC.ReliefCpCv = drumBLL.DrumVaporStream.BulkCPCVRatio;
            modelSC.ReliefZ = drumBLL.DrumVaporStream.VaporZFmKVal;

            ScenarioDAL scdal = new ScenarioDAL();
            scdal.Update(modelSC, SessionPS);

            
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
        }
    }
}
