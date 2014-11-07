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
using ReliefProModel.HXs;
using ReliefProDAL.HXs;
using ReliefProBLL;
using ReliefProCommon.Enum;

namespace ReliefProMain.ViewModel
{
    public class HeatExchangerVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { set; get; }
        public string FileName { set; get; }

        public int op=1;
        private ProIIEqData ProIIHX;
        private ObservableCollection<string> _HXTypes;
        public ObservableCollection<string> HXTypes
        {
            get
            {
                return this._HXTypes;
            }
            set
            {
                this._HXTypes = value;
                OnPropertyChanged("HXTypes");
            }
        }
        private string _HXName;
        public string HXName
        {
            get
            {
                return this._HXName;
            }
            set
            {
                this._HXName = value;
                OnPropertyChanged("HXName");
            }
        }
        private string _HXType;
        public string HXType
        {
            get
            {
                return this._HXType;
            }
            set
            {
                this._HXType = value;
                OnPropertyChanged("HXType");
            }
        }
        public HeatExchanger CurrentHX { get; set; }
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
        public string ColdInlet;
        public string HotInlet;
        public string ColdOutlet;
        public string HotOutlet;
        List<string> dicFeeds = new List<string>();
        List<string> dicProducts = new List<string>();
        List<string> dicProductTypes = new List<string>();
        public HeatExchangerVM(string HXName, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            HXTypes = GetHXTypes();
            HXType = HXTypes[0];
            this.HXName = HXName;
            if (!string.IsNullOrEmpty(HXName))
            {
                op = 1;
                HXType = HXTypes[0];
                Feeds = GetStreams(SessionProtectedSystem, false);
                Products = GetStreams(SessionProtectedSystem, true);

                HeatExchangerDAL dbHX = new HeatExchangerDAL();
                CurrentHX = dbHX.GetModel(SessionProtectedSystem);
                if (CurrentHX != null)
                {
                    HXName = CurrentHX.HXName;
                    Duty = CurrentHX.Duty;
                    HXType = CurrentHX.HXType;
                    SourceFileBLL sfbll=new SourceFileBLL(sessionPlant);
                    SourceFileInfo = sfbll.GetSourceFileInfo(CurrentHX.SourceFile);
                }
            }
            else
            {
                op = 0;
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
            if (!string.IsNullOrEmpty(this.HXName))
            {
                MessageBoxResult r = MessageBox.Show("Data was Imported,Are you sure you want to reimport?", "Message Box");
                if (r == MessageBoxResult.No)
                {
                    return;
                }
            }
            SelectEquipmentView v = new SelectEquipmentView();
            SelectEquipmentVM vm = new SelectEquipmentVM("Hx",  SessionPlant);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(vm.SelectedEq))
                {
                    if (op == 1)
                    {
                        op = 2;
                    }
                    //根据设该设备名称来获取对应的物流线信息和其他信息。
                    ProIIEqDataDAL dbEq = new ProIIEqDataDAL();
                    FileName = vm.SelectedFile;
                    ProIIHX = dbEq.GetModel(SessionPlant, FileName, vm.SelectedEq, "Hx");
                    HXName = ProIIHX.EqName;
                    Duty = (double.Parse(ProIIHX.DutyCalc) * 3600);
                    HXType = HXTypes[0];
                    dicFeeds = new List<string>();
                    dicProducts = new List<string>();
                    dicProductTypes = new List<string>();
                    Feeds = new ObservableCollection<CustomStream>();
                    Products = new ObservableCollection<CustomStream>();
                    GetEqFeedProduct(ProIIHX, ref dicFeeds, ref dicProducts, ref dicProductTypes);
                    ProIIStreamDataDAL dbStreamData = new ProIIStreamDataDAL();

                    foreach (string k in dicFeeds)
                    {
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k, FileName);
                        CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                        cstream.IsProduct = false;
                        Feeds.Add(cstream);
                    }
                    for (int i = 0; i < dicProducts.Count; i++)
                    {
                        string k = dicProducts[i];
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k, FileName);
                        CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                        cstream.IsProduct = true;
                        cstream.ProdType = dicProductTypes[i];
                        Products.Add(cstream);
                    }
                    ColorImport = ColorBorder.blue.ToString();
                    op = 0;
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
            if (string.IsNullOrEmpty(HXName))
            {
                MessageBox.Show("You must Import Data first.", "Message Box");
                ColorImport = ColorBorder.red.ToString();
                return;
            }
           
