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
using ReliefProMain.View.TowerFire;
using ReliefProMain.Model;
using UOMLib;
using ReliefProMain.ViewModel;

namespace ReliefProMain.ViewModel.TowerFire
{
    public class TowerFireVM:ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        private List<string> HeatInputModels { get; set; }
        private Latent latent;

        public TowerFireModel model { get; set; }
        public TowerFireModel CurrentModel { get; set; }
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

       
        public TowerFireVM(int ScenarioID, string dbPSFile, string dbPFile)
        {
            CurrentModel = new TowerFireModel();
            HeatInputModels = GetHeatInputModels();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
            }
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc = new UnitConvert();
                var Session = helper.GetCurrentSession();
                dbTowerFire db = new dbTowerFire();
                model.CurrentTowerFire = db.GetModel(Session, ScenarioID);

                dbTowerFireEq dbtfeq = new dbTowerFireEq();
                IList<TowerFireEq> list = dbtfeq.GetAllList(Session, model.CurrentTowerFire.ID);
                model.EqList = new ObservableCollection<TowerFireEq>();
                foreach (TowerFireEq eq in list)
                {
                    model.EqList.Add(eq);
                }

                dbLatent dblatent = new dbLatent();
                latent = dblatent.GetModel(Session);


            }

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

            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
            }
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerFire db = new dbTowerFire();
                ReliefProModel.TowerFire m = db.GetModel(model.CurrentTowerFire.ID, Session);
                m.HeatInputModel = model.CurrentTowerFire.HeatInputModel;
                m.IsExist = model.CurrentTowerFire.IsExist;
                m.ReliefCpCv = model.CurrentTowerFire.ReliefCpCv;
                m.ReliefLoad = model.CurrentTowerFire.ReliefLoad;
                m.ReliefMW = model.CurrentTowerFire.ReliefMW;
                m.ReliefPressure = model.CurrentTowerFire.ReliefPressure;
                m.ReliefTemperature = model.CurrentTowerFire.ReliefTemperature;
                m.ReliefZ = model.CurrentTowerFire.ReliefZ;
                db.Update(m, Session);
                Session.Flush();
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        public List<string> GetHeatInputModels()
        {
            List<string> list = new List<string>();
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
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();

                double C1 = 0;
                if (model.CurrentTowerFire.IsExist)
                {
                    C1 = 43200;
                }
                else
                {
                    C1 = 70900;
                }
                int id = int.Parse(obj.ToString());

                dbTowerFireEq db = new dbTowerFireEq();
                TowerFireEq eq = db.GetModel(id, Session);

                double latentEnthalpy = double.Parse(latent.LatentEnthalpy);
                if (eq.Type == "Column" || eq.Type == "Side Column")
                {
                    TowerFireColumnView v = new TowerFireColumnView();
                    TowerFireColumnVM vm = new TowerFireColumnVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {
                        model.EqList.Clear();
                        eq.WettedArea = vm.Area.ToString(); ;
                        eq.HeatInput = Algorithm.GetTowerQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                        eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                        db.Update(eq, Session);
                        IList<TowerFireEq> list = db.GetAllList(Session, model.CurrentTowerFire.ID);
                        foreach (TowerFireEq q in list)
                        {
                            model.EqList.Add(q);
                        }
                        Session.Flush();
                    }
                }
                else if (eq.Type == "Drum")
                {
                    TowerFireDrumView v = new TowerFireDrumView();
                    TowerFireDrumVM vm = new TowerFireDrumVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {
                        model.EqList.Clear();
                        eq.WettedArea = vm.Area.ToString();
                        eq.HeatInput = Algorithm.GetTowerQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                        eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                        db.Update(eq, Session);
                        IList<TowerFireEq> list = db.GetAllList(Session, model.CurrentTowerFire.ID);
                        foreach (TowerFireEq q in list)
                        {
                            model.EqList.Add(q);
                        }
                        Session.Flush();
                    }
                }
                else if (eq.Type == "Shell-Tube HX")
                {
                    TowerFireHXView v = new TowerFireHXView();
                    TowerFireHXVM vm = new TowerFireHXVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {
                        model.EqList.Clear();
                        eq.WettedArea = vm.Area.ToString();
                        eq.HeatInput = Algorithm.GetTowerQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                        eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                        db.Update(eq, Session);
                        IList<TowerFireEq> list = db.GetAllList(Session, model.CurrentTowerFire.ID);
                        foreach (TowerFireEq q in list)
                        {
                            model.EqList.Add(q);
                        }
                        Session.Flush();
                    }
                }
                else if (eq.Type == "Air Cooler")
                {
                    TowerFireCoolerView v = new TowerFireCoolerView();
                    TowerFireCoolerVM vm = new TowerFireCoolerVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {
                        model.EqList.Clear();
                        eq.WettedArea = vm.Area.ToString();
                        eq.HeatInput = Algorithm.GetTowerQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                        eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                        db.Update(eq, Session);
                        IList<TowerFireEq> list = db.GetAllList(Session, model.CurrentTowerFire.ID);
                        foreach (TowerFireEq q in list)
                        {
                            model.EqList.Add(q);
                        }
                        Session.Flush();
                    }
                }
                else if (eq.Type == "Other HX")
                {
                    TowerFireOtherView v = new TowerFireOtherView();
                    TowerFireOtherVM vm = new TowerFireOtherVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {
                        model.EqList.Clear();
                        eq.WettedArea = vm.Area.ToString();
                        eq.HeatInput = Algorithm.GetTowerQ(C1, double.Parse(eq.FFactor), double.Parse(eq.WettedArea)).ToString();
                        eq.ReliefLoad = (double.Parse(eq.HeatInput) / latentEnthalpy).ToString();
                        db.Update(eq, Session);
                        IList<TowerFireEq> list = db.GetAllList(Session, model.CurrentTowerFire.ID);
                        foreach (TowerFireEq q in list)
                        {
                            model.EqList.Add(q);
                        }
                        Session.Flush();
                    }
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
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc = new UnitConvert();
                var Session = helper.GetCurrentSession();
                dbTowerFire db = new dbTowerFire();
                ReliefProModel.TowerFire m = db.GetModel(Session, model.CurrentTowerFire.ID);

                foreach (TowerFireEq eq in model.EqList)
                {
                    if (eq.FireZone)
                    {
                        m.ReliefLoad = (double.Parse(m.ReliefLoad ?? "0") + double.Parse(eq.ReliefLoad ?? "0")).ToString();
                    }
                }
                db.Update(m, Session);
                model.CurrentTowerFire = m;
                Session.Flush();
            }

        }

    }
}
