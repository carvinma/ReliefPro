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

        public TowerFireHX model;

        public TowerFireHXVM(int EqID, string dbPSFile, string dbPFile)
        {
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
                m.PipingContingency = model.PipingContingency;
                db.Update(m, Session);

                double length = double.Parse(m.Length);
                double pipingContingency = double.Parse(m.PipingContingency);
                double od = double.Parse(m.OD);
                double D = double.Parse(m.Elevation);
                double Area = Algorithm.GetHXArea(m.ExposedToFire, m.Type, length, pipingContingency, od, D);

               


                Session.Flush();
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }
    }
}
