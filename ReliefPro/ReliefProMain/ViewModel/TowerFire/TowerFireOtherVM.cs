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
using UOMLib;

namespace ReliefProMain.ViewModel.TowerFire
{
    public class TowerFireOtherVM
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        public double Area { get; set; }
        public TowerFireOther model { get; set; }

        public TowerFireOtherVM(int EqID, string dbPSFile, string dbPFile)
        {
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
                dbTowerFireOther db = new dbTowerFireOther();
                model = db.GetModel(Session, EqID);
                if (model == null)
                {
                    model = new TowerFireOther();
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
            model.WettedArea = model.WettedArea.Trim();
            if (model.WettedArea == "")
            {
                throw new ArgumentException("Please type in WettedArea.");
            }
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
                dbTowerFireOther db = new dbTowerFireOther();
                TowerFireOther m = db.GetModel(model.ID, Session);
                m.WettedArea = model.WettedArea;
                m.PipingContingency = model.PipingContingency;
                db.Update(m, Session);
                Session.Flush();
                Area = double.Parse(m.WettedArea);
                Area = Area + Area * double.Parse(model.PipingContingency) / 100;
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }
    }
}
