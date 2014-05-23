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
using NHibernate;

namespace ReliefProMain.ViewModel.TowerFire
{
    public class TowerFireColumnVM
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public TowerFireColumnModel model { get; set; }
        public double Area { get; set; }
        public ObservableCollection<string> Internals { get; set; }
        public ObservableCollection<TowerFireColumnDetail> LastDetails { get; set; }
        public TowerFireColumnVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            Internals = getInternals();

            dbTowerFireColumnDetail dbDetail = new dbTowerFireColumnDetail();
            dbTowerFireColumn db = new dbTowerFireColumn();
            model = new TowerFireColumnModel();
            model.Instance = db.GetModel(sessionProtectedSystem, EqID);
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
                    db.Add(model.Instance, sessionProtectedSystem);
                }
                catch (Exception ex)
                {
                }
            }

            IList<TowerFireColumnDetail> list = dbDetail.GetAllList(sessionProtectedSystem, model.Instance.ID);
            model.Details = new ObservableCollection<TowerFireColumnDetail>();
            foreach (TowerFireColumnDetail detail in list)
            {
                model.Details.Add(detail);
            }

            LastDetails = model.Details;

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

            dbTowerFireColumn db = new dbTowerFireColumn();
            TowerFireColumn m = db.GetModel(model.Instance.ID, SessionProtectedSystem);
            m.BNLL = model.Instance.BNLL;
            m.NumberOfSegment = model.Instance.NumberOfSegment;
            m.LiquidHoldup = model.Instance.LiquidHoldup;
            m.PipingContingency = model.Instance.PipingContingency;
            m.Elevation = model.Instance.Elevation;
            db.Update(m, SessionProtectedSystem);


            dbTowerFireColumnDetail dbDetail = new dbTowerFireColumnDetail();
            for (int i = 0; i < LastDetails.Count; i++)
            {
                if (LastDetails[i].ID != 0)
                {
                    dbDetail.Delete(LastDetails[i], SessionProtectedSystem);
                }
            }

            foreach (TowerFireColumnDetail detail in model.Details)
            {
                dbDetail.Add(detail, SessionProtectedSystem);
                double L3 = double.Parse(model.Instance.Elevation);
                double L1 = double.Parse(model.Instance.BNLL);
                double hw = double.Parse(detail.Height);
                int n = int.Parse(detail.Trays); ;
                double L2 = (hw + 0.05) * n;
                double diameter = double.Parse(detail.Diameter);
                Area = Area + Algorithm.GetColumnArea(detail.Internal, n, L1, L2, L3, diameter);

            }
            SessionProtectedSystem.Flush();

            Area = Area + Area * double.Parse(model.Instance.PipingContingency) / 100;



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
