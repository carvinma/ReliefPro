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

namespace ReliefProMain.View
{
    /// <summary>
    /// UCDrawingControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCDrawingControl : UserControl
    {
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
                TreeViewItemData data = this.Tag as TreeViewItemData;
                AxDrawingControl dc = host.Child as AxDrawingControl;
                visioControl = dc;
                visioControl.Window.Zoom = 1;
                visioControl.Window.ShowGrid = 0;
                visioControl.Window.ShowRulers = 0;
                visioControl.Window.ShowConnectPoints = -1;
                visioControl.Src = data.FullName;
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
        private void ShapeDoubleClick()
        {
            AxDrawingControl dc = this.visioControl;
            foreach (Visio.Shape shp in this.visioControl.Window.Selection)
            {
                string name = shp.Text;
                if (shp.NameU.ToLower().Contains("dis"))
                {
                    try
                    {
                        TowerView v = new TowerView();
                        TowerVM vm = new TowerVM(name, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                        v.DataContext = vm;
                        v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        Window parentWindow = Window.GetWindow(this);
                        v.Owner = parentWindow;
                        bool? dlgresult=v.ShowDialog();
                        if (dlgresult == true)
                        {
                            SourceFileInfo = vm.SourceFileInfo;
                            SessionProtectedSystem = vm.SessionProtectedSystem;
                            EqName = vm.TowerName;
                            EqType = "Tower";
                            DrawTower(shp, vm);

                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (shp.NameU.ToLower().Contains("column"))
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
                            DrawDrum(shp, vm);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (shp.NameU.Contains("connector"))
                {
                    CustomStreamView v = new CustomStreamView();
                    CustomStreamVM vm = new CustomStreamVM(name, SessionPlant, SessionProtectedSystem);
                    v.DataContext = vm;

                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {
                        shp.Text = v.txtName.Text;
                    }

                }
                else if (shp.NameU.Contains("Vessel"))
                {
                    AccumulatorView v = new AccumulatorView();
                    AccumulatorVM vm = new AccumulatorVM(name, SessionPlant, SessionProtectedSystem);
                    v.DataContext = vm;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    v.ShowDialog();

                }
                else if (shp.NameU.Contains("Kettle reboiler"))
                {
                    TowerHXVM vm = new TowerHXVM(name, SessionPlant, SessionProtectedSystem);
                    TowerHXView v = new TowerHXView();
                    v.DataContext = vm;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
                else if (shp.NameU.Contains("Carrying vessel"))
                {
                    try
                    {
                        SourceView v = new SourceView();
                        SourceVM vm = new SourceVM(name,SourceFileInfo, SessionPlant, SessionProtectedSystem);
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
                else if (shp.NameU.Contains("Clarifier"))
                {
                    try
                    {
                        SinkView v = new SinkView();
                        SinkVM vm = new SinkVM(name,  SessionPlant, SessionProtectedSystem);
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
                else if (shp.NameU.Contains("Heat exchanger1"))
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
                else if (shp.NameU.Contains("Tank"))
                {
                    StorageTankView v = new StorageTankView();
                    StorageTankVM vm = new StorageTankVM(name, SessionPlant, SessionProtectedSystem,DirPlant,DirProtectedSystem);
                    v.DataContext = vm;

                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    if (v.ShowDialog() == true)
                    {
                        SourceFileInfo = vm.SourceFileInfo;
                        EqName = vm.CurrentModel.StreamName;
                        EqType = "StorageTank";
                        DrawTank(shp, vm);
                        shp.Text = v.txtName.Text;
                    }

                }
                if (shp.NameU.ToLower().Contains("heat exchanger2"))
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
                            DrawHX(shp, vm);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (shp.NameU.ToLower().Contains("selectable compressor1"))
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
                            EqName = vm.CompressorName;
                            EqType = "Compressor";
                            DrawCompressor(shp, vm);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (shp.NameU.ToLower().Contains("reaction vessel"))
                {
                    try
                    {
                        ReactorLoopView v = new ReactorLoopView();
                        ReactorLoopVM vm = new ReactorLoopVM( SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                        v.DataContext = vm;
                        v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        Window parentWindow = Window.GetWindow(this);
                        v.Owner = parentWindow;
                        if (v.ShowDialog() == true)
                        {
                            SourceFileInfo = vm.SourceFileInfo;
                            EqName = "";
                            EqType = "ReactorLoop";
                            DrawReactor(shp, vm);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            this.visioControl.Window.DeselectAll();

        }
        private void DrawTower(Visio.Shape shape, TowerVM vm)
        {

            shape.get_Cells("Height").ResultIU = 3;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = vm.TowerName;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger1");
            Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");


            int stagenumber = vm.StageNumber;

            int start = 16;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 13;
            foreach (CustomStream cs in vm.Feeds)
            {
                Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                ConnectShapes(shape, start, connector, 0);
                connector.Text = cs.StreamName;

                Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 1.2, pinY + (start - center) * multiple * height);
                startShp.get_Cells("Height").ResultIU = 0.1;
                startShp.get_Cells("Width").ResultIU = 0.2;
                startShp.Text = connector.Text + "_Source";
                
                ConnectShapes(startShp, 2, connector, 1);
                start--;
                if (start < 11)
                {
                    start = 16;
                }
                startShp.Cells["EventDblClick"].Formula = "=0";
            }


            Visio.Shape condenser;
            Visio.Shape condenserVessel = null;
            double twidth = 0;
            double theight = 0;
            double tpinX = 0;
            double tpinY = 0;
            if (vm.Condensers.Count > 0)
            {
                condenser = visioControl.Window.Application.ActivePage.Drop(condenserMaster, pinX + 1, pinY + height / 2 + 0.2);
                condenserVessel = visioControl.Window.Application.ActivePage.Drop(condenserVesselMaster, pinX + 1.5, pinY + height / 2 - 0.2);
                condenserVessel.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Width").ResultIU = 0.2;
                Visio.Shape connector1 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                Visio.Shape connector3 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                ConnectShapes(shape, 1, connector1, 1);//从塔到冷凝器
                ConnectShapes(condenser, 2, connector1, 0);

                ConnectShapes(condenser, 1, connector2, 1);
                ConnectShapes(condenserVessel, 1, connector2, 0);

                ConnectShapes(condenserVessel, 8, connector3, 1);
                ConnectShapes(shape, 2, connector3, 0);
                twidth = condenserVessel.get_Cells("Width").ResultIU;
                theight = condenserVessel.get_Cells("Height").ResultIU;
                tpinX = condenserVessel.get_Cells("PinX").ResultIU;
                tpinY = condenserVessel.get_Cells("PinY").ResultIU;
                condenserVessel.Cells["EventDblClick"].Formula = "=0";
                condenserVessel.Text = "AC1";
                condenser.Text = vm.Condensers[0].HeaterName;
                condenser.Cells["EventDblClick"].Formula = "=0";
            }

            for (int i = 1; i <= vm.HxCondensers.Count; i++)
            {
                condenser = visioControl.Window.Application.ActivePage.Drop(condenserMaster, pinX, pinY + height / 2 - i * 0.4);
                //condenserVessel = visioControl.Window.Application.ActivePage.Drop(condenserVesselMaster, pinX + 1.5, pinY + height / 2 + 0.1);
                //condenserVessel.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Width").ResultIU = 0.2;
                condenser.Text = vm.HxCondensers[i - 1].HeaterName;
                condenser.Cells["EventDblClick"].Formula = "=0";
            }




            Visio.Shape reboiler = null;
            double bwidth = 0;
            double bheight = 0;
            double bpinX = 0;
            double bpinY = 0;
            if (vm.Reboilers.Count > 0)
            {
                reboiler = visioControl.Window.Application.ActivePage.Drop(reboilerMaster, pinX + 1, pinY - height / 2 - 0.2);
                Visio.Shape connector1 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                ConnectShapes(shape, 9, connector1, 1);//从塔到加热器
                ConnectShapes(reboiler, 4, connector1, 0);

                ConnectShapes(shape, 8, connector2, 0);//从加热器到塔
                ConnectShapes(reboiler, 3, connector2, 1);
                reboiler.Text = vm.Reboilers[0].HeaterName;
                reboiler.Cells["EventDblClick"].Formula = "=0";
            }

            double endShpWidth = 0.15;
            double endShpHeight = 0.1;

            start = 3;
            center = 5;
            int topcount = 1;
            int bottomcount = 1;
            foreach (CustomStream cs in vm.Products)
            {
                int tray = -1;
                
                tray =cs.Tray;
                
                if (tray == 1)
                {
                    if (vm.Condensers.Count == 0)
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(shape, 1, connector, 1);
                        connector.Text = cs.StreamName;
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY - 2 - height / 2);
                        endShp.get_Cells("Height").ResultIU = endShpHeight;
                        endShp.get_Cells("Width").ResultIU = endShpWidth;
                        endShp.Text = connector.Text + "_Sink";
                        ConnectShapes(endShp, 7, connector, 0);
                        endShp.Cells["EventDblClick"].Formula = "=0";
                    }
                    else
                    {
                        if (topcount == 1) //开放3，6，7
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 3, connector, 1);
                            connector.Text = cs.StreamName;

                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 1.5, tpinY - 0.35);
                            endShp.get_Cells("Height").ResultIU = endShpHeight;
                            endShp.get_Cells("Width").ResultIU = endShpWidth;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);
                            topcount++;
                        }
                        else if (topcount == 2)
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 6, connector, 1);
                            connector.Text = cs.StreamName;

                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 2.1, tpinY - 0.35);
                            endShp.get_Cells("Height").ResultIU = endShpHeight;
                            endShp.get_Cells("Width").ResultIU = endShpWidth;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);
                            topcount++;
                        }
                        else
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 7, connector, 1);
                            connector.Text = cs.StreamName;
                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 1.8, tpinY - 0.35);
                            endShp.get_Cells("Height").ResultIU = endShpHeight;
                            endShp.get_Cells("Width").ResultIU = endShpWidth;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);
                            topcount++;
                        }
                    }

                }
                else if (tray == stagenumber)
                {
                    if (vm.Reboilers.Count == 0)
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(shape, 9, connector, 1);
                        connector.Text = cs.StreamName;
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY  - height / 2- 0.5);
                        endShp.get_Cells("Height").ResultIU = endShpHeight;
                        endShp.get_Cells("Width").ResultIU = endShpWidth;
                        endShp.Text = connector.Text + "_Sink";
                        endShp.Cells["EventDblClick"].Formula = "=0";
                        ConnectShapes(endShp, 7, connector, 0);
                    }
                    else
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(reboiler, 1, connector, 1);
                        connector.Text = cs.StreamName;
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY - 0.5 - height / 2);
                        endShp.get_Cells("Height").ResultIU = endShpHeight;
                        endShp.get_Cells("Width").ResultIU = endShpWidth;
                        endShp.Text = connector.Text + "_Sink";
                        endShp.Cells["EventDblClick"].Formula = "=0";
                        ConnectShapes(endShp, 7, connector, 0);
                    }
                }
                else
                {
                    Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, start, connector, 1);
                    connector.Text = cs.StreamName;

                    Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX +1.2, pinY + (center - start-0.5) * multiple * height);
                    endShp.get_Cells("Height").ResultIU = endShpHeight;
                    endShp.get_Cells("Width").ResultIU = endShpWidth;
                    endShp.Text = connector.Text + "_Sink";
                    endShp.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp, 7, connector, 0);
                    start++;
                    if (start > 8)
                    {
                        start = 3;
                    }
                }
            }



            currentStencil_1.Close();
            currentStencil_2.Close();
            currentStencil_3.Close();


            visioControl.Document.SaveAs(visioControl.Src);

        }


        private void DrawDrum(Visio.Shape shape, DrumVM vm)
        {
            shape.get_Cells("Height").ResultIU = 2;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = vm.DrumName;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger1");
            Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");

            int start = 4;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 5;
            foreach (CustomStream cs in vm.Feeds)
            {
                Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                ConnectShapes(shape, start, connector, 0);
                connector.Text = cs.StreamName;

                Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY + (start + 1 - center) * multiple * height);
                startShp.get_Cells("Height").ResultIU = 0.1;
                startShp.get_Cells("Width").ResultIU = 0.2;
                startShp.Text = connector.Text + "_Source";
                ConnectShapes(startShp, 2, connector, 1);
                start = start + 1;
                startShp.Cells["EventDblClick"].Formula = "=0";
            }



        }

        private void DrawHX(Visio.Shape shape, HeatExchangerVM vm)
        {
            shape.get_Cells("Height").ResultIU = 1.5;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = vm.HXName;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger1");
            Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");

            int start = 4;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 5;
            foreach (CustomStream cs in vm.Feeds)
            {
                Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                ConnectShapes(shape, start, connector, 0);
                connector.Text = cs.StreamName;

                Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY + (start + 1 - center) * multiple * height);
                startShp.get_Cells("Height").ResultIU = 0.1;
                startShp.get_Cells("Width").ResultIU = 0.2;
                startShp.Text = connector.Text + "_Source";
                ConnectShapes(startShp, 2, connector, 1);
                start = start + 1;
                startShp.Cells["EventDblClick"].Formula = "=0";
            }



        }

        private void DrawCompressor(Visio.Shape shape, CompressorVM vm)
        {
            shape.get_Cells("Height").ResultIU = 2;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = vm.CompressorName;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger1");
            Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");

            int start = 4;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 5;
            foreach (CustomStream cs in vm.Feeds)
            {
                Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                ConnectShapes(shape, start, connector, 0);
                connector.Text = cs.StreamName;

                Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY + (start + 1 - center) * multiple * height);
                startShp.get_Cells("Height").ResultIU = 0.1;
                startShp.get_Cells("Width").ResultIU = 0.2;
                startShp.Text = connector.Text + "_Source";
                ConnectShapes(startShp, 2, connector, 1);
                start = start + 1;
                startShp.Cells["EventDblClick"].Formula = "=0";
            }



        }

        private void DrawReactor(Visio.Shape shape, ReactorLoopVM vm)
        {
            shape.get_Cells("Height").ResultIU = 2;         
        }

        private void DrawTank(Visio.Shape shape, StorageTankVM vm)
        {
            shape.get_Cells("Height").ResultIU = 1;
            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = vm.CurrentModel.StreamName;
            deleteShapesExcept(shape);
        }

        private void ConnectShapes(Visio.Shape shape, int connectPoint, Visio.Shape connector, int direction)
        {
            // get the cell from the source side of the connector
            Visio.Cell beginXCell = connector.get_CellsSRC(
            (short)Visio.VisSectionIndices.visSectionObject,
            (short)Visio.VisRowIndices.visRowXForm1D,
            (short)Visio.VisCellIndices.vis1DBeginX);
            // glue the source side of the connector to the first shape

            //shape1.AutoConnect(shape2, Visio.VisAutoConnectDir.visAutoConnectDirRight,connector);

            Visio.Cell fromCell = shape.get_CellsSRC(
            (short)Visio.VisSectionIndices.visSectionConnectionPts,
            (short)connectPoint, 0);

            //beginXCell.GlueTo(fromCell);
            //shape1.get_Cells("FillForegnd").Formula = "3";


            //get the cell from the destination side of the connector
            Visio.Cell endXCell = connector.get_CellsSRC(
            (short)Visio.VisSectionIndices.visSectionObject,
            (short)Visio.VisRowIndices.visRowXForm1D,
            (short)Visio.VisCellIndices.vis1DEndX);

            //// glue the destination side of the connector to the second shape

            //Visio.Cell toXCell = shape2.get_CellsSRC(
            //(short)Visio.VisSectionIndices.visSectionObject,
            //(short)Visio.VisRowIndices.visRowXFormOut,
            //(short)Visio.VisCellIndices.visXFormPinX);
            //endXCell.GlueTo(toXCell);

            //Visio.Cell arrowCell = connector.get_CellsSRC((short)Visio.VisSectionIndices.visSectionObject, (short)Visio.VisRowIndices.visRowLine, (short)Visio.VisCellIndices.visLineEndArrow);
            if (direction == 0)
            {
                //connector.get_Cells("BeginArrow").Formula = "=5";
                connector.get_Cells("EndArrow").Formula = "=5";
                endXCell.GlueTo(fromCell);
            }
            else
            {
                connector.get_Cells("EndArrow").Formula = "=5";
                beginXCell.GlueTo(fromCell);
            }
            //connector.get_Cells("LineColor").Formula = "3";

        }

        private void deleteShapesExcept(Visio.Shape shape)
        {
            ArrayList arr = new ArrayList();

            int count = visioControl.Window.Document.Pages[1].Shapes.Count;
            for (int i = 1; i <= count; i++)
            {
                Visio.Shape shp = visioControl.Window.Document.Pages[1].Shapes[i];
                if (shp.NameID != shape.NameID)
                {
                    arr.Add(shp);
                }
            }
            foreach (Visio.Shape shp in arr)
            {
                shp.Delete();
            }

        }
        private void ToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.ToolTip.ToString() == "PSV")
            {
                if (SessionProtectedSystem == null)
                {
                    MessageBox.Show("数据还未导入");
                    return;
                }
                PSVView v = new PSVView();
                PSVVM vm = new PSVVM(EqName, EqType, SourceFileInfo, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();
            }
            else if (btn.ToolTip.ToString() == "Scenario")
            {
                PSVDAL dbpsv = new PSVDAL();
                PSV psv = dbpsv.GetModel(SessionProtectedSystem);
                if (psv == null)
                {
                    MessageBox.Show("Psv 还未计算");
                    return;
                }

                ScenarioListView v = new ScenarioListView();
                ScenarioListVM vm = new ScenarioListVM(EqName, EqType,SourceFileInfo, SessionPlant, SessionProtectedSystem, DirPlant, DirProtectedSystem);
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                v.ShowDialog();
            }
        }


        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.btnPSV.IsEnabled = false;
            TreeViewItemData data = this.Tag as TreeViewItemData;
            dbPlantFile = data.dbPlantFile;
            dbProtectedSystemFile = data.dbProtectedSystemFile;
            DirPlant = System.IO.Path.GetDirectoryName(dbPlantFile);
            DirProtectedSystem = System.IO.Path.GetDirectoryName(dbProtectedSystemFile);

            var task = Task.Factory.StartNew<ISession>(() =>
            {
                NHibernateHelper helperPlant = new NHibernateHelper(dbPlantFile);
                SessionPlant = helperPlant.GetCurrentSession();
                return SessionPlant;
            });
            var t2 = task.ContinueWith((i) =>
            {
                UnitInfo unitInfo = new UnitInfo();
                UOMEnum.lstBasicUnitDefault = unitInfo.GetBasicUnitDefaultUserSet(i.Result);
                UOMEnum.lstSystemUnit = unitInfo.GetSystemUnit(i.Result);

                var basicUnit = unitInfo.GetBasicUnitUOM(i.Result);
                UOMEnum.BasicUnitID = basicUnit.ID;
            });


            var task2 = t2.ContinueWith((i) =>
                {
                    NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbProtectedSystemFile);
                    SessionProtectedSystem = helperProtectedSystem.GetCurrentSession();
                    ProtectedSystemDAL psDAL=new ProtectedSystemDAL();
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
                                    EqName = "";
                                    SourceFileInfo = sfdal.GetModel(reactor.SourceFile, SessionPlant);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    

                });

            Task.WaitAll(task, t2, task2);
            this.btnPSV.IsEnabled = true;
        }


        

    }

}
