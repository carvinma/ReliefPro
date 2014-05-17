using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.Model;
using UOMLib;

namespace ReliefProMain.ViewModel.TowerFire
{
    public class TowerFireColumnVM
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        public TowerFireColumnModel model { get; set; }
        public double Area { get; set; }
        public ObservableCollection<string> Internals { get; set; }
        public ObservableCollection<TowerFireColumnDetail> LastDetails { get; set; }
        public TowerFireColumnVM(int EqID, string dbPSFile, string dbPFile)
        {
            Internals = getInternals();
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
                dbTowerFireColumnDetail dbDetail = new dbTowerFireColumnDetail();
                dbTowerFireColumn db = new dbTowerFireColumn();
                model = new TowerFireColumnModel();
                model.Instance = db.GetModel(Session, EqID);
                if (model.Instance == null)
                {
                    model.Instance = new TowerFireColumn();
                    model.Instance.NumberOfSegment = "0";
                    model.Instance = new TowerFireColumn();
                    model.Instance.NumberOfSegment = "0";
                    model.Instance.EqID = EqID;
                    model.Instance.PipingContingency = "10";
                    try
                    {
                        db.Add(model.Instance, Session);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                IList<TowerFireColumnDetail> list = dbDetail.GetAllList(Session, model.Instance.ID);
                model.Details = new ObservableCollection<TowerFireColumnDetail>();
                foreach (TowerFireColumnDetail detail in list)
                {
                    model.Details.Add(detail);
                }

                LastDetails = model.Details;



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
                TowerFireColumn m = db.GetModel(model.Instance.ID, Session);
                m.BNLL = model.Instance.BNLL;
                m.NumberOfSegment = model.Instance.NumberOfSegment;
                m.LiquidHoldup = model.Instance.LiquidHoldup;
                m.PipingContingency = model.Instance.PipingContingency;
                m.Elevation = model.Instance.Elevation;
                db.Update(m, Session);


                dbTowerFireColumnDetail dbDetail = new dbTowerFireColumnDetail();
                for (int i = 0; i < LastDetails.Count; i++)
                {
                    if (LastDetails[i].ID != 0)
                    {
                        dbDetail.Delete(LastDetails[i], Session);
                    }
                }

                foreach (TowerFireColumnDetail detail in model.Details)
                {
                    dbDetail.Add(detail, Session);
                    double L3 = double.Parse(model.Instance.Elevation);
                    double L1 = double.Parse(model.Instance.BNLL);
                    double hw = double.Parse(detail.Height);
                    int n = int.Parse(detail.Trays); ;
                    double L2 = (hw + 0.05) * n;
                    double diameter = double.Parse(detail.Diameter);
                    Area = Area + Algorithm.GetColumnArea(detail.Internal, n, L1, L2, L3, diameter);

                }
                Session.Flush();

                Area = Area + Area * double.Parse(model.Instance.PipingContingency) / 100;


            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private ObservableCollection<string> getInternals()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Trayed");
            list.Add("Packed");
            return list;
        }
    }
}
