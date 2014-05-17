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
    public class TowerFireDrumVM
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        public double Area { get; set; }
        public TowerFireDrum model { get; set; }
        public List<string> Orientations { get; set; }
        public List<string> HeadTypes { get; set; } 

        public TowerFireDrumVM(int EqID, string dbPSFile, string dbPFile)
        {
            Orientations = getOrientations();
            HeadTypes = getHeadTypes();
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
                if (model == null)
                {
                    model = new TowerFireDrum();
                    model.EqID = EqID;
                    db.Add(model,Session);
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
                dbTowerFireDrum db = new dbTowerFireDrum();
                TowerFireDrum m = db.GetModel(model.ID, Session);
                m.Elevation = model.Elevation;
                m.BootDiameter = model.BootDiameter;
                m.BootHeight = model.BootHeight;
                m.Diameter = model.Diameter;
                m.HeadNumber = model.HeadNumber;
                m.HeadType = model.HeadType;
                m.Length = model.Length;
                m.NormalLiquidLevel = model.NormalLiquidLevel;
                m.Orientation = model.Orientation;
                m.PipingContingency = model.PipingContingency;
                try
                {
                    db.Update(m, Session);

                    Session.Flush();
                }
                catch (Exception ex)
                {
                }

                
                double elevation=double.Parse(m.Elevation);
                double diameter=double.Parse(m.Diameter);
                double length=double.Parse(m.Length);
                double NLL=double.Parse(m.NormalLiquidLevel);
                double bootheight = double.Parse(m.BootHeight);
                double bootdiameter = double.Parse(m.BootDiameter);
                
                Area = Algorithm.GetDrumArea(m.Orientation, model.HeadType, elevation, diameter, length, NLL, bootheight, bootdiameter);               
                Area = Area + Area * double.Parse(model.PipingContingency) / 100;
            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult=true;
            }
        }

        private List<string> getOrientations()
        {
            List<string> list=new List<string>();
            list.Add("Horiz");
            list.Add("Vertical");            
            return list;
        }
        private List<string> getHeadTypes()
        {
            List<string> list = new List<string>();
            list.Add("Eclipse");
            list.Add("Flat");
            return list;
        }

    }
}
