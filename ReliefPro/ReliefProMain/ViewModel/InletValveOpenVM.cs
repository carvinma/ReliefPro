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
namespace ReliefProMain.ViewModel
{
    /// <summary>
    /// 对于入口阀全开，ReliefPressure=Pset，和之前其他的case不同
    /// </summary>
    public class InletValveOpenVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        private string SourceFile;
        private string EqName;
        private string EqType;
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
                
                UnitConvert uc=new UnitConvert();
                UpVesselType = dicEqData[SelectedVessel].EqType;
                if (UpVesselType == "Column")
                {
                    UpStreamNames = GetTowerProducts(dicEqData[SelectedVessel]);
                    MaxOperatingPressure= UpStreamVaporData.Pressure;
                }
                else
                {
                    UpStreamNames = GetFlashProducts(dicEqData[SelectedVessel]);
                    MaxOperatingPressure = uc.Convert("KPA", "MPAG", double.Parse(dicEqData[_SelectedVessel].PressCalc)).ToString();
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
        private string _ReliefPressure{ get; set; }
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
        private Dictionary<string, ProIIEqData> dicEqData = new Dictionary<string, ProIIEqData>();
        private CustomStream UpStreamVaporData = new CustomStream();
        private CustomStream UpStreamLiquidData = new CustomStream();
        private CustomStream UpVesselNormalVapor = new CustomStream();
        private string rMass;
        private string version;
        private string rootdir;
        private string tempdir;
        private string przFile;
        private string UpVesselType;
        
        public InletValveOpenVM(string eqType, int ScenarioID, string dbPSFile, string dbPFile)
        {
            OperatingPhases = GetOperatingPhases();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            EqType = eqType;
            
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbPSV dbpsv = new dbPSV();
                PSV psv = dbpsv.GetModel(Session);
                ReliefPressure = psv.Pressure;
                dbTower dbtower = new dbTower();
                Tower m = dbtower.GetModel(Session);
                if (m != null)
                {
                    SourceFile = m.PrzFile;
                    EqName = m.TowerName;
                    rootdir = System.IO.Path.GetDirectoryName(dbPlantFile);
                    przFile = rootdir + @"\" + SourceFile;
                    version = ProIIFactory.GetProIIVerison(przFile, rootdir);
                    tempdir = System.IO.Path.GetDirectoryName(dbProtectedSystemFile)+@"\temp\";
                }

                
            }
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
                Vessels = GetVessels(Session);
            }


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

            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        private ObservableCollection<string> GetVessels(ISession Session)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();

