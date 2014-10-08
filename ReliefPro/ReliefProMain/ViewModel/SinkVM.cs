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
using System.Windows;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class SinkVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        SinkDAL db;
        public SinkModel MainModel { get; set; }
        public Sink sink;

        UOMLib.UOMEnum uomEnum;
        public SinkVM(string name, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            db = new SinkDAL();
            sink= db.GetModel(SessionProtectedSystem, name);
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

            bool bEdit = false;
            if (MainModel.dbmodel.MaxPossiblePressure != UnitConvert.Convert(MainModel.PressureUnit, UOMEnum.Pressure, MainModel.MaxPossiblePressure) || MainModel.dbmodel.SinkType != MainModel.SinkType)
            {
                bEdit = true;
            }
            if (bEdit)
            {
                MessageBoxResult r = MessageBox.Show("Are you sure to edit data? it need to rerun all Scenario", "Message Box", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {                    
                    ScenarioBLL scBLL = new ScenarioBLL(SessionProtectedSystem);
                    scBLL.DeleteSCOther();
                    scBLL.ClearScenario();

                    WriteConvert();
                    MainModel.dbmodel.SinkType_Color = MainModel.SinkType_Color;
                    MainModel.dbmodel.MaxPossiblePressure_Color = MainModel.MaxPossiblePressure_Color;
                    MainModel.dbmodel.SinkType = MainModel.SinkType;
                    db.Update(MainModel.dbmodel, SessionProtectedSystem);
                    //SessionProtectedSystem.Flush();  //update必须带着它。 之所以没写入基类，是为了日后transaction
                }
            }
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
            MainModel.dbmodel.MaxPossiblePressure = UnitConvert.Convert(MainModel.PressureUnit, UOMEnum.Pressure, MainModel.MaxPossiblePressure);
        }
        private void InitUnit()
        {
            MainModel.PressureUnit = uomEnum.UserPressure;
        }

    }
}
