﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Model.HXs;
using UOMLib;

namespace ReliefProMain.ViewModel.HXs
{
    public class HXFireSizeVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public HXFireSizeModel model { get; set; }
        private string selectedExposedToFire;
        private string SelectedExposedToFire
        {
            get { return selectedExposedToFire; }
            set
            {
                selectedExposedToFire = value;
                this.OnPropertyChanged("SelectedExposedToFire");
            }
        }
        private string selectedType;
        private string SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                this.OnPropertyChanged("SelectedType");
            }
        }
        public List<string> lstExposedToFires { get; set; }
        public List<string> lstTypes { get; set; }
        private HXBLL hxBLL;
        public HXFireSizeVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            lstExposedToFires = new List<string> { };
            lstTypes = new List<string> { };
            hxBLL = new HXBLL(SessionPS, SessionPF);
            var fireModel = hxBLL.GetHXFireSizeModel(ScenarioID);
            fireModel = hxBLL.ReadConvertHXFireSizeModel(fireModel);

            model = new HXFireSizeModel(fireModel);
            model.dbmodel.ScenarioID = ScenarioID;
            SelectedExposedToFire = fireModel.ExposedToFire; 
            if (!string.IsNullOrEmpty(fireModel.Type))
            { SelectedType = fireModel.Type; }

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model.ODUnit = uomEnum.UserLength;
            model.LengthUnit = uomEnum.UserLength;
            model.ElevationUnit = uomEnum.UserLength;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.ExposedToFire = this.selectedExposedToFire; ;
            model.dbmodel.Type = this.selectedType;
            model.dbmodel.OD = UnitConvert.Convert(model.ODUnit, UOMLib.UOMEnum.Length.ToString(), model.OD.Value);
            model.dbmodel.Length = UnitConvert.Convert(model.LengthUnit, UOMLib.UOMEnum.Length.ToString(), model.Length.Value);
            model.dbmodel.Elevation = UnitConvert.Convert(model.ElevationUnit, UOMLib.UOMEnum.Length.ToString(), model.Elevation.Value);
            model.dbmodel.PipingContingency = model.PipingContingency;
        }

        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    hxBLL.SaveHXFireSize(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}