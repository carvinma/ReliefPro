﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Commands;
using ReliefProMain.Models;
using UOMLib;
using ReliefProDAL.Drums;
using System.Windows;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumFireFluidVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public DrumFireFluidModel model { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumFireFluidVM(int DrumFireCalcID, ISession SessionPS, ISession SessionPF,int FireType)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;

            DrumFireFluidBLL fluidBll = new DrumFireFluidBLL(SessionPS, SessionPF);
            var fireModel = fluidBll.GetFireFluidModel(DrumFireCalcID,FireType);
            fireModel = fluidBll.ReadConvertModel(fireModel);
            model = new DrumFireFluidModel(fireModel);
            //  model = new DrumBlockedOutletModel(fireModel);
            ////  model.dbmodel.DrumFireCalcID = drum.GetDrumID(dbPSFile);
            //  //model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.VesselUnit = uomEnum.UserArea;
            model.PressureUnit = uomEnum.UserPressure;
            model.TemperatureUnit = uomEnum.UserTemperature;
            model.PSVPressureUnit = uomEnum.UserPressure;
            model.TWUnit = uomEnum.UserTemperature;
            OKCMD = new DelegateCommand<object>(Save);
        }

        private void WriteConvertModel()
        {
            //model.dbmodel.GasVaporMW = uc.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.GasVaporMW);
            model.dbmodel.GasVaporMW = model.VaporMW;
            model.dbmodel.ExposedVesse = UnitConvert.Convert(model.VesselUnit, UOMLib.UOMEnum.Area.ToString(), model.Vessel);
            model.dbmodel.NormaTemperature = UnitConvert.Convert(model.TemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.Temperature);
            model.dbmodel.NormalPressure = UnitConvert.Convert(model.PressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.Pressure);
            model.dbmodel.PSVPressure = UnitConvert.Convert(model.PSVPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.PSVPressure);
            model.dbmodel.TW = UnitConvert.Convert(model.TWUnit, UOMLib.UOMEnum.Temperature.ToString(), model.TW);
            model.dbmodel.NormalCpCv = model.NormalCpCv;
            //model.dbmodel.TW = uc.Convert(model.t, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
        }
        private void Save(object obj)
        {
            //if (!CheckData()) return;
            if (model.VaporMW == 0)
            {
                MessageBox.Show("Vapor MW must be greater than zero", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (model.NormalCpCv == 0)
            {
                MessageBox.Show("CpCv must be greater than zero", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (model.TW == 0)
            {
                MessageBox.Show("TW must be greater than zero", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    if (model.dbmodel.ID > 0)
                    {
                        DrumFireFluidDAL dal = new DrumFireFluidDAL();
                        dal.SaveDrumFireFluid(SessionPS, model.dbmodel);
                    }
                    wd.DialogResult = true;
                }
            }
        }
    }
}
