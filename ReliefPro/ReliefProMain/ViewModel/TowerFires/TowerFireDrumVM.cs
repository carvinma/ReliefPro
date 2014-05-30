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
using NHibernate;

namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireDrumVM
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public double Area { get; set; }
        public TowerFireDrum model { get; set; }
        public List<string> Orientations { get; set; }
        public List<string> HeadTypes { get; set; }

        public TowerFireDrumVM(int EqID,  ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            Orientations = getOrientations();
            HeadTypes = getHeadTypes();

            TowerFireDrumDAL db = new TowerFireDrumDAL();
            model = db.GetModel(SessionProtectedSystem, EqID);
            if (model == null)
            {
                model = new TowerFireDrum();
                model.EqID = EqID;
                db.Add(model, SessionProtectedSystem);
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
            
                TowerFireDrumDAL db = new TowerFireDrumDAL();
                TowerFireDrum m = db.GetModel(model.ID, SessionProtectedSystem);
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
                    db.Update(m, SessionProtectedSystem);

                    SessionProtectedSystem.Flush();
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
