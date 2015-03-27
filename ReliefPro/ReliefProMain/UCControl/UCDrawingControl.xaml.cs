using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using AxMicrosoft.Office.Interop.VisOcx;
using Visio = Microsoft.Office.Interop.Visio;
using System.Data;
using System.IO;
using System.Collections;
using Microsoft.Win32;
using ReliefProCommon.CommonLib;
using ReliefProMain.ViewModel;
using ReliefProModel;
using ReliefProModel.HXs;
using ReliefProModel.Drums;
using ReliefProMain.ViewModel.Drums;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProDAL.Drums;
using ReliefProMain.View.Drums;
using UOMLib;
using ReliefProMain.View.StorageTanks;
using ReliefProMain.ViewModel.StorageTanks;
using ReliefProDAL.HXs;
using ReliefProMain.View.HXs;
using ReliefProDAL.Compressors;
using ReliefProMain.View.Compressors;
using ReliefProDAL.ReactorLoops;
using ReliefProMain.View.ReactorLoops;
using ReliefProMain.ViewModel.ReactorLoops;
using ReliefProModel.ReactorLoops;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using CustomVisio;

namespace ReliefProMain.View
{
    /// <summary>
    /// UCDrawingControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCDrawingControl : UserControl
    {
        public Window ownerWindow;
        private string dbPlantFile { set; get; }
        private string dbProtectedSystemFile { set; get; }
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private string EqName { set; get; }
        private string EqType { set; get; }
        private string DataFileFullPath { set; get; }
        private SourceFile SourceFileInfo;

        public AxDrawingControl visioControl = new AxDrawingControl();
        double endShpWidth = 0.15;
        double endShpHeight = 0.1;
        public UCDrawingControl()
        {
            InitializeComponent();
            visioControl.MouseUpEvent += new EVisOcx_MouseUpEventHandler(axDrawingControl_MouseUpEvent);
            this.host.Child = this.visioControl;
        }
        void host_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.Integration.WindowsFormsHost host = sender as System.Windows.Forms.Integration.WindowsFormsHost;
                TVFileViewModel data = this.Tag as TVFileViewModel;
                AxDrawingControl dc = host.Child as AxDrawingControl;
                visioControl = dc;
                visioControl.Window.Zoom = 1.2;
                visioControl.Window.ShowGrid = 0;
                visioControl.Window.ShowRulers = 0;
                visioControl.Window.ShowConnectPoints = -1;
                visioControl.Src = data.tvFile.FullPath;
                visioControl.Window.ShowPageTabs = false;
            }
            catch (Exception ex)
            {
            }
        }
        private DateTime firstClick = DateTime.Now;

        private void axDrawingControl_MouseUpEvent(object sender, AxMicrosoft.Office.Interop.VisOcx.EVisOcx_MouseUpEvent e)
        {
            if (DateTime.Now < firstClick.AddMilliseconds(System.Windows.Forms.SystemInformation.DoubleClickTime))
            {
                ShapeDoubleClick();
            }

            firstClick = DateTime.Now;
        }
        public void ShapeDoubleClick()
        {
            string visioVersion = "2003";
            using (RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\visio.exe", false))
            {
                if (key.GetValue("Path").ToString().Contains("Office11"))
                {
                    visioVersion = "2003";
                }
                else if (key.GetValue("Path").ToString().Contains("Office12"))
                {
                    visioVersion = "2007";
                }
                else if (key.GetValue("Path").ToString().Contains("Office14"))
                {
                    visioVersion = "2010";
                }
                else if (key.GetValue("Path").ToString().Contains("Office15"))
                {
                    visioVersion = "2013";
                }
            }
            
            IVisio curVisio = VisioFactory.CreateVisio(visioVersion);
            string[] shapeNames = curVisio.GetDoubleShapeName(this.visioControl);
            if (string.IsNullOrEmpty(shapeNames[1]))
                return;
            string name = shapeNames[0];
            if (shapeNames[1].ToLower().Contains("dis"))
            {
                try
                {
                    TowerView v = new TowerView();
                    TowerVM vm = new TowerVM(name, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                    v.DataContext = vm;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    bool? dlgresult = v.ShowDialog();
                    if (dlgresult == true)
                    {
                        SourceFileInfo = vm.SourceFileInfo;
                        SessionProtectedSystem = vm.SessionProtectedSystem;
                        EqName = vm.TowerName;
                        EqType = "Tower";
                        curVisio.DrawTower(EqName, vm.StageNumber, vm.Feeds, vm.Products, vm.Condensers, vm.HxCondensers, vm.Reboilers, vm.HxReboilers);

                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (shapeNames[1].ToLower().Contains("column"))
            {
                try
                {
                    DrumView v = new DrumView();
                    DrumVM vm = new DrumVM(name, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                    v.DataContext = vm;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    if (v.ShowDialog() == true)
                    {
                        SourceFileInfo = vm.SourceFileInfo;
                        EqName = vm.DrumName;
                        EqType = "Drum";
                        curVisio.DrawDrum(EqName, vm.Feeds, vm.Products);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else if (shapeNames[1].Contains("connector"))
            {
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("this stream is not exist!", "Message Box");
                    return;
                }
                CustomStreamView v = new CustomStreamView();
                CustomStreamVM vm = new CustomStreamVM(name, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;

                Window parentWindow = Window.GetWindow(this);
                v.Owner = parentWindow;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();

            }
            else if (shapeNames[1].Contains("Vessel"))
            {
                AccumulatorView v = new AccumulatorView();
                AccumulatorVM vm = new AccumulatorVM(name, SessionPlant, SessionProtectedSystem);
                v.DataContext = vm;
                Window parentWindow = Window.GetWindow(this);
                v.Owner = parentWindow;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();

            }
            else if (shapeNames[1].Contains("Kettle reboiler"))
            {
                TowerHXVM vm = new TowerHXVM(name, SessionPlant, SessionProtectedSystem);
                TowerHXView v = new TowerHXView();
                v.DataContext = vm;
                Window parentWindow = Window.GetWindow(this);
                v.Owner = parentWindow;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();
            }
            else if (shapeNames[1].Contains("Carrying vessel"))
            {
                try
                {
                    SourceView v = new SourceView();
                    SourceVM vm = new SourceVM(name, SourceFileInfo, SessionPlant, SessionProtectedSystem);
                    v.DataContext = vm;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
                catch (Exception ex)
                {
                }
            }
            else if (shapeNames[1].Contains("Clarifier"))
            {
                try
                {
                    SinkView v = new SinkView();
                    SinkVM vm = new SinkVM(name, SessionPlant, SessionProtectedSystem);
                    v.DataContext = vm;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
                catch (Exception ex)
                {
                }
            }
            else if (shapeNames[1].Contains("Heat exchanger2"))
            {
                TowerHXVM vm = new TowerHXVM(name, SessionPlant, SessionProtectedSystem);
                TowerHXView v = new TowerHXView();
                v.DataContext = vm;
                Window parentWindow = Window.GetWindow(this);
                v.Owner = parentWindow;
                //frmReboiler.txtName.Text = shp.Text;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();
            }
            else if (shapeNames[1].Contains("Tank"))
            {
                StorageTankView v = new StorageTankView();
                StorageTankVM vm = new StorageTankVM(name, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                v.DataContext = vm;

                Window parentWindow = Window.GetWindow(this);
                v.Owner = parentWindow;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    SourceFileInfo = vm.SourceFileInfo;
                    EqName = vm.CurrentModel.StreamName;
                    EqType = "StorageTank";
                    curVisio.DrawTank(EqName);
                }

            }
            if (shapeNames[1].ToLower().Contains("heat exchanger1"))
            {
                try
                {
                    HeatExchangerView v = new HeatExchangerView();
                    HeatExchangerVM vm = new HeatExchangerVM(name, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                    v.DataContext = vm;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    if (v.ShowDialog() == true)
                    {
                        SourceFileInfo = vm.SourceFileInfo;
                        EqName = vm.HXName;
                        EqType = "HX";
                        curVisio.DrawHX(EqName, vm.HXType, vm.Feeds, vm.Products, vm.TubeFeedStreams, vm.ShellFeedStreams, vm.ColdInlet, vm.ColdOutlet, vm.HotInlet, vm.HotOutlet);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (shapeNames[1].ToLower().Contains("selectable compressor1"))
            {
                try
                {
                    CompressorView v = new CompressorView();
                    CompressorVM vm = new CompressorVM(name, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                    v.DataContext = vm;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    if (v.ShowDialog() == true)
                    {
                        SourceFileInfo = vm.SourceFileInfo;
                        EqName = vm.model.CompressorName;
                        EqType = "Compressor";
                        curVisio.DrawCompressor(EqName, vm.model.Feeds, vm.model.Products);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (shapeNames[1].ToLower().Contains("reaction vessel"))
            {
                try
                {
                    ReactorLoopView v = new ReactorLoopView();
                    ReactorLoopVM vm = new ReactorLoopVM(SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                    v.DataContext = vm;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    if (v.ShowDialog() == true)
                    {
                        SourceFileInfo = vm.SourceFileInfo;
                        EqName = "";
                        EqType = "ReactorLoop";
                        curVisio.DrawReactor(EqName);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            this.visioControl.Window.DeselectAll();

        }
        
       
        private void ToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.ToolTip.ToString() == "PSV")
            {
                ProtectedSystemDAL psdal = new ProtectedSystemDAL();
                ProtectedSystem ps = psdal.GetModel(SessionProtectedSystem);
                if (ps == null)
                {
                    MessageBox.Show("Current Equipment Data is not imported!", "Message Box");
                    return;
                }
                PSVView v = new PSVView();
                PSVVM vm = new PSVVM(EqName, EqType, SourceFileInfo, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                v.DataContext = vm;
                v.Owner = ownerWindow;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();
            }
            else if (btn.ToolTip.ToString() == "Scenario")
            {
                PSVDAL dbpsv = new PSVDAL();
                PSV psv = dbpsv.GetModel(SessionProtectedSystem);
                if (psv == null)
                {
                    MessageBox.Show("PSV have not been calculated!", "Message Box");
                    return;
                }


                ScenarioListVM vm = new ScenarioListVM(EqName, EqType, SourceFileInfo, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                ScenarioListView v = new ScenarioListView();
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();
            }
        }


        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.btnPSV.IsEnabled = false;
            TVFileViewModel data = this.Tag as TVFileViewModel;
            dbPlantFile = data.tvFile.dbPlantFile;
            dbProtectedSystemFile = data.tvFile.dbProtectedSystemFile;
            DirPlant = System.IO.Path.GetDirectoryName(dbPlantFile);
            DirProtectedSystem = System.IO.Path.GetDirectoryName(dbProtectedSystemFile);
            SessionPlant = UOMSingle.UomEnums.First(p => p.SessionDBPath.Contains(dbPlantFile)).SessionPlant;

            var task = Task.Factory.StartNew(() =>
                {
                    NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbProtectedSystemFile);
                    SessionProtectedSystem = helperProtectedSystem.GetCurrentSession();
                    ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
                    ProtectedSystem ps = psDAL.GetModel(SessionProtectedSystem);

                    SourceFileDAL sfdal = new SourceFileDAL();

                    if (ps != null)
                    {
                        switch (ps.PSType)
                        {
                            case 1:
                                TowerDAL dbtower = new TowerDAL();
                                Tower tower = dbtower.GetModel(SessionProtectedSystem);
                                if (tower != null)
                                {
                                    EqType = "Tower";
                                    EqName = tower.TowerName;
                                    SourceFileInfo = sfdal.GetModel(tower.SourceFile, SessionPlant);
                                }
                                break;

                            case 2:
                                DrumDAL dbdrum = new DrumDAL();
                                Drum drum = dbdrum.GetModel(SessionProtectedSystem);
                                if (drum != null)
                                {
                                    EqType = "Drum";
                                    EqName = drum.DrumName;
                                    SourceFileInfo = sfdal.GetModel(drum.SourceFile, SessionPlant);
                                }
                                break;
                            case 3:
                                CompressorDAL compressorDAL = new CompressorDAL();
                                Compressor compressor = compressorDAL.GetModel(SessionProtectedSystem);
                                if (compressor != null)
                                {
                                    EqType = "Compressor";
                                    EqName = compressor.CompressorName;
                                    SourceFileInfo = sfdal.GetModel(compressor.SourceFile, SessionPlant);
                                }
                                break;
                            case 4:
                                HeatExchangerDAL heatExchangerDAL = new HeatExchangerDAL();
                                HeatExchanger heatExchanger = heatExchangerDAL.GetModel(SessionProtectedSystem);
                                if (heatExchanger != null)
                                {
                                    EqType = "HX";                                    
                                    EqName = heatExchanger.HXName;
                                    SourceFileInfo = sfdal.GetModel(heatExchanger.SourceFile, SessionPlant);
                                }
                                break;
                            case 5:
                                StorageTankDAL storageTankDAL = new StorageTankDAL();
                                StorageTank tank = storageTankDAL.GetModel(SessionProtectedSystem);
                                if (tank != null)
                                {
                                    EqType = "StorageTank";
                                    EqName = tank.StorageTankName;
                                    SourceFileInfo = sfdal.GetModel(tank.SourceFile, SessionPlant);
                                }
                                break;
                            case 6:
                                ReactorLoopDAL reactorLoopDAL = new ReactorLoopDAL();
                                ReactorLoop reactor = reactorLoopDAL.GetModel(SessionProtectedSystem);
                                if (reactor != null)
                                {
                                    EqType = "ReactorLoop";
                                    EqName = reactor.ColdHighPressureSeparator;//这是为了显示locaiton 名字
                                    SourceFileInfo = sfdal.GetModel(reactor.SourceFile, SessionPlant);
                                }
                                break;
                            default:
                                break;
                        }
                    }


                });

            Task.WaitAll(task);
            this.btnPSV.IsEnabled = true;
        }




    }

}
