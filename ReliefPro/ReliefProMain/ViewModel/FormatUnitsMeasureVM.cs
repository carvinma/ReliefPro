
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
        private IList<UnitType> lstUnitType;
        private IList<BasicUnit> lstBasicUnit;
        private IList<BasicUnitCurrent> SelectedCurrent;
        private ISession SessionPlant;

        private UOMEnum uomEnum;
        public FormatUnitsMeasureVM(ISession SessionPT)
        {
            SessionPlant = SessionPT;
            foreach (UOMEnum uom in UOMSingle.UomEnums)
            {
                if (uom.SessionDBPath == SessionPT.Connection.ConnectionString)
                {
                    uomEnum = uom;
                }
            }

            this.model = new FormatUnitsMeasureModel();
            model.handler += new FormatUnitsMeasureModel.SelectDefaultUnitDelegate(InitCboSelected);
            model.handlerChange += new FormatUnitsMeasureModel.ChangeDefaultUnitDelegate(InitBasicUnitDefalut);

            SelectedCurrent = new List<BasicUnitCurrent>();
            unitInfo = new UnitInfo();
            /*************Gloab************************/
            lstBasicUnit = unitInfo.GetBasicUnit(SessionPT);
            //UOMEnum.lstBasicUnitDefault = unitInfo.GetBasicUnitDefault(SessionPT);
            //UOMEnum.lstBasicUnitCurrent = unitInfo.GetBasicUnitCurrent(SessionPT);
            //UOMEnum.lstSystemUnit = unitInfo.GetSystemUnit(SessionPT);
            //UOMEnum.lstUnitType = unitInfo.GetUnitType(SessionPT);
            lstUnitType = unitInfo.GetUnitType(SessionPT);

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
                unitInfo.BasicUnitDel(model.BasicUnitselectLocation, UOMSingle.Session, SessionPlant);
                MessageBox.Show("Delete Successful!");
            }
        }
        private void SetBasicUnitDefault(object obj)
        {
            try
            {
                int id = model.BasicUnitselectLocation.ID;
                //unitInfo.BasicUnitSetDefault(id);
                unitInfo.BasicUnitSetDefault(id, UOMSingle.Session);
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
                BasicUnit item = new BasicUnit();
                item.ID = 0;
                item.UnitName = basicUnitInfo.BasicNewName;
                item.IsDefault = 0;
                int basicUnitID = unitInfo.BasicUnitAdd(item, SessionPlant);    
                 basicUnitID = unitInfo.BasicUnitAdd(item, UOMSingle.Session); 
                if (basicUnitID > 0)
                {
                    model.ObBasicUnit.Add(item);
                    var listCopy = uomEnum.lstBasicUnitDefault.Where(p => p.BasicUnitID == model.BasicUnitselectLocation.ID)
                   .Select(p => { p.ID = 0; p.BasicUnitID = basicUnitID; return p; }).ToList();
                    unitInfo.Save(listCopy, SessionPlant);
                    listCopy = uomEnum.lstBasicUnitDefault.Where(p => p.BasicUnitID == model.BasicUnitselectLocation.ID)
                   .Select(p => { p.ID = 0; p.BasicUnitID = basicUnitID; return p; }).ToList();

                    unitInfo.Save(listCopy, UOMSingle.Session);
                    //uomEnum.lstBasicUnitDefault = unitInfo.GetBasicUnitDefault(SessionPlant);
                    int index = model.ObBasicUnit.ToList().FindIndex(p => p.ID == basicUnitID);
                    if (index >= 0)
                        model.BasicUnitselectLocation = model.ObBasicUnit[index];
                }
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
                return basciUnitCurrent.SystemUnitID;
            return 0;
        }
        private int GetUnitDefalut(int unitTypeid, int basicid)
        {
            if (uomEnum.lstBasicUnitDefault != null && uomEnum.lstBasicUnitDefault.Count > 0)
            {
                var basciUnitDefault = uomEnum.lstBasicUnitDefault.Where(p => p.BasicUnitID == basicid && p.UnitTypeID == unitTypeid).FirstOrDefault();
                if (basciUnitDefault != null)
                    return basciUnitDefault.SystemUnitID;
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
                    findThisUnit.SystemUnitID = changeSystemUnit.ID;
                    SelectedCurrent.Add(findThisUnit);
                }
            }
        }
        private void InitCboSelected(object SelectDefaultUnit)
        {
            try
            {
                var selectedBasicUnit = SelectDefaultUnit as BasicUnit;
                int basicid = selectedBasicUnit.ID;
                model.TemperatureSelectLocation = model.ObcTemperature.Where(p => p.ID == GetUnit(1, basicid)).FirstOrDefault();
                model.PressureSelectLocation = model.ObcPressure.Where(p => p.ID == GetUnit(2, basicid)).FirstOrDefault();
                model.WeightSelectLocation = model.ObcWeight.Where(p => p.ID == GetUnit(3, basicid)).FirstOrDefault();
                model.MolarSelectLocation = model.ObcMolar.Where(p => p.ID == GetUnit(4, basicid)).FirstOrDefault();
                model.StandardVolumeRateSelectLocation = model.ObcStandardVolumeRate.Where(p => p.ID == GetUnit(5, basicid)).FirstOrDefault();
                model.ViscositySelectLocation = model.ObcViscosity.Where(p => p.ID == GetUnit(6, basicid)).FirstOrDefault();
                model.HeatCapacitySelectLocation = model.ObcHeatCapacity.Where(p => p.ID == GetUnit(7, basicid)).FirstOrDefault();
                model.ThermalConductivitySelectLocation = model.ObcThermalConductivity.Where(p => p.ID == GetUnit(8, basicid)).FirstOrDefault();
                model.HeatTransCoeffcientSelectLocation = model.ObcHeatTransCoeffcient.Where(p => p.ID == GetUnit(9, basicid)).FirstOrDefault();
                model.SurfaceTensionSelectLocation = model.ObcSurfaceTension.Where(p => p.ID == GetUnit(10, basicid)).FirstOrDefault();
                //model.CompositionSelectLocation = model.ObcComposition.Where(p => p.ID == GetUnitDefalut(11, basicid)).FirstOrDefault();
                model.MachineSpeedSelectLocation = model.ObcMachineSpeed.Where(p => p.ID == GetUnit(12, basicid)).FirstOrDefault();
                model.VolumeSelectLocation = model.ObcVolume.Where(p => p.ID == GetUnit(13, basicid)).FirstOrDefault();
                model.LengthSelectLocation = model.ObcLength.Where(p => p.ID == GetUnit(14, basicid)).FirstOrDefault();
                model.AeraSelectLocation = model.ObcAera.Where(p => p.ID == GetUnit(15, basicid)).FirstOrDefault();
                model.EnergySelectLocation = model.ObcEnergy.Where(p => p.ID == GetUnit(16, basicid)).FirstOrDefault();
                model.TimeSelectLocation = model.ObcTime.Where(p => p.ID == GetUnit(17, basicid)).FirstOrDefault();
                model.FlowConductanceSelectLocation = model.ObcFlowConductance.Where(p => p.ID == GetUnit(18, basicid)).FirstOrDefault();

                model.MassRateSelectLocation = model.ObcMassRate.Where(p => p.ID == GetUnit(19, basicid)).FirstOrDefault();
                model.VolumeRateSelectLocation = model.ObcVolumeRate.Where(p => p.ID == GetUnit(20, basicid)).FirstOrDefault();
                model.DensitySelectLocation = model.ObcDensity.Where(p => p.ID == GetUnit(21, basicid)).FirstOrDefault();
                model.SpecificEnthalpySelectLocation = model.ObcSpecificEnthalpy.Where(p => p.ID == GetUnit(22, basicid)).FirstOrDefault();
                model.EnthalpySelectLocation = model.ObcEnthalpy.Where(p => p.ID == GetUnit(24, basicid)).FirstOrDefault();
                model.FineLenthSelectLocation = model.ObcFineLength.Where(p => p.ID == GetUnit(23, basicid)).FirstOrDefault();
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

            model.ObBasicUnit = new ObservableCollection<BasicUnit>(lstBasicUnit);
            model.ObcTemperature = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 1));
            model.ObcPressure = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 2));
            model.ObcWeight = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 3));
            model.ObcMolar = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 4));
            model.ObcStandardVolumeRate = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 5));
            model.ObcViscosity = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 6));
            model.ObcHeatCapacity = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 7));
            model.ObcThermalConductivity = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 8));
            model.ObcHeatTransCoeffcient = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 9));
            model.ObcSurfaceTension = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 10));
            model.ObcMachineSpeed = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 12));
            model.ObcVolume = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 13));
            model.ObcLength = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 14));
            model.ObcAera = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 15));
            model.ObcEnergy = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 16));
            model.ObcTime = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 17));
            model.ObcFlowConductance = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 18));
            model.ObcMassRate = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 19));
            model.ObcVolumeRate = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 20));
            model.ObcDensity = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 21));
            model.ObcSpecificEnthalpy = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 22));
            model.ObcEnthalpy = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 24));
            model.ObcFineLength = new ObservableCollection<SystemUnit>(uomEnum.lstSystemUnit.Where(p => p.UnitType == 23));
            //if (uomEnum.UnitFromFlag && uomEnum.lstBasicUnitCurrent != null && uomEnum.lstBasicUnitCurrent.Count > 0)
            //{
            //    model.BasicUnitselectLocation = lstBasicUnit.Where(p => p.ID == uomEnum.lstBasicUnitCurrent.First().BasicUnitID).First();

            //    model.BasicUnitselectLocation = new BasicUnit();
            //}
            //else
                model.BasicUnitselectLocation = lstBasicUnit[lstBasicUnit.ToList().FindIndex(p => p.IsDefault == 1)];


            //model.CompositionSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 11).Single().SystemUnitID)]; 
            //model.ObcComposition = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 11));

        }

        public void Save(object obj)
        {
            //var listCopy = UOMEnum.lstBasicUnitDefault.Where(p => p.BasicUnitID == model.BasicUnitselectLocation.ID).ToList();
            //unitInfo.Save(listCopy);
            //lstBasciUnitCurrent = new List<BasicUnitCurrent>();
            //listCopy.ForEach(p =>
            //{
            //    BasicUnitCurrent current = new BasicUnitCurrent();
            //    current.BasicUnitID = p.BasicUnitID;
            //    current.SystemUnitID = p.SystemUnitID;
            //    current.UnitTypeID = p.UnitTypeID;
            //    lstBasciUnitCurrent.Add(current);
            //});
            unitInfo.SaveCurrent(SelectedCurrent, this.SessionPlant);
            int findindex = UOMSingle.UomEnums.FindIndex(p => p.SessionDBPath == uomEnum.SessionDBPath);
            UOMEnum uomEnumNew = new UOMEnum(SessionPlant);
            UOMSingle.UomEnums.RemoveAt(findindex);
            UOMSingle.UomEnums.Add(uomEnumNew);
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
                foreach (var uom in UOMSingle.UomEnums)
                {
                    if (uom.SessionDBPath == this.SessionPlant.Connection.ConnectionString)
                    {
                        uom.UnitFromFlag = true;
                        break;
                    }
                }
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
                        foreach (var uom in UOMSingle.UomEnums)
                        {
                            if (uom.SessionDBPath == this.SessionPlant.Connection.ConnectionString)
                            {
                                uom.UnitFromFlag = true;
                                break;
                            }
                        }
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
