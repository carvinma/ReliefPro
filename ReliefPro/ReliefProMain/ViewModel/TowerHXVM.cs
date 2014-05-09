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
using ReliefProMain.Model;

namespace ReliefProMain.ViewModel
{
    public class TowerHXVM
    {       
        
        public string dbProtectedSystemFile { set; get; }
        public string dbPlantFile { set; get; }
        public TowerHXModel model { set; get; }

        public TowerHXVM(string name,string dbPSFile,string dbPFile)
        {
            //ShowAddCommand = new DelegateCommand<object>(ShowAddDialog);
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            model = new TowerHXModel(dbProtectedSystemFile);
            model.HeaterName = name;
            
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerHX db = new dbTowerHX();
                TowerHX hx = db.GetModel(Session, model.HeaterName);
                model.ID = hx.ID;
                model.HeaterDuty = hx.HeaterDuty;
                model.HeaterType = hx.HeaterType;
                model.Details = model.GetTowerHXDetails();

            }
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
            d.SeqNumber =  model.Details.Count + 1;
            d.DetailName = d.SeqNumber.ToString();
            d.Parent = model;
            model.Details.Add(d);
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
            int idx=int.Parse(obj.ToString());
            model.Details.RemoveAt(idx - 1);
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
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerHXDetail db = new dbTowerHXDetail();
                foreach (TowerHXDetailModel m in model.Details)
                {
                    if (m.ID == 0)
                    {
                        TowerHXDetail detail = new TowerHXDetail();
                        detail = ConvertToDBModel(detail, m);
                        db.Add(detail,Session);
                    }
                    else
                    {
                        TowerHXDetail detail = db.GetModel(Session,m.ID);
                        detail = ConvertToDBModel(detail, m);
                        db.Update(detail, Session);
                    }
                }
                Session.Flush();
            }

            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult=true;
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
            d.Duty = m.Duty;
            d.DutyPercentage = m.DutyPercentage;
            return d;
        }
    }
}
