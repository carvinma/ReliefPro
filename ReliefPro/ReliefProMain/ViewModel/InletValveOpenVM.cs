using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using NHibernate;
using ProII;
using ReliefProCommon.CommonLib;
using System.IO;
using UOMLib;
using System.Windows;


namespace ReliefProMain.ViewModel
{
    /// <summary>
    /// 对于入口阀全开，ReliefPressure=Pset，和之前其他的case不同
    /// </summary>
    public class InletValveOpenVM : ViewModelBase
    {
        private string SourceFile;
        private string EqName;
        private string EqType;
        private ISession SessionPlant { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private Dictionary<string, ProIIEqData> dicEqData = new Dictionary<string, ProIIEqData>();
        private CustomStream UpStreamVaporData;
        private CustomStream UpStreamLiquidData;
        //private CustomStream UpVesselNormalVapor;
        private CustomStream CurrentEqNormalVapor;
        private double rMass;

        private string tempdir;
        public SourceFile SourceFileInfo { get; set; }
        public string FileFullPath { get; set; }
        private string UpVesselType;

        public List<string> OperatingPhases { get; set; }


        private string _SelectedOperatingPhase { get; set; }
        public string SelectedOperatingPhase
        {
            get { return _SelectedOperatingPhase; }
            set
            {
                _SelectedOperatingPhase = value;
                OnPropertyChanged("SelectedOperatingPhase");
            }
        }
        private string _SelectedVessel { get; set; }
        public string SelectedVessel
        {
            get { return _SelectedVessel; }
            set
            {
                _SelectedVessel = value;

                if (!string.IsNullOrEmpty(SelectedVessel))
                {
                    UpVesselType = dicEqData[SelectedVessel].EqType;
                    SplashScreenManager.Show();
                    SplashScreenManager.SentMsgToScreen("Loading Data is in progress, please wait…");
                    if (UpVesselType == "Column")
                    {
                        UpStreamNames = GetTowerProducts(dicEqData[SelectedVessel]);
                        MaxOperatingPressure = UpStreamVaporData.Pressure;
                        UpStreamCpCv = UpStreamVaporData.BulkCPCVRatio;
                    }
                    else
                    {
                        UpStreamNames = GetFlashProducts(dicEqData[SelectedVessel]);
                        
                        MaxOperatingPressure = UnitConvert.Convert("KPA", "MPAG", double.Parse(dicEqData[_SelectedVessel].PressCalc));
                        if (UpStreamVaporData!=null)
                        {
                            UpStreamCpCv = UpStreamVaporData.BulkCPCVRatio;
                        }
                    }
                    SplashScreenManager.SentMsgToScreen("Loading finished");
                    SplashScreenManager.Close();
                }

                OnPropertyChanged("SelectedVessel");
            }
        }


        private double _ReliefTemperature;
        public double ReliefTemperature
        {
            get { return _ReliefTemperature; }
            set
            {
                _ReliefTemperature = value;
                OnPropertyChanged("ReliefTemperature");
            }
        }
        private double _ReliefPressure;
        public double ReliefPressure
        {
            get { return _ReliefPressure; }
            set
            {
                _ReliefPressure = value;
                OnPropertyChanged("ReliefPressure");
            }
        }
        private double _ReliefLoad;
        public double ReliefLoad
        {
            get { return _ReliefLoad; }
            set
            {
                _ReliefLoad = value;
                OnPropertyChanged("ReliefLoad");
            }
        }
        private double _ReliefMW;
        public double ReliefMW
        {
            get { return _ReliefMW; }
            set
            {
                _ReliefMW = value;
                OnPropertyChanged("ReliefMW");
            }
        }

        private double _MaxOperatingPressure;
        public double MaxOperatingPressure
        {
            get { return _MaxOperatingPressure; }
            set
            {
                _MaxOperatingPressure = value;

                OnPropertyChanged("MaxOperatingPressure");
            }
        }
        private double _UpStreamCpCv;
        public double UpStreamCpCv
        {
            get { return _UpStreamCpCv; }
            set
            {
                _UpStreamCpCv = value;

                OnPropertyChanged("UpStreamCpCv");
            }
        }
        private double _XT=0.7;
        public double XT
        {
            get { return _XT; }
            set
            {
                _XT = value;

                OnPropertyChanged("XT");
            }
        }
        private double _CV;
        public double CV
        {
            get { return _CV; }
            set
            {
                _CV = value;

                OnPropertyChanged("CV");
            }
        }
        int ScenarioID { get; set; }

        private ObservableCollection<string> _Vessels;
        public ObservableCollection<string> Vessels
        {
            get { return _Vessels; }
            set
            {
                _Vessels = value;
                OnPropertyChanged("Vessels");
            }
        }
        public List<string> GetOperatingPhases()
        {
            List<string> list = new List<string>();
            list.Add("Full Liquid");
            list.Add("Vapor-liquid");
            return list;
        }

        private ObservableCollection<string> _UpStreamNames;
        public ObservableCollection<string> UpStreamNames
        {
            get { return _UpStreamNames; }
            set
            {
                _UpStreamNames = value;
                OnPropertyChanged("UpStreamNames");
            }
        }
        private double _ReliefCpCv;
        public double ReliefCpCv
        {
            get { return _ReliefCpCv; }
            set
            {
                _ReliefCpCv = value;
                this.NotifyPropertyChanged("ReliefCpCv");
            }
        }

        private double _ReliefZ;
        public double ReliefZ
        {
            get { return _ReliefZ; }
            set
            {
                _ReliefZ = value;
                this.NotifyPropertyChanged("ReliefZ");
            }
        }

        InletValveOpen model;
        InletValveOpenDAL dbinlet;
        ScenarioDAL dbsc;
        UOMLib.UOMEnum uomEnum;
        string HeatMethod = string.Empty;
        public InletValveOpenVM(int scenarioID, string eqName, string eqType, SourceFile sourceFileInfo, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            SessionPlant = sessionPlant;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            InitUnit();
            ScenarioID = scenarioID;
            EqName = eqName;
            EqType = eqType;
            DirPlant = dirPlant;
            SourceFileInfo = sourceFileInfo;
            FileFullPath = DirPlant + @"\" + sourceFileInfo.FileNameNoExt + @"\" + sourceFileInfo.FileName;

            SessionProtectedSystem = sessionProtectedSystem;
            OperatingPhases = GetOperatingPhases();

            TowerDAL dbtower = new TowerDAL();
            Tower tower = dbtower.GetModel(SessionProtectedSystem);

            StreamDAL dbstream = new StreamDAL();
            IList<CustomStream> list = dbstream.GetAllList(this.SessionProtectedSystem, true);
            foreach (CustomStream s in list)
            {
                if (eqType == "Tower")
                {
                    //if (s.ProdType == "3" || (s.ProdType == "1" && s.Tray == tower.StageNumber))
                    if (s.ProdType == "3" || (s.ProdType == "1" && s.Tray == 1))
                    {
                        CurrentEqNormalVapor = s;
                    }
                }
                else if (eqType == "Drum")
                {
                    if (s.ProdType == "1")
                    {
                        CurrentEqNormalVapor = s;
                    }
                }
            }

            SourceFile = SourceFileInfo.FileName;
            Vessels = GetVessels(SessionPlant);
            tempdir = dirProtectedSystem + @"\temp\";
            dbinlet = new InletValveOpenDAL();
            //读取当前入口阀全开的信息
            model = dbinlet.GetModel(SessionProtectedSystem,ScenarioID);
            if (model == null)
            {
               
            }
            else
            {
                ReliefLoad = model.ReliefLoad;
                ReliefMW = model.ReliefMW;
                ReliefPressure = model.ReliefPressure;
                ReliefTemperature = model.ReliefTemperature;
                SelectedVessel = model.VesselName;
                SelectedOperatingPhase = model.OperatingPhase;
                MaxOperatingPressure = model.MaxOperatingPressure;
                CV = model.CV;
                XT = model.XT;
                UpStreamCpCv = model.UpStreamCpCv;
            }
            ReadConvert();
        }



        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(Save);

                }
                return _SaveCommand;
            }
        }
        private ICommand _CalculatePressureCommand;
        public ICommand CalculatePressureCommand
        {
            get
            {
                if (_CalculatePressureCommand == null)
                {
                    _CalculatePressureCommand = new RelayCommand(CalculatePressure);

                }
                return _CalculatePressureCommand;
            }
        }

        private void CalculatePressure(object o)
        {
           
        }
        
        private void Save(object window)
        {

            WriteConvert();
            dbsc = new ScenarioDAL();
            Scenario sc = dbsc.GetModel(ScenarioID, SessionProtectedSystem);
            sc.ReliefLoad = ReliefLoad;
            sc.ReliefMW = ReliefMW;
            sc.ReliefPressure = ReliefPressure;
            sc.ReliefTemperature = ReliefTemperature;
            sc.ReliefCpCv = ReliefCpCv;
            sc.ReliefZ = ReliefZ;
            dbsc.Update(sc, SessionProtectedSystem);
            if (model == null)
            {
                model = new InletValveOpen();
                model.CV = CV;
                model.MaxOperatingPressure = MaxOperatingPressure;
                model.OperatingPhase = SelectedOperatingPhase;
                model.ReliefLoad = ReliefLoad;
                model.ReliefMW = ReliefMW;
                model.ReliefPressure = ReliefPressure;
                model.ReliefTemperature = ReliefTemperature;
                model.VesselName = SelectedVessel;
                model.XT = XT;
                model.UpStreamCpCv = UpStreamCpCv;
                model.ReliefCpCv = ReliefCpCv;
                model.ReliefZ = ReliefZ;
                model.ScenarioID = sc.ID;
                dbinlet.Add(model, SessionProtectedSystem);
            }
            else
            {
                model.CV = CV;
                model.MaxOperatingPressure = MaxOperatingPressure;
                model.OperatingPhase = SelectedOperatingPhase;
                model.ReliefLoad = ReliefLoad;
                model.ReliefMW = ReliefMW;
                model.ReliefPressure = ReliefPressure;
                model.ReliefTemperature = ReliefTemperature;
                model.VesselName = SelectedVessel;
                model.XT = XT;
                model.UpStreamCpCv = UpStreamCpCv;
                model.ReliefCpCv = ReliefCpCv;
                model.ReliefZ = ReliefZ;
                model.ScenarioID = sc.ID;
                dbinlet.Update(model, SessionProtectedSystem);
            }
            
            
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private ObservableCollection<string> GetVessels(ISession Session)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();

            ProIIEqDataDAL dbeq = new ProIIEqDataDAL();
            IList<ProIIEqData> eqlist = dbeq.GetAllList(Session, SourceFile, "Flash");
            foreach (ProIIEqData d in eqlist)
            {
                if (d.EqName != EqName)
                {
                    list.Add(d.EqName);
                    dicEqData.Add(d.EqName, d);
                }
            }
            eqlist = dbeq.GetAllList(Session, SourceFile, "Column");
            foreach (ProIIEqData d in eqlist)
            {
                if (d.EqName != EqName)
                {
                    list.Add(d.EqName);
                    dicEqData.Add(d.EqName, d);
                }
            }
            return list;
        }
        public ObservableCollection<string> GetFlashProducts(ProIIEqData data)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();

            ProIIStreamDataDAL db = new ProIIStreamDataDAL();
            string productdata = data.ProductData;
            string producttype = data.ProductStoreData;
            string[] arrProducts = productdata.Split(',');
            string[] arrProductTypes = producttype.Split(',');
            for (int i = 0; i < arrProducts.Length; i++)
            {
                if (arrProductTypes[i] == "1" || arrProductTypes[i] == "2")
                {
                    string pName = arrProducts[i];
                    list.Add(pName);
                    ProIIStreamData s = db.GetModel(SessionPlant, pName, SourceFile);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(s);

                    if (arrProductTypes[i] == "1")
                    {
                        UpStreamVaporData = cs;
                        //UpVesselNormalVapor = cs;
                    }
                    else
                    {
                        UpStreamLiquidData = cs;
                    }
                }

            }


            return list;
        }

        public ObservableCollection<string> GetTowerProducts(ProIIEqData data)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            string productdata = data.ProductData;
            string prodtype = data.ProdType;
            string[] arrProducts = productdata.Split(',');
            string[] arrProdTypes = prodtype.Split(',');

            ProIIStreamDataDAL db = new ProIIStreamDataDAL();
            for (int i = 0; i < arrProducts.Length; i++)
            {
                string pName = arrProducts[i];
                if (arrProdTypes[i] == "5")
                {
                    list.Add(pName);
                    ProIIStreamData s = db.GetModel(SessionPlant, pName, SourceFile);
                    UpStreamLiquidData = ProIIToDefault.ConvertProIIStreamToCustomStream(s);

                }
                //else if (arrProdTypes[i] == "3")
                // {
                //     ProIIStreamData s = db.GetModel(SessionPlant, pName, SourceFile);
                //     UpVesselNormalVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(s);
                // }
            }


            //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
            //reader.InitProIIReader(FileFullPath);
            //ProIIStreamData vapor = reader.CopyStream(data.EqName, trayNumber, 1, 1);

            int trayNumber = int.Parse(data.NumberOfTrays);
            ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, FileFullPath);
            ProIIStreamData vapor = reader.CopyStreamInfo(EqName, trayNumber, 1, 1);


            list.Add(vapor.StreamName);
            UpStreamVaporData = ProIIToDefault.ConvertProIIStreamToCustomStream(vapor);

            return list;
        }

        private double GetProIIStreamDataPressure(string streamName)
        {
            double press = 0;

            ProIIStreamDataDAL db = new ProIIStreamDataDAL();

            ProIIStreamData data = db.GetModel(SessionPlant, streamName, SourceFile);
            CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
            press = cs.Pressure;

            return press;
        }


        private double Darcy(double Rmass, double Cv, double UPStreamPressure, double DownStreamPressure)
        {
            double dRmass = Rmass;
            double dCv = Cv;
            double dUPStreamPressure = UPStreamPressure;
            double dDownStreamPressure = DownStreamPressure;
            double detaP = dUPStreamPressure - dDownStreamPressure;
            if (detaP < 0)
                return 0;
            else
            {
                double w = 0.00075397 * dCv * Math.Pow(detaP * 1000 * dRmass, 0.5) * 3600;
                return w;
            }
        }

        private double VaporMethod(double Rmass, double Cv, double UPStreamPressure, double DownStreamPressure)
        {
            double w = 0;
            double dUPStreamPressure = UPStreamPressure;
            double dDownStreamPressure = DownStreamPressure;
            double detaP = dUPStreamPressure - dDownStreamPressure;

            double x = detaP / UPStreamPressure;
            double fr = UpStreamCpCv / 1.4;
            if (x >= fr*XT)
            {
                w = Cv * 2.73 * 0.667 * Math.Pow(fr * XT * dUPStreamPressure * Rmass, 0.5);
            }
            else
            {
                double y = 1 - x / (3 * fr * XT);
                w = Cv * 2.73 * y * Math.Pow(detaP * Rmass, 0.5);
            }
            return w;
        }

        private void LiquidFlashing(double Rmass, double Cv, double UPStreamPressure, double DownStreamPressure, ref double FLReliefLoad, ref double FLReliefMW, ref double FLReliefTemperature)
        {
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            string rMass = string.Empty;

            string dirInletValveOpen = tempdir + "InletValveOpen_LiquidMethod" + ScenarioID.ToString(); ;
            if (Directory.Exists(dirInletValveOpen))
            {
                Directory.Delete(dirInletValveOpen, true);
            }
                Directory.CreateDirectory(dirInletValveOpen);

            string vapor = Guid.NewGuid().ToString().Substring(0, 6);
            string liquid = Guid.NewGuid().ToString().Substring(0, 6);

            double p1 = UnitConvert.Convert(UOMEnum.Pressure, "Mpa", UPStreamPressure);
            double p2 = UnitConvert.Convert(UOMEnum.Pressure, "Mpa", DownStreamPressure);

            double Wliquidvalve = Darcy(Rmass, Cv, p1, p2);
            //IFlashCalculate flashCalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
            string content = PROIIFileOperator.getUsableContent(UpStreamLiquidData.StreamName, dir);
            string[] sourceFiles = Directory.GetFiles(dir, "*.inp");
            string sourceFile = sourceFiles[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            HeatMethod = ProIIMethod.GetHeatMethod(lines, EqName);

            int ImportResult = 0;
            int RunResult = 0;
            UpStreamLiquidData.TotalMolarRate = Wliquidvalve/UpStreamLiquidData.BulkMwOfPhase/3600; //单位是kgm/s
            //string f = flashCalc.Calculate(content, 1, DownStreamPressure.ToString(), 5, "0",HeatMethod, UpStreamLiquidData, vapor, liquid, dirInletValveOpen, ref ImportResult, ref RunResult);
            
            ProIICalculate proiicalc = new ProIICalculate(SourceFileInfo.FileVersion);
            string f = proiicalc.FlashCalculate(content, 1, DownStreamPressure.ToString(), 5, "0", HeatMethod, UpStreamLiquidData, vapor, liquid, dirInletValveOpen, ref ImportResult, ref RunResult);
            
            ProIIStreamData proIIStreamData;
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    //reader.InitProIIReader(f);
                    //proIIStreamData = reader.GetSteamInfo(vapor);
                    //reader.ReleaseProIIReader();

                    ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, f);
                    proIIStreamData = reader.GetStreamInfo(vapor);
                }

                else
                {
                    MessageBox.Show("Prz file is error", "Message Box");
                    return;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
                return;
            }
            CustomStream vaporStream = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIStreamData);
            if (vaporStream.WeightFlow != null && CurrentEqNormalVapor != null)
            {
                FLReliefLoad = vaporStream.WeightFlow - CurrentEqNormalVapor.WeightFlow;
                FLReliefMW = vaporStream.BulkMwOfPhase;
                FLReliefTemperature = vaporStream.Temperature;
                ReliefCpCv = vaporStream.BulkCPCVRatio;
                ReliefZ = vaporStream.VaporFraction;
            }
            else
            {
                FLReliefLoad = 0;
                FLReliefMW = 0;
                FLReliefTemperature = 0;
                ReliefCpCv = 0;
                ReliefZ = 0;
            }

        }

        private void VaporBreakthrough(ref double VBReliefLoad, ref double VBReliefMW, ref double VBReliefTemperature)
        {
            string dirInletValveOpen = tempdir + "InletValveOpen_VaporMethod"+ScenarioID.ToString();
            if (Directory.Exists(dirInletValveOpen))
            {
                Directory.Delete(dirInletValveOpen, true);
            }
                Directory.CreateDirectory(dirInletValveOpen);
            //double reliefLoad = Darcy(rMass, CV, MaxOperatingPressure, ReliefPressure);

            double p1 = UnitConvert.Convert(UOMEnum.Pressure, "kpa", MaxOperatingPressure);
            double p2 = UnitConvert.Convert(UOMEnum.Pressure, "kpa", ReliefPressure);
            double reliefLoad = VaporMethod(rMass, CV, p1, p2);
            double wf = 0;
            if (CurrentEqNormalVapor != null)
            {               
                if (CurrentEqNormalVapor.WeightFlow != null)
                {
                    wf = CurrentEqNormalVapor.WeightFlow;
                }               
            }
            VBReliefLoad = (reliefLoad - wf);
            VBReliefMW = this.UpStreamVaporData.BulkMwOfPhase;
            VBReliefTemperature = UpStreamVaporData.Temperature;
            ReliefCpCv = UpStreamCpCv;
            ReliefZ = UpStreamVaporData.VaporFraction;
        }


        private ICommand _CalculateCommand;
        public ICommand CalculateCommand
        {
            get
            {
                if (_CalculateCommand == null)
                {
                    _CalculateCommand = new RelayCommand(Calculate);

                }
                return _CalculateCommand;
            }
        }

        private void Calculate(object window)
        {
            if (string.IsNullOrEmpty(SelectedVessel))
            {
                MessageBox.Show("Please Select Vessel.", "Message Box");
                return;
            }
            if (string.IsNullOrEmpty(SelectedOperatingPhase))
            {
                MessageBox.Show("Please Select Operating Phase.", "Message Box");
                return;
            }
            if (CV == 0)
            {
                MessageBox.Show("Please Input CV.", "Message Box");
                return;
            }
            if (MaxOperatingPressure == 0)
            {
                MessageBox.Show("Max Operating Pressure can't be empty.", "Message Box");
                return;
            }
            if (SelectedOperatingPhase != "Full Liquid")
            {
                if (XT == 0)
                {
                    MessageBox.Show("Please Input XT.", "Message Box");
                    return;
                }
                if (UpStreamCpCv == 0)
                {
                    MessageBox.Show("Please Input UpStream CpCv.", "Message Box");
                    return;
                }
            }
            try
            {
                SplashScreenManager.Show();
                SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");

                PSVDAL dbpsv = new PSVDAL();
                PSV psv = dbpsv.GetModel(SessionProtectedSystem);
                ReliefPressure = psv.Pressure * psv.ReliefPressureFactor;

                double flReliefLoad = 0;
                double flReliefMW = 0;
                double flReliefTemperature = 0;
                if (UpStreamLiquidData != null)
                {
                    rMass = UpStreamLiquidData.BulkDensityAct;
                    if (rMass != 0)
                    {
                        LiquidFlashing(rMass, CV, MaxOperatingPressure, ReliefPressure, ref flReliefLoad, ref flReliefMW, ref flReliefTemperature);
                    }

                }
                if (SelectedOperatingPhase == "Full Liquid")
                {
                    ReliefLoad = flReliefLoad;
                    ReliefMW = flReliefMW;
                    ReliefTemperature = flReliefTemperature;
                }
                else
                {
                    if (UpStreamVaporData == null)
                    {
                        ReliefLoad = 0;
                    }
                    else
                    {
                        double vbReliefLoad = 0;
                        double vbReliefMW = 0;
                        double vbReliefTemperature = 0;
                        rMass = UpStreamVaporData.BulkDensityAct;
                        VaporBreakthrough(ref vbReliefLoad, ref vbReliefMW, ref vbReliefTemperature);
                        if (vbReliefLoad > flReliefLoad)
                        {
                            ReliefLoad = vbReliefLoad;
                            ReliefMW = vbReliefMW;
                            ReliefTemperature = vbReliefTemperature;
                        }
                        else
                        {
                            ReliefLoad = flReliefLoad;
                            ReliefMW = flReliefMW;
                            ReliefTemperature = flReliefTemperature;
                        }


                    }

                }
                if (ReliefLoad < 0)
                    ReliefLoad = 0;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SplashScreenManager.Close();
            }

        }

        private void ReadConvert()
        {
            if (MaxOperatingPressure != null)
            {
                this.MaxOperatingPressure = UnitConvert.Convert(UOMEnum.Pressure, _MaxOperatingPressureUnit, MaxOperatingPressure);
            }
            if (ReliefLoad != null)
            {
                this.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, reliefloadUnit, ReliefLoad);
            }
            if (ReliefPressure != null)
            {
                this.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, reliefPressureUnit, ReliefPressure);
            }
            if (ReliefTemperature != null)
            {
                this.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, reliefTemperatureUnit, ReliefTemperature);
            }
        }
        private void WriteConvert()
        {
            if (MaxOperatingPressure != null)
            {
                this.MaxOperatingPressure = UnitConvert.Convert(_MaxOperatingPressureUnit, UOMEnum.Pressure, MaxOperatingPressure);
            }
            if (ReliefLoad != null)
            {
                this.ReliefLoad = UnitConvert.Convert(reliefloadUnit, UOMEnum.MassRate, ReliefLoad);
            }
            if (ReliefPressure != null)
            {
                this.ReliefPressure = UnitConvert.Convert(reliefPressureUnit, UOMEnum.Pressure, ReliefPressure);
            }
            if (ReliefTemperature != null)
            {
                this.ReliefTemperature = UnitConvert.Convert(reliefTemperatureUnit, UOMEnum.Temperature, ReliefTemperature);
            }
        }
        private void InitUnit()
        {
            this._MaxOperatingPressureUnit = uomEnum.UserPressure;
            this.reliefloadUnit = uomEnum.UserMassRate;
            this.reliefPressureUnit = uomEnum.UserPressure;
            this.reliefTemperatureUnit = uomEnum.UserTemperature;
        }
        #region 单位字段
        private string _MaxOperatingPressureUnit { get; set; }
        public string MaxOperatingPressureUnit
        {
            get { return _MaxOperatingPressureUnit; }
            set
            {
                _MaxOperatingPressureUnit = value;

                OnPropertyChanged("MaxOperatingPressureUnit");
            }
        }


        private string reliefloadUnit;
        public string ReliefLoadUnit
        {
            get { return reliefloadUnit; }
            set
            {
                reliefloadUnit = value;
                this.OnPropertyChanged("ReliefLoadUnit");
            }
        }

        private string reliefTemperatureUnit;
        public string ReliefTemperatureUnit
        {
            get { return reliefTemperatureUnit; }
            set
            {
                reliefTemperatureUnit = value;
                this.OnPropertyChanged("ReliefTemperatureUnit");
            }
        }

        private string reliefPressureUnit;
        public string ReliefPressureUnit
        {
            get { return reliefPressureUnit; }
            set
            {
                reliefPressureUnit = value;
                this.OnPropertyChanged("ReliefPressureUnit");
            }
        }
        #endregion
    }
}
