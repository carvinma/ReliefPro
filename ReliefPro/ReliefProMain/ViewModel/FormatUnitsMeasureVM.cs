
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using ReliefProBLL;
using ReliefProMain.Models;
using ReliefProModel;
using ReliefProMain.View;
using UOMLib;
using System.Windows.Forms;
using NHibernate;

namespace ReliefProMain.ViewModel
{
    public class FormatUnitsMeasureVM : ViewModelBase
    {
        /// <summary>
        /// Save Commond
        /// </summary>
        public ICommand SaveCommand { get; set; }
        public ICommand NewBasicCommand { get; set; }
        public ICommand BasicUnitDefaultCommand { get; set; }
        public ICommand DelBasicUnitCommand { get; set; }
        public ICommand CancleCommand { get; set; }

        public FormatUnitsMeasureModel model { get; set; }
        private UnitInfo unitInfo;
        private IList<systbUnitType> lstUnitType;
        private IList<systbBasicUnit> lstBasicUnit;
        private IList<systbBasicUnitCurrent> SelectedCurrent;
        private ISession SessionPlant;

        private UOMEnum uomEnum;
        private UOMEnum uomEnumBasic;
        private string plantPath;
        private string plantName;
        public FormatUnitsMeasureVM(string plantName)
        {
            this.plantName = plantName;
            uomEnum= UOMSingle.plantsInfo.FirstOrDefault(p => p.Name == plantName).UnitInfo;

            if (uomEnum.lstBasicUnitCurrent != null && uomEnum.lstBasicUnitCurrent.Count() > 0)
                UOMSingle.BaseUnitSelectedID = uomEnum.lstBasicUnitCurrent[0].BasicUnitID;

            this.model = new FormatUnitsMeasureModel();
            model.handler += new FormatUnitsMeasureModel.SelectDefaultUnitDelegate(InitCboSelected);
            model.handlerChange += new FormatUnitsMeasureModel.ChangeDefaultUnitDelegate(InitBasicUnitDefalut);

            SelectedCurrent = new List<systbBasicUnitCurrent>();
            unitInfo = new UnitInfo();
            /*************Gloab************************/
            uomEnumBasic = UOMSingle.plantsInfo.First(p => p.Id == 0).UnitInfo;
            lstBasicUnit = uomEnumBasic.lstBasicUnit;
            lstUnitType = uomEnumBasic.lstUnitType;
            /*************************************/

            InitModelInfo(null);
            SaveCommand = new DelegateCommand<object>(Save);
            NewBasicCommand = new DelegateCommand<object>(OpenAddWin);
            BasicUnitDefaultCommand = new DelegateCommand<object>(SetBasicUnitDefault);
            DelBasicUnitCommand = new DelegateCommand<object>(DelBasicUnit);
            CancleCommand = new DelegateCommand<object>(Cancle);
        }

