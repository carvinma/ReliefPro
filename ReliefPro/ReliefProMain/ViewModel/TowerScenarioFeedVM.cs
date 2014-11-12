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
using System.Collections.ObjectModel;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View;
using NHibernate;
using ReliefProMain.Models;
using System.Windows;

namespace ReliefProMain.ViewModel
{
    public class TowerScenarioFeedVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }     
        public string dbProtectedSystemFile { set; get; }
        public string dbPlantFile { set; get; }
        SourceFile SourceFileInfo; 
        public int ScenarioID { set; get; }
        private TowerScenarioStreamDAL db;
        private ObservableCollection<TowerScenarioStreamModel> _Feeds;
        public ObservableCollection<TowerScenarioStreamModel> Feeds
        {
            get
            {
                return this._Feeds;
            }
            set
            {
                this._Feeds = value;
                OnPropertyChanged("Feeds");
            }
        }


        private ObservableCollection<TowerScenarioStreamModel> GetTowerHXScenarioFeeds( bool IsProduct)
        {
            ObservableCollection<TowerScenarioStreamModel> feeds=new ObservableCollection<TowerScenarioStreamModel>();
            IList<TowerScenarioStream> list = db.GetAllList(SessionProtectedSystem, ScenarioID, IsProduct);   
            foreach(TowerScenarioStream s in list)
            {
                TowerScenarioStreamModel model = ConvertToModel(s);
                feeds.Add(model);
            }
            return feeds;
        }

        public TowerScenarioFeedVM(int scenarioID,SourceFile sourceFileInfo,  ISession sessionPlant, ISession sessionProtectedSystem,bool IsProduct)
        {
            db = new TowerScenarioStreamDAL();
            ScenarioID=scenarioID;
            SessionPlant = sessionPlant;
            SourceFileInfo = sourceFileInfo; 
            SessionProtectedSystem = sessionProtectedSystem;
            Feeds = GetTowerHXScenarioFeeds(IsProduct);
        }

        private TowerScenarioStreamModel ConvertToModel(TowerScenarioStream m)
        {
            TowerScenarioStreamModel model = new TowerScenarioStreamModel();
            model.ID = m.ID;
            model.ScenarioID = m.ScenarioID;
            model.SourceType = m.SourceType;
            model.StreamName = m.StreamName;
            model.FlowCalcFactor = m.FlowCalcFactor;
            model.FlowStop = m.FlowStop;
            model.IsNormal = m.IsNormal;
            model.ReliefNormalFactor = m.ReliefNormalFactor;
            return model;
        }
        private TowerScenarioStream ConvertToDBModel(TowerScenarioStreamModel m, ref TowerScenarioStream model)
        {
            model.ID = m.ID;
            model.ScenarioID = m.ScenarioID;
            model.SourceType = m.SourceType;
            model.StreamName = m.StreamName;
            model.FlowCalcFactor = m.FlowCalcFactor;
            model.FlowStop = m.FlowStop;
            model.IsNormal = m.IsNormal;
            model.ReliefNormalFactor = m.ReliefNormalFactor;
            return model;
        }

        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get { return _SaveCommand ?? (_SaveCommand = new RelayCommand(Save)); }
        }


        private void Save(object window)
        {
            foreach (TowerScenarioStreamModel m in Feeds)
            {
                TowerScenarioStream s=db.GetModel(m.ID,SessionProtectedSystem);
                ConvertToDBModel(m, ref s);
                db.Update(s,SessionProtectedSystem);
            }
            SessionProtectedSystem.Flush();
                
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
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

        private void Calculate(object obj)
        {
            int SCStreamID = int.Parse(obj.ToString());
            ScenarioHeatSourceListView v = new ScenarioHeatSourceListView();
            ScenarioHeatSourceListVM vm = new ScenarioHeatSourceListVM(SCStreamID, SourceFileInfo, SessionPlant, SessionProtectedSystem, "Feed/Bottom HX");
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (v.ShowDialog() == true)
            {

            }
        }

        private ICommand _Calculate2Command;
        public ICommand Calculate2Command
        {
            get
            {
                if (_Calculate2Command == null)
                {
                    _Calculate2Command = new RelayCommand(Calculate2);

                }
                return _Calculate2Command;
            }
        }

        private void Calculate2(object obj)
        {
            int SCStreamID = int.Parse(obj.ToString());
            TowerScenarioStream scs = db.GetModel(SCStreamID, SessionProtectedSystem);
            if (scs.FlowStop || scs.FlowCalcFactor == 0)
            {
                ScenarioHeatSourceListView v = new ScenarioHeatSourceListView();
                ScenarioHeatSourceListVM vm = new ScenarioHeatSourceListVM(SCStreamID, SourceFileInfo, SessionPlant, SessionProtectedSystem, "Fired Heater");
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {

                }
            }
            else
            {
                if (MessageBox.Show("Are you sure to edit it?", "Message Box", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ScenarioHeatSourceListView v = new ScenarioHeatSourceListView();
                    ScenarioHeatSourceListVM vm = new ScenarioHeatSourceListVM(SCStreamID, SourceFileInfo, SessionPlant, SessionProtectedSystem, "Fired Heater");
                    v.DataContext = vm;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {

                    }
                }
            }
            
        }
    
    }
}
