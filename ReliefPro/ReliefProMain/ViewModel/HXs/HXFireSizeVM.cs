using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Models.HXs;
using UOMLib;
using ReliefProDAL;
using ReliefProModel;

namespace ReliefProMain.ViewModel.HXs
{
    public class HXFireSizeVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public HXFireSizeModel model { get; set; }
        private string selectedExposedToFire;
        public string SelectedExposedToFire
        {
            get { return selectedExposedToFire; }
            set
            {
                selectedExposedToFire = value;
                lstTypes = GetTypes(selectedExposedToFire);
                this.OnPropertyChanged("SelectedExposedToFire");
            }
        }
        private string selectedType;
        public string SelectedType
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
            lstExposedToFires = GetExposedToFires();
            
            hxBLL = new HXBLL(SessionPS, SessionPF);
            var fireModel = hxBLL.GetHXFireSizeModel(ScenarioID);
            fireModel = hxBLL.ReadConvertHXFireSizeModel(fireModel);            
            model = new HXFireSizeModel(fireModel);
            model.dbmodel.ScenarioID = ScenarioID;

            PSVDAL psvdal = new PSVDAL();
            PSV psv = psvdal.GetModel(SessionPS);
            
            SelectedExposedToFire = psv.LocationDescription;
            if (!string.IsNullOrEmpty(fireModel.Type))
            { SelectedType = fireModel.Type; }

            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.ODUnit = uomEnum.UserLength;
            model.LengthUnit = uomEnum.UserLength;
            model.ElevationUnit = uomEnum.UserLength;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.ExposedToFire = this.selectedExposedToFire; ;
            model.dbmodel.Type = this.selectedType;
            model.dbmodel.OD = UnitConvert.Convert(model.ODUnit, UOMLib.UOMEnum.Length.ToString(), model.OD);
            model.dbmodel.Length = UnitConvert.Convert(model.LengthUnit, UOMLib.UOMEnum.Length.ToString(), model.Length);
            model.dbmodel.Elevation = UnitConvert.Convert(model.ElevationUnit, UOMLib.UOMEnum.Length.ToString(), model.Elevation);
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


        public List<string> GetExposedToFires()
        {
            List<string> list = new List<string>();
            list.Add("Shell");
            list.Add("Tube");
            return list;
        }
        public List<string> GetTypes(string hxType)
        {
            List<string> list = new List<string>();
            list.Add("Fixed");
            list.Add("U-Tube");
            if (hxType == "Shell")
            {
                list.Add("Floating head");
                list.Add("Kettle type");
            }
            return list;
        }
    }
}
