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
using ReliefProMain.View;
using ReliefProMain.Model;

namespace ReliefProMain.ViewModel
{
    public class SinkVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string PrzFile;
        SinkDAL db;
        public List<string> SinkTypes { get; set; }
        public SinkModel MainModel{ get; set; }

        public List<string> GetSinkTypes()
        {
            List<string> list = new List<string>();
            list.Add("Compressor(Motor)");
            list.Add("Compressor(Steam Turbine Driven)");
            list.Add("Pump(Steam Turbine Driven)");
            list.Add("Pump(Motor)");
            list.Add("Pressurized Vessel");
            return list;
        }

        public SinkVM(string name, string PrzFile, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            this.PrzFile = PrzFile;
            SinkTypes = GetSinkTypes();
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            BasicUnit BU;
            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            UnitConvert uc = new UnitConvert();
            db = new SinkDAL();
            
            Sink sink = db.GetModel(SessionProtectedSystem, name);
            MainModel=new SinkModel(sink);

        }

        private ICommand _Update;
        
        public ICommand Update
        {
            get { return _Update ?? (_Update = new RelayCommand(OKClick)); }
        }


        private void OKClick(object window)
        {
            MainModel.SinkName.Trim();
            if (MainModel.SinkName == "")
            {
                throw new ArgumentException("Please input a name for the Sink.");
            }
            BasicUnit BU;

            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(SessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();


            UnitConvert uc = new UnitConvert();
            
            db.Update(MainModel.model, SessionProtectedSystem);
            SessionProtectedSystem.Flush();  //update必须带着它。 之所以没写入基类，是为了日后transaction

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

       

    }
}
