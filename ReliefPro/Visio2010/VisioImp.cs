using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomVisio;
using AxMicrosoft.Office.Interop.VisOcx;
using Visio=Microsoft.Office.Interop.Visio;
using Microsoft.Office.Interop.VisOcx;
using System.Collections;
using ReliefProModel;
using System.Collections.ObjectModel;

namespace Visio2010
{
    public class VisioImp : IVisio
    {
        double endShpWidth = 0.15;
        double endShpHeight = 0.1;
        private AxDrawingControl visioControl = null;
        Visio.Shape shape = null;

        public string[] GetDoubleShapeName(AxDrawingControl vc)
        {
            string[] result = new string[3];
            visioControl = vc;
            foreach (Visio.Shape shp in visioControl.Window.Selection)
            {
                result[0] = shp.Text;
                result[1] = shp.NameU;
                result[2] = shp.NameID;
                shape = shp;
                break;
            }
            return result;
        }

        //private Visio.Shape GetCurrentShape()
        //{
        //    Visio.Shape shape = null;
        //    foreach (Visio.Shape shp in visioControl.Window.Application.ActivePage.Shapes)
        //    {
        //        if (shp.NameID == shapeNameID)
        //        {
        //            shape = shp;
        //            break;
        //        }
        //    }
        //    return shape;
        //}

        public void DrawTank(string shapeText)
        {
            if (shape != null)
            {
                shape.get_Cells("Height").ResultIU = 1;
                double width = shape.get_Cells("Width").ResultIU;
                double height = shape.get_Cells("Height").ResultIU;
                double pinX = shape.get_Cells("PinX").ResultIU;
                double pinY = shape.get_Cells("PinY").ResultIU;
                shape.Text = shapeText;
            }
            //deleteShapesExcept(shape);
        }

        public void DrawTower(string shapeText, int StageNumber, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products, ObservableCollection<TowerHX> Condensers, ObservableCollection<TowerHX> HxCondensers, ObservableCollection<TowerHX> Reboilers, ObservableCollection<TowerHX> HxReboilers)
        {
            shape.get_Cells("Height").ResultIU = 3;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = shapeText;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger2");
            Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");


            int stagenumber = StageNumber;

            int start = 16;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 13;
            foreach (CustomStream cs in Feeds)
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
            if (Condensers.Count > 0)
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
                condenser.Text = Condensers[0].HeaterName;
                condenser.Cells["EventDblClick"].Formula = "=0";
            }
            Visio.Shape reboiler = null;
            double bwidth = 0;
            double bheight = 0;
            double bpinX = 0;
            double bpinY = 0;
            if (Reboilers.Count > 0)
            {
                reboiler = visioControl.Window.Application.ActivePage.Drop(reboilerMaster, pinX + 1, pinY - height / 2 - 0.2);
                Visio.Shape connector1 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                ConnectShapes(shape, 9, connector1, 1);//从塔到加热器
                ConnectShapes(reboiler, 4, connector1, 0);

                ConnectShapes(shape, 8, connector2, 0);//从加热器到塔
                ConnectShapes(reboiler, 3, connector2, 1);
                reboiler.Text = Reboilers[0].HeaterName;
                reboiler.Cells["EventDblClick"].Formula = "=0";
            }

            int pumpinterval = 1;
            for (int i = 1; i <= HxCondensers.Count; i++)
            {
                condenser = visioControl.Window.Application.ActivePage.Drop(condenserMaster, pinX, pinY + height / 2 - pumpinterval * 0.4);
                //condenserVessel = visioControl.Window.Application.ActivePage.Drop(condenserVesselMaster, pinX + 1.5, pinY + height / 2 + 0.1);
                //condenserVessel.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Height").ResultIU = 0.2;
                condenser.get_Cells("Width").ResultIU = 0.2;
                condenser.Text = HxCondensers[i - 1].HeaterName;
                condenser.Cells["EventDblClick"].Formula = "=0";
                pumpinterval++;
            }
            for (int i = 1; i <= HxReboilers.Count; i++)
            {
                reboiler = visioControl.Window.Application.ActivePage.Drop(condenserMaster, pinX, pinY + height / 2 - pumpinterval * 0.4);
                //condenserVessel = visioControl.Window.Application.ActivePage.Drop(condenserVesselMaster, pinX + 1.5, pinY + height / 2 + 0.1);
                //condenserVessel.get_Cells("Height").ResultIU = 0.2;
                reboiler.get_Cells("Height").ResultIU = 0.2;
                reboiler.get_Cells("Width").ResultIU = 0.2;
                reboiler.Text = HxReboilers[i - 1].HeaterName;
                reboiler.Cells["EventDblClick"].Formula = "=0";
                pumpinterval++;
            }



