using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using UOMLib;
using NHibernate;
using ReliefProMain.View;
using ReliefProMain.Models;

namespace ReliefProMain.ViewModel.StorageTanks
{
    public class StorageTankVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { set; get; }
        public string FileName { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private CustomStreamModel _CurrentModel;
        public CustomStreamModel CurrentModel
        {
            get
            {
                return this._CurrentModel;
            }
            set
            {
                this._CurrentModel = value;
                OnPropertyChanged("CurrentModel");
            }
        }
        private CustomStreamDAL db;
        UOMLib.UOMEnum uomEnum;
        public StorageTankVM(string name, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == sessionPlant.Connection.ConnectionString);
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            BasicUnit BU;
            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            db = new CustomStreamDAL();
            if (name != string.Empty)
            {
                CustomStream cs = db.GetModel(SessionProtectedSystem, name);
                CurrentModel = new CustomStreamModel(cs);
                InitUnit();
                ReadConvert();
            }
            OKCMD = new DelegateCommand<object>(Save);
        }

        private ICommand _ImportCommand;
        public ICommand ImportCommand
        {
            get
            {
                if (_ImportCommand == null)
                {
                    _ImportCommand = new RelayCommand(Import);

                }
                return _ImportCommand;
            }
        }
        private void Import(object obj)
        {
            SelectStreamView v = new SelectStreamView();
            SelectStreamVM vm = new SelectStreamVM(SessionPlant);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(vm.SelectedEq))
                {
                    //根据设该设备名称来获取对应的物流线信息和其他信息。
                    ProIIStreamDataDAL proIIStreamDataDAL = new ProIIStreamDataDAL();
                    FileName = vm.SelectedFile;
                    ProIIStreamData data = proIIStreamDataDAL.GetModel(SessionPlant, vm.SelectedEq, FileName);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
                    CurrentModel = new CustomStreamModel(cs);
                    SourceFileInfo = vm.SourceFileInfo;
                }
            }
        }

        private void InitUnit()
        {
            if (CurrentModel != null)
            {
                CurrentModel.TemperatureUnit = uomEnum.UserTemperature;
                CurrentModel.PressureUnit = uomEnum.UserPressure;
                CurrentModel.WeightFlowUnit = uomEnum.UserMassRate;
                CurrentModel.SpEnthalpyUnit = uomEnum.UserSpecificEnthalpy;
            }
        }

        private void ReadConvert()
        {
            //if (CurrentModel.Temperature != null)
            CurrentModel.Temperature = UnitConvert.Convert(UOMEnum.Temperature, CurrentModel.TemperatureUnit, CurrentModel.Temperature);
            //if (CurrentModel.Pressure != null)
            CurrentModel.Pressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.PressureUnit, CurrentModel.Pressure);
            //if (CurrentModel.WeightFlow != null)
            CurrentModel.WeightFlow = UnitConvert.Convert(UOMEnum.MassRate, CurrentModel.WeightFlowUnit, CurrentModel.WeightFlow);
            //if (CurrentModel.SpEnthalpy != null)
            CurrentModel.SpEnthalpy = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, CurrentModel.SpEnthalpyUnit, CurrentModel.SpEnthalpy);
        }
        private void WriteConvert()
        {
            //if (CurrentModel.Temperature != null)
            CurrentModel.Temperature = UnitConvert.Convert(CurrentModel.TemperatureUnit, UOMEnum.Temperature, CurrentModel.Temperature);
            //if (CurrentModel.Pressure != null)
            CurrentModel.Pressure = UnitConvert.Convert(CurrentModel.PressureUnit, UOMEnum.Pressure, CurrentModel.Pressure);
            //if (CurrentModel.WeightFlow != null)
            CurrentModel.WeightFlow = UnitConvert.Convert(CurrentModel.WeightFlowUnit, UOMEnum.MassRate, CurrentModel.WeightFlow);
            //if (CurrentModel.SpEnthalpy != null)
            CurrentModel.SpEnthalpy = UnitConvert.Convert(CurrentModel.SpEnthalpyUnit, UOMEnum.SpecificEnthalpy, CurrentModel.SpEnthalpy);
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                WriteConvert();
                if (CurrentModel.ID == 0)
                {
                    db.Add(CurrentModel.model, SessionProtectedSystem);
                    StorageTankDAL storageTankDAL = new StorageTankDAL();
                    StorageTank tank = new StorageTank();
                    tank.StorageTankName = CurrentModel.model.StreamName;
                    tank.SourceFile = FileName;
                    storageTankDAL.Add(tank, SessionProtectedSystem);

                    ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
                    ProtectedSystem ps = new ProtectedSystem();
                    ps.PSType = 5;
                    psDAL.Add(ps, SessionProtectedSystem);
                }
                else
                {
                    db.Update(CurrentModel.model, SessionProtectedSystem);
                    StorageTankDAL storageTankDAL = new StorageTankDAL();
                    StorageTank tank = storageTankDAL.GetModel(SessionProtectedSystem);
                    tank.StorageTankName = CurrentModel.model.StreamName;
                    tank.SourceFile = FileName;
                    storageTankDAL.Update(tank, SessionProtectedSystem);
                    ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
                    ProtectedSystem ps = psDAL.GetModel(SessionProtectedSystem);
                    ps.PSType = 5;
                    psDAL.Update(ps, SessionProtectedSystem);
                    SessionProtectedSystem.Flush();

                    SourceFileDAL sfdal = new SourceFileDAL();
                    SourceFileInfo = sfdal.GetModel(tank.SourceFile, SessionPlant);
                }


                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {

                    wd.DialogResult = true;
                }
            }
        }

    }
}
