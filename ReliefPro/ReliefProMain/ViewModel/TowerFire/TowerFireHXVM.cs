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
using ReliefProMain;

namespace ReliefProMain.ViewModel.TowerFire
{
    public class TowerFireHXVM
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        public double Area { get; set; }
        public TowerFireHX model { get; set; }
        public List<string> ExposedToFires { get; set; }
        public List<string> Types { get; set; }
        public TowerFireHXVM(int EqID, string dbPSFile, string dbPFile)
        {
            ExposedToFires = GetExposedToFires();
            Types = GetTypes();
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
                dbTowerFireHX db = new dbTowerFireHX();
                model = db.GetModel(Session,EqID);
                if (model == null)
                {
                    model = new TowerFireHX();
                    model.EqID = EqID;
                    model.PipingContingency = "10";
                    db.Add(model, Session);
                }
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
                dbTowerFireHX db = new dbTowerFireHX();
                TowerFireHX m = db.GetModel(model.ID, Session);
                m.ExposedToFire = model.ExposedToFire;
                m.Length = model.Length;
                m.OD = model.OD;
                m.Type = model.Type;
                m.Elevation = model.Elevation;
                m.PipingContingency = model.PipingContingency;
                db.Update(m, Session);
                Session.Flush();

                double length = double.Parse(m.Length);
                double pipingContingency = double.Parse(m.PipingContingency);
                double od = double.Parse(m.OD);
                double D = double.Parse(m.Elevation);
                Area = Algorithm.GetHXArea(m.ExposedToFire, m.Type, length, od, D);
                Area = Area + Area * double.Parse(model.PipingContingency) / 100;

            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult=true;
            }
        }


        public List<string> GetExposedToFires()
        {
            List<string> list = new List<string>();
            list.Add("Shell");
            list.Add("Tube");
            return list;
        }
        public List<string> GetTypes()
        {
            List<string> list = new List<string>();
            list.Add("Fixed");
            list.Add("U-Tube");
            list.Add("Floating head");
            return list;
        }
    }
}
