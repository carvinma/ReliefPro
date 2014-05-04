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

namespace ReliefProMain.ViewModel
{
    public class TowerHXVM:ViewModelBase
    {        
        public ICommand ShowAddCommand{ set; get; }
        
        private ObservableCollection<TowerHXDetailVM> details = null;
        public string dbProtectedSystemFile;
        public string dbPlantFile;
        public int ID { set; get; }
        public string HeaterName { set; get; }
        public string HeaterDuty { set; get; }
        public int HeaterType { set; get; }

        public TowerHXVM(string name,string dbPSFile,string dbPFile)
        {
            ShowAddCommand = new DelegateCommand<object>(ShowAddDialog);
            HeaterName = name;
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerHX db = new dbTowerHX();
                TowerHX hx = db.GetModel(Session, HeaterName);
                ID = hx.ID;
                HeaterDuty = hx.HeaterDuty;
                HeaterType = hx.HeaterType;
            }
        }

        //list of orders from the customer
        public ObservableCollection<TowerHXDetailVM> Details
        {
            get { return GetTowerHXDetails(); }
            set
            {
                details = value;
                OnPropertyChanged("Details");
            }
        }
        internal ObservableCollection<TowerHXDetailVM> GetTowerHXDetails()
        {
            details = new ObservableCollection<TowerHXDetailVM>();
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerHXDetail db = new dbTowerHXDetail();
                foreach(var obj in db.GetAllList(Session, this.ID))
                {
                    TowerHXDetailVM d=new TowerHXDetailVM(obj);
                    d.ObjTowerHXVM = this;
                    details.Add(d);
                    
                }
            }
            return details;
        }



        private void ShowAddDialog(object obj)
        {
            TowerHXDetailView v=new TowerHXDetailView();
            TowerHXDetailVM vm=new TowerHXDetailVM(this);
            v.DataContext=vm;
            v.ShowDialog();
            Details=GetTowerHXDetails();
        } 
    }
}