            HeatExchangerDAL dbHX = new HeatExchangerDAL();
            if (op == 0)
            {
                ReImportBLL reimportbll = new ReImportBLL(SessionProtectedSystem);
                reimportbll.DeleteAllData();
                Create();
            }
            else if (op == 1)
            {
                CurrentHX = dbHX.GetModel(SessionProtectedSystem);
                CurrentHX.HXType = HXType;
                dbHX.Update(CurrentHX, SessionProtectedSystem);
                SessionProtectedSystem.Flush();
            }
            else
            {
                MessageBoxResult r = MessageBox.Show("Are you sure to reimport all data?", "Message Box", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    ReImportBLL reimportbll = new ReImportBLL(SessionProtectedSystem);
                    reimportbll.DeleteAllData();
                    Create();
                }
            }

            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }
        private void Create()
        {
            CustomStreamDAL dbCS = new CustomStreamDAL();
            SourceDAL dbsr = new SourceDAL();
            SinkDAL sinkdal = new SinkDAL();
            HeatExchangerDAL dbHX = new HeatExchangerDAL();
            ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
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


            HeatExchanger HX = new HeatExchanger();
            HX.HXName = HXName;
            HX.Duty = Duty;
            HX.HXType = HXType;
            HX.SourceFile = FileName;
            //HX.Temperature = UnitConvert.Convert("K", "C", double.Parse(ProIIHX.TempCalc)); 
            //HX.Pressure = UnitConvert.Convert("KPA", "MPAG", double.Parse(ProIIHX.PressCalc)); 
            HX.FirstFeed = ProIIHX.FirstFeed;
            HX.FirstProduct = ProIIHX.FirstProduct;
            HX.LastFeed = ProIIHX.LastFeed;
            HX.LastProduct = ProIIHX.LastProduct;

            //计算hx的coldinlet,hotinlet,coldoutlet,hotoutlet
             

            string[] firstfeeds = this.ProIIHX.FirstFeed.Split(',');
            string[] lastfeeds = this.ProIIHX.LastFeed.Split(',');

            string[] firstproducts = this.ProIIHX.FirstProduct.Split(',');
            string[] lastproducts = this.ProIIHX.LastProduct.Split(',');

            if (Products.Count == 1)
            {
                //说明单侧进单侧出
                HX.ColdInlet = this.ProIIHX.FeedData;
                HX.ColdOutlet = this.ProIIHX.ProductData;
            }
            else
            {
                int startfeed1 = int.Parse(firstfeeds[0]);
                int endfeed1 = int.Parse(firstfeeds[0]);
                int startproduct1 = int.Parse(firstproducts[0]);
                int endproduct1 = int.Parse(firstproducts[0]);

                int startfeed2 = int.Parse(firstfeeds[1]);
                int endfeed2 = int.Parse(firstfeeds[1]);
                int startproduct2 = int.Parse(firstproducts[1]);
                int endproduct2 = int.Parse(firstproducts[1]);

                if (startfeed2 > endfeed2)
                {
                    //说明也是单侧进单侧出
                    HX.ColdInlet = this.ProIIHX.FeedData;
                    HX.ColdOutlet = this.ProIIHX.ProductData;
                }
                else
                {
                    //说明双侧进，双侧出。 我们需要判断那个是冷侧进，那个是热侧进
                    int feed1count = endfeed1 - startfeed1 + 1;
                    int feed2count = endfeed2 - startfeed2 + 1;
                    if (feed1count == 1)
                    {
                        CustomStream feed1 = Feeds[0];
                        CustomStream product1 = Products[0];
                        if (feed1.Temperature < product1.Temperature)
                        {
                            //冷侧进
                            HX.ColdInlet = feed1.StreamName;
                            HX.ColdOutlet = product1.StreamName;

                            //热侧
                            StringBuilder sb = new StringBuilder();
                            for (int i = startfeed2; i <= endfeed2; i++)
                            {
                                sb.Append(",").Append(Feeds[i - 1].StreamName);
                            }
                            HX.HotInlet = sb.ToString().Substring(1);
                            HX.HotOutlet = Products[1].StreamName;
                        }
                        else
                        {                            
                            //冷测
                            StringBuilder sb = new StringBuilder();
                            for (int i = startfeed2; i <= endfeed2; i++)
                            {
                                sb.Append(",").Append(Feeds[i - 1].StreamName);
                            }
                            HX.ColdInlet = sb.ToString().Substring(1);
                            HX.ColdOutlet = Products[1].StreamName;

                            //热侧进
                            HX.HotInlet = feed1.StreamName;
                            HX.HotOutlet = product1.StreamName;
                        }
                    }
                    else if(feed2count==1)
                    {
                        CustomStream feed2 = Feeds[Feeds.Count-1];
                        CustomStream product2 = Products[1];
                        if (feed2.Temperature < product2.Temperature)
                        {
                            //冷侧进
                            HX.ColdInlet = feed2.StreamName;
                            HX.ColdOutlet = product2.StreamName;

                            StringBuilder sb = new StringBuilder();
                            for (int i = startfeed1; i <= endfeed1; i++)
                            {
                                sb.Append(",").Append(Feeds[i - 1].StreamName);
                            }
                            HX.HotInlet = sb.ToString().Substring(1);
                            HX.HotOutlet = Products[0].StreamName;
                        }
                        else
                        {
                            //热侧进

                            StringBuilder sb = new StringBuilder();
                            for (int i = startfeed1; i <= endfeed1; i++)
                            {
                                sb.Append(",").Append(Feeds[i - 1].StreamName);
                            }
                            HX.ColdInlet = sb.ToString().Substring(1);
                            HX.ColdOutlet = Products[0].StreamName;

                            HX.HotInlet = feed2.StreamName;
                            HX.HotOutlet = product2.StreamName;
                        }
                    }

                }

            }
            ColdInlet = HX.ColdInlet;
            ColdOutlet = HX.ColdOutlet;
            HotInlet = HX.HotInlet;
            HotOutlet = HX.HotOutlet;

            dbHX.Add(HX, SessionProtectedSystem);


            ProtectedSystem ps = new ProtectedSystem();
            ps.PSType = 4;
            psDAL.Add(ps, SessionProtectedSystem);

            SourceFileDAL sfdal = new SourceFileDAL();
            SourceFileInfo = sfdal.GetModel(HX.SourceFile, SessionPlant);
            SessionProtectedSystem.Flush();
        }
        private ObservableCollection<string> GetHXTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Shell-Tube");
            list.Add("Air cooled");
            return list;
        }
    }
}
