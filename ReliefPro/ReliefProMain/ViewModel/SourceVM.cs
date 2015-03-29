using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows;
using ReliefProMain.Models;
using ReliefProCommon.Enum;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class SourceVM : ViewModelBase
    {
        public SourceModel model { get; set; }
        public int deviceId;
        UOMLib.UOMEnum uomEnum;
        public aSourceBLL sourceBLL = new aSourceBLL();
        aScenarioBLL scenarioBLL;
        
        public SourceVM(string streamName, int deviceId)
        {
            this.deviceId = deviceId;
            uomEnum = UOMSingle.plantsInfo.FirstOrDefault(p => p.Id == 0).UnitInfo;
            tbSource source = sourceBLL.GetModel(streamName, deviceId);
            model = new SourceModel(source);            
            InitUnit();
            ReadConvert();
        }

        private ICommand _Update;

        public ICommand Update
        {
            get { return _Update ?? (_Update = new RelayCommand(OKClick)); }
        }


        private void OKClick(object window)
        {
            if (model.SourceName.Trim() == "")
            {
                throw new ArgumentException("Please type in a name for the Source.");
            }
            
            bool bEdit = false;
            if (model.dbmodel.IsSteam!=model.IsSteam||model.dbmodel.Maxpossiblepressure != UnitConvert.Convert(model.PressureUnit, UOMEnum.Pressure, model.MaxPossiblePressure) || model.dbmodel.Sourcetype != model.SourceType ||model.dbmodel.IsHeatSource!=model.IsHeatSource)
            {
                bEdit = true;
            }
            System.Windows.Window wd = window as System.Windows.Window;
            if (bEdit)
            {
                List<tbScenario> scList = scenarioBLL.GetList(deviceId);
                if (scList.Count > 0)
                {
                    MessageBoxResult r = MessageBox.Show("Are you sure to edit data? it need to rerun all Scenario", "Message Box", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        scenarioBLL.DeleteSCOther();
                        scenarioBLL.ClearScenario();
                    }
                    else
                    {
                        if (wd != null)
                        {
                            wd.Close();
                            return;
                        }
                    }
                }
                WriteConvert();
                model.dbmodel.SourcetypeColor = model.SourceType_Color;
                model.dbmodel.MaxpossiblepressureColor = model.MaxPossiblePressure_Color;
                model.dbmodel.Sourcetype = model.SourceType;
                model.dbmodel.IsSteam = model.IsSteam;
                model.dbmodel.IsHeatSource = model.IsHeatSource;
                model.dbmodel.Description = model.Description;
                sourceBLL.SaveSource(model.dbmodel);
            }
            
            if (wd != null)
            {
                wd.Close();
            }
        }

        private ICommand _ShowHeatSourceListCommand;
        public ICommand ShowHeatSourceListCommand
        {
            get
            {
                if (_ShowHeatSourceListCommand == null)
                {
                    _ShowHeatSourceListCommand = new DelegateCommand(ShowHeatSourceList);

                }
                return _ShowHeatSourceListCommand;
            }
        }

        public void ShowHeatSourceList()
        {
            HeatSourceListView v = new HeatSourceListView();
            HeatSourceListVM vm = new HeatSourceListVM(model.ID);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }
        private void ReadConvert()
        {
            model.MaxPossiblePressure = UnitConvert.Convert(UOMEnum.Pressure, model.PressureUnit, model.dbmodel.Maxpossiblepressure??0);
        }
        private void WriteConvert()
        {
            model.dbmodel.Maxpossiblepressure = UnitConvert.Convert(model.PressureUnit, UOMEnum.Pressure, model.MaxPossiblePressure);
        }
        private void InitUnit()
        {
            model.PressureUnit = uomEnum.UserPressure;
        }
    }
}