            dbProIIEqData dbeq = new dbProIIEqData();
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
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                dbProIIStreamData db=new dbProIIStreamData();
                var Session = helper.GetCurrentSession();                
                string productdata = data.ProductData;
                string producttype = data.ProductStoreData;
                string[] arrProducts = productdata.Split(',');
                string[] arrProductTypes = producttype.Split(',');
                for (int i = 0; i < arrProducts.Length; i++)
                {
                    if (arrProductTypes[i] == "1" || arrProductTypes[i] == "2")
                    {
                        string pName=arrProducts[i];
                        list.Add(pName);
                        ProIIStreamData s = db.GetModel(Session, pName, SourceFile);
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(s);

                        if (arrProductTypes[i] == "1")
                        {
                            UpStreamVaporData = cs;
                            UpVesselNormalVapor = cs;
                        }
                        else
                        {
                            UpStreamLiquidData = cs;
                        }
                    }
                }
            }
           

            return list;
        }

        public ObservableCollection<string> GetTowerProducts(ProIIEqData data)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            string productdata = data.ProductData;
            string prodtype=data.ProdType;
            string[] arrProducts = productdata.Split(',');
            string[] arrProdTypes = prodtype.Split(',');
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                dbProIIStreamData db = new dbProIIStreamData();
                var Session = helper.GetCurrentSession();
                for (int i = 0; i < arrProducts.Length; i++)
                {
                    string pName = arrProducts[i];
                    if (arrProdTypes[i] == "5")
                    {                        
                        list.Add(pName);
                        ProIIStreamData s = db.GetModel(Session, pName, SourceFile);
                        UpStreamLiquidData = ProIIToDefault.ConvertProIIStreamToCustomStream(s);                     

                    }
                    else if (arrProdTypes[i] == "3")
                    {
                        ProIIStreamData s = db.GetModel(Session, pName, SourceFile);
                        UpVesselNormalVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(s);
                    }
                }
            }
            string tempdir = System.IO.Path.GetDirectoryName(dbProtectedSystemFile) + @"\temp\";
            if (!Directory.Exists(tempdir))
                Directory.CreateDirectory(tempdir);
            IProIIReader reader = ProIIFactory.CreateReader(version);
            reader.InitProIIReader(przFile);
            ProIIStreamData vapor = reader.CopyStream(data.EqName, int.Parse(data.NumberOfTrays), 1, 1);
            list.Add(vapor.StreamName);
            UpStreamVaporData = ProIIToDefault.ConvertProIIStreamToCustomStream(vapor);
            
            return list;
        }

        private string GetProIIStreamDataPressure(string streamName)
        {
            string press = "0";
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbProIIStreamData db = new dbProIIStreamData();

                ProIIStreamData data = db.GetModel(Session, streamName, SourceFile);
                CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
                press = cs.Pressure;
            }
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
             string rMass=string.Empty; 
            
            string dirInletValveOpen = tempdir + "InletValveOpen";
            if (!Directory.Exists(dirInletValveOpen))
                Directory.CreateDirectory(dirInletValveOpen);
            
            string vapor = Guid.NewGuid().ToString().Substring(0, 6);
            string liquid = Guid.NewGuid().ToString().Substring(0, 6);           
            
            double Wliquidvalve = Darcy(Rmass, Cv, UPStreamPressure, DownStreamPressure);
            IFlashCalculateW flashCalc = ProIIFactory.CreateFlashCalculateW(version);
            string content = PRIIFileOperator.getUsableContent(UpStreamLiquidData.StreamName, rootdir);
            string f = flashCalc.Calculate(content, 1, DownStreamPressure, 5, "0", UpStreamLiquidData, vapor, liquid, Wliquidvalve.ToString(), dirInletValveOpen);

            IProIIReader reader = ProIIFactory.CreateReader(version);
            reader.InitProIIReader(f);
            ProIIStreamData proIIStreamData = reader.GetSteamInfo(vapor);
            reader.ReleaseProIIReader();
            CustomStream vaporStream=ProIIToDefault.ConvertProIIStreamToCustomStream(proIIStreamData);
            FLReliefLoad = (double.Parse(vaporStream.WeightFlow)-double.Parse(UpVesselNormalVapor.WeightFlow)).ToString();
            FLReliefMW = vaporStream.BulkMwOfPhase;
            FLReliefTemperature = vaporStream.Temperature;
            
        }

        private void VaporBreakthrough(ref string VBReliefLoad, ref string VBReliefMW, ref string VBReliefTemperature)
        {                   
            string dirInletValveOpen = tempdir + "InletValveOpen";
            if (!Directory.Exists(dirInletValveOpen))
                Directory.CreateDirectory(dirInletValveOpen);
           double reliefLoad = Darcy(rMass, CV, MaxOperatingPressure, ReliefPressure);
           VBReliefLoad = (reliefLoad-double.Parse(UpVesselNormalVapor.WeightFlow)).ToString();
           VBReliefMW = UpVesselNormalVapor.BulkMwOfPhase;
           VBReliefTemperature = UpVesselNormalVapor.Temperature;
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
            string flReliefLoad = string.Empty;
            string flReliefMW = string.Empty;
            string flReliefTemperature = string.Empty;
            if (UpStreamLiquidData != null)
            {
                rMass = UpStreamLiquidData.BulkDensityAct;               
                LiquidFlashing(rMass, CV, MaxOperatingPressure, ReliefPressure, przFile, ref flReliefLoad, ref flReliefMW, ref flReliefTemperature);
                              
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
                    VaporBreakthrough(ref vbReliefLoad, ref vbReliefMW,ref vbReliefTemperature);
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
    }
}
