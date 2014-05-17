using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using NHibernate;
using ReliefProMain.View;

namespace ReliefProMain.ViewModel
{
    public class DrumVM:ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        private string przFile;
        private ProIIEqData ProIIDrum;
        private string _DrumName;
        public string DrumName
        {
            get
            {
                return this._DrumName;
            }
            set
            {
                this._DrumName = value;
                OnPropertyChanged("DrumName");
            }
        }
        private string _DrumType;
        public string DrumType
        {
            get
            {
                return this._DrumType;
            }
            set
            {
                this._DrumType = value;
                OnPropertyChanged("DrumType");
            }
        }
        public ReliefProModel.Drum.Drum CurrentDrum { get; set; }
        private string _Duty;
        public string Duty
        {
            get
            {
                return this._Duty;
            }
            set
            {
                this._Duty = value;
                OnPropertyChanged("Duty");
            }
        }

        private string _Temperature;
        public string Temperature
        {
            get
            {
                return this._Temperature;
            }
            set
            {
                this._Temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        private string _Pressure;
        public string Pressure
        {
            get
            {
                return this._Pressure;
            }
            set
            {
                this._Pressure = value;
                OnPropertyChanged("Pressure");
            }
        }

        private ObservableCollection<string> _DrumTypes;
        public ObservableCollection<string> DrumTypes
        {
            get { return _DrumTypes; }
            set
            {
                _DrumTypes = value;
                OnPropertyChanged("DrumTypes");
            }
        }

        private ObservableCollection<CustomStream> _Feeds;
        public ObservableCollection<CustomStream> Feeds
        {
            get { return _Feeds; }
            set
            {
                _Feeds = value;
                OnPropertyChanged("Feeds");
            }
        }
        private ObservableCollection<CustomStream> _Products;
        public ObservableCollection<CustomStream> Products
        {
            get { return _Products; }
            set
            {
                _Products = value;
                OnPropertyChanged("Products");
            }
        }
        List<string> dicFeeds = new List<string>();
        List<string> dicProducts = new List<string>();
        List<string> dicProductTypes = new List<string>();
        public DrumVM(string drumName, string dbPSFile, string dbPFile)
        {
            DrumTypes = GetDrumTypes();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            DrumName = drumName;
            if (!string.IsNullOrEmpty(DrumName))
            {
                using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                {
                    var Session = helper.GetCurrentSession();
                    Feeds = GetStreams(Session, false);
                    Products = GetStreams(Session, true);

                    dbDrum dbdrum=new dbDrum();
                    CurrentDrum=dbdrum.GetModel(Session);
                    if (CurrentDrum != null)
                    {
                        DrumName = CurrentDrum.DrumName;
                        Duty = CurrentDrum.Duty;
                        Pressure = CurrentDrum.Pressure;
                        Temperature = CurrentDrum.Temperature;
                        DrumType = CurrentDrum.DrumType;
                    }
                   
                }
            }
            else
            {
                
            }
        }



        private ICommand _ImportCommand;
        public ICommand ImportCommand
        {
            get
            {
                if (_ImportCommand == null)
                {
                    _ImportCommand = new RelayCommand(Import);

                }
                return _ImportCommand;
            }
        }
        private void Import(object obj)
        {
            SelectEquipmentView v = new SelectEquipmentView();
            SelectEquipmentVM vm = new SelectEquipmentVM("Flash", dbProtectedSystemFile, dbPlantFile);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(vm.SelectedEq))
                {
                    //根据设该设备名称来获取对应的物流线信息和其他信息。

                    using (var helper = new NHibernateHelper(dbPlantFile))
                    {
                        var Session = helper.GetCurrentSession();
                        dbProIIEqData dbEq = new dbProIIEqData();
                        przFile = vm.SelectedFile + ".prz";
                        ProIIDrum = dbEq.GetModel(Session, przFile, vm.SelectedEq, "Flash");
                        DrumName = ProIIDrum.EqName;
                        Duty = ProIIDrum.DutyCalc;
                        Temperature = ProIIDrum.TempCalc;
                        Pressure = ProIIDrum.PressCalc;
                        Duty = ProIIDrum.DutyCalc;


                        Feeds = new ObservableCollection<CustomStream>();
                        Products = new ObservableCollection<CustomStream>();
                        GetEqFeedProduct(ProIIDrum, ref dicFeeds, ref dicProducts,ref dicProductTypes);
                        dbProIIStreamData dbStreamData = new dbProIIStreamData();
                        
                        foreach (string k in dicFeeds)
                        {
                            ProIIStreamData d = dbStreamData.GetModel(Session, k);
                            CustomStream cstream = ConvertProIIStreamToCustomStream(d);
                            cstream.IsProduct = false;
                            Feeds.Add(cstream);
                        }
                        for(int i=0;i<dicProducts.Count;i++)
                        {
                            string k=dicProducts[i];
                            ProIIStreamData d = dbStreamData.GetModel(Session, k);
                            CustomStream cstream = ConvertProIIStreamToCustomStream(d);
                            cstream.IsProduct = true;
                            cstream.ProdType=dicProductTypes[i];
                            Products.Add(cstream);
                        }

                    }


                }
            }
        }

