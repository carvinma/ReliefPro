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
    public class TowerFireDrumVM
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; } 

        public TowerFireDrum model;

        public TowerFireDrumVM(int EqID, string dbPSFile, string dbPFile)
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
                dbTowerFireDrum db = new dbTowerFireDrum();
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
                dbTowerFireDrum db = new dbTowerFireDrum();
                TowerFireDrum m = db.GetModel(model.ID, Session);
                m.BootDiameter = model.BootDiameter;
                m.BootHeight = model.BootHeight;
                m.Diameter = model.Diameter;
                m.HeadNumber = model.HeadNumber;
                m.HeadType = model.HeadType;
                m.Length = model.Length;
                m.NormalLiquidLevel = model.NormalLiquidLevel;
                m.Orientation = model.Orientation;
                m.PipingContingency = model.PipingContingency;
                db.Update(m, Session);

                double Area=0;
                double elevation=double.Parse(m.Elevation);
                double diameter=double.Parse(m.Diameter);
                double length=double.Parse(m.Length);
                double NLL=double.Parse(m.NormalLiquidLevel);
                double bootheight = double.Parse(m.BootHeight);
                double bootdiameter = double.Parse(m.BootDiameter);
                Area = Algorithm.GetDrumArea(m.Orientation, model.HeadType, elevation, diameter, length, NLL, bootheight, bootdiameter);
                
                Area = Area + Area * double.Parse(model.PipingContingency) / 100;








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
