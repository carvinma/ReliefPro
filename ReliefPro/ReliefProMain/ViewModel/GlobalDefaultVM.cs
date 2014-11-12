using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProMain.CustomControl;
using ReliefProMain.Models;
using ReliefProMain.Util;
using ReliefProModel.GlobalDefault;
using UOMLib;
using System.Windows;
using ReliefProDAL;
using ReliefProModel;
using ReliefProBLL.Common;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class GlobalDefaultVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public ICommand DelCMD { get; set; }
        public ICommand AddCMD { get; set; }
        public GlobalDefaultModel model { get; set; }
        private GlobalDefaultBLL globalDefaultBLL;

        public UOMLib.UOMEnum uomEnum { get; set; }
        private string TargetUnit;
        private ISession SessionPlant;
        private string DirPlant;
        public GlobalDefaultVM(ISession sessionPlant,string DirPlant)
        {
            SessionPlant = sessionPlant;
            this.DirPlant = DirPlant;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            OKCMD = new DelegateCommand<object>(Save);
            DelCMD = new DelegateCommand<object>(DelRow);
            AddCMD = new DelegateCommand<object>(AddRow);
            model = new GlobalDefaultModel();
            globalDefaultBLL = new GlobalDefaultBLL(SessionPlant);
            var lstFlareSystem = globalDefaultBLL.GetFlareSystem();
            foreach (var flareSystem in lstFlareSystem)
            {
                flareSystem.RowGuid = Guid.NewGuid();
                GlobalFlareSystem gls = new GlobalFlareSystem(flareSystem);
                model.lstFlareSystem.Add(gls);
            }
            model.conditSetModel = globalDefaultBLL.GetConditionsSettings();
            if (model.conditSetModel != null)
                model.conditSetModel = globalDefaultBLL.ReadConvertModel(model.conditSetModel);
            else
                model.conditSetModel = new ConditionsSettings();
            model.LatentHeatSettingsUnit = UOMEnum.SpecificEnthalpy;
            model.DrumSurgeTimeSettingsUnit = UOMEnum.Time;
            ReadConvert();
            changeUnit += new ChangeUnitDelegate(ExcuteThumbMoved);
        }
        private void WriteConvertModel()
        {
            if (!string.IsNullOrEmpty(TargetUnit))
            {
                foreach (var t in model.lstFlareSystem)
                {
                    t.DesignBackPressure = UnitConvert.Convert(TargetUnit, UOMEnum.Pressure, t.DesignBackPressure);
                }
            }
            if (model.conditSetModel.LatentHeatSettings != null)
                model.conditSetModel.LatentHeatSettings = UnitConvert.Convert(model.LatentHeatSettingsUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.conditSetModel.LatentHeatSettings);
            if (model.conditSetModel.DrumSurgeTimeSettings != null)
                model.conditSetModel.DrumSurgeTimeSettings = UnitConvert.Convert(model.DrumSurgeTimeSettingsUnit, UOMLib.UOMEnum.Time.ToString(), model.conditSetModel.DrumSurgeTimeSettings);
        }
        private void DelRow(object obj)
        {
            if (obj == null)
                return;
            var rowGuid = (Guid)obj;
            var findFlareSystem = model.lstFlareSystem.FirstOrDefault(p => p.RowGuid == rowGuid);
            if (findFlareSystem.RowGuid != null)
            {
                model.lstFlareSystem.Remove(findFlareSystem);
                if (findFlareSystem.ID > 0)
                    globalDefaultBLL.DelFlareSystemByID(findFlareSystem.ID);
            }
        }
        private void AddRow(object obj)
        {
            var flareSystem = new FlareSystem();
            flareSystem.isDel = true;
            flareSystem.RowGuid = Guid.NewGuid();
            GlobalFlareSystem gls = new GlobalFlareSystem(flareSystem);
            model.lstFlareSystem.Add(gls);
        }
        private bool CheckFlareData()
        {
            foreach (var t in model.lstFlareSystem)
            {
                if (string.IsNullOrEmpty(t.FlareName))
                {
                    MessageBox.Show(LanguageHelper.GetValueByKey("GlobalDefaultViewErrorInfo"));
                    return false;
                }
            }
            return true;
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    bool IsEdit = IsChange();                   
                    if (CheckFlareData())
                    {
                        if (IsEdit)
                        {
                            bool IsExistSC=false;
                            TreeUnitDAL unitdal=new TreeUnitDAL();
                            IList<TreeUnit>unitlist=unitdal.GetAllList(SessionPlant);
                            foreach (TreeUnit unit in unitlist)
                            {
                                TreePSDAL psdal = new TreePSDAL();
                                IList<TreePS> list = psdal.GetAllList(unit.ID, SessionPlant);
                                foreach (TreePS ps in list)
                                {
                                    string dbPath = DirPlant + @"\" + unit.UnitName + @"\" + ps.PSName + @"\protectedsystem.mdb";                                   
                                    using (var helper = new NHibernateHelper(dbPath))
                                    {
                                        ISession Session = helper.GetCurrentSession();
                                        ScenarioDAL scdal = new ScenarioDAL();
                                        IList<Scenario> scList = scdal.GetAllList(Session);
                                        if (scList.Count > 0)
                                        {
                                            IsExistSC = true;
                                            break;
                                        }
                                    }
                                }
                                if (IsExistSC)
                                    break;
                            }

                            if (IsExistSC)
                            {
                                MessageBoxResult r = MessageBox.Show("Are you sure to edit data? it will delete all Scenario", "Message Box", MessageBoxButton.YesNo);
                                if (r == MessageBoxResult.Yes)
                                {
                                    foreach (TreeUnit unit in unitlist)
                                    {
                                        TreePSDAL psdal = new TreePSDAL();
                                        IList<TreePS> list = psdal.GetAllList(unit.ID, SessionPlant);
                                        foreach (TreePS ps in list)
                                        {
                                            string dbPath = DirPlant + @"\" + unit.UnitName + @"\" + ps.PSName + @"\protectedsystem.mdb";
                                            using (var helper = new NHibernateHelper(dbPath))
                                            {
                                                ISession Session = helper.GetCurrentSession();
                                                ScenarioBLL scBLL = new ScenarioBLL(Session);
                                                scBLL.DeleteSCOther();
                                                scBLL.ClearScenario();
                                            }
                                        }
                                    }
                                    WriteConvertModel();
                                    globalDefaultBLL.Save(model.lstFlareSystem.Select(p => p.dbmodel).ToList(), model.conditSetModel);

                                }                              
                            }
                            else
                            {
                                WriteConvertModel();
                                globalDefaultBLL.Save(model.lstFlareSystem.Select(p => p.dbmodel).ToList(), model.conditSetModel);
                            }
                            wd.DialogResult = true;
                        }
                    }
                    
                    
                }
            }
        }

        private void ReadConvert()
        {
            this.TargetUnit = uomEnum.UserPressure;
            foreach (var t in model.lstFlareSystem)
            {
                t.DesignBackPressure = UnitConvert.Convert(UOMEnum.Pressure, uomEnum.UserPressure, t.DesignBackPressure);
            }
        }
        public ChangeUnitDelegate changeUnit { get; set; }
        public void ExcuteThumbMoved(object ColInfo, object OrigionUnit, object TargetUnit)
        {
            int k = string.Compare(OrigionUnit.ToString(), TargetUnit.ToString(), true);
            if (k != 0 && ColInfo != null && ColInfo.ToString() == "2")
            {
                this.TargetUnit = TargetUnit.ToString();
                foreach (var t in model.lstFlareSystem)
                {
                    t.DesignBackPressure = UnitConvert.Convert(OrigionUnit.ToString(), TargetUnit.ToString(), t.DesignBackPressure);
                }
            }
        }

        public bool IsChange()
        {
            bool b = false;
            ConditionsSettings cs= globalDefaultBLL.GetConditionsSettings();
            if (model != null && cs == null)
            {
                return true;
            }
            if (model.SteamCondition != cs.SteamCondition || model.AirCondition != cs.AirCondition || model.CoolingWaterCondition != cs.CoolingWaterCondition)
            {
                return true;
            }
            if (model.DrumSurgeTimeSettings != cs.DrumSurgeTimeSettings || model.LatentHeatSettings != cs.LatentHeatSettings)
            {
                return true;
            }


            return b;
        }
    }
}
