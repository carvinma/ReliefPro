﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model.Compressors;
using UOMLib;

namespace ReliefProMain.ViewModel.Compressors
{
    public class CentrifugalVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public CentrifugalBlockedOutletModel model { get; set; }
        private CompressorBlockedBLL blockBLL;
        public CentrifugalVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            blockBLL = new CompressorBlockedBLL(SessionPS, SessionPF);
            var BlockedModel = blockBLL.GetCentrifugalModel(ScenarioID);
            BlockedModel = blockBLL.ReadConvertCentrifugalModel(BlockedModel);

            model = new CentrifugalBlockedOutletModel(BlockedModel);
            model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
            model.ReliefloadUnit = uomEnum.UserMassRate;
            model.ReliefTempUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.Scale = model.Scale;
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.Reliefload = UnitConvert.Convert(model.ReliefloadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.Reliefload);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTempUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemp);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }
        private void CalcResult(object obj)
        {
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    blockBLL.SaveCentrifugal(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
