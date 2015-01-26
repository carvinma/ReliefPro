using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using ReliefProMain.Commands;
using ReliefProMain.Models.Reports;
using ReliefProMain.View.Reports;
using ReliefProMain.ViewModel.Trees;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using ReliefProModel.Reports;
using UOMLib;

namespace ReliefProMain.ViewModel.Reports
{
    public class PUsummaryVM : ViewModelBase
    {
        private ReportBLL reportBLL;
        PUsummaryGridDS SumDs = new PUsummaryGridDS();
        StackPanel stackPanel;
        private bool isHideAir;
        public ICommand OKCMD { get; set; }
        public ICommand BtnReportCMD { get; set; }
        public ICommand ExportExcelCMD { get; set; }
        public ICommand NextUnitCMD { get; set; }
        private PUsummaryModel notifyModel;
        public PUsummaryModel model
        {
            get { return notifyModel; }
            set
            {
                notifyModel = value;
                this.NotifyPropertyChanged("model");
            }
        }
        private string selectedDischargeTo;
        public string SelectedDischargeTo
        {
            get
            {
                return selectedDischargeTo;
            }
            set
            {
                selectedDischargeTo = value;
                this.OnPropertyChanged("SelectedDischargeTo");
                InitModel(value);
                CreateReport();
            }
        }
        public List<FlareSystem> listDischargeTo
        {
            get;
            set;
        }
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

        private string reportName;
        public string ReportName
        {
            get { return reportName; }
            set
            {
                reportName = value;
                this.OnPropertyChanged("ReportName");
            }
        }

