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
using ReliefProMain.View.TowerFire;

namespace ReliefProMain.ViewModel.TowerFire
{
    public class TowerFireVM
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }

        public IList<TowerFireEq> EqList{ get; set; }
        public ReliefProModel.TowerFire model { get; set; }
        public List<string> HeatInputModels { get; set; }

        public TowerFireVM(int ScenarioID, string dbPSFile, string dbPFile)
        {
            HeatInputModels = GetHeatInputModels();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list=dbBU.GetAllList(Session);
                BU = list.Where(s=>s.IsDefault==1).Single();
            }
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc=new UnitConvert();
                var Session = helper.GetCurrentSession();
                dbTowerFire db = new dbTowerFire();
                model = db.GetModel(Session, ScenarioID);

                dbTowerFireEq dbtfeq = new dbTowerFireEq();
                EqList = dbtfeq.GetAllList(Session,model.ID);
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
                ReliefProModel.TowerFire m = db.GetModel(model.ID, Session);
                m.HeatInputModel = model.HeatInputModel;
                m.IsExist = model.IsExist;
                m.ReliefCpCv = model.ReliefCpCv;
                m.ReliefLoad = model.ReliefLoad;
                m.ReliefMW = model.ReliefMW;
                m.ReliefPressure = model.ReliefPressure;
                m.ReliefTemperature = model.ReliefTemperature;
                m.ReliefZ = model.ReliefZ;
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
                int id=int.Parse(obj.ToString());
                var Session = helper.GetCurrentSession();
                dbTowerFireEq db = new dbTowerFireEq();
                TowerFireEq eq = db.GetModel(id, Session);
                if (eq.Type == "Column" || eq.Type=="Side Column")
                {
                    TowerFireColumnView v = new TowerFireColumnView();
                    TowerFireColumnVM vm = new TowerFireColumnVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
                else if (eq.Type == "Drum" )
                {
                    TowerFireDrumView v = new TowerFireDrumView();
                    TowerFireDrumVM vm = new TowerFireDrumVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
                else if (eq.Type == "Shell-Tube HX")
                {
                    TowerFireHXView v = new TowerFireHXView();
                    TowerFireHXVM vm = new TowerFireHXVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
                else if (eq.Type == "Air Cooler")
                {
                    TowerFireCoolerView v = new TowerFireCoolerView();
                    TowerFireCoolerVM vm = new TowerFireCoolerVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
                else if (eq.Type == "Other HX")
                {
                    TowerFireOtherView v = new TowerFireOtherView();
                    TowerFireOtherVM vm = new TowerFireOtherVM(eq.ID, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    v.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
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
                return _OKClick;
            }
        }

        private void Run(object obj)
        {

           
        }

    }
}
