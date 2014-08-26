
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

        public FormatUnitsMeasureModel model { get; set; }
        private UnitInfo unitInfo;
        private IList<UnitType> lstUnitType;
        private IList<SystemUnit> lstSystemUnit;
        private IList<BasicUnit> lstBasicUnit;
        private IList<BasicUnitDefault> lstBasciUnitDefault;
        private IList<BasicUnitCurrent> lstBasciUnitCurrent;
        public FormatUnitsMeasureVM()
        {
            unitInfo = new UnitInfo();
            this.model = new FormatUnitsMeasureModel();


            lstBasicUnit = unitInfo.GetBasicUnit();

            lstBasciUnitDefault = unitInfo.GetBasicUnitDefault();
            lstBasciUnitCurrent = unitInfo.GetBasicUnitCurrent();
            lstUnitType = unitInfo.GetUnitType();
            lstSystemUnit = unitInfo.GetSystemUnit();

            model.handler += new FormatUnitsMeasureModel.SelectDefaultUnitDelegate(InitCboSelected);
            model.handlerChange += new FormatUnitsMeasureModel.ChangeDefaultUnitDelegate(InitBasicUnitDefalut);

            InitModelInfo(null);
            SaveCommand = new DelegateCommand<object>(Save);
            NewBasicCommand = new DelegateCommand<object>(OpenAddWin);
            BasicUnitDefaultCommand = new DelegateCommand<object>(SetBasicUnitDefault);

        }

        private void SetBasicUnitDefault(object obj)
        {
            try
            {
                int id = model.BasicUnitselectLocation.ID;
                unitInfo.BasicUnitSetDefault(id);
                UOMEnum.BasicUnitID = id;
                MessageBox.Show("Set Successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Set Failed!");
            }
        }
        private void OpenAddWin(object obj)
        {
            BasicUnitInfo basicUnitInfo = new BasicUnitInfo();
            if (basicUnitInfo.ShowDialog() == true)
            {
                BasicUnit item = new BasicUnit();
                item.ID = 0;
                item.UnitName = basicUnitInfo.BasicNewName;
                item.IsDefault = 0;
                int basicUnitID = unitInfo.BasicUnitAdd(item);
                if (basicUnitID > 0)
                {
                    model.ObBasicUnit.Add(item);
                    var listCopy = lstBasciUnitDefault.Where(p => p.BasicUnitID == model.BasicUnitselectLocation.ID)
                   .Select(p => { p.ID = 0; p.BasicUnitID = basicUnitID; return p; }).ToList();
                    unitInfo.Save(listCopy);
                    lstBasciUnitDefault = unitInfo.GetBasicUnitDefault();
                    int index = model.ObBasicUnit.ToList().FindIndex(p => p.ID == basicUnitID);
                    if (index >= 0)
                        model.BasicUnitselectLocation = model.ObBasicUnit[index];
                }
            }
        }
        private int GetUnit(int unitTypeid, int basicid)
        {
            if (lstBasciUnitCurrent != null && lstBasciUnitCurrent.Count > 0)
            {
                return GetUnitCurrent(unitTypeid);
            }
            return GetUnitDefalut(unitTypeid, basicid);
        }
        private int GetUnitCurrent(int unitTypeid)
        {
            var basciUnitCurrent = lstBasciUnitCurrent.Where(p => p.UnitTypeID == unitTypeid).FirstOrDefault();
            if (basciUnitCurrent != null)
                return basciUnitCurrent.SystemUnitID;
            return 0;
        }
        private int GetUnitDefalut(int unitTypeid, int basicid)
        {
            if (lstBasciUnitDefault != null && lstBasciUnitDefault.Count > 0)
            {
                var basciUnitDefault = lstBasciUnitDefault.Where(p => p.BasicUnitID == basicid && p.UnitTypeID == unitTypeid).FirstOrDefault();
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
                var findThisUnit = lstBasciUnitDefault.Where(p => p.BasicUnitID == model.BasicUnitselectLocation.ID && p.UnitTypeID == changeSystemUnit.UnitType).FirstOrDefault();
                if (findThisUnit == null)
                {
                    BasicUnitDefault item = new BasicUnitDefault();
                    item.ID = 0;
                    item.BasicUnitID = model.BasicUnitselectLocation.ID;
                    item.UnitTypeID = changeSystemUnit.UnitType;
                    item.SystemUnitID = changeSystemUnit.ID;
                    lstBasciUnitDefault.Add(item);
                }
                else
                {
                    lstBasciUnitDefault.Remove(findThisUnit);
                    findThisUnit.SystemUnitID = changeSystemUnit.ID;
                    lstBasciUnitDefault.Add(findThisUnit);
                }
            }
        }
        private void InitCboSelected(object SelectDefaultUnit)
        {
            try
            {
                lstBasciUnitDefault = unitInfo.GetBasicUnitDefault();
                var selectedBasicUnit = SelectDefaultUnit as BasicUnit;
                int basicid = selectedBasicUnit.ID;
                model.TemperatureSelectLocation = model.ObcTemperature.Where(p => p.ID == GetUnitDefalut(1, basicid)).FirstOrDefault();
                model.PressureSelectLocation = model.ObcPressure.Where(p => p.ID == GetUnitDefalut(2, basicid)).FirstOrDefault();
                model.WeightSelectLocation = model.ObcWeight.Where(p => p.ID == GetUnitDefalut(3, basicid)).FirstOrDefault();
                model.MolarSelectLocation = model.ObcMolar.Where(p => p.ID == GetUnitDefalut(4, basicid)).FirstOrDefault();
                model.StandardVolumeRateSelectLocation = model.ObcStandardVolumeRate.Where(p => p.ID == GetUnitDefalut(5, basicid)).FirstOrDefault();
                model.ViscositySelectLocation = model.ObcViscosity.Where(p => p.ID == GetUnitDefalut(6, basicid)).FirstOrDefault();
                model.HeatCapacitySelectLocation = model.ObcHeatCapacity.Where(p => p.ID == GetUnitDefalut(7, basicid)).FirstOrDefault();
                model.ThermalConductivitySelectLocation = model.ObcThermalConductivity.Where(p => p.ID == GetUnitDefalut(8, basicid)).FirstOrDefault();
                model.HeatTransCoeffcientSelectLocation = model.ObcHeatCapacity.Where(p => p.ID == GetUnitDefalut(9, basicid)).FirstOrDefault();
                model.SurfaceTensionSelectLocation = model.ObcSurfaceTension.Where(p => p.ID == GetUnitDefalut(10, basicid)).FirstOrDefault();
                //model.CompositionSelectLocation = model.ObcComposition.Where(p => p.ID == GetUnitDefalut(11, basicid)).FirstOrDefault();
                model.MachineSpeedSelectLocation = model.ObcMachineSpeed.Where(p => p.ID == GetUnitDefalut(12, basicid)).FirstOrDefault();
                model.VolumeSelectLocation = model.ObcVolume.Where(p => p.ID == GetUnitDefalut(13, basicid)).FirstOrDefault();
                model.LengthSelectLocation = model.ObcLength.Where(p => p.ID == GetUnitDefalut(14, basicid)).FirstOrDefault();
                model.AeraSelectLocation = model.ObcAera.Where(p => p.ID == GetUnitDefalut(15, basicid)).FirstOrDefault();
                model.EnergySelectLocation = model.ObcEnergy.Where(p => p.ID == GetUnitDefalut(16, basicid)).FirstOrDefault();
                model.TimeSelectLocation = model.ObcTime.Where(p => p.ID == GetUnitDefalut(17, basicid)).FirstOrDefault();
                model.FlowConductanceSelectLocation = model.ObcFlowConductance.Where(p => p.ID == GetUnitDefalut(18, basicid)).FirstOrDefault();

                model.MassRateSelectLocation = model.ObcMassRate.Where(p => p.ID == GetUnitDefalut(19, basicid)).FirstOrDefault();
                model.VolumeRateSelectLocation = model.ObcVolumeRate.Where(p => p.ID == GetUnitDefalut(20, basicid)).FirstOrDefault();
                model.DensitySelectLocation = model.ObcDensity.Where(p => p.ID == GetUnitDefalut(21, basicid)).FirstOrDefault();
                model.SpecificEnthalpySelectLocation = model.ObcSpecificEnthalpy.Where(p => p.ID == GetUnitDefalut(22, basicid)).FirstOrDefault();
                model.EnthalpySelectLocation = model.ObcEnthalpy.Where(p => p.ID == GetUnitDefalut(14, basicid)).FirstOrDefault();
                model.FineLenthSelectLocation = model.ObcFineLength.Where(p => p.ID == GetUnitDefalut(23, basicid)).FirstOrDefault();
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

            //BasicUnitDefault systemUnit = lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 1).Single();

            //model.TemperatureSelectLocation = lstSystemUnit[lstSystemUnit.ToList().FindIndex(p =>p.ID==int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 1).Single().SystemUnitID))];
            model.ObcTemperature = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 1));

            //model.PressureSelectLocation = lstSystemUnit[lstSystemUnit.ToList().FindIndex(p =>p.ID==int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 2).Single().SystemUnitID))];            
            model.ObcPressure = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 2));

            //model.WeightSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 3).Single().SystemUnitID)];                       
            model.ObcWeight = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 3));

            //model.MolarSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 4).Single().SystemUnitID)];                                  
            model.ObcMolar = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 4));

            //model.StandardVolumeRateSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 5).Single().SystemUnitID)]; 
            model.ObcStandardVolumeRate = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 5));

            // model.ViscositySelectLocation =lstSystemUnit[lstSystemUnit.ToList().FindIndex(p =>p.ID==int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 6).Single().SystemUnitID))]; 
            model.ObcViscosity = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 6));

            //model.HeatCapacitySelectLocation = lstSystemUnit[lstSystemUnit.ToList().FindIndex(p =>p.ID==int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 7).Single().SystemUnitID))]; 
            model.ObcHeatCapacity = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 7));

            //model.ThermalConductivitySelectLocation = lstSystemUnit[lstSystemUnit.ToList().FindIndex(p =>p.ID==int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 8).Single().SystemUnitID))]; 
            model.ObcThermalConductivity = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 8));

            //model.HeatTransCoeffcientSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 9).Single().SystemUnitID)]; 
            model.ObcHeatTransCoeffcient = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 9));

            //model.SurfaceTensionSelectLocation = lstSystemUnit[lstSystemUnit.ToList().FindIndex(p =>p.ID==int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 10).Single().SystemUnitID))]; 
            model.ObcSurfaceTension = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 10));


            //model.MachineSpeedSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 12).Single().SystemUnitID)]; 
            model.ObcMachineSpeed = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 12));

            //model.VolumeSelectLocation = lstSystemUnit[lstSystemUnit.ToList().FindIndex(p => p.ID == int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 13).Single().SystemUnitID))]; 
            model.ObcVolume = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 13));

            //model.LengthSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 14).Single().SystemUnitID)]; 
            model.ObcLength = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 14));

            //model.AeraSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 15).Single().SystemUnitID)]; 
            model.ObcAera = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 15));

            //model.EnergySelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 16).Single().SystemUnitID)]; 
            model.ObcEnergy = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 16));

            //model.ThermalConductivitySelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 17).Single().SystemUnitID)]; 
            model.ObcTime = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 17));

            //model.FlowConductanceSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 18).Single().SystemUnitID)]; 
            model.ObcFlowConductance = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 18));

            model.ObcMassRate = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 19));

            model.ObcVolumeRate = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 20));

            model.ObcDensity = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 21));

            model.ObcSpecificEnthalpy = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 22));

            model.ObcEnthalpy = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 24));

            model.ObcFineLength = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 23));
            if (lstBasciUnitCurrent != null && lstBasciUnitCurrent.Count > 0)
                model.BasicUnitselectLocation = lstBasicUnit.Where(p => p.ID == lstBasciUnitCurrent.First().BasicUnitID).First();
            else
                model.BasicUnitselectLocation = lstBasicUnit[lstBasicUnit.ToList().FindIndex(p => p.IsDefault == 1)];

            //model.CompositionSelectLocation = lstSystemUnit[int.Parse(lstBasciUnitDefault.Where(s => s.BasicUnitID == model.BasicUnitselectLocation.ID && s.UnitTypeID == 11).Single().SystemUnitID)]; 
            //model.ObcComposition = new ObservableCollection<SystemUnit>(lstSystemUnit.Where(p => p.UnitType == 11));

        }

        public void Save(object obj)
        {
            var listCopy = lstBasciUnitDefault.Where(p => p.BasicUnitID == model.BasicUnitselectLocation.ID).ToList();
            //unitInfo.Save(listCopy);
            lstBasciUnitCurrent = new List<BasicUnitCurrent>();
            listCopy.ForEach(p =>
            {
                BasicUnitCurrent current = new BasicUnitCurrent();
                current.BasicUnitID = p.BasicUnitID;
                current.SystemUnitID = p.SystemUnitID;
                current.UnitTypeID = p.UnitTypeID;
                lstBasciUnitCurrent.Add(current);
            });
            unitInfo.SaveCurrent(lstBasciUnitCurrent);
            // lstBasciUnitDefault = unitInfo.GetBasicUnitDefault();
            UOMEnum.lstBasicUnitCurrent = lstBasciUnitCurrent;

            System.Windows.Window wd = obj as System.Windows.Window;
            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }
    }
}
