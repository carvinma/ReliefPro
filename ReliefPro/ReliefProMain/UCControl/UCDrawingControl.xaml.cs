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

namespace ReliefProMain.View
{
    /// <summary>
    /// UCDrawingControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCDrawingControl : UserControl
    {
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
                //visioControl.Window.Zoom = 1;
                //visioControl.Window.ShowGrid = 0;
                // visioControl.Window.ShowRulers = 0;
                // visioControl.Window.ShowConnectPoints = -1;
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
                string name=shp.Text;
                if (shp.NameU.ToLower().Contains("dis"))
                {
                    try
                    {
                        TowerView towerV = new TowerView();
                        towerV.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                        towerV.dbPlantFile = dbPlantFile;
                        towerV.dbProtectSystemFile = dbProtectedSystemFile;
                        towerV.eqType = "Column";
                        if (towerV.ShowDialog() == true)
                        {
                            DrawTower(shp, towerV);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (shp.NameU.Contains("connector"))
                {
                    CustomStreamView v = new CustomStreamView();
                    
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.txtName.Text = shp.Text;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    v.dbProtectedSystemFile = dbProtectedSystemFile;
                    v.streamName = shp.Text;
                    if (v.ShowDialog() == true)
                    {
                        shp.Text = v.txtName.Text;
                    }

                }
                else if (shp.NameU.Contains("Vessel"))
                {
                    AccumulatorView v = new AccumulatorView();
                    AccumulatorVM vm = new AccumulatorVM(name, dbProtectedSystemFile, dbPlantFile);
                    v.DataContext = vm;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    v.txtName.Text = shp.Text;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    v.ShowDialog();

                }
                else if (shp.NameU.Contains("Kettle reboiler"))
                {                    
                    TowerHXVM vm = new TowerHXVM(name,dbProtectedSystemFile,dbPlantFile);
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
                        SourceVM vm = new SourceVM(name, dbProtectedSystemFile, dbPlantFile);
                        v.DataContext = vm;
                        //if (vm.CloseAction == null)
                        //    vm.CloseAction = new Action(() => v.Close());

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
                    TowerHXVM vm = new TowerHXVM(name, dbProtectedSystemFile, dbPlantFile);
                    TowerHXView v = new TowerHXView();
                    v.DataContext = vm;
                    Window parentWindow = Window.GetWindow(this);
                    v.Owner = parentWindow;
                    //frmReboiler.txtName.Text = shp.Text;
                    v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    v.ShowDialog();
                }
               

            }
            this.visioControl.Window.DeselectAll();

        }
        private void DrawTower(Visio.Shape shape, TowerView frm)
        {

            shape.get_Cells("Height").ResultIU = 2;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            if (frm.op == 1)
            {
                shape.Text = frm.txtName.Text;
                //return;
            }
            shape.Text = frm.txtName.Text;
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


            int stagenumber = int.Parse(frm.txtStageNumber.Text);
            DataTable dtFeed = (DataTable)Application.Current.Properties["FeedData"];
            int start = 16;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 13;
            foreach (DataRow dr in dtFeed.Rows)
            {
                Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                ConnectShapes(shape, start, connector, 0);
                connector.Text = dr["streamname"].ToString();

                Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY + (start - center) * multiple * height);
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

            DataTable dtProd = (DataTable)Application.Current.Properties["ProdData"];
            DataTable dtReboiler = (DataTable)Application.Current.Properties["Reboiler"];
            DataTable dtHxReboiler = (DataTable)Application.Current.Properties["HxReboiler"];
            DataTable dtCondenser = (DataTable)Application.Current.Properties["Condenser"];
            DataTable dtHxCondenser = (DataTable)Application.Current.Properties["HxCondenser"];

            dtReboiler.Merge(dtHxReboiler);

            Visio.Shape condenser;
            Visio.Shape condenserVessel = null;
            double twidth = 0;
            double theight = 0;
            double tpinX = 0;
            double tpinY = 0;
            if (dtCondenser.Rows.Count > 0)
            {
                condenser = visioControl.Window.Application.ActivePage.Drop(condenserMaster, pinX + 1, pinY + height / 2 + 0.2);
                condenserVessel = visioControl.Window.Application.ActivePage.Drop(condenserVesselMaster, pinX + 1.5, pinY + height / 2 + 0.1);
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
                condenserVessel.Text = dtCondenser.Rows[0]["heatername"].ToString()+"_Accumulator";
                condenser.Text = dtCondenser.Rows[0]["heatername"].ToString();
                condenser.Cells["EventDblClick"].Formula = "=0";
            }

            for (int i = 1; i <= dtHxCondenser.Rows.Count; i++)
            {
                DataRow dr = dtHxCondenser.Rows[i - 1];
                condenser = visioControl.Window.Application.ActivePage.Drop(condenserMaster, pinX, pinY + height / 2 - i * 0.4);
                condenserVessel = visioControl.Window.Application.ActivePage.Drop(condenserVesselMaster, pinX + 1.5, pinY + height / 2 + 0.1);
                condenserVessel.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Width").ResultIU = 0.2;
                condenser.Text = dr["heatername"].ToString();
                condenser.Cells["EventDblClick"].Formula = "=0";
            }




            Visio.Shape reboiler = null;
            double bwidth = 0;
            double bheight = 0;
            double bpinX = 0;
            double bpinY = 0;
            if (dtReboiler.Rows.Count > 0)
            {
                reboiler = visioControl.Window.Application.ActivePage.Drop(reboilerMaster, pinX + 1, pinY - height / 2 - 0.2);
                Visio.Shape connector1 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                ConnectShapes(shape, 9, connector1, 1);//从塔到加热器
                ConnectShapes(reboiler, 4, connector1, 0);

                ConnectShapes(shape, 8, connector2, 0);//从加热器到塔
                ConnectShapes(reboiler, 3, connector2, 1);
                reboiler.Text = dtReboiler.Rows[0]["heatername"].ToString();
                reboiler.Cells["EventDblClick"].Formula = "=0";
            }


            start = 3;
            center = 5;
            int topcount = 1;
            int bottomcount = 1;
            foreach (DataRow dr in dtProd.Rows)
            {
                int tray = -1;
                if (dr["tray"].ToString() != string.Empty)
                {
                    tray = int.Parse(dr["tray"].ToString());
                }
                if (tray == 1)
                {
                    if (dtCondenser.Rows.Count == 0)
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(shape, 1, connector, 1);
                        connector.Text = dr["streamname"].ToString();
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY - 2 - height / 2);
                        endShp.get_Cells("Height").ResultIU = 0.1;
                        endShp.get_Cells("Width").ResultIU = 0.2;
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
                            connector.Text = dr["streamname"].ToString();

                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 2, tpinY - 0.2);
                            endShp.get_Cells("Height").ResultIU = 0.1;
                            endShp.get_Cells("Width").ResultIU = 0.2;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);
                            topcount++;
                        }
                        else if (topcount == 2)
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 6, connector, 1);
                            connector.Text = dr["streamname"].ToString();

                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 2, tpinY + 0.4);
                            endShp.get_Cells("Height").ResultIU = 0.1;
                            endShp.get_Cells("Width").ResultIU = 0.2;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);
                            topcount++;
                        }
                        else
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 7, connector, 1);
                            connector.Text = dr["streamname"].ToString();


                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 2, tpinY - 0.1);
                            endShp.get_Cells("Height").ResultIU = 0.1;
                            endShp.get_Cells("Width").ResultIU = 0.2;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);
                            topcount++;
                        }
                    }

                }
                else if (tray == stagenumber)
                {
                    if (dtReboiler.Rows.Count == 0)
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(shape, 9, connector, 1);
                        connector.Text = dr["streamname"].ToString();
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY + 0.5 - height / 2);
                        endShp.get_Cells("Height").ResultIU = 0.1;
                        endShp.get_Cells("Width").ResultIU = 0.2;
                        endShp.Text = connector.Text + "_Sink";
                        endShp.Cells["EventDblClick"].Formula = "=0";
                        ConnectShapes(endShp, 7, connector, 0);
                    }
                    else
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(reboiler, 1, connector, 1);
                        connector.Text = dr["streamname"].ToString();
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY - 0.5 - height / 2);
                        endShp.get_Cells("Height").ResultIU = 0.1;
                        endShp.get_Cells("Width").ResultIU = 0.2;
                        endShp.Text = connector.Text + "_Sink";
                        endShp.Cells["EventDblClick"].Formula = "=0";
                        ConnectShapes(endShp, 7, connector, 0);
                    }
                }
                else
                {
                    Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, start, connector, 1);
                    connector.Text = dr["streamname"].ToString();

                    Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY + (center - start) * multiple * height);
                    endShp.get_Cells("Height").ResultIU = 0.1;
                    endShp.get_Cells("Width").ResultIU = 0.2;
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

            Application.Current.Properties.Remove("Condenser");
            Application.Current.Properties.Remove("HxCondenser");
            Application.Current.Properties.Remove("Reboiler");
            Application.Current.Properties.Remove("HxReboiler");
            Application.Current.Properties.Remove("FeedData");
            Application.Current.Properties.Remove("ProdData");
            visioControl.Document.SaveAs(visioControl.Src);

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
                PSVView psv = new PSVView();
                psv.dbProtectedSystemFile = dbProtectedSystemFile;
                psv.dbPlantFile = dbPlantFile;
                psv.ShowDialog();
            }
            else if (btn.ToolTip.ToString() == "Scenario")
            {
                TowerScenarioView ts = new TowerScenarioView();
                ts.dbProtectedSystemFile = dbProtectedSystemFile;
                ts.dbPlantFile = dbPlantFile;
                ts.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ts.ShowDialog();
            }
        }

        private string dbPlantFile;
        private string dbProtectedSystemFile;
        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            TreeViewItemData data = this.Tag as TreeViewItemData;
            dbPlantFile = data.dbPlantFile;
            dbProtectedSystemFile = data.dbProtectedSystemFile;
        }
    

    }

}