        private bool reportFresh;
        public bool ReportFresh
        {
            get { return reportFresh; }
            set
            {
                reportFresh = value;
                this.OnPropertyChanged("ReportFresh");
            }
        }
        private ObservableCollection<PlantVM> PlantCollection;
        private List<UnitVM> PlantList;
        private PUsummaryView view;
        public PUsummaryVM(ObservableCollection<PlantVM> plantCollection)
        {
            PlantList = new List<UnitVM>();
            if (plantCollection.Count > 0)
            {
                PlantCollection = plantCollection;
                string dbPlantFile = string.Empty;
                string unitPath = string.Empty;
                List<string> ReportPath = new List<string>();

                foreach (PlantVM plantvm in PlantCollection)
                {
                    dbPlantFile = plantvm.PlantDir + @"\plant.mdb";
                    foreach (UnitVM uvm in plantvm.UnitCollection)
                    {
                        ReportPath.Clear();
                        if (!PlantList.Contains(uvm))
                        {
                            PlantList.Add(uvm);
                            unitPath = plantvm.PlantDir + @"\" + uvm.UnitName;
                            ReportPath.Add(dbPlantFile);
                            foreach (PSVM p in uvm.PSCollection)
                            {
                                ReportPath.Add(unitPath + @"\" + p.PSName + @"\protectedsystem.mdb");
                            }
                            view = new PUsummaryView();
                            //PUsummaryVM vm = new PUsummaryVM(uvm.ID, ReportPath);
                            this.LoadSingleUnit(uvm.ID, ReportPath);
                            view.WindowState = WindowState.Maximized;
                            view.DataContext = this;
                            view.ShowDialog();  
                        }
                        break;
                    }
                }
            }
        }
        private void LoadSingleUnit(int UnitID, List<string> ReportPath)
        {
            StackpanelDraw = new StackPanel();
            OKCMD = new DelegateCommand<object>(Save);
            BtnReportCMD = new DelegateCommand<object>(BtnReprotClick);
            ExportExcelCMD = new DelegateCommand<object>(BtnExportExcel);
            NextUnitCMD = new DelegateCommand<object>(BtnNextUnit);
            reportBLL = new ReportBLL(UnitID, ReportPath);
            listDischargeTo = reportBLL.GetDisChargeTo();
            if (listDischargeTo != null)
            {
                FlareSystem fs = new FlareSystem();
                fs.FlareName = "ALL";
                listDischargeTo.Insert(0, fs);
                this.selectedDischargeTo = "ALL";
            }
            PUsummary PU = reportBLL.GetPUsummaryModel(UnitID);
            if (PU == null) { PU = new PUsummary(); PU.UnitID = UnitID; PU.PlantName = reportBLL.PlantName; PU.ProcessUnitName = reportBLL.ProcessUnitName; }
            CreateControl(listDischargeTo);
            isHideAir =reportBLL.isHideAir();
            reportBLL.ClearSession();
            model = new PUsummaryModel(PU);
            //model.ProcessUnitName = reportBLL.ProcessUnitName;
            model.listGrid = new List<PUsummaryGridDS>();
            InitModel("ALL");
            CreateReport();
        }
        public PUsummaryVM(int UnitID, List<string> ReportPath)
        {
            StackpanelDraw = new StackPanel();
            OKCMD = new DelegateCommand<object>(Save);
            BtnReportCMD = new DelegateCommand<object>(BtnReprotClick);
            ExportExcelCMD = new DelegateCommand<object>(BtnExportExcel);
            NextUnitCMD = new DelegateCommand<object>(BtnNextUnit);
            reportBLL = new ReportBLL(UnitID, ReportPath);
            listDischargeTo = reportBLL.GetDisChargeTo();
            if (listDischargeTo != null)
            {
                FlareSystem fs = new FlareSystem();
                fs.FlareName = "ALL";
                listDischargeTo.Insert(0, fs);
                this.selectedDischargeTo = "ALL";
            }
            PUsummary PU = reportBLL.GetPUsummaryModel(UnitID);
            if (PU == null) { PU = new PUsummary(); PU.UnitID = UnitID; }
            CreateControl(listDischargeTo);

            reportBLL.ClearSession();
            model = new PUsummaryModel(PU);
            model.listGrid = new List<PUsummaryGridDS>();
            InitModel("ALL");
            CreateReport();
        }

        private void Save(object obj)
        {
            reportBLL.SavePUsummary(model.pu);
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
            reportName = "PUsummaryRpt.rdlc";

            List<EffectFactorModel> listEffect = new List<EffectFactorModel>();
            List<PlantReprotHead> reportHeadDS = new List<PlantReprotHead>();
            RptDataSouce.ReportDS.Clear();
            RptDataSouce.ReportDS.Add(new ReportDataSource("PUDataSet", CreateReportDataSource()));
            ReportFresh = !ReportFresh;
        }
        private List<PUsummaryReportSource> CreateReportDataSource()
        {
            List<PUsummaryReportSource> listRS = new List<PUsummaryReportSource>();
            listRS = model.listGrid.Select(p => new PUsummaryReportSource
            {
                HideAir = isHideAir,
                Device = p.psv.PSVName,
                //ProtectedSystem = p.psv.PSVName,
                ProtectedSystem=p.ProtectedSystem,
                DeviceType = p.psv.ValveType,
                SetPressure = p.psv.Pressure,
                DischargeTo = p.psv.DischargeTo,

                ScenarioReliefRate = p.SingleDS.ReliefLoad,
                ScenarioPhase = p.SingleDS.Phase,
                ScenarioMWorSpGr = p.SingleDS.ReliefMW,
                ScenarioT = p.SingleDS.ReliefTemperature,
                ScenarioZ = p.SingleDS.ReliefZ,
                ScenarioName = p.SingleDS.ScenarioName,

                PowerReliefRate = p.PowerDS.ReliefLoad,
                PowerPhase = p.PowerDS.Phase,
                PowerMWorSpGr = p.PowerDS.ReliefMW,
                PowerT = p.PowerDS.ReliefTemperature,
                PowerZ = p.PowerDS.ReliefZ,
                PowerCpCv=p.PowerDS.ReliefCpCv,

                WaterReliefRate = p.WaterDS.ReliefLoad,
                WaterPhase = p.WaterDS.Phase,
                WaterMWorSpGr = p.WaterDS.ReliefMW,
                WaterT = p.WaterDS.ReliefTemperature,
                WaterZ = p.WaterDS.ReliefZ,
                WaterCpCv=p.WaterDS.ReliefCpCv,

                AirReliefRate = p.AirDS.ReliefLoad,
                AirPhase = p.AirDS.Phase,
                AirMWorSpGr = p.AirDS.ReliefMW,
                AirT = p.AirDS.ReliefTemperature,
                AirZ = p.AirDS.ReliefZ,
                AirCpCv=p.AirDS.ReliefCpCv,

                SteamReliefRate = p.SteamDS.ReliefLoad,
                SteamPhase = p.SteamDS.Phase,
                SteamMWorSpGr = p.SteamDS.ReliefMW,
                SteamT = p.SteamDS.ReliefTemperature,
                SteamZ = p.SteamDS.ReliefZ,
                SteamCpCv=p.SteamDS.ReliefCpCv,

                FireReliefRate = p.FireDS.ReliefLoad,
                FirePhase = p.FireDS.Phase,
                FireMWorSpGr = p.FireDS.ReliefMW,
                FireT = p.FireDS.ReliefTemperature,
                FireZ = p.FireDS.ReliefZ,
                FireCpCv=p.FireDS.ReliefCpCv
            }).Take(model.listGrid.Count - 2).ToList();
            return listRS;
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
            export.ExportToExcelPUsummary(model.PlantName, model.ProcessUnitName, model.listGrid, "PUsummary.xls");
        }
        private void BtnNextUnit(object obj)
        {
            if (PlantCollection.Count > 0)
            {
                string dbPlantFile = string.Empty;
                string unitPath = string.Empty;
                List<string> ReportPath = new List<string>();

                foreach (PlantVM plantvm in PlantCollection)
                {
                    dbPlantFile = plantvm.PlantDir + @"\plant.mdb";
                    foreach (UnitVM uvm in plantvm.UnitCollection)
                    {
                        ReportPath.Clear();
                        if (!PlantList.Contains(uvm))
                        {
                            PlantList.Add(uvm);
                            unitPath = plantvm.PlantDir + @"\" + uvm.UnitName;
                            ReportPath.Add(dbPlantFile);
                            foreach (PSVM p in uvm.PSCollection)
                            {
                                ReportPath.Add(unitPath + @"\" + p.PSName + @"\protectedsystem.mdb");
                            }
                            this.LoadSingleUnit(uvm.ID, ReportPath);
                            break;
                        }
                       
                    }
                }
            }
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
