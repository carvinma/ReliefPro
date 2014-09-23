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
using UOMLib;
using ReliefProModel.Drums;
using ReliefProDAL.Drums;
using ReliefProCommon.Enum;


namespace ReliefProMain.ViewModel
{
    public class DrumVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private SourceFile SourceFileinfo;
        private string SourceFileName;
        private ProIIEqData ProIIDrum;
        public SourceFile SourceFileInfo;

        private string _ColorImport;
        public string ColorImport
        {
            get
            {
                return this._ColorImport;
            }
            set
            {
                this._ColorImport = value;
                OnPropertyChanged("ColorImport");
            }
        }
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
        public Drum CurrentDrum { get; set; }
        private double _Duty;
        public double Duty
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

        private double _Temperature;
        public double Temperature
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

        private double _Pressure;
        public double Pressure
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
        public DrumVM(string drumName, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            DrumName = drumName;
            if (!string.IsNullOrEmpty(DrumName))
            {
                Feeds = GetStreams(SessionProtectedSystem, false);
                Products = GetStreams(SessionProtectedSystem, true);

                DrumDAL dbdrum = new DrumDAL();
                CurrentDrum = dbdrum.GetModel(SessionProtectedSystem);
                if (CurrentDrum != null)
                {
                    DrumName = CurrentDrum.DrumName;
                    Duty = CurrentDrum.Duty;
                    Pressure = CurrentDrum.Pressure;
                    Temperature = CurrentDrum.Temperature;
                    DrumType = CurrentDrum.DrumType;

                    SourceFileDAL sfdal = new SourceFileDAL();
                    SourceFileinfo = sfdal.GetModel(CurrentDrum.SourceFile, SessionPlant);
                }
                ColorImport = ColorBorder.blue.ToString();

            }
            else
            {
                ColorImport = ColorBorder.red.ToString();
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
            SelectEquipmentVM vm = new SelectEquipmentVM("Flash",  SessionPlant);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(vm.SelectedEq))
                {
                    //根据设该设备名称来获取对应的物流线信息和其他信息。
                    SourceFileInfo = vm.SourceFileInfo;
                    ProIIEqDataDAL dbEq = new ProIIEqDataDAL();
                    SourceFileName = vm.SelectedFile;
                    ProIIDrum = dbEq.GetModel(SessionPlant, SourceFileName, vm.SelectedEq, "Flash");
                    DrumName = ProIIDrum.EqName;
                    Duty = (double.Parse(ProIIDrum.DutyCalc) * 3600);
                    Temperature = UnitConvert.Convert("K", "C", double.Parse(ProIIDrum.TempCalc));
                    Pressure = UnitConvert.Convert("KPA", "MPAG", double.Parse(ProIIDrum.PressCalc));

                    DrumType = "Flashing Drum";
                    if (Duty == 0)
                    {
                        DrumType = "General Seperator";
                    }

                    dicFeeds = new List<string>();
                    dicProducts = new List<string>();
                    dicProductTypes = new List<string>();
                    Feeds = new ObservableCollection<CustomStream>();
                    Products = new ObservableCollection<CustomStream>();
                    GetEqFeedProduct(ProIIDrum, ref dicFeeds, ref dicProducts, ref dicProductTypes);
                    ProIIStreamDataDAL dbStreamData = new ProIIStreamDataDAL();

                    foreach (string k in dicFeeds)
                    {
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k, SourceFileName);
                        CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                        cstream.IsProduct = false;
                        Feeds.Add(cstream);
                    }
                    for (int i = 0; i < dicProducts.Count; i++)
                    {
                        string k = dicProducts[i];
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k, SourceFileName);
                        CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                        cstream.IsProduct = true;
                        cstream.ProdType = dicProductTypes[i];
                        Products.Add(cstream);
                    }
                    ColorImport = ColorBorder.blue.ToString();
                    
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

        
        private ObservableCollection<CustomStream> GetStreams(ISession Session, bool IsProduct)
        {
            ObservableCollection<CustomStream> list = new ObservableCollection<CustomStream>();
            CustomStreamDAL db = new CustomStreamDAL();
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
            if (string.IsNullOrEmpty(DrumName))
            {
                MessageBox.Show("You must Import Data first.", "Message Box");
                ColorImport = ColorBorder.red.ToString();
                return;
            }
            CustomStreamDAL dbCS = new CustomStreamDAL();
            SourceDAL dbsr = new SourceDAL();
            SinkDAL sinkdal = new SinkDAL();
            DrumDAL dbdrum = new DrumDAL();
            if (CurrentDrum==null|| CurrentDrum.ID == 0)
            {
                foreach (CustomStream cs in Feeds)
                {
                    Source sr = new Source();
                    sr.MaxPossiblePressure = cs.Pressure;
                    sr.StreamName = cs.StreamName;
                    sr.SourceType = "Pump(Motor)";
                    sr.SourceName = cs.StreamName + "_Source";
                    dbsr.Add(sr, SessionProtectedSystem);
                    dbCS.Add(cs, SessionProtectedSystem);
                }


                foreach (CustomStream cs in Products)
                {
                    Sink sink = new Sink();
                    sink.MaxPossiblePressure = cs.Pressure;
                    sink.StreamName = cs.StreamName;
                    sink.SinkName = cs.StreamName + "_Sink";
                    sink.SinkType = "Pump(Motor)";
                    sinkdal.Add(sink, SessionProtectedSystem);
                    dbCS.Add(cs, SessionProtectedSystem);
                }

                
                Drum drum = new Drum();
                drum.DrumName = DrumName;
                drum.Duty = Duty;
                drum.DrumType = DrumType;
                drum.SourceFile = SourceFileName;
                drum.Pressure = Pressure;
                drum.Temperature = Temperature;
                dbdrum.Add(drum, SessionProtectedSystem);

                ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
                ProtectedSystem ps = new ProtectedSystem();
                ps.PSType = 2;
                psDAL.Add(ps, SessionProtectedSystem);

                SourceFileDAL sfdal = new SourceFileDAL();
                SourceFileInfo = sfdal.GetModel(drum.SourceFile, SessionPlant);
            }
            else
            {
                CurrentDrum.DrumType = DrumType;
                dbdrum.Update(CurrentDrum, SessionProtectedSystem);
                SessionProtectedSystem.Flush();
            }


            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

       
    }
}
