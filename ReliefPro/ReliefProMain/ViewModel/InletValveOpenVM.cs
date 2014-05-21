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



        private string _SelectedUpStream { get; set; }
        public string SelectedUpStream
        {
            get { return _SelectedUpStream; }
            set
            {
                _SelectedUpStream = value;
                MaxOperatingPressure = GetProIIStreamDataPressure(_SelectedUpStream);
                OnPropertyChanged("SelectedUpStream");
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
        private List<CustomStream> arrStreamData = new List<CustomStream>();
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
                ReliefPressure = (double.Parse(psv.Pressure) * double.Parse(psv.ReliefPressureFactor)).ToString();
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
                        ProIIStreamData s = db.GetModel(Session, pName);
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(s);
                        arrStreamData.Add(cs);
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
            for (int i = 0; i < arrProducts.Length; i++)
            {
                if (arrProdTypes[i] == "5")
                {
                    string pName = arrProducts[i];
                    list.Add(pName);
                    
                    using (var helper = new NHibernateHelper(dbPlantFile))
                    {
                        var Session = helper.GetCurrentSession();                        
                        dbProIIStreamData db = new dbProIIStreamData();
                        ProIIStreamData s = db.GetModel(Session, pName);
                        CustomStream LiquidStream = ProIIToDefault.ConvertProIIStreamToCustomStream(s) ;
                        arrStreamData.Add( LiquidStream);
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
            CustomStream vaporStream = ProIIToDefault.ConvertProIIStreamToCustomStream(vapor);
            arrStreamData.Add(vaporStream);
            return list;
        }

        private string GetProIIStreamDataPressure(string streamName)
        {
            string press = "0";
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbProIIStreamData db = new dbProIIStreamData();

                ProIIStreamData data = db.GetModel(Session, streamName);
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

        private void LiquidFlashing(string Rmass, string Cv, string UPStreamPressure, string DownStreamPressure,string przFile)
        {
             string rMass=string.Empty; 
            
            string dirInletValveOpen = tempdir + "InletValveOpen";
            if (!Directory.Exists(dirInletValveOpen))
                Directory.CreateDirectory(dirInletValveOpen);
            CustomStream LiquidStream = new CustomStream();
            if(UpVesselType=="Column")
            {
                LiquidStream=arrStreamData[0];
            }
            else
            {
                foreach (CustomStream cs in arrStreamData)
                {
                    if (cs.ProdType == "2")
                    {
                        LiquidStream = cs;
                    }
                }
            }
            
            string vapor = Guid.NewGuid().ToString().Substring(0, 6);
            string liquid = Guid.NewGuid().ToString().Substring(0, 6);           
            
            double Wliquidvalve = Darcy(Rmass, Cv, UPStreamPressure, DownStreamPressure);
            IFlashCalculateW flashCalc = ProIIFactory.CreateFlashCalculateW(version);
            string content = PRIIFileOperator.getUsableContent(LiquidStream.StreamName, dirInletValveOpen);
            string f = flashCalc.Calculate(content, 1, DownStreamPressure, 5, "0", LiquidStream, vapor, liquid, Wliquidvalve.ToString(), dirInletValveOpen);

            IProIIReader reader = ProIIFactory.CreateReader(version);
            reader.InitProIIReader(f);
            ProIIStreamData proIIStreamData = reader.GetSteamInfo(vapor);
            reader.ReleaseProIIReader();
            CustomStream vaporStream=ProIIToDefault.ConvertProIIStreamToCustomStream(proIIStreamData);
            ReliefLoad = vaporStream.WeightFlow;
            
        }

        private void VaporBreakthrough()
        {       
            
            string dirInletValveOpen = tempdir + "InletValveOpen";
            if (!Directory.Exists(dirInletValveOpen))
                Directory.CreateDirectory(dirInletValveOpen);

            if (UpVesselType == "Column")
            {
                ProIIEqData eq=dicEqData[SelectedVessel];
                int tray=int.Parse(eq.NumberOfTrays);
                string version=ProIIFactory.GetProIIVerison(przFile,dirInletValveOpen);
                //Copy 塔底的最后一块塔板的Vapor
                IProIIReader reader=ProIIFactory.CreateReader(version);
                reader.InitProIIReader(przFile);
                ProIIStreamData proIIStream= reader.CopyStream(EqName,tray,1,1);
                reader.ReleaseProIIReader();
                CustomStream cs=ProIIToDefault.ConvertProIIStreamToCustomStream(proIIStream);
                rMass=cs.BulkDensityAct;
            }
            else if (UpVesselType == "Flash")
            {
                //正常Vapor 的压力
                ProIIEqData eq = dicEqData[SelectedVessel];
                string[] productdata = eq.ProductData.Split(',');
                string[] producttype = eq.ProductStoreData.Split(',');
                int count = producttype.Length;
                string vapor = string.Empty;

                for (int i = 0; i < count; i++)
                {
                    if (producttype[i] == "1")
                    {
                        vapor = productdata[i];
                    }
                }
                using (var helper = new NHibernateHelper(dbPlantFile))
                {
                    var Session = helper.GetCurrentSession();
                    dbProIIStreamData db = new dbProIIStreamData();
                    ProIIStreamData proIIStream = db.GetModel(Session, vapor);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIStream);
                    rMass = cs.BulkDensityAct;
                }
            }
           double reliefLoad = Darcy(rMass, CV, MaxOperatingPressure, ReliefPressure);
           ReliefLoad = reliefLoad.ToString();
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
            rMass = arrStreamData[0].BulkDensityAct;
            string UpStreamPress=arrStreamData[0].Pressure;
            if (SelectedOperatingPhase == "Full Liquid")
            {
                LiquidFlashing(rMass, CV, UpStreamPress, ReliefPressure, przFile);
            }
            else
            {
                VaporBreakthrough();
            }
            
        }
    }
}
