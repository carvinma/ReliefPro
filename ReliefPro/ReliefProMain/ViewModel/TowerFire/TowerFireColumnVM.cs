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

namespace ReliefProMain.ViewModel.TowerFire
{
    public class TowerFireColumnVM
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        public string Number { get; set; }
        public double Area{ get; set; }
        public TowerFireColumn model { get; set; } 
        public IList<TowerFireColumnDetail> Details { get; set; }
        public IList<TowerFireColumnDetail> LastDetails { get; set; }
        public TowerFireColumnVM(int EqID, string dbPSFile, string dbPFile)
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
                dbTowerFireColumnDetail dbDetail = new dbTowerFireColumnDetail();
                dbTowerFireColumn db = new dbTowerFireColumn();
                model = db.GetModel(Session,EqID);
                if (model == null)
                {
                    model = new TowerFireColumn();
                    db.Add(model, Session);
                }
                else
                {                   
                    Details = dbDetail.GetAllList(Session, model.ID);
                    LastDetails = Details;
                }
                for (int i = 0; i < LastDetails.Count; i++)
                {
                    dbDetail.Delete(LastDetails[i], Session);
                }
                

                foreach (TowerFireColumnDetail detail in Details)
                {
                    dbDetail.Add(detail, Session);
                    double L3 = double.Parse(model.Elevation);
                    double L1 = double.Parse(model.BNLL);
                    double hw = double.Parse(detail.Height);
                    int n = int.Parse(detail.Trays); ;
                    double L2 = (hw+0.05)*n;
                    double diameter=double.Parse(detail.Diameter);
                    Area = Area + Algorithm.GetColumnArea(detail.Internal, n, L1, L2, L3, diameter);
                    
                }

                Area = Area + Area * double.Parse(model.PipingContingency)/100;
                
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
                dbTowerFireColumn db = new dbTowerFireColumn();
                TowerFireColumn m = db.GetModel(model.ID, Session);
                if (m == null)
                {
                    m.BNLL = model.BNLL;
                    m.Number = model.Number;
                    m.LiquidHoldup = model.LiquidHoldup;
                    m.PipingContingency = model.PipingContingency;
                    db.Add(m, Session);
                }
                else
                {
                    m.BNLL = model.BNLL;
                    m.Number = model.Number;
                    m.LiquidHoldup = model.LiquidHoldup;
                    m.PipingContingency = model.PipingContingency;
                    db.Update(m, Session);
                }

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