        public void GetEqFeedProduct(ProIIEqData data, ref List<string> dicFeeds, ref List<string> dicProducts,ref List<string> dicProductTypes)
        {
            string feeddata = data.FeedData;
            string productdata = data.ProductData;
            string producttype = data.ProductStoreData;
            string[] arrFeeds = feeddata.Split(',');
            string[] arrProducts = productdata.Split(',');
            string[] arrProductTypes = producttype.Split(',');
            for (int i = 0; i < arrFeeds.Length; i++)
            {
                dicFeeds.Add(arrFeeds[i]);
            }
            for (int i = 0; i < arrProducts.Length; i++)
            {
                dicProducts.Add(arrProducts[i]);
            }
            for (int i = 0; i < arrProductTypes.Length; i++)
            {
                dicProductTypes.Add(arrProductTypes[i]);
            }

        }

        private CustomStream ConvertProIIStreamToCustomStream(ProIIStreamData s)
        {
            CustomStream cs = new CustomStream();
            cs.StreamName = s.StreamName;
            cs.BulkCP = s.BulkCP;
            cs.BulkCPCVRatio = cs.BulkCPCVRatio;
            cs.BulkDensityAct = s.BulkDensityAct;
            cs.BulkMwOfPhase = s.BulkMwOfPhase;
            cs.BulkSurfTension = s.BulkSurfTension;
            cs.BulkThermalCond = s.BulkThermalCond;
            cs.BulkViscosity = s.BulkViscosity;
            cs.CompIn = s.CompIn;
            cs.Componentid = s.Componentid;
            cs.InertWeightEnthalpy = s.InertWeightEnthalpy;
            cs.InertWeightRate = s.InertWeightRate;
            cs.Pressure = s.Pressure;
            cs.ProdType = s.ProdType;
            // cs.SpEnthalpy = s.SpEnthalpy;
            cs.Temperature = s.Temperature;
            cs.TotalComposition = s.TotalComposition;
            cs.TotalMolarEnthalpy = s.TotalMolarEnthalpy;
            cs.TotalMolarRate = s.TotalMolarRate;
            cs.Tray = s.Tray;
            cs.VaporFraction = s.VaporFraction;
            cs.VaporZFmKVal = s.VaporZFmKVal;
            //cs.WeightFlow = s.WeightFlow;

            return cs;


        }
        private ObservableCollection<CustomStream> GetStreams(ISession Session, bool IsProduct)
        {
            ObservableCollection<CustomStream> list = new ObservableCollection<CustomStream>();
            dbCustomStream db = new dbCustomStream();
            IList<CustomStream> lt = db.GetAllList(Session, IsProduct);
            foreach (CustomStream c in lt)
            {
                list.Add(c);
            }

            return list;
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
        public void Save(object obj)
        {
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
               
                dbCustomStream dbCS = new dbCustomStream();
                dbSource dbsr = new dbSource();
                foreach (CustomStream cs in Feeds)
                {
                    Source sr = new Source();
                    sr.MaxPossiblePressure = cs.Pressure;
                    sr.StreamName = cs.StreamName;
                    sr.SourceType = "Compressor(Motor)";
                    dbsr.Add(sr, Session);


                    dbCS.Add(cs, Session);
                }


                foreach (CustomStream cs in Products)
                {                   
                    dbCS.Add(cs, Session);
                }

                dbDrum dbdrum = new dbDrum();
                ReliefProModel.Drum.Drum drum = new ReliefProModel.Drum.Drum();
                drum.DrumName = DrumName;
                drum.DrumType = DrumType;
                drum.PrzFile = przFile;
                drum.Pressure = Pressure;
                drum.Temperature = Temperature;
                dbdrum.Add(drum, Session);


                Session.Flush();
            }

            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private ObservableCollection<string> GetDrumTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("General Seperator");
            list.Add("Flashing Drum");
            return list;
        }
    }
}
