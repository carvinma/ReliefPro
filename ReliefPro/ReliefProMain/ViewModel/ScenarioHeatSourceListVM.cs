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
using ReliefProMain.Model;
using NHibernate;
using ReliefProMain.ViewModel.Drums;
using System.Windows;
using System.IO;

namespace ReliefProMain.ViewModel
{
    public class ScenarioHeatSourceListVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string PrzFile;

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



        public ScenarioHeatSourceListVM(int ScenarioStreamID,string PrzFile, ISession SessionPlant, ISession SessionProtectedSystem)
        {
            this.ScenarioStreamID = ScenarioStreamID;
            this.SessionPlant = SessionPlant;
            this.SessionProtectedSystem = SessionProtectedSystem;
            this.PrzFile = PrzFile;
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
            int ID=int.Parse(obj.ToString());
            ScenarioHeatSource shs=db.GetModel(ID,SessionProtectedSystem);
            int HeatSourceID = shs.HeatSourceID;            
            FeedBottomHXView v = new FeedBottomHXView();
            FeedBottomHXVM vm = new FeedBottomHXVM(HeatSourceID, PrzFile,SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                SelectedHeatSource.IsFB = true ;
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
           // IList<ScenarioHeatSource> list = db.GetScenarioStreamList(SessionProtectedSystem, ScenarioStreamID);
           // for (int i = 0; i < list.Count; i++)
           // {
           //     db.Delete(list[i], SessionProtectedSystem);
          //  }


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

        private ObservableCollection<ScenarioHeatSourceModel> GetHeatSources(int ScenarioStreamID)
        {
            ObservableCollection<ScenarioHeatSourceModel> list = new ObservableCollection<ScenarioHeatSourceModel>();
            IList<ScenarioHeatSource> eqs = db.GetScenarioStreamList(SessionProtectedSystem, ScenarioStreamID);
            foreach (ScenarioHeatSource eq in eqs)
            {
                HeatSource hs = dbHS.GetModel(eq.HeatSourceID, SessionProtectedSystem);
                ScenarioHeatSourceModel model = new ScenarioHeatSourceModel(eq);
                model.HeatSourceName = hs.HeatSourceName;
                model.HeatSourceType = hs.HeatSourceType;
                model.Duty = hs.Duty;
                list.Add(model);
            }
            return list;
        }
       
    }
}
