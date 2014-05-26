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
   public class CustomStreamVM
    {
       private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public CustomStreamModel CurrentModel { set; get; }
       private dbCustomStream db;
        public CustomStreamVM(string name, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            BasicUnit BU;
            dbBasicUnit dbBU = new dbBasicUnit();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            UnitConvert uc = new UnitConvert();
            db = new dbCustomStream();
            CustomStream cs = db.GetModel(SessionProtectedSystem, name);
            CurrentModel = new CustomStreamModel(cs);

        }
    }
}
