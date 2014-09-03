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
using ReliefProMain.Models;
using UOMLib;
using NHibernate;
using System.Windows;

namespace ReliefProMain.ViewModel.TowerFires
{
    public class TowerFireColumnVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public TowerFireColumnModel model { get; set; }
        public double Area { get; set; }
        

        public UOMLib.UOMEnum uomEnum { get; set; }
        public TowerFireColumnVM(int EqID, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();

            

            TowerFireColumnDetailDAL dbDetail = new TowerFireColumnDetailDAL();
            TowerFireColumnDAL db = new TowerFireColumnDAL();

            TowerFireColumn c = db.GetModel(sessionProtectedSystem, EqID);
            
            if (c == null)
            {
                c = new TowerFireColumn();
                c.NumberOfSegment = 0;
                c.EqID = EqID;
                c.PipingContingency = 10;
                model = new TowerFireColumnModel(c);              
                try
                {
                    db.Add(model.dbmodel, sessionProtectedSystem);
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                model = new TowerFireColumnModel(c);                                
            }
            ReadConvert();
            IList<TowerFireColumnDetail> list = dbDetail.GetAllList(sessionProtectedSystem, model.ID);
            model.Details = new ObservableCollection<TowerFireColumnDetailModel>();
            foreach (TowerFireColumnDetail detail in list)
            {
                TowerFireColumnDetailModel dm = new TowerFireColumnDetailModel(detail);
                dm.Height = UnitConvert.Convert(UOMEnum.Length, uomEnum.UserLength, detail.Height);
                dm.Diameter = UnitConvert.Convert(UOMEnum.Length, uomEnum.UserLength, detail.Diameter);
                model.Details.Add(dm);
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
            try
            {
                if (!model.CheckData()) return;
                TowerFireColumnDAL db = new TowerFireColumnDAL();
                
                WriteConvert();
                model.dbmodel.NumberOfSegment = model.NumberOfSegment;
                //model.dbmodel.LiquidHoldup = model.InstaLiquidHoldup;
                model.dbmodel.PipingContingency = model.PipingContingency;
                               
                db.Update(model.dbmodel, SessionProtectedSystem);
                SessionProtectedSystem.Flush();

                TowerFireColumnDetailDAL dbDetail = new TowerFireColumnDetailDAL();
                IList<TowerFireColumnDetail> list = dbDetail.GetAllList(SessionProtectedSystem, model.ID);
                //model.Details = new ObservableCollection<TowerFireColumnDetail>();
                foreach (TowerFireColumnDetail d in list)
                {
                    dbDetail.Delete(d, SessionProtectedSystem);
                }

                foreach (TowerFireColumnDetailModel detail in model.Details)
                {
                    detail.dbmodel.ColumnID = detail.ColumnID;
                    detail.dbmodel.Diameter = UnitConvert.Convert(uomEnum.UserLength, UOMEnum.Length, detail.Diameter); 
                    detail.dbmodel.Height = UnitConvert.Convert(uomEnum.UserLength, UOMEnum.Length, detail.Height);
                    detail.dbmodel.Segment = detail.Segment;
                    detail.dbmodel.Internal = detail.Internal;
                    detail.dbmodel.Trays = detail.Trays;
                    detail.dbmodel.Diameter_Color = detail.Diameter_Color;
                    detail.dbmodel.Height_Color = detail.Height_Color;
                    dbDetail.Add(detail.dbmodel, SessionProtectedSystem);
                    double L3 = model.Elevation;
                    double L1 = model.BNLL;
                    double hw = detail.Height;
                    int n = detail.Trays;
                    double L2 = (hw + 0.05) * n;
                    double diameter = detail.Diameter;
                    Area = Area + Algorithm.GetColumnArea(detail.Internal, n, L1, L2, L3, diameter);

                }
                

                Area = Area + Area * model.PipingContingency / 100;



                System.Windows.Window wd = window as System.Windows.Window;

                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

       
        private void ReadConvert()
        {
            model.Elevation = UnitConvert.Convert(UOMEnum.Length, elevationUnit, model.dbmodel.Elevation);
            model.BNLL = UnitConvert.Convert(UOMEnum.Length, levelUnit, model.dbmodel.BNLL);
        }
        private void WriteConvert()
        {
            model.dbmodel.Elevation = UnitConvert.Convert(elevationUnit, UOMEnum.Length, model.Elevation);
            model.dbmodel.BNLL = UnitConvert.Convert(levelUnit, UOMEnum.Length, model.BNLL);
        }
        private void InitUnit()
        {
            this.elevationUnit = uomEnum.UserLength;
            this.levelUnit = uomEnum.UserLength;

        }

        #region 单位字段
       
        private string levelUnit;
        public string LevelUnit
        {
            get { return levelUnit; }
            set
            {
                levelUnit = value;
                OnPropertyChanged("LevelUnit");
            }
        }
        private string elevationUnit;
        public string ElevationUnit
        {
            get { return elevationUnit; }
            set
            {
                elevationUnit = value;
                OnPropertyChanged("ElevationUnit");
            }
        }
       
        #endregion
    }
}
