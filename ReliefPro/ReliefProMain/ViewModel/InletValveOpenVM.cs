﻿using System;
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
        private string rMass;
        private string PrzVersion;
        private string tempdir;
        private string PrzFile;
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
                    if (UpVesselType == "Column")
                    {
                        UpStreamNames = GetTowerProducts(dicEqData[SelectedVessel]);
                        MaxOperatingPressure = UpStreamVaporData.Pressure;
                    }
                    else
                    {
                        UpStreamNames = GetFlashProducts(dicEqData[SelectedVessel]);
                        MaxOperatingPressure = UnitConvert.Convert("KPA", "MPAG", double.Parse(dicEqData[_SelectedVessel].PressCalc)).ToString();
                    }
                }

                OnPropertyChanged("SelectedVessel");
            }
        }


        private string _ReliefTemperature { get; set; }
        public string ReliefTemperature
        {
            get { return _ReliefTemperature; }
            set
            {
                _ReliefTemperature = value;
                OnPropertyChanged("ReliefTemperature");
            }
        }
        private string _ReliefPressure { get; set; }
        public string ReliefPressure
        {
            get { return _ReliefPressure; }
            set
            {
                _ReliefPressure = value;
                OnPropertyChanged("ReliefPressure");
            }
        }
        private string _ReliefLoad { get; set; }
        public string ReliefLoad
        {
            get { return _ReliefLoad; }
            set
            {
                _ReliefLoad = value;
                OnPropertyChanged("ReliefLoad");
            }
        }
        private string _ReliefMW { get; set; }
        public string ReliefMW
        {
            get { return _ReliefMW; }
            set
            {
                _ReliefMW = value;
                OnPropertyChanged("ReliefMW");
            }
        }

        private string _MaxOperatingPressure { get; set; }
        public string MaxOperatingPressure
        {
            get { return _MaxOperatingPressure; }
            set
            {
                _MaxOperatingPressure = value;

                OnPropertyChanged("MaxOperatingPressure");
            }
        }

        private string _CV { get; set; }
        public string CV
        {
            get { return _CV; }
            set
            {
                _CV = value;

                OnPropertyChanged("CV");
            }
        }
        int ScenarioID { get; set; }

        private ObservableCollection<string> _Vessels { get; set; }
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

        private ObservableCollection<string> _UpStreamNames { get; set; }
        public ObservableCollection<string> UpStreamNames
        {
            get { return _UpStreamNames; }
            set
            {
                _UpStreamNames = value;
                OnPropertyChanged("UpStreamNames");
            }
        }

        InletValveOpen model;
        InletValveOpenDAL dbinlet;
        ScenarioDAL dbsc;
        UOMLib.UOMEnum uomEnum;
        public InletValveOpenVM(int scenarioID, string eqName, string eqType, string przFile, string version, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {

            uomEnum = new UOMLib.UOMEnum(sessionPlant);
            InitUnit();
            ScenarioID = scenarioID;
            EqName = eqName;
            EqType = eqType;
            DirPlant = dirPlant;
            PrzFile = przFile;
            PrzVersion = version;
            SessionPlant = sessionPlant;
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
                    if (s.ProdType == "3" || (s.ProdType == "1" && s.Tray == tower.StageNumber))
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

            SourceFile = System.IO.Path.GetFileName(przFile);
            Vessels = GetVessels(SessionPlant);
            tempdir = dirProtectedSystem + @"\temp\";
            dbinlet = new InletValveOpenDAL();
            //读取当前入口阀全开的信息
            model = dbinlet.GetModel(SessionProtectedSystem);
            if (model == null)
            {
                PSVDAL dbpsv = new PSVDAL();
                PSV psv = dbpsv.GetModel(sessionProtectedSystem);
                ReliefPressure = psv.Pressure;
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

        private void Save(object window)
        {
            WriteConvert();
            dbsc = new ScenarioDAL();
            if (model == null)
            {
                model = new InletValveOpen();
                model.CV = CV;
                model.MaxOperatingPressure = MaxOperatingPressure;
                model.OperatingPhase = SelectedOperatingPhase;
                model.ReliefLoad = ReliefLoad;
                model.ReliefMW = ReliefMW;
                model.ReliefPressure = ReliefPressure;
                model.ReliefTemperature = _ReliefTemperature;
                model.VesselName = SelectedVessel;
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
                model.ReliefTemperature = _ReliefTemperature;
                model.VesselName = SelectedVessel;
                dbinlet.Update(model, SessionProtectedSystem);
            }
            Scenario sc = dbsc.GetModel(ScenarioID, SessionProtectedSystem);
            sc.ReliefLoad = ReliefLoad;
            sc.ReliefMW = ReliefMW;
            sc.ReliefPressure = ReliefPressure;
            sc.ReliefTemperature = _ReliefTemperature;
            dbsc.Update(sc, SessionProtectedSystem);
            SessionProtectedSystem.Flush();



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


            IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
            reader.InitProIIReader(PrzFile);
            ProIIStreamData vapor = reader.CopyStream(data.EqName, int.Parse(data.NumberOfTrays), 1, 1);
            list.Add(vapor.StreamName);
            UpStreamVaporData = ProIIToDefault.ConvertProIIStreamToCustomStream(vapor);

            return list;
        }

        private string GetProIIStreamDataPressure(string streamName)
        {
            string press = "0";

            ProIIStreamDataDAL db = new ProIIStreamDataDAL();

            ProIIStreamData data = db.GetModel(SessionPlant, streamName, SourceFile);
            CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
            press = cs.Pressure;

            return press;
        }


        private double Darcy(string Rmass, string Cv, string UPStreamPressure, string DownStreamPressure)
        {
            double dRmass = double.Parse(Rmass);
            double dCv = double.Parse(Cv);
            double dUPStreamPressure = double.Parse(UPStreamPressure);
            double dDownStreamPressure = double.Parse(DownStreamPressure);
            double detaP = dUPStreamPressure - dDownStreamPressure;
            if (detaP < 0)
                return 0;
            else
            {
                double w = 0.00075397 * dCv * Math.Pow(detaP * 1000 * dRmass, 0.5) * 3600;
                return w;
            }
        }

        private void LiquidFlashing(string Rmass, string Cv, string UPStreamPressure, string DownStreamPressure, string przFile, ref string FLReliefLoad, ref string FLReliefMW, ref string FLReliefTemperature)
        {
            string rMass = string.Empty;

            string dirInletValveOpen = tempdir + "InletValveOpen";
            if (!Directory.Exists(dirInletValveOpen))
                Directory.CreateDirectory(dirInletValveOpen);

            string vapor = Guid.NewGuid().ToString().Substring(0, 6);
            string liquid = Guid.NewGuid().ToString().Substring(0, 6);

            double Wliquidvalve = Darcy(Rmass, Cv, UPStreamPressure, DownStreamPressure);
            IFlashCalculateW flashCalc = ProIIFactory.CreateFlashCalculateW(PrzVersion);
            string content = PROIIFileOperator.getUsableContent(UpStreamLiquidData.StreamName, DirPlant);
            int ImportResult = 0;
            int RunResult = 0;
            string f = flashCalc.Calculate(content, 1, DownStreamPressure, 5, "0", UpStreamLiquidData, vapor, liquid, Wliquidvalve.ToString(), dirInletValveOpen, ref ImportResult, ref RunResult);
            ProIIStreamData proIIStreamData;
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
                    reader.InitProIIReader(f);
                    proIIStreamData = reader.GetSteamInfo(vapor);
                    reader.ReleaseProIIReader();
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
            if (!string.IsNullOrEmpty(vaporStream.WeightFlow) && CurrentEqNormalVapor != null)
            {
                FLReliefLoad = (double.Parse(vaporStream.WeightFlow) - double.Parse(CurrentEqNormalVapor.WeightFlow)).ToString();
                FLReliefMW = vaporStream.BulkMwOfPhase;
                FLReliefTemperature = vaporStream.Temperature;
            }
            else
            {
                FLReliefLoad = "0";
                FLReliefMW = "0";
                FLReliefTemperature = "0";
            }

        }

        private void VaporBreakthrough(ref string VBReliefLoad, ref string VBReliefMW, ref string VBReliefTemperature)
        {
            string dirInletValveOpen = tempdir + "InletValveOpen";
            if (!Directory.Exists(dirInletValveOpen))
                Directory.CreateDirectory(dirInletValveOpen);
            double reliefLoad = Darcy(rMass, CV, MaxOperatingPressure, ReliefPressure);
            if (CurrentEqNormalVapor != null)
            {
                VBReliefLoad = (reliefLoad - double.Parse(CurrentEqNormalVapor.WeightFlow)).ToString();
                VBReliefMW = this.UpStreamVaporData.BulkMwOfPhase;
                VBReliefTemperature = UpStreamVaporData.Temperature;
            }
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
            if (string.IsNullOrEmpty(CV))
            {
                MessageBox.Show("Please Input CV.", "Message Box");
                return;
            }
            if (string.IsNullOrEmpty(MaxOperatingPressure))
            {
                MessageBox.Show("Max Operating Pressure can't be empty.", "Message Box");
                return;
            }
            string flReliefLoad = string.Empty;
            string flReliefMW = string.Empty;
            string flReliefTemperature = string.Empty;
            if (UpStreamLiquidData != null)
            {
                rMass = UpStreamLiquidData.BulkDensityAct;
                if (rMass != "" && rMass != "0")
                {
                    LiquidFlashing(rMass, CV, MaxOperatingPressure, ReliefPressure, PrzFile, ref flReliefLoad, ref flReliefMW, ref flReliefTemperature);
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
                    ReliefLoad = "0";
                }
                else
                {
                    string vbReliefLoad = string.Empty;
                    string vbReliefMW = string.Empty;
                    string vbReliefTemperature = string.Empty;
                    rMass = UpStreamVaporData.BulkDensityAct;
                    VaporBreakthrough(ref vbReliefLoad, ref vbReliefMW, ref vbReliefTemperature);
                    if (double.Parse(vbReliefLoad) > double.Parse(flReliefLoad))
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

        }

        private void ReadConvert()
        {
            if (!string.IsNullOrEmpty(MaxOperatingPressure))
            {
                this.MaxOperatingPressure = UnitConvert.Convert(UOMEnum.Pressure, _MaxOperatingPressureUnit, double.Parse(MaxOperatingPressure)).ToString();
            }
            if (!string.IsNullOrEmpty(ReliefLoad))
            {
                this.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, reliefloadUnit, double.Parse(ReliefLoad)).ToString();
            }
            if (!string.IsNullOrEmpty(ReliefPressure))
            {
                this.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, reliefPressureUnit, double.Parse(ReliefPressure)).ToString();
            }
            if (!string.IsNullOrEmpty(ReliefTemperature))
            {
                this.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, reliefTemperatureUnit, double.Parse(ReliefTemperature)).ToString();
            }
        }
        private void WriteConvert()
        {
            this.MaxOperatingPressure = UnitConvert.Convert(_MaxOperatingPressureUnit, UOMEnum.Pressure, double.Parse(MaxOperatingPressure)).ToString();
            this.ReliefLoad = UnitConvert.Convert(reliefloadUnit, UOMEnum.MassRate, double.Parse(ReliefLoad)).ToString();
            this.ReliefPressure = UnitConvert.Convert(reliefPressureUnit, UOMEnum.Pressure, double.Parse(ReliefPressure)).ToString();
            this.ReliefTemperature = UnitConvert.Convert(reliefTemperatureUnit, UOMEnum.Temperature, double.Parse(ReliefTemperature)).ToString();
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
