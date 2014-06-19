using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using UOMLib;
using NHibernate;
using ReliefProMain.View;
using ReliefProMain.Model;

namespace ReliefProMain.ViewModel
{
   public class CustomStreamVM:ViewModelBase
    {
       public ICommand OKCMD { get; set; }
       private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private CustomStreamModel _CurrentModel;
        public CustomStreamModel CurrentModel
        {
            get
            {
                return this._CurrentModel;
            }
            set
            {
                this._CurrentModel = value;
                OnPropertyChanged("CurrentModel");
            }
        }
       private CustomStreamDAL db;
        public CustomStreamVM(string name, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            BasicUnit BU;
            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            UnitConvert uc = new UnitConvert();
            db = new CustomStreamDAL();
            CustomStream cs = db.GetModel(SessionProtectedSystem, name);
            CurrentModel = new CustomStreamModel(cs);
            OKCMD = new DelegateCommand<object>(Save);
        }

        private void Save(object obj)
        {
            if (obj != null)
            {
                if (CurrentModel.ID == 0)
                {
                    db.Add(CurrentModel.model, SessionProtectedSystem);
                    
                }
                else
                {
                    db.Update(CurrentModel.model, SessionProtectedSystem);                   
                    SessionProtectedSystem.Flush();
                }


                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {

                    wd.DialogResult = true;
                }
            }
        }

    }
}
