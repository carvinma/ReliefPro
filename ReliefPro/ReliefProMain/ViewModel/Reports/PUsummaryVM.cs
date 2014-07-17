using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Reporting.WinForms;
using ReliefProBLL;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model.Reports;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using ReliefProModel.Reports;

namespace ReliefProMain.ViewModel.Reports
{
    public class PUsummaryVM : ViewModelBase
    {
        private ReportBLL reportBLL;
        PUsummaryGridDS SumDs = new PUsummaryGridDS();
        StackPanel stackPanel;
        public ICommand OKCMD { get; set; }
        public ICommand BtnReportCMD { get; set; }
        public ICommand ExportExcelCMD { get; set; }
        public PUsummaryModel model { get; set; }
        List<string> listScenario = new List<string> { "PowerDS", "WaterDS", "AirDS", "SteamDS", "FireDS" };
        List<string> listProperty = new List<string> { "ReliefLoad", "ReliefMW", "ReliefTemperature", "ReliefZ" };
        public StackPanel Stackpanel
        {
            get { return stackPanel; }
            set { stackPanel = value; }
        }
        public StackPanel StackpanelDraw
        {
            get;
            set;
        }

        public ReportViewer ReportViewerDS { get; set; }
        public PUsummaryVM(List<string> ReportPath)
        {
            BtnReportCMD = new DelegateCommand<object>(BtnReprotClick);
            ExportExcelCMD = new DelegateCommand<object>(BtnExportExcel);
            reportBLL = new ReportBLL(ReportPath);
            CreateControl(reportBLL.GetDisChargeTo());

            model = new PUsummaryModel();
            model.listGrid = new List<PUsummaryGridDS>();
            InitModel("ALL");
            StackpanelDraw = new StackPanel();
            CreateReport();
            // DrawingPUReport draw = new DrawingPUReport(model.listGrid);
            // StackpanelDraw.Children.Add(draw);
        }
        private void InitModel(string ReportDischargeTo)
        {
            List<PSV> listPSV = reportBLL.PSVBag.ToList();
            if (ReportDischargeTo != "ALL")
                listPSV = listPSV.Where(p => p.DischargeTo == ReportDischargeTo).ToList();
            model.listGrid = reportBLL.GetPuReprotDS(listPSV);
            model.listGrid = reportBLL.CalcMaxSum(model.listGrid);
        }

        private void CreateReport()
        {
            WindowsFormsHost host = new WindowsFormsHost();
            host.Width = 1340;
            host.Height = 500;
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local;
            //string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "View\\Reports\\PUsummaryRpt.rdlc";
            reportViewer.LocalReport.ReportEmbeddedResource = "ReliefProMain.View.Reports.PUsummaryRpt.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("PUDataSet", CreateReportDataSource()));
            reportViewer.RefreshReport();
            ReportViewerDS = reportViewer;
            host.Child = reportViewer;

            StackpanelDraw.Children.Add(host);
        }
        private List<PUsummaryReportSource> CreateReportDataSource()
        {
            List<PUsummaryReportSource> listRS = new List<PUsummaryReportSource>();
            listRS = model.listGrid.Select(p => new PUsummaryReportSource
            {
                Device = p.psv.PSVName,
                ProtectedSystem = p.psv.PSVName,
                DeviceType = p.psv.ValveType,
                SetPressure = p.psv.Pressure,
                DischargeTo = p.psv.DischargeTo,

                ScenarioReliefRate = GetDouble(p.SingleDS.ReliefLoad),
                ScenarioPhase = p.SingleDS.Phase,
                ScenarioMWorSpGr = GetDouble(p.SingleDS.ReliefMW),
                ScenarioT = GetDouble(p.SingleDS.ReliefTemperature),
                ScenarioZ = GetDouble(p.SingleDS.ReliefZ),
                ScenarioName = p.SingleDS.ScenarioName,

                PowerReliefRate = GetDouble(p.PowerDS.ReliefLoad),
                PowerPhase = p.PowerDS.Phase,
                PowerMWorSpGr = GetDouble(p.PowerDS.ReliefMW),
                PowerT = GetDouble(p.PowerDS.ReliefTemperature),
                PowerZ = GetDouble(p.PowerDS.ReliefZ),

                WaterReliefRate = GetDouble(p.WaterDS.ReliefLoad),
                WaterPhase = p.WaterDS.Phase,
                WaterMWorSpGr = GetDouble(p.WaterDS.ReliefMW),
                WaterT = GetDouble(p.WaterDS.ReliefTemperature),
                WaterZ = GetDouble(p.WaterDS.ReliefZ),

                AirReliefRate = GetDouble(p.AirDS.ReliefLoad),
                AirPhase = p.AirDS.Phase,
                AirMWorSpGr = GetDouble(p.AirDS.ReliefMW),
                AirT = GetDouble(p.AirDS.ReliefTemperature),
                AirZ = GetDouble(p.AirDS.ReliefZ),

                SteamReliefRate = GetDouble(p.SteamDS.ReliefLoad),
                SteamPhase = p.SteamDS.Phase,
                SteamMWorSpGr = GetDouble(p.SteamDS.ReliefMW),
                SteamT = GetDouble(p.SteamDS.ReliefTemperature),
                SteamZ = GetDouble(p.SteamDS.ReliefZ),

                FireReliefRate = GetDouble(p.FireDS.ReliefLoad),
                FirePhase = p.FireDS.Phase,
                FireMWorSpGr = GetDouble(p.FireDS.ReliefMW),
                FireT = GetDouble(p.FireDS.ReliefTemperature),
                FireZ = GetDouble(p.FireDS.ReliefZ)
            }).Take(model.listGrid.Count - 2).ToList();
            return listRS;
        }
        private double? GetDouble(string value)
        {
            double reslut = 0;
            if (double.TryParse(value, out reslut))
                return reslut;
            return null;
        }
        private void BtnReprotClick(object obj)
        {
            if (obj != null)
            {
                InitModel(obj.ToString());
            }
        }
        private void BtnExportExcel(object obj)
        {
            ExportLib.ExportExcel export = new ExportLib.ExportExcel();
            export.ExportToExcelPUsummary(model.listGrid, "PUsummary.xlsx");
        }
        private void CreateControl(List<FlareSystem> lstFlareSystem)
        {
            Stackpanel = new StackPanel();
            Stackpanel.Orientation = Orientation.Horizontal;
            int i = 1;
            foreach (var flareSystem in lstFlareSystem)
            {
                Button btn = new Button();
                btn.Content = flareSystem.FlareName;
                btn.Command = BtnReportCMD;
                btn.Margin = new System.Windows.Thickness(20, 0, 0, 0); i++;
                btn.CommandParameter = flareSystem.FlareName;
                Stackpanel.Children.Add(btn);
            }
            Button btnALL = new Button();
            btnALL.Content = "ALL";
            btnALL.Command = BtnReportCMD;
            btnALL.CommandParameter = btnALL.Content;
            btnALL.Width = 50;
            btnALL.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            Stackpanel.Children.Add(btnALL);
        }
    }


