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
    public class TowerFireCoolerVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public TowerFireCooler model { get; set; }
        public double Area { get; set; }
        UnitConvert unitConvert;
        UOMLib.UOMEnum uomEnum;
        public TowerFireCoolerVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            unitConvert = new UnitConvert();
            uomEnum = new UOMLib.UOMEnum(sessionPlant);
            InitUnit();
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            TowerFireCoolerDAL db = new TowerFireCoolerDAL();
            model = db.GetModel(SessionProtectedSystem, EqID);
            if (model == null)
            {
                model = new TowerFireCooler();
                model.EqID = EqID;
                model.PipingContingency = "10";
                db.Add(model, SessionProtectedSystem);
            }
            else
            {
                ReadConvert();
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

            TowerFireCoolerDAL db = new TowerFireCoolerDAL();
            TowerFireCooler m = db.GetModel(model.ID, SessionProtectedSystem);
            WriteConvert();
            m.WettedArea = model.WettedArea;
            m.PipingContingency = model.PipingContingency;
            db.Update(m, SessionProtectedSystem);
            SessionProtectedSystem.Flush();
            Area = double.Parse(m.WettedArea);
            Area = Area + Area * double.Parse(model.PipingContingency) / 100;


            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private void ReadConvert()
        {
            if (!string.IsNullOrEmpty(model.WettedArea))
                model.WettedArea = unitConvert.Convert(UOMEnum.Area, wetteAreaUnit, double.Parse(model.WettedArea)).ToString();
        }
        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(model.WettedArea))
                model.WettedArea = unitConvert.Convert(wetteAreaUnit, UOMEnum.Area, double.Parse(model.WettedArea)).ToString();
        }
        private void InitUnit()
        {
            this.wetteAreaUnit = uomEnum.UserArea;
        }
        private string wetteAreaUnit;
        public string WetteAreaUnit
        {
            get { return wetteAreaUnit; }
            set
            {
                wetteAreaUnit = value;
                this.OnPropertyChanged("WetteAreaUnit");
            }
        }
    }
}
