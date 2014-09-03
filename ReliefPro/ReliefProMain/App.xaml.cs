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
            UnitInfo unitInfo = new UnitInfo();
            UnitConvert.tmpSystemUnit = unitInfo.GetSystemUnit(UOMSingle.Session);
            if (null != UnitConvert.tmpSystemUnit)
            {
                UnitConvert.lkpSystemUnit = UnitConvert.tmpSystemUnit.ToLookup(p => p.Name.ToLower());
                UnitConvert.lkpSystemUnitByUnitType = UnitConvert.tmpSystemUnit.ToLookup(p => p.UnitType);
            }

            UnitConvert.tmpUnitType = unitInfo.GetUnitType(UOMSingle.Session);
            if (null != UnitConvert.tmpUnitType)
                UnitConvert.lkpUnitType = UnitConvert.tmpUnitType.ToLookup(p => p.ShortName.ToLower());

            UnitConvert.lstBasicUnit = unitInfo.GetBasicUnit(UOMSingle.Session);
            UnitConvert.lstBasicUnitDefault = unitInfo.GetBasicUnitDefault(UOMSingle.Session);
        }
    }
}