    public class DrawingPUReport : FrameworkElement
    {
        DrawingVisual dv = new DrawingVisual();
        List<PUsummaryGridDS> listPU;
        public DrawingPUReport(List<PUsummaryGridDS> listPU)
        {
            this.listPU = listPU;
            this.AddVisualChild(dv);
            this.Draw();
        }
        private void Draw()
        {
            Pen blackp = new Pen(Brushes.Black, 0.2);
            Brush borderBrush = Brushes.LightBlue;
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("zh-cn");
            FlowDirection fd = FlowDirection.LeftToRight;
            Typeface tf = new Typeface("宋体");
            double d = 12;
            using (DrawingContext dc = dv.RenderOpen())
            {
                double DeviceLength = listPU.Max(p =>
                {
                    if (p.psv.PSVName == null) return 50;
                    else return p.psv.PSVName.Length;
                });
                double ProtectedLength = listPU.Max(p =>
                {
                    if (p.psv.PSVName == null) return 70;
                    else return p.psv.PSVName.Length;
                });
                DeviceLength = DeviceLength + 2;
                dc.DrawRectangle(borderBrush, blackp, new Rect(10, 0, DeviceLength, 50));
                dc.DrawRectangle(borderBrush, blackp, new Rect(DeviceLength + 10, 0, ProtectedLength, 50));

                //for (int i = 0; i < 5; i++)
                //{
                //    DeviceLength = DeviceLength + 2;
                //    dc.DrawRectangle(borderBrush, blackp, new Rect(10, 0, DeviceLength, 50));
                //    //dc.DrawRectangle(borderBrush, blackp, new Rect(50 * i, 0, 50, 50));
                //    // dc.DrawText(new FormattedText("第" + (i + 1).ToString() + "周", ci, fd, tf, d, Brushes.Green), new Point(217 * i + 70, 10));
                //}
                //for (int i = 0; i < 35; i++)
                //    dc.DrawRectangle(null, blackp, new Rect(31 * i, 30, 31, 30));
                //int w = new DateTime(2011, 3, 1).DayOfWeek.GetHashCode();
                //for (int i = w; i < w + 31; i++)
                //    dc.DrawText(new FormattedText((i - w + 1).ToString(), ci, fd, tf, d, Brushes.Black), new Point(31 * i + 5, 40));
                //for (int i = 0; i < 35; i++)
                //    dc.DrawRectangle(null, blackp, new Rect(31 * i, 60, 31, 60));
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    int week = ((DateTime)dt.Rows[i][1]).DayOfWeek.GetHashCode();
                //    int day = ((DateTime)dt.Rows[i][1]).Day;
                //    string s = dt.Rows[i][0].ToString();
                //    double left = (double)(week + (day + week / 7 * 7 + 1) * 31 + 5);
                //    double top = i > 30 ? 100 : 70;
                //    dc.DrawText(new FormattedText(s, ci, fd, tf, d, Brushes.Black), new Point(left, top));
                //}
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return dv;
            throw new IndexOutOfRangeException();
        }
    }
}
