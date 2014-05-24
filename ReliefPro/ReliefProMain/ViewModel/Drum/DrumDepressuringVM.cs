using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model;
using UOMLib;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumDepressuringVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public ICommand CalcCMD { get; set; }
        public ICommand DetailedCMD { get; set; }
        public ICommand DepressuringCurveCMD { get; set; }
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
        private DrumDepressuringBLL drumBLL;
        public DrumDepressuringVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(Calc);
            DetailedCMD = new DelegateCommand<object>(CalcDetailed);
            DepressuringCurveCMD = new DelegateCommand<object>(DepressuringCurve);

            lstShortCut = new List<string>(new[] { "Shortcut", "PROII DEPR Unit" });
            lstDeprRqe = new List<string>{
                "21bar/min","7bar/min","50% Design pressure in 15min","7barg in 15min","Specify"};
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

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model = new DrumDepressuringModel(drumModel);
            model.InitialPressureUnit = uomEnum.UserPressure;
            model.VaporDensityUnit = uomEnum.UserDensity;
            model.TotalVaporVolumeUnit = uomEnum.UserVolume;
            model.VesseldesignpressureUnit = uomEnum.UserPressure;
            model.TotalWettedAreaUnit = uomEnum.UserArea;
            model.InitialDepressuringRateUnit = uomEnum.UserWeightFlow;

            model.TimespecifyUnit = uomEnum.UserTime;
            model.CalculatedVesselPressureUnit = uomEnum.UserPressure;
            model.CalculatedDepressuringRateUnit = uomEnum.UserWeightFlow;

            model.DetailPUnit = uomEnum.UserPressure;
            model.DetailPTimeUnit = uomEnum.UserTime;
            model.TimeStepUnit = uomEnum.UserTime;
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            model.dbmodel.InitialPressure = uc.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Area.ToString(), model.InitialPressure);
            model.dbmodel.VaporDensity = uc.Convert(model.VaporDensityUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.VaporDensity.Value);
            model.dbmodel.TotalVaporVolume = uc.Convert(model.TotalVaporVolumeUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.TotalVaporVolume);
            model.dbmodel.Vesseldesignpressure = uc.Convert(model.VesseldesignpressureUnit, UOMLib.UOMEnum.WeightFlow.ToString(), model.Vesseldesignpressure);
            model.dbmodel.TotalWettedArea = uc.Convert(model.TotalWettedAreaUnit, UOMLib.UOMEnum.Pressure.ToString(), model.TotalWettedArea);
            //model.dbmodel.ValveConstantforSonicFlow = uc.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.InitialPressure = uc.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.InitialPressure);
            model.dbmodel.Timespecify = uc.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.Timespecify);
            model.dbmodel.CalculatedDepressuringRate = uc.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.CalculatedDepressuringRate);
            model.dbmodel.CalculatedVesselPressure = uc.Convert(model.InitialPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.CalculatedVesselPressure);

            model.dbmodel.DeltaP = uc.Convert(model.DetailPUnit, UOMLib.UOMEnum.Pressure.ToString(), model.DetailP);
            model.dbmodel.DeltaPTime = uc.Convert(model.DetailPTimeUnit, UOMLib.UOMEnum.Time.ToString(), model.DetailPTime);
            model.dbmodel.TimeStep = uc.Convert(model.TimeStepUnit, UOMLib.UOMEnum.Time.ToString(), model.TimeStep);

            model.dbmodel.ShortCut = selectedShotCut;
            model.dbmodel.DepressuringRequirements = selectedDeprRqe;
            model.dbmodel.FireHeatInput = enableFireHeatInput;
            model.dbmodel.HeatInputModel = selectedHeatInput;
        }
        private void Calc(object obj)
        {
        }
        private void CalcDetailed(object obj)
        { }
        private void DepressuringCurve(object obj)
        { }
        private void Save(object obj)
        {
            WriteConvertModel();
            drumBLL.SaveData(model.dbmodel, SessionPS);
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