            start = 3;
            center = 5;
            //int topcount = 1;
            int bottomcount = 1;
            foreach (CustomStream cs in Products)
            {
                int tray = -1;

                tray = cs.Tray;

                if (tray == 1)
                {
                    if (Condensers.Count == 0)
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(shape, 1, connector, 1);
                        connector.Text = cs.StreamName;
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY + height * 1 / 5);
                        //Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY - 2 - height / 2);
                        endShp.get_Cells("Height").ResultIU = endShpHeight;
                        endShp.get_Cells("Width").ResultIU = endShpWidth;
                        endShp.Text = connector.Text + "_Sink";
                        ConnectShapes(endShp, 7, connector, 0);
                        endShp.Cells["EventDblClick"].Formula = "=0";
                    }
                    else
                    {
                        if (cs.ProdType == "1" || cs.ProdType == "3") //开放visio 里的4，7，8  对应程序里的3，6，7。  3 water， 6气相  7液相
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 6, connector, 1);
                            connector.Text = cs.StreamName;

                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 2.5, tpinY - 0.35);
                            endShp.get_Cells("Height").ResultIU = endShpHeight;
                            endShp.get_Cells("Width").ResultIU = endShpWidth;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);
                        }
                        else if (cs.ProdType == "2" || cs.ProdType == "4")
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 7, connector, 1);
                            connector.Text = cs.StreamName;

                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 2.1, tpinY - 0.35);
                            endShp.get_Cells("Height").ResultIU = endShpHeight;
                            endShp.get_Cells("Width").ResultIU = endShpWidth;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);

                        }
                        else
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                            ConnectShapes(condenserVessel, 3, connector, 1);
                            connector.Text = cs.StreamName;
                            Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, tpinX + 1.8, tpinY - 0.35);
                            endShp.get_Cells("Height").ResultIU = endShpHeight;
                            endShp.get_Cells("Width").ResultIU = endShpWidth;
                            endShp.Text = connector.Text + "_Sink";
                            endShp.Cells["EventDblClick"].Formula = "=0";
                            ConnectShapes(endShp, 7, connector, 0);

                        }
                    }

                }
                else if (tray == stagenumber || tray == stagenumber - 1)
                {
                    if (Reboilers.Count == 0 && tray == stagenumber)
                    {
                        Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                        ConnectShapes(shape, 9, connector, 1);
                        connector.Text = cs.StreamName;
                        Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 2, pinY - height / 2 - 0.5);
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

                    Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 1.2, pinY + (center - start - 0.5) * multiple * height);
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


        }


        public void DrawDrum(string shapeText, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products)
        {
            shape.get_Cells("Height").ResultIU = 2;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = shapeText;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            //Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger1");
            //Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            //Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");

            int start = 1;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 5;
            foreach (CustomStream cs in Feeds)
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
                if (start >= 8)
                    start = 1;
            }
            start = 14;
            foreach (CustomStream cs in Products)
            {
                if (cs.VaporFraction == 1)
                {
                    int h = 7;
                    Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, 16, connector, 1);
                    connector.Text = cs.StreamName;
                    Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + (15 - h) * 0.6, pinY - (h + center - 14) * multiple * height);
                    endShp.get_Cells("Height").ResultIU = endShpHeight;
                    endShp.get_Cells("Width").ResultIU = endShpWidth;
                    endShp.Text = connector.Text + "_Sink";
                    endShp.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp, 7, connector, 0);
                }
                else
                {
                    Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, start, connector, 1);
                    connector.Text = cs.StreamName;
                    Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + (15 - start) * 0.6, pinY - (start + center - 14) * multiple * height);
                    endShp.get_Cells("Height").ResultIU = endShpHeight;
                    endShp.get_Cells("Width").ResultIU = endShpWidth;
                    endShp.Text = connector.Text + "_Sink";
                    endShp.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp, 7, connector, 0);
                    start = start - 1;
                    if (start <= 8)
                        start = 14;
                }
            }

            currentStencil_1.Close();
            currentStencil_2.Close();
            currentStencil_3.Close();
        }

        public void DrawHX(string shapeText, string hxType, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products, string TubeFeedStreams, string ShellFeedStreams, string ColdInlet, string ColdOutlet, string HotInlet, string HotOutlet)
        {
            shape.get_Cells("Height").ResultIU = 0.6;
            shape.get_Cells("Width").ResultIU = 0.6;
            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = shapeText;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            //Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger1");
            //Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            //Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");

            int start = 4;
            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 5;
            //2,3 入
            int feedcount = Feeds.Count;
            int productcount = Products.Count;

            if (hxType == "Shell-Tube")
            {
                //ShellFeedStreams --从左侧进
                if (!string.IsNullOrEmpty(TubeFeedStreams))
                {
                    string[] arrTubeFeedStreams = TubeFeedStreams.Split(',');
                    for (int i = 0; i < TubeFeedStreams.Length; i++)
                    {
                        if (arrTubeFeedStreams[i] != string.Empty)
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                            ConnectShapes(shape, 2, connector, 0);
                            connector.Text = arrTubeFeedStreams[i];
                            int diff = 0;
                            if (i % 2 == 0)
                            {
                                diff = i;
                            }
                            else
                            {
                                diff = -1 * i;
                            }
                            Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY - diff);
                            startShp.get_Cells("Height").ResultIU = 0.1;
                            startShp.get_Cells("Width").ResultIU = 0.2;
                            startShp.Text = connector.Text + "_Source";
                            ConnectShapes(startShp, 2, connector, 1);
                            startShp.Cells["EventDblClick"].Formula = "=0";
                        }
                    }
                }

                //从上面进
                if (!string.IsNullOrEmpty(ShellFeedStreams))
                {
                    string[] arrShellFeedStreams = ShellFeedStreams.Split(',');
                    for (int i = 0; i < ShellFeedStreams.Length; i++)
                    {
                        if (arrShellFeedStreams[i] != string.Empty)
                        {
                            Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                            ConnectShapes(shape, 3, connector2, 0);
                            connector2.Text = arrShellFeedStreams[i];

                            int diff = 0;
                            if (i % 2 == 0)
                            {
                                diff = i;
                            }
                            else
                            {
                                diff = -1 * i;
                            }
                            Visio.Shape startShp2 = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2 + diff, pinY + 0.2);
                            startShp2.get_Cells("Height").ResultIU = 0.1;
                            startShp2.get_Cells("Width").ResultIU = 0.2;
                            startShp2.Text = connector2.Text + "_Source";
                            ConnectShapes(startShp2, 2, connector2, 1);
                            startShp2.Cells["EventDblClick"].Formula = "=0";
                        }
                    }
                }


                //从右侧出
                if (!string.IsNullOrEmpty(TubeFeedStreams))
                {

                    Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, 1, connector, 1);
                    if (TubeFeedStreams == ColdInlet)
                    {
                        connector.Text = ColdOutlet;
                    }
                    else
                    {
                        connector.Text = HotOutlet;
                    }
                    Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 1.8, pinY + 0.35);
                    endShp.get_Cells("Height").ResultIU = endShpHeight;
                    endShp.get_Cells("Width").ResultIU = endShpWidth;
                    endShp.Text = connector.Text + "_Sink";
                    endShp.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp, 7, connector, 0);
                }


                if (!string.IsNullOrEmpty(ShellFeedStreams))
                {
                    Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, 4, connector2, 1);
                    if (ShellFeedStreams == ColdInlet)
                    {
                        connector2.Text = ColdOutlet;
                    }
                    else
                    {
                        connector2.Text = HotOutlet;
                    }
                    Visio.Shape endShp2 = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 1.8, pinY - 0.6);
                    endShp2.get_Cells("Height").ResultIU = endShpHeight;
                    endShp2.get_Cells("Width").ResultIU = endShpWidth;
                    endShp2.Text = connector2.Text + "_Sink";
                    endShp2.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp2, 7, connector2, 0);
                }
            }
            else
            {
                //ShellFeedStreams --从左侧进
                if (!string.IsNullOrEmpty(ColdInlet))
                {
                    string[] ColdInlets = ColdInlet.Split(',');
                    for (int i = 0; i < ColdInlets.Length; i++)
                    {
                        if (ColdInlets[i] != string.Empty)
                        {
                            Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                            ConnectShapes(shape, 2, connector, 0);
                            connector.Text = ColdInlets[i];
                            int diff = 0;
                            if (i % 2 == 0)
                            {
                                diff = i;
                            }
                            else
                            {
                                diff = -1 * i;
                            }
                            Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY - diff);
                            startShp.get_Cells("Height").ResultIU = 0.1;
                            startShp.get_Cells("Width").ResultIU = 0.2;
                            startShp.Text = connector.Text + "_Source";
                            ConnectShapes(startShp, 2, connector, 1);
                            startShp.Cells["EventDblClick"].Formula = "=0";
                        }
                    }
                }

                //从上面进
                if (!string.IsNullOrEmpty(HotInlet))
                {
                    string[] HotInlets = HotInlet.Split(',');
                    for (int i = 0; i < HotInlets.Length; i++)
                    {
                        if (HotInlets[i] != string.Empty)
                        {
                            Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                            ConnectShapes(shape, 3, connector2, 0);
                            connector2.Text = HotInlets[i];

                            int diff = 0;
                            if (i % 2 == 0)
                            {
                                diff = i;
                            }
                            else
                            {
                                diff = -1 * i;
                            }
                            Visio.Shape startShp2 = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2 + diff, pinY + 0.2);
                            startShp2.get_Cells("Height").ResultIU = 0.1;
                            startShp2.get_Cells("Width").ResultIU = 0.2;
                            startShp2.Text = connector2.Text + "_Source";
                            ConnectShapes(startShp2, 2, connector2, 1);
                            startShp2.Cells["EventDblClick"].Formula = "=0";
                        }
                    }
                }


                //从右侧出
                if (!string.IsNullOrEmpty(ColdOutlet))
                {

                    Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, 1, connector, 1);
                    connector.Text = ColdOutlet;

                    Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 1.8, pinY + 0.35);
                    endShp.get_Cells("Height").ResultIU = endShpHeight;
                    endShp.get_Cells("Width").ResultIU = endShpWidth;
                    endShp.Text = connector.Text + "_Sink";
                    endShp.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp, 7, connector, 0);
                }


                if (!string.IsNullOrEmpty(HotOutlet))
                {
                    Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, 4, connector2, 1);
                    connector2.Text = HotOutlet;

                    Visio.Shape endShp2 = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 1.8, pinY - 0.6);
                    endShp2.get_Cells("Height").ResultIU = endShpHeight;
                    endShp2.get_Cells("Width").ResultIU = endShpWidth;
                    endShp2.Text = connector2.Text + "_Sink";
                    endShp2.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp2, 7, connector2, 0);
                }

            }
            currentStencil_1.Close();
            currentStencil_2.Close();
            currentStencil_3.Close();
        }

        public void DrawCompressor(string shapeText, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products)
        {
            shape.get_Cells("Height").ResultIU = 1;

            double width = shape.get_Cells("Width").ResultIU;
            double height = shape.get_Cells("Height").ResultIU;
            double pinX = shape.get_Cells("PinX").ResultIU;
            double pinY = shape.get_Cells("PinY").ResultIU;

            shape.Text = shapeText;
            deleteShapesExcept(shape);

            Visio.Document currentStencil_1 = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_2 = visioControl.Document.Application.Documents.OpenEx("CONNEC_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Document currentStencil_3 = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            //Visio.Master condenserMaster = currentStencil_1.Masters.get_ItemU(@"Heat exchanger1");
            //Visio.Master reboilerMaster = currentStencil_1.Masters.get_ItemU(@"Kettle reboiler");
            Visio.Master streamMaster = currentStencil_2.Masters.get_ItemU(@"Dynamic connector");
            //Visio.Master condenserVesselMaster = currentStencil_3.Masters.get_ItemU(@"Vessel");

            Visio.Document startStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
            Visio.Master startMaster = startStencil.Masters.get_ItemU(@"Carrying vessel");
            Visio.Master endMaster = startStencil.Masters.get_ItemU(@"Clarifier");


            double multiple = 0.125;
            //double leftmultiple = 1.5;
            int center = 5;
            int feedcount = Feeds.Count;
            if (feedcount > 0)
            {
                CustomStream cs = Feeds[0];
                Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                ConnectShapes(shape, 2, connector, 0);
                connector.Text = cs.StreamName;

                Visio.Shape startShp = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY);
                startShp.get_Cells("Height").ResultIU = 0.1;
                startShp.get_Cells("Width").ResultIU = 0.2;
                startShp.Text = connector.Text + "_Source";
                ConnectShapes(startShp, 2, connector, 1);
                startShp.Cells["EventDblClick"].Formula = "=0";
                if (feedcount > 1)
                {
                    CustomStream cs2 = Feeds[1];
                    Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 4, pinY);
                    ConnectShapes(shape, 0, connector2, 0);
                    connector2.Text = cs2.StreamName;

                    Visio.Shape startShp2 = visioControl.Window.Application.ActivePage.Drop(startMaster, pinX - 2, pinY + 0.2);
                    startShp2.get_Cells("Height").ResultIU = 0.1;
                    startShp2.get_Cells("Width").ResultIU = 0.2;
                    startShp2.Text = connector2.Text + "_Source";
                    ConnectShapes(startShp2, 2, connector2, 1);
                    startShp2.Cells["EventDblClick"].Formula = "=0";
                }
            }

            int productcount = Products.Count;
            if (productcount > 0)
            {
                CustomStream cs = Products[0];
                Visio.Shape connector = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                ConnectShapes(shape, 3, connector, 1);
                connector.Text = cs.StreamName;
                Visio.Shape endShp = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 1.8, pinY + 0.35);
                endShp.get_Cells("Height").ResultIU = endShpHeight;
                endShp.get_Cells("Width").ResultIU = endShpWidth;
                endShp.Text = connector.Text + "_Sink";
                endShp.Cells["EventDblClick"].Formula = "=0";
                ConnectShapes(endShp, 7, connector, 0);
                if (productcount > 1)
                {
                    CustomStream cs2 = Products[1];
                    Visio.Shape connector2 = visioControl.Window.Application.ActivePage.Drop(streamMaster, 5, 5);
                    ConnectShapes(shape, 4, connector2, 1);
                    connector2.Text = cs2.StreamName;
                    Visio.Shape endShp2 = visioControl.Window.Application.ActivePage.Drop(endMaster, pinX + 1.8, pinY - 0.6);
                    endShp2.get_Cells("Height").ResultIU = endShpHeight;
                    endShp2.get_Cells("Width").ResultIU = endShpWidth;
                    endShp2.Text = connector2.Text + "_Sink";
                    endShp2.Cells["EventDblClick"].Formula = "=0";
                    ConnectShapes(endShp2, 7, connector2, 0);
                }
            }
            currentStencil_1.Close();
            currentStencil_2.Close();
            currentStencil_3.Close();
        }

        public void DrawReactor(string shapeText)
        {
            shape.get_Cells("Height").ResultIU = 2;
            shape.Text = shapeText;
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

    }
}