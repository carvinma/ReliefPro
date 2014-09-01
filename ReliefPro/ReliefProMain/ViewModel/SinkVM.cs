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
using ReliefProMain.Models;

namespace ReliefProMain.ViewModel
{
    public class SinkVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        SinkDAL db;
        public SinkModel MainModel { get; set; }


        UOMLib.UOMEnum uomEnum;
        public SinkVM(string name, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            db = new SinkDAL();
            Sink sink = db.GetModel(SessionProtectedSystem, name);
            MainModel = new SinkModel(sink);
            InitUnit();
            ReadConvert();
        }

        private ICommand _SaveCommand;

        public ICommand SaveCommand
        {
            get { return _SaveCommand ?? (_SaveCommand = new RelayCommand(Save)); }
        }


        private void Save(object window)
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

            WriteConvert();
            db.Update(MainModel.dbmodel, SessionProtectedSystem);
            SessionProtectedSystem.Flush();  //update必须带着它。 之所以没写入基类，是为了日后transaction

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        private void ReadConvert()
        {
            MainModel.MaxPossiblePressure = UnitConvert.Convert(UOMEnum.Pressure, MainModel.PressureUnit, MainModel.MaxPossiblePressure);
        }
        private void WriteConvert()
        {
            MainModel.MaxPossiblePressure = UnitConvert.Convert(MainModel.PressureUnit, UOMEnum.Pressure, MainModel.MaxPossiblePressure);
        }
        private void InitUnit()
        {
            MainModel.PressureUnit = uomEnum.UserPressure;
        }

    }
}
