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
using System.Windows;
using ReliefProMain.Models;

namespace ReliefProMain.ViewModel
{
    public class SourceVM : ViewModelBase
    {
        public ISession SessionPlant { set; get; }
        public ISession SessionProtectedSystem { set; get; }
        public SourceFile SourceFileInfo;
        public SourceModel model { get; set; }
        public SourceDAL sourcedal = new SourceDAL();

        UOMLib.UOMEnum uomEnum;
        public SourceVM(string name, SourceFile sourceFileInfo, ISession sessionPlant, ISession sessionProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            BasicUnit BU;
            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();

            uomEnum = new UOMLib.UOMEnum(SessionPlant);
            Source source = sourcedal.GetModel(SessionProtectedSystem, name);
            model = new SourceModel(source);
            InitUnit();
            
            ReadConvert();
            
        }

        private ICommand _Update;

        public ICommand Update
        {
            get { return _Update ?? (_Update = new RelayCommand(OKClick)); }
        }


        private void OKClick(object window)
        {
            if (model.SourceName.Trim() == "")
            {
                throw new ArgumentException("Please type in a name for the Source.");
            }
            BasicUnit BU;

            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(SessionPlant);
            BU = list.Where(s => s.IsDefault == 1).Single();           
            WriteConvert();
            sourcedal.Update(model.dbmodel, SessionProtectedSystem);
            SessionProtectedSystem.Flush();  //update必须带着它。 之所以没写入基类，是为了日后transaction

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        private ICommand _ShowHeatSourceListCommand;
        public ICommand ShowHeatSourceListCommand
        {
            get
            {
                if (_ShowHeatSourceListCommand == null)
                {
                    _ShowHeatSourceListCommand = new DelegateCommand(ShowHeatSourceList);

                }
                return _ShowHeatSourceListCommand;
            }
        }

        public void ShowHeatSourceList()
        {
            HeatSourceListView v = new HeatSourceListView();
            HeatSourceListVM vm = new HeatSourceListVM(model.ID, SourceFileInfo, SessionPlant, SessionProtectedSystem);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.ShowDialog();
        }
        private void ReadConvert()
        {
            model.MaxPossiblePressure = UnitConvert.Convert(UOMEnum.Pressure, model.PressureUnit, model.MaxPossiblePressure);
        }
        private void WriteConvert()
        {
            model.MaxPossiblePressure = UnitConvert.Convert(model.PressureUnit, UOMEnum.Pressure, model.MaxPossiblePressure);
        }
        private void InitUnit()
        {
            model.PressureUnit = uomEnum.UserPressure;
        }
    }
}
