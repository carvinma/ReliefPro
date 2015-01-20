using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View;
using ReliefProMain.Models;
using NHibernate;
using ReliefProMain.ViewModel.Drums;
using System.Windows;
using System.IO;
using UOMLib;

namespace ReliefProMain.ViewModel
{
    public class ScenarioHeatSourceListVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        SourceFile SourceFileInfo;
        private ScenarioHeatSourceDAL db;
        private HeatSourceDAL dbHS;
        private int ScenarioStreamID;
        private ObservableCollection<ScenarioHeatSourceModel> _HeatSources;
        public ObservableCollection<ScenarioHeatSourceModel> HeatSources
        {
            get { return _HeatSources; }
            set
            {
                _HeatSources = value;
                OnPropertyChanged("_HeatSources");
            }
        }
        private ScenarioHeatSourceModel _SelectedHeatSource;
        public ScenarioHeatSourceModel SelectedHeatSource
        {
            get { return _SelectedHeatSource; }
            set
            {
                _SelectedHeatSource = value;
                OnPropertyChanged("SelectedHeatSource");
            }
        }

        public UOMLib.UOMEnum uomEnum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScenarioStreamID"></param>
        /// <param name="sourceFileInfo"></param>
        /// <param name="SessionPlant"></param>
        /// <param name="SessionProtectedSystem"></param>
        /// <param name="HeatSourceType">1:FB 2Fired Heater</param>
        public ScenarioHeatSourceListVM(int ScenarioStreamID, SourceFile sourceFileInfo, ISession SessionPlant, ISession SessionProtectedSystem,string HeatSourceType)
        {
            this.SessionPlant = SessionPlant;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            this.ScenarioStreamID = ScenarioStreamID;

            this.SessionProtectedSystem = SessionProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            db = new ScenarioHeatSourceDAL();
            dbHS = new HeatSourceDAL();
            HeatSources = GetHeatSources(ScenarioStreamID,HeatSourceType);
            if (HeatSources.Count > 0)
                SelectedHeatSource = HeatSources[0];
        }

        public ScenarioHeatSourceListVM(int ScenarioStreamID, SourceFile sourceFileInfo, ISession SessionPlant, ISession SessionProtectedSystem)
        {
            this.SessionPlant = SessionPlant;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            this.ScenarioStreamID = ScenarioStreamID;

            this.SessionProtectedSystem = SessionProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            db = new ScenarioHeatSourceDAL();
            dbHS = new HeatSourceDAL();
            HeatSources = GetHeatSources(ScenarioStreamID);
            if (HeatSources.Count > 0)
                SelectedHeatSource = HeatSources[0];
        }

        private ICommand _CalculateCommand;
        public ICommand CalculateCommand
        {
            get
            {
                if (_CalculateCommand == null)
                {
                    _CalculateCommand = new RelayCommand(Calculate);

                }
                return _CalculateCommand;
            }
        }

        public void Calculate(object obj)
        {
            int ID = int.Parse(obj.ToString());
            ScenarioHeatSource shs = db.GetModel(ID, SessionProtectedSystem);
            int HeatSourceID = shs.HeatSourceID;
            FeedBottomHXView v = new FeedBottomHXView();
            FeedBottomHXVM vm = new FeedBottomHXVM(HeatSourceID, SourceFileInfo, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (v.ShowDialog() == true)
            {
                SelectedHeatSource.IsFB = true;
            }
        }




        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(Save);

                }
                return _SaveCommand;
            }
        }

        public void Save(object obj)
        {            
            foreach (ScenarioHeatSourceModel m in HeatSources)
            {
                db.Update(m.model, SessionProtectedSystem);
            }
            SessionProtectedSystem.Flush();
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private ObservableCollection<ScenarioHeatSourceModel> GetHeatSources(int ScenarioStreamID,string HeatSourceType)
        {
            ObservableCollection<ScenarioHeatSourceModel> list = new ObservableCollection<ScenarioHeatSourceModel>();
            IList<ScenarioHeatSource> eqs = db.GetScenarioStreamHeatSourceList(SessionProtectedSystem, ScenarioStreamID, HeatSourceType);
            foreach (ScenarioHeatSource eq in eqs)
            {
                HeatSource hs = dbHS.GetModel(eq.HeatSourceID, SessionProtectedSystem);
                ScenarioHeatSourceModel model = new ScenarioHeatSourceModel(eq);
                model.HeatSourceName = hs.HeatSourceName;
                model.HeatSourceType = hs.HeatSourceType;
                model.Duty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, uomEnum.UserEnthalpyDuty, hs.Duty);
                list.Add(model);
            }
            return list;
        }

        private ObservableCollection<ScenarioHeatSourceModel> GetHeatSources(int ScenarioStreamID)
        {
            ObservableCollection<ScenarioHeatSourceModel> list = new ObservableCollection<ScenarioHeatSourceModel>();
            IList<ScenarioHeatSource> eqs = db.GetScenarioStreamHeatSourceList(SessionProtectedSystem, ScenarioStreamID);
            foreach (ScenarioHeatSource eq in eqs)
            {
                HeatSource hs = dbHS.GetModel(eq.HeatSourceID, SessionProtectedSystem);
                ScenarioHeatSourceModel model = new ScenarioHeatSourceModel(eq);
                model.HeatSourceName = hs.HeatSourceName;
                model.HeatSourceType = hs.HeatSourceType;
                model.Duty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, uomEnum.UserEnthalpyDuty, hs.Duty);                
                list.Add(model);
            }
            return list;
        }

    }
}