        private void DelBasicUnit(object obj)
        {
            if (MessageBox.Show("Are you sure you want to delete?", "Delete Bassic Unit", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                var tmpContext = UOMSingle.currentPlant.DataContext;
                UOMSingle.currentPlant.DataContext = UOMSingle.templatePlantContext;//模板增加数据
                unitInfo.BasicUnitDel(model.BasicUnitselectLocation);
                model.ObBasicUnit.Remove(model.BasicUnitselectLocation);
                var findDefalut= model.ObBasicUnit.Where(p=>p.IsDefault==1);
                if (findDefalut == null || findDefalut.Count() == 0)
                {
                    int id = model.ObBasicUnit.Min(p => p.Id);
                    model.BasicUnitselectLocation = model.ObBasicUnit.First(p => p.Id == id);
                    UOMSingle.BaseUnitSelectedID = id;
                    unitInfo.BasicUnitSetDefault(id);
                }
                else
                {
                    model.BasicUnitselectLocation = findDefalut.First();
                    UOMSingle.BaseUnitSelectedID = model.BasicUnitselectLocation.Id;
                }
                UOMSingle.currentPlant.DataContext = tmpContext;
                MessageBox.Show("Delete Successful!");
            }
        }
        private void SetBasicUnitDefault(object obj)
        {
            try
            {
                int id = model.BasicUnitselectLocation.Id;
                foreach (var basicUnit in model.ObBasicUnit)
                {
                    basicUnit.IsDefault = basicUnit.Id == id ? 1 : 0;
                }
              
                //unitInfo.BasicUnitSetDefault(id);
                var tmpDataContext = UOMSingle.currentPlant.DataContext;
                UOMSingle.currentPlant.DataContext = UOMSingle.templatePlantContext;//模板增加数据
                unitInfo.BasicUnitSetDefault(id);
                UOMSingle.currentPlant.DataContext = tmpDataContext;

                MessageBox.Show("Set Successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Set Failed!");
            }
        }
        private void OpenAddWin(object obj)
        {
            BasicUnitInfo basicUnitInfo = new BasicUnitInfo(SessionPlant);
            if (basicUnitInfo.ShowDialog() == true)
            {
                systbBasicUnit item = new systbBasicUnit();
                item.Id = 0;
                item.UnitName = basicUnitInfo.BasicNewName;
                item.IsDefault = 0;
                var tmpContext = UOMSingle.currentPlant.DataContext;
                UOMSingle.currentPlant.DataContext = UOMSingle.templatePlantContext;//模板增加数据
                int basicUnitID = unitInfo.BasicUnitAdd(item);
                if (basicUnitID > 0)
                {
                    model.ObBasicUnit.Add(item);
                    IList<systbBasicUnitDefault> listCopy = new List<systbBasicUnitDefault>();
                    //Copy的不应该是Basic，应该是当前的所有单位制
                    foreach(systbBasicUnitCurrent current in SelectedCurrent)
                    {
                        systbBasicUnitDefault defalut = new systbBasicUnitDefault();
                        defalut.BasicUnitID = basicUnitID;
                        defalut.UnitTypeID=current.UnitTypeID;
                        defalut.SystemUnitID=current.SystemUnitID;
                        listCopy.Add(defalut);
                    }
                  
                    unitInfo.Save(listCopy);
                    uomEnumBasic = new UOMEnum();
                    UOMSingle.plantsInfo.First(p => p.Id == 0).UnitInfo = uomEnumBasic;
                }
                UOMSingle.currentPlant.DataContext = tmpContext;
            }
        }
        private int GetUnit(int unitTypeid, int basicid)
        {
            if (uomEnum.UnitFromFlag && uomEnum.lstBasicUnitCurrent != null && uomEnum.lstBasicUnitCurrent.Count > 0)
            {
                return GetUnitCurrent(unitTypeid);
            }
            return GetUnitDefalut(unitTypeid, basicid);
        }
        private int GetUnitCurrent(int unitTypeid)
        {
            var basciUnitCurrent = uomEnum.lstBasicUnitCurrent.Where(p => p.UnitTypeID == unitTypeid).FirstOrDefault();
            if (basciUnitCurrent != null)
                return basciUnitCurrent.SystemUnitID.Value;
            return 0;
        }
        private int GetUnitDefalut(int unitTypeid, int basicid)
        {
            if (uomEnumBasic.lstBasicUnitDefault != null && uomEnumBasic.lstBasicUnitDefault.Count > 0)
            {
                var basciUnitDefault = uomEnumBasic.lstBasicUnitDefault.Where(p => p.BasicUnitID == basicid && p.UnitTypeID == unitTypeid).FirstOrDefault();
                if (basciUnitDefault != null)
                    return basciUnitDefault.SystemUnitID.Value;
            }
            return 0;
        }
        private void InitBasicUnitDefalut(object systemUnit)
        {
            var changeSystemUnit = systemUnit as SystemUnit;
            if (changeSystemUnit != null)
            {
                var findThisUnit = SelectedCurrent.Where(p => p.UnitTypeID == changeSystemUnit.UnitType).FirstOrDefault();
                if (findThisUnit == null)
                {
                    BasicUnitCurrent item = new BasicUnitCurrent();
                    item.ID = 0;
                    item.BasicUnitID = model.BasicUnitselectLocation == null ? 0 : model.BasicUnitselectLocation.ID;
                    item.UnitTypeID = changeSystemUnit.UnitType;
                    item.SystemUnitID = changeSystemUnit.ID;
                    SelectedCurrent.Add(item);
                }
                else
                {
                    SelectedCurrent.Remove(findThisUnit);
                    findThisUnit.BasicUnitID = model.BasicUnitselectLocation.ID;
                    findThisUnit.UnitTypeID = changeSystemUnit.UnitType;
                    findThisUnit.SystemUnitID = changeSystemUnit.ID;
                    SelectedCurrent.Add(findThisUnit);
                }
            }
        }
        private void InitCboSelected(object SelectDefaultUnit)
        {
            try
            {
                var selectedBasicUnit = SelectDefaultUnit as systbBasicUnit;
                int basicid = selectedBasicUnit.Id;
                UOMSingle.BaseUnitSelectedID = basicid;
                model.canUseDelButtn= basicid<=5? false :true;

                model.TemperatureSelectLocation = model.ObcTemperature.Where(p => p.Id == GetUnit(1, basicid)).FirstOrDefault();
                model.PressureSelectLocation = model.ObcPressure.Where(p => p.Id == GetUnit(2, basicid)).FirstOrDefault();
                model.WeightSelectLocation = model.ObcWeight.Where(p => p.Id == GetUnit(3, basicid)).FirstOrDefault();
                model.MolarSelectLocation = model.ObcMolar.Where(p => p.Id == GetUnit(4, basicid)).FirstOrDefault();
                model.StandardVolumeRateSelectLocation = model.ObcStandardVolumeRate.Where(p => p.Id == GetUnit(5, basicid)).FirstOrDefault();
                model.ViscositySelectLocation = model.ObcViscosity.Where(p => p.Id == GetUnit(6, basicid)).FirstOrDefault();
                model.HeatCapacitySelectLocation = model.ObcHeatCapacity.Where(p => p.Id == GetUnit(7, basicid)).FirstOrDefault();
                model.ThermalConductivitySelectLocation = model.ObcThermalConductivity.Where(p => p.Id == GetUnit(8, basicid)).FirstOrDefault();
                model.HeatTransCoeffcientSelectLocation = model.ObcHeatTransCoeffcient.Where(p => p.Id == GetUnit(9, basicid)).FirstOrDefault();
                model.SurfaceTensionSelectLocation = model.ObcSurfaceTension.Where(p => p.Id == GetUnit(10, basicid)).FirstOrDefault();
                //model.CompositionSelectLocation = model.ObcComposition.Where(p => p.ID == GetUnitDefalut(11, basicid)).FirstOrDefault();
                model.MachineSpeedSelectLocation = model.ObcMachineSpeed.Where(p => p.Id == GetUnit(12, basicid)).FirstOrDefault();
                model.VolumeSelectLocation = model.ObcVolume.Where(p => p.Id == GetUnit(13, basicid)).FirstOrDefault();
                model.LengthSelectLocation = model.ObcLength.Where(p => p.Id == GetUnit(14, basicid)).FirstOrDefault();
                model.AeraSelectLocation = model.ObcAera.Where(p => p.Id == GetUnit(15, basicid)).FirstOrDefault();
                model.EnergySelectLocation = model.ObcEnergy.Where(p => p.Id == GetUnit(16, basicid)).FirstOrDefault();
                model.TimeSelectLocation = model.ObcTime.Where(p => p.Id == GetUnit(17, basicid)).FirstOrDefault();
                model.FlowConductanceSelectLocation = model.ObcFlowConductance.Where(p => p.Id == GetUnit(18, basicid)).FirstOrDefault();

                model.MassRateSelectLocation = model.ObcMassRate.Where(p => p.Id == GetUnit(19, basicid)).FirstOrDefault();
                model.VolumeRateSelectLocation = model.ObcVolumeRate.Where(p => p.Id == GetUnit(20, basicid)).FirstOrDefault();
                model.DensitySelectLocation = model.ObcDensity.Where(p => p.Id == GetUnit(21, basicid)).FirstOrDefault();
                model.SpecificEnthalpySelectLocation = model.ObcSpecificEnthalpy.Where(p => p.Id == GetUnit(22, basicid)).FirstOrDefault();
                model.EnthalpySelectLocation = model.ObcEnthalpy.Where(p => p.Id == GetUnit(24, basicid)).FirstOrDefault();
                model.FineLenthSelectLocation = model.ObcFineLength.Where(p => p.Id == GetUnit(23, basicid)).FirstOrDefault();
                if (uomEnum.lstBasicUnitCurrent != null && uomEnum.lstBasicUnitCurrent.Count > 0)
                    uomEnum.UnitFromFlag = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Query each Unit information
        /// </summary>
        private void InitModelInfo(object obj)
        {

            model.ObBasicUnit = new ObservableCollection<systbBasicUnit>(lstBasicUnit);
            model.ObcTemperature = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 1));
            model.ObcPressure = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 2));
            model.ObcWeight = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 3));
            model.ObcMolar = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 4));
            model.ObcStandardVolumeRate = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 5));
            model.ObcViscosity = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 6));
            model.ObcHeatCapacity = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 7));
            model.ObcThermalConductivity = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 8));
            model.ObcHeatTransCoeffcient = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 9));
            model.ObcSurfaceTension = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 10));
            model.ObcMachineSpeed = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 12));
            model.ObcVolume = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 13));
            model.ObcLength = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 14));
            model.ObcAera = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 15));
            model.ObcEnergy = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 16));
            model.ObcTime = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 17));
            model.ObcFlowConductance = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 18));
            model.ObcMassRate = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 19));
            model.ObcVolumeRate = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 20));
            model.ObcDensity = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 21));
            model.ObcSpecificEnthalpy = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 22));
            model.ObcEnthalpy = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 24));
            model.ObcFineLength = new ObservableCollection<systbSystemUnit>(uomEnumBasic.lstSystemUnit.Where(p => p.UnitType == 23));
            if (uomEnum.UnitFromFlag && uomEnum.lstBasicUnitCurrent != null && uomEnum.lstBasicUnitCurrent.Count > 0)
            {
                model.BasicUnitselectLocation = lstBasicUnit.Where(p => p.Id == UOMSingle.BaseUnitSelectedID).First();
            }
            else
                model.BasicUnitselectLocation = lstBasicUnit[lstBasicUnit.ToList().FindIndex(p => p.IsDefault == 1)];
        }

        public void Save(object obj)
        {
            unitInfo.SaveCurrent(SelectedCurrent);

            UOMSingle.plantsInfo.First(p => p.Name == this.plantName).UnitInfo = new UOMEnum();
            System.Windows.Window wd = obj as System.Windows.Window;
            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        public void Cancle(object obj)
        {
            System.Windows.Window wd = obj as System.Windows.Window;
            if (wd != null)
            {

                UOMSingle.plantsInfo.First(p => p.Name == this.plantName).UnitInfo.UnitFromFlag = true;
                wd.DialogResult = true;
            }
        }
        public ICommand LoadedCommand
        {
            get
            {
                return new DelegateCommand<System.Windows.Window>(win =>
                {
                    win.Closing += (sender, e) =>
                    {
                        UOMSingle.plantsInfo.First(p => p.Name == this.plantName).UnitInfo.UnitFromFlag = true;
                        //if (System.Windows.MessageBox.Show("确认要关闭窗口吗？", "提示", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        //{
                        //    e.Cancel = true;
                        //}
                    };
                });
            }
        }
    }
}
