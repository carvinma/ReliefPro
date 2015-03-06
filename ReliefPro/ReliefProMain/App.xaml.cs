using System;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using ReliefProMain.View;
using ReliefProMain.ViewModel;
using UOMLib;
using System.Threading.Tasks;
using ReliefProMain.View.Reports;

namespace ReliefProMain
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {

        }

        protected override void OnDeactivated(EventArgs e)
        {

        }
        protected void OnStartup(object sender, StartupEventArgs e)
        {
            //TowerScenarioView mainF = new TowerScenarioView();
            //Task.Factory.StartNew(() => { InitData(); });

            InitData();
            MainWindow v = new MainWindow();
            MainWindowVM vm = new MainWindowVM();
            v.DataContext = vm;
            //Window1 v = new Window1();
            v.WindowState = WindowState.Maximized;
            v.Show();

            //Window1 v = new Window1();
            //v.Show();

        }
        private void APP_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Message Box");
            e.Handled = true;
        }
        private void InitData()
        {
            PlantInfo plant = new PlantInfo();
            plant.Id = 0;
            plant.Name = "";
            plant.DataContext = UOMSingle.templatePlantContext;
            plant.UnitInfo = new UOMEnum();//模板单位制信息
            UOMSingle.plantsInfo.Add(plant);

            //UnitInfo unitInfo = new UnitInfo();
            //var SystemUnits = unitInfo.GetSystemUnit();
            //var UnitTypes = unitInfo.GetUnitType();

            var SystemUnits = plant.UnitInfo.lstSystemUnit;
            var UnitTypes = plant.UnitInfo.lstUnitType;

            if (null != SystemUnits)
            {
                UnitConvert.lkpSystemUnit = SystemUnits.ToLookup(p => p.Name.ToLower());
                UnitConvert.lkpSystemUnitByUnitType = SystemUnits.ToLookup(p => p.UnitType);
            }

            if (null != UnitTypes)
                UnitConvert.lkpUnitType = UnitTypes.ToLookup(p => p.ShortName.ToLower());
            SystemUnits.Clear();
            UnitTypes.Clear();
        }
    }
}
