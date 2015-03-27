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
using System.Collections.ObjectModel;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View;
using ReliefProMain.Models;
using NHibernate;
using UOMLib;
using System.Windows;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class TowerHXVM : ViewModelBase
    {

        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public TowerHXModel model { set; get; }
        public UOMLib.UOMEnum uomEnum { get; set; }

        private ObservableCollection<TowerHXDetailModel> _Details = null;
        public ObservableCollection<TowerHXDetailModel> Details
        {
            get { return _Details; }
            set
            {
                _Details = value;
                OnPropertyChanged("Details");
            }
        }
        internal ObservableCollection<TowerHXDetailModel> GetTowerHXDetails()
        {
            _Details = new ObservableCollection<TowerHXDetailModel>();

            TowerHXDetailDAL towerHXDetailDAL = new TowerHXDetailDAL();
            int i = 0;
            foreach (var obj in towerHXDetailDAL.GetAllList(SessionProtectedSystem, model.ID))
            {
                TowerHXDetailModel d = new TowerHXDetailModel();
                d.Parent = model;
                d.SeqNumber = i;
                d.DetailName = obj.DetailName;
                d.ProcessSideFlowSource = obj.ProcessSideFlowSource;
                d.Medium = obj.Medium;
                d.MediumSideFlowSource = obj.MediumSideFlowSource;
                d.ID = obj.ID;
                d.HXID = obj.HXID;
                d.Duty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, uomEnum.UserEnthalpyDuty, obj.Duty);
                d.DutyPercentage = obj.DutyPercentage;

                _Details.Add(d);
                i = i + 1;

            }

            return _Details;
        }

        private TowerHXDetailModel _SelectedDetail;
        public TowerHXDetailModel SelectedDetail
        {
            get
            {
                return this._SelectedDetail;
            }
            set
            {
                this._SelectedDetail = value;
                OnPropertyChanged("SelectedDetail");
            }
        }
        public TowerHXVM(string name, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;

            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();

            TowerHXDAL db = new TowerHXDAL();
            TowerHX hx = db.GetModel(SessionProtectedSystem, name);
            model = new TowerHXModel();
            model.ID = hx.ID;
            model.HeaterDuty = hx.HeaterDuty;
            model.HeaterType = hx.HeaterType;
            model.HeaterName = name;
            Details = GetTowerHXDetails();//函数里面已经把detail的duty转换为用户指定单位制了。
            ReadConvert(); //只转换model.HeaterDuty.
        }


        private ICommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new DelegateCommand(Add);

                }
                return _AddCommand;
            }
        }

        public void Add()
        {
            TowerHXDetailModel d = new TowerHXDetailModel();
            d.HXID = model.ID;
            d.SeqNumber = Details.Count - 1;
            d.DetailName = model.HeaterName + "_" + (Details.Count + 1).ToString();
            d.Parent = model;
            d.ProcessSideFlowSource = d.ProcessSideFlowSources[0]; ;
            d.Medium = d.Mediums[0];
            d.MediumSideFlowSource = d.MediumSideFlowSources[0];
            Details.Add(d);
        }

        private ICommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new RelayCommand(Delete);

                }
                return _DeleteCommand;
            }
        }

        public void Delete(object obj)
        {
            int idx = int.Parse(obj.ToString());
            Details.RemoveAt(idx);
            for (int i = 0; i < Details.Count; i++)
            {
                TowerHXDetailModel detail = Details[i];
                detail.SeqNumber = i;
            }

        }





        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(Save);

                }
                return _SaveCommand;
            }
        }

        public void Save(object obj)
        {
            TowerHXDetailDAL db = new TowerHXDetailDAL();
            IList<TowerHXDetail> list = db.GetAllList(SessionProtectedSystem, model.ID);

            //先判断是否有变化，有变化，则需要更新所有的Scenario.
            bool bEdit=false;
            if (Details.Count != list.Count)
                bEdit = true;
            else
            {
                foreach (TowerHXDetailModel m in Details)
                {
                    if (m.ID == 0)
                    {
                        bEdit = true;
                        break;
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    TowerHXDetail detail = list[i];
                    foreach (TowerHXDetailModel m in Details)
                    {
                        if (m.ID == detail.ID)
                        {
                            if (m.Medium != detail.Medium || m.ProcessSideFlowSource != detail.ProcessSideFlowSource || m.MediumSideFlowSource != detail.MediumSideFlowSource || m.DutyPercentage != detail.DutyPercentage)
                            {
                                bEdit = true;
                                break;
                            }                            
                        }
                    }                    
                }

            }

            if (bEdit)
            {
                ScenarioDAL scdal = new ScenarioDAL();
                IList<Scenario> scList = scdal.GetAllList(SessionProtectedSystem);
                if (scList.Count > 0)
                {
                    MessageBoxResult r = MessageBox.Show("Are you sure to edit data? it need to rerun all Scenario", "Message Box", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        ScenarioBLL scBLL = new ScenarioBLL(SessionProtectedSystem);
                        scBLL.DeleteSCOther();
                        scBLL.ClearScenario();
                    }
                    else
                    {
                        return;
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    bool b = false;
                    TowerHXDetail detail = list[i];
                    foreach (TowerHXDetailModel m in Details)
                    {
                        if (m.ID == detail.ID)
                        {
                            b = true;
                            break;
                        }
                    }
                    if (!b)
                    {
                        db.Delete(list[i], SessionProtectedSystem);
                    }
                }

                foreach (TowerHXDetailModel m in Details)
                {
                    if (m.ID == 0)
                    {
                        TowerHXDetail detail = new TowerHXDetail();
                        detail = ConvertToDBModel(detail, m);
                        db.Add(detail, SessionProtectedSystem);
                    }
                    else
                    {
                        TowerHXDetail detail = db.GetModel(SessionProtectedSystem, m.ID);
                        detail = ConvertToDBModel(detail, m);
                        db.Update(detail, SessionProtectedSystem);
                    }

                }

            }

            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        public TowerHXDetail ConvertToDBModel(TowerHXDetail d, TowerHXDetailModel m)
        {
            d.ID = m.ID;
            d.DetailName = m.DetailName;
            d.DetailName = m.DetailName;
            d.ProcessSideFlowSource = m.ProcessSideFlowSource;
            d.Medium = m.Medium;
            d.MediumSideFlowSource = m.MediumSideFlowSource;
            d.ID = m.ID;
            d.HXID = m.HXID;

            double sysDuty = UnitConvert.Convert( uomEnum.UserEnthalpyDuty,UOMEnum.EnthalpyDuty, m.Duty);
            d.Duty = sysDuty;
            d.DutyPercentage = m.DutyPercentage;
            return d;
        }

        private void ReadConvert()
        {
            if (model.HeaterDuty != null)
                model.HeaterDuty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, dutyUnit, model.HeaterDuty);
        }
        private void InitUnit()
        {
            this.dutyUnit = uomEnum.UserEnthalpyDuty;
        }
        private string dutyUnit;
        public string DutyUnit
        {
            get { return dutyUnit; }
            set
            {
                dutyUnit = value;
                this.OnPropertyChanged("DutyUnit");
            }
        }
    }
}
