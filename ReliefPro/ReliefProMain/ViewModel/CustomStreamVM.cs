﻿/*
 * 该文件是物流线信息页面的viewmodel
 * 
 * 单位制都ok,目前无bug
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
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

namespace ReliefProMain.ViewModel
{
    public class CustomStreamVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
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
        public CustomStreamVM(string name, ISession sessionPlant, ISession sessionProtectedSystem)
        {

            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            BasicUnit BU;
            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            UnitConvert uc = new UnitConvert();
            db = new CustomStreamDAL();
            CustomStream cs = db.GetModel(SessionProtectedSystem, name);
            CurrentModel = new CustomStreamModel(cs);
            InitUnit();
            OKCMD = new DelegateCommand<object>(Save);
            ReadConvert();
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
            //if (CurrentModel.Temperature!=null)
            CurrentModel.Temperature = UnitConvert.Convert(UOMEnum.Temperature, CurrentModel.TemperatureUnit, CurrentModel.Temperature);
            //if (CurrentModel.Pressure!=null)
            CurrentModel.Pressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.PressureUnit, CurrentModel.Pressure);
            //if (CurrentModel.WeightFlow!=null)
            CurrentModel.WeightFlow = UnitConvert.Convert(UOMEnum.MassRate, CurrentModel.WeightFlowUnit, CurrentModel.WeightFlow);
            //if (CurrentModel.SpEnthalpy!=null)
            CurrentModel.SpEnthalpy = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, CurrentModel.SpEnthalpyUnit, CurrentModel.SpEnthalpy);
        }
        private void WriteConvert()
        {
            //if (CurrentModel.Temperature!=null)
            CurrentModel.Temperature = UnitConvert.Convert(CurrentModel.TemperatureUnit, UOMEnum.Temperature, CurrentModel.Temperature);
            //if (CurrentModel.Pressure!=null)
            CurrentModel.Pressure = UnitConvert.Convert(CurrentModel.PressureUnit, UOMEnum.Pressure, CurrentModel.Pressure);
            //if (CurrentModel.WeightFlow!=null)
            CurrentModel.WeightFlow = UnitConvert.Convert(CurrentModel.WeightFlowUnit, UOMEnum.MassRate, CurrentModel.WeightFlow);
            //if (CurrentModel.SpEnthalpy!=null)
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

                }
                else
                {
                    db.Update(CurrentModel.model, SessionProtectedSystem);
                    SessionProtectedSystem.Flush();
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
