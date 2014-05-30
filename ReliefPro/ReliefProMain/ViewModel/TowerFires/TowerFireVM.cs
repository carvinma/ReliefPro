using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
using ReliefProMain.View.TowerFires;
using ReliefProMain.Model;
using UOMLib;
using ReliefProMain.ViewModel;
using NHibernate;



namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public ObservableCollection<string> HeatInputModels { get; set; }
        private Latent latent;

        public TowerFireModel MainModel { get; set; }
        //public ReliefProModel.TowerFire CurrentModel { get; set; }
        private ObservableCollection<TowerFireEq> _EqList;
        public ObservableCollection<TowerFireEq> EqList
        {
            get
            {
                return this._EqList;
            }
            set
            {
                this._EqList = value;
                OnPropertyChanged("EqList");
            }
        }


        public TowerFireVM(int ScenarioID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            HeatInputModels = GetHeatInputModels();
            
            UnitConvert uc = new UnitConvert();
            TowerFireDAL db = new TowerFireDAL();
            ReliefProModel.TowerFire model = db.GetModel(SessionProtectedSystem, ScenarioID);
            MainModel = new TowerFireModel(model);

            TowerFireEqDAL dbtfeq = new TowerFireEqDAL();
            IList<TowerFireEq> list = dbtfeq.GetAllList(SessionProtectedSystem, MainModel.ID);
            EqList = new ObservableCollection<TowerFireEq>();
            foreach (TowerFireEq eq in list)
            {
                EqList.Add(eq);
            }

            LatentDAL dblatent = new LatentDAL();
            latent = dblatent.GetModel(SessionProtectedSystem);

        }

        private ICommand _OKClick;
        public ICommand OKClick
        {
            get
            {
                if (_OKClick == null)
                {
                    _OKClick = new RelayCommand(Update);

                }
                return _OKClick;
            }
        }

        private void Update(object window)
        {
                TowerFireDAL db = new TowerFireDAL();
                //ReliefProModel.TowerFire m = db.GetModel(CurrentModel.ID, SessionProtectedSystem);
                //m.HeatInputModel = CurrentModel.HeatInputModel;
                //m.IsExist = CurrentModel.IsExist;
                //m.ReliefCpCv = CurrentModel.ReliefCpCv;
                //m.ReliefLoad = CurrentModel.ReliefLoad;
                //m.ReliefMW = CurrentModel.ReliefMW;
                //m.ReliefPressure = CurrentModel.ReliefPressure;
                //m.ReliefTemperature = CurrentModel.ReliefTemperature;
                //m.ReliefZ = CurrentModel.ReliefZ;
                db.Update(MainModel.model, SessionProtectedSystem);
                SessionProtectedSystem.Flush();
            
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        public ObservableCollection<string> GetHeatInputModels()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("API 521");
            return list;
        }

        private ICommand _EditClick;
        public ICommand EditClick
        {
            get
            {
                if (_EditClick == null)
                {
                    _EditClick = new RelayCommand(Edit);

                }
                return _EditClick;
            }
        }
        private void Edit(object obj)
        {

            double C1 = 0;
            if (MainModel.IsExist)
            {
                C1 = 43200;
            }
            else
            {
                C1 = 70900;
            }
            int id = int.Parse(obj.ToString());

            TowerFireEqDAL db = new TowerFireEqDAL();
            TowerFireEq eq = db.GetModel(id, SessionProtectedSystem);

            double latentEnthalpy = double.Parse(latent.LatentEnthalpy);
            if (eq.Type == "Column" || eq.Type == "Side Column")
            {
                TowerFireColumnView v = new TowerFireColumnView();
                TowerFireColumnVM vm = new TowerFireColumnVM(eq.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    EqList.Clear();
                    eq.WettedArea = vm.Area.ToString(); ;
                    eq.HeatInput = Algorithm.GetQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                    eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                    db.Update(eq, SessionProtectedSystem);
                    IList<TowerFireEq> list = db.GetAllList(SessionProtectedSystem, MainModel.ID);
                    foreach (TowerFireEq q in list)
                    {
                        EqList.Add(q);
                    }
                    SessionProtectedSystem.Flush();
                }
            }
            else if (eq.Type == "Drum")
            {
                TowerFireDrumView v = new TowerFireDrumView();
                TowerFireDrumVM vm = new TowerFireDrumVM(eq.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    EqList.Clear();
                    eq.WettedArea = vm.Area.ToString();
                    eq.HeatInput = Algorithm.GetQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                    eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                    db.Update(eq, SessionProtectedSystem);
                    IList<TowerFireEq> list = db.GetAllList(SessionProtectedSystem, MainModel.ID);
                    foreach (TowerFireEq q in list)
                    {
                        EqList.Add(q);
                    }
                    SessionProtectedSystem.Flush();
                }
            }
            else if (eq.Type == "Shell-Tube HX")
            {
                TowerFireHXView v = new TowerFireHXView();
                TowerFireHXVM vm = new TowerFireHXVM(eq.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    EqList.Clear();
                    eq.WettedArea = vm.Area.ToString();
                    eq.HeatInput = Algorithm.GetQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                    eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                    db.Update(eq, SessionProtectedSystem);
                    IList<TowerFireEq> list = db.GetAllList(SessionProtectedSystem, MainModel.ID);
                    foreach (TowerFireEq q in list)
                    {
                        EqList.Add(q);
                    }
                    SessionProtectedSystem.Flush();
                }
            }
            else if (eq.Type == "Air Cooler")
            {
                TowerFireCoolerView v = new TowerFireCoolerView();
                TowerFireCoolerVM vm = new TowerFireCoolerVM(eq.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    EqList.Clear();
                    eq.WettedArea = vm.Area.ToString();
                    eq.HeatInput = Algorithm.GetQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                    eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                    db.Update(eq, SessionProtectedSystem);
                    IList<TowerFireEq> list = db.GetAllList(SessionProtectedSystem, MainModel.ID);
                    foreach (TowerFireEq q in list)
                    {
                        EqList.Add(q);
                    }
                    SessionProtectedSystem.Flush();
                }
            }
            else if (eq.Type == "Other HX")
            {
                TowerFireOtherView v = new TowerFireOtherView();
                TowerFireOtherVM vm = new TowerFireOtherVM(eq.ID, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    EqList.Clear();
                    eq.WettedArea = vm.Area.ToString();
                    eq.HeatInput = Algorithm.GetQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                    eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                    db.Update(eq, SessionProtectedSystem);
                    IList<TowerFireEq> list = db.GetAllList(SessionProtectedSystem, MainModel.ID);
                    foreach (TowerFireEq q in list)
                    {
                       EqList.Add(q);
                    }
                    SessionProtectedSystem.Flush();
                }
            }


        }


        private ICommand _TotalClick;
        public ICommand TotalClick
        {
            get
            {
                if (_TotalClick == null)
                {
                    _TotalClick = new RelayCommand(Run);

                }
                return _TotalClick;
            }
        }

        private void Run(object obj)
        {
            double reliefload = 0;
            foreach (TowerFireEq eq in EqList)
            {
                if (eq.FireZone)
                {
                    reliefload = reliefload + double.Parse(eq.ReliefLoad ?? "0");
                }
            }
            MainModel.ReliefLoad = reliefload.ToString();

            
        }

    }
}
