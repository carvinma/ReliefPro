using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model;
using ReliefProMain.View;
using ReliefProModel.Drum;
using UOMLib;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumFireVM : ViewModelBase
    {
        public ICommand FluidCMD { get; set; }
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private string selectedHeatInputModel = "API 521";
        public string SelectedHeatInputModel
        {
            get { return selectedHeatInputModel; }
            set
            {
                selectedHeatInputModel = value;
                OnPropertyChanged("SelectedHeatInputModel");
            }
        }
        public List<string> lstHeatInputModel { get; set; }
        public DrumFireModel model { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private int ScenarioID;
        private DrumFireBLL fireBLL;
        private DrumFireFluid fireFluidModle;
        public DrumFireVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.ScenarioID = ScenarioID;
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            FluidCMD = new DelegateCommand<object>(OpenFluidWin);
            CalcCMD = new DelegateCommand<object>(Calc);
            OKCMD = new DelegateCommand<object>(Save);
            lstHeatInputModel = new List<string>();
            lstHeatInputModel.Add("API 521");
            lstHeatInputModel.Add("API 2000");

            fireBLL = new DrumFireBLL(SessionPS, SessionPF);
            var fireModel = fireBLL.GetDrumFireModel(ScenarioID);
            fireModel = fireBLL.ReadConvertModel(fireModel);
            if (!string.IsNullOrEmpty(fireModel.HeatInputModel))
                selectedHeatInputModel = fireModel.HeatInputModel;

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model = new DrumFireModel(fireModel);
            model.WettedAreaUnit = uomEnum.UserArea;
            model.LatentHeatUnit = uomEnum.UserSpecificEnthalpy;
            model.CrackingHeatUnit = uomEnum.UserSpecificEnthalpy;
            model.ReliefLoadUnit = uomEnum.UserWeightFlow;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            model.dbmodel.WettedArea = uc.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.WettedArea);
            model.dbmodel.LatentHeat = uc.Convert(model.LatentHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentHeat);
            model.dbmodel.CrackingHeat = uc.Convert(model.CrackingHeatUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.CrackingHeat);
            model.dbmodel.ReliefLoad = uc.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.WeightFlow.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefPressure = uc.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            model.dbmodel.ReliefTemperature = uc.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.HeavyOilFluid = model.HeavyOilFluid;
            model.dbmodel.AllGas = model.AllGas;
            model.dbmodel.EquipmentExist = model.EquipmentExist;
            model.dbmodel.HeatInputModel = selectedHeatInputModel;
        }
        private void OpenFluidWin(object obj)
        {
            Drum_fireFluid win = new Drum_fireFluid();
            DrumFireFluidVM vm = new DrumFireFluidVM(model.dbmodel.ID, SessionPS, SessionPF);
            win.DataContext = vm;
            if (win.ShowDialog() == true)
            {
                fireFluidModle = vm.model.dbmodel;
            }
        }
        private void Calc(object obj)
        {
        }
        private void Save(object obj)
        {
            WriteConvertModel();
            fireBLL.SaveData(model.dbmodel, fireFluidModle, SessionPS);
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
