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
using ReliefProMain.Models;
using ReliefProCommon.Enum;
namespace ReliefProMain.ViewModel.TowerFires
{
    public class AreaVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public TowerFireCoolerModel model { get; set; }





        public double Area { get; set; }
        UOMLib.UOMEnum uomEnum;
        public AreaVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();
            SessionProtectedSystem = sessionProtectedSystem;
            TowerFireCoolerDAL db = new TowerFireCoolerDAL();
            TowerFireCooler sizemodel = db.GetModel(SessionProtectedSystem, EqID);
            if (sizemodel == null)
            {
                sizemodel = new TowerFireCooler();
                sizemodel.EqID = EqID;
                sizemodel.PipingContingency = 10;
                sizemodel.WettedArea_Color = ColorBorder.green.ToString();
                sizemodel.PipingContingency_Color = ColorBorder.green.ToString();
                db.Add(sizemodel, SessionProtectedSystem);
            }
            model = new TowerFireCoolerModel(sizemodel);
            ReadConvert();
            

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
            if (!CheckData()) return;
            
            TowerFireCoolerDAL db = new TowerFireCoolerDAL();
            
            WriteConvert();
          
            db.Update(model.dbmodel, SessionProtectedSystem);
            SessionProtectedSystem.Flush();
            Area = model.WettedArea;
            Area = Area + Area * model.PipingContingency / 100;


            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private void ReadConvert()
        {
            if (model.WettedArea!=null)
            model.WettedArea = UnitConvert.Convert(UOMEnum.Area, wetteAreaUnit, model.dbmodel.WettedArea);
        }
        private void WriteConvert()
        {
            if (model.WettedArea!=null)
            model.dbmodel.WettedArea = UnitConvert.Convert(wetteAreaUnit, UOMEnum.Area, model.WettedArea);
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
