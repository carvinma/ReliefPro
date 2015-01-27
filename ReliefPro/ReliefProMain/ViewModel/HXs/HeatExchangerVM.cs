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
using System.IO;
using ReliefProCommon.CommonLib;

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
        private string _HXType_Color;
        public string HXType_Color
        {
            get
            {
                return this._HXType_Color;
            }
            set
            {
                this._HXType_Color = value;
                OnPropertyChanged("HXType_Color");
            }
        }
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
        private string _TubeFeedStreams;
        public string TubeFeedStreams
        {
            get
            {
                return this._TubeFeedStreams;
            }
            set
            {
                this._TubeFeedStreams = value;
                if (value == _ShellFeedStreams && !string.IsNullOrEmpty(value) || string.IsNullOrEmpty(_ShellFeedStreams) && !string.IsNullOrEmpty(value))
                {
                    if (FeedStreams[1] == value)
                    {
                        if (FeedStreams.Count == 2)
                        {
                            ShellFeedStreams = string.Empty;
                        }
                        else
                        {
                            ShellFeedStreams = FeedStreams[2];
                        }
                    }
                    else
                    {
                        ShellFeedStreams = FeedStreams[1];
                    }
                }
               
                OnPropertyChanged("TubeFeedStreams");
            }
        }
        private string _ShellFeedStreams;
        public string ShellFeedStreams
        {
            get
            {
                return this._ShellFeedStreams;
            }
            set
            {
                this._ShellFeedStreams = value;
                if (value == _TubeFeedStreams && !string.IsNullOrEmpty(value) || string.IsNullOrEmpty(_TubeFeedStreams) && !string.IsNullOrEmpty(value))
                {
                    if (FeedStreams[1] == value)
                    {
                        if (FeedStreams.Count == 2)
                        {
                            TubeFeedStreams = string.Empty;
                        }
                        else
                        {
                            TubeFeedStreams = FeedStreams[2];
                        }
                    }
                    else
                    {
                        TubeFeedStreams = FeedStreams[1];
                    }
                }
                OnPropertyChanged("ShellFeedStreams");
            }
        }
        private string _TubeFeedStreams_Color;
        public string TubeFeedStreams_Color
        {
            get
            {
                return this._TubeFeedStreams_Color;
            }
            set
            {
                this._TubeFeedStreams_Color = value;
                OnPropertyChanged("TubeFeedStreams_Color");
            }
        }
        private string _ShellFeedStreams_Color;
        public string ShellFeedStreams_Color
        {
            get
            {
                return this._ShellFeedStreams_Color;
            }
            set
            {
                this._ShellFeedStreams_Color = value;
                OnPropertyChanged("ShellFeedStreams_Color");
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

        private ObservableCollection<string> _FeedStreams;
        public ObservableCollection<string> FeedStreams
        {
            get { return _FeedStreams; }
            set
            {
                _FeedStreams = value;
                OnPropertyChanged("FeedStreams");
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
        public HeatExchanger CurrentHX { get; set; }
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
                    ColdInlet = CurrentHX.ColdInlet;
                    ColdOutlet = CurrentHX.ColdOutlet;
                    HotInlet = CurrentHX.HotInlet;
                    HotOutlet = CurrentHX.HotOutlet;
                    FeedStreams = GetFeedStreams();
                    TubeFeedStreams = CurrentHX.TubeFeedStreams;
                    ShellFeedStreams = CurrentHX.ShellFeedStreams;
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
                    //计算hx的coldinlet,hotinlet,coldoutlet,hotoutlet
                    ColdInlet = string.Empty;
                    ColdOutlet = string.Empty;
                    HotInlet = string.Empty;
                    HotOutlet = string.Empty;
                    GetColdHotStream();
                    FeedStreams = GetFeedStreams();
                    TubeFeedStreams = string.Empty;
                    ShellFeedStreams = string.Empty;
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
            if (HXType == "Air cooled")
            {
                if (TubeFeedStreams == string.Empty)
                {
                    MessageBox.Show("You must choose Tube Feed Streams.", "Message Box");
                    return;
                }
            }



            SourceFileDAL sfdal = new SourceFileDAL();
            SourceFileInfo = sfdal.GetModel(FileName, SessionPlant);
            if (HXType == "Shell-Tube")
            {
                if (Products.Count == 1)
                {
                    if (string.IsNullOrEmpty(TubeFeedStreams) && string.IsNullOrEmpty(ShellFeedStreams))
                    {
                        MessageBox.Show("TubeFeedStreams or ShellFeedStreams must be selected.", "Message Box");
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(TubeFeedStreams))
                    {
                        MessageBox.Show("TubeFeedStreams  must be selected.", "Message Box");
                        return;
                    }
                    if (string.IsNullOrEmpty(ShellFeedStreams))
                    {
                        MessageBox.Show("ShellFeedStreams  must be selected.", "Message Box");
                        return;
                    }
                }
            }
            else
            {
                if (Products.Count > 1)
                {
                    MessageBox.Show("Air Cooled Hx must be single product hx.", "Message Box");
                    return;
                }
                if(Feeds.Count==1)
                {
                    if(Feeds[0].Temperature<Products[0].Temperature)
                    {
                        MessageBox.Show("Air Cooled feed temperature must be greater than product temperature.", "Message Box");
                        return ;
                    }
                }
                else
                {
                    ProIIStreamDataDAL proiistreamdal = new ProIIStreamDataDAL();
                    CustomStreamDAL csdal=new CustomStreamDAL();
                    List<CustomStream> lstFeed=new List<CustomStream>();
                    List<string> lstFeedName=new List<string>();
                    string[] hotinlets=HotInlet.Split(',');
                    lstFeedName=hotinlets.ToList();
                    foreach(string s in lstFeedName)
                    {
                        if (s == string.Empty)
                        {
                            MessageBox.Show("Air Cooled feed temperature must be greater than product temperature.", "Message Box");
                            return;
                        }
                        ProIIStreamData data = proiistreamdal.GetModel(SessionPlant, s, SourceFileInfo.FileName);
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
                        lstFeed.Add(cs);
                    }

                    int ErrorType = 0;
                    string tempdir = DirProtectedSystem + @"\temp";
                    if (Directory.Exists(tempdir))
                    {
                        Directory.Delete(tempdir, true);
                    }
                    Directory.CreateDirectory(tempdir);
                    string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
                    PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                    string mixDir = DirProtectedSystem + @"\temp\ColdMixed";
                    CustomStream mixCS = ProIIMethod.MixStream(SourceFileInfo.FileVersion, lstFeedName, lstFeed, tempdir, mixDir, ref ErrorType);
                    
                    if (mixCS.Temperature < Products[0].Temperature)
                    {
                        MessageBox.Show("Air Cooled feed temperature must be greater than product temperature.", "Message Box");
                        return;
                    }
                }
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
                CurrentHX.ShellFeedStreams = ShellFeedStreams;
                CurrentHX.TubeFeedStreams = TubeFeedStreams;
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
            SourceFileInfo = sfdal.GetModel(CurrentHX.SourceFile, SessionPlant);
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }
        private void Create()
        {
            CustomStreamDAL csdal = new CustomStreamDAL();
            SourceDAL dbsr = new SourceDAL();
            SinkDAL sinkdal = new SinkDAL();
            HeatExchangerDAL dbHX = new HeatExchangerDAL();
            ProtectedSystemDAL psDAL = new ProtectedSystemDAL();

            List<CustomStream> lstColdFeed = new List<CustomStream>();
            List<CustomStream> lstHotFeed = new List<CustomStream>();
            CustomStream ColdProduct = new CustomStream();
            CustomStream HotProduct = new CustomStream();

            string[] arrColdFeed = ColdInlet.Split(',');
            string[] arrHotFeed = HotInlet.Split(',');
            foreach (CustomStream cs in Feeds)
            {
                Source sr = new Source();
                sr.MaxPossiblePressure = cs.Pressure;
                sr.StreamName = cs.StreamName;
                sr.SourceType = "Pump(Motor)";
                sr.SourceName = cs.StreamName + "_Source";
                dbsr.Add(sr, SessionProtectedSystem);
                csdal.Add(cs, SessionProtectedSystem);
                if (cs.StreamName == ColdInlet || arrColdFeed.Contains(cs.StreamName))
                {
                    lstColdFeed.Add(cs);
                }
                else if (cs.StreamName == HotInlet || arrHotFeed.Contains(cs.StreamName))
                {
                    lstHotFeed.Add(cs);
                }


            }

            foreach (CustomStream cs in Products)
            {
                Sink sink = new Sink();
                sink.MaxPossiblePressure = cs.Pressure;
                sink.StreamName = cs.StreamName;
                sink.SinkName = cs.StreamName + "_Sink";
                sink.SinkType = "Pump(Motor)";
                sinkdal.Add(sink, SessionProtectedSystem);
                csdal.Add(cs, SessionProtectedSystem);
                if (cs.StreamName == ColdOutlet)
                {
                    ColdProduct = cs;
                }
                else
                {
                    HotProduct = cs;
                }
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
           

            HX.ColdInlet= ColdInlet;
            HX.ColdOutlet =ColdOutlet;
            HX.HotInlet=HotInlet;
            HX.HotOutlet=HotOutlet;

            //温度升高的是冷侧，降低的是热侧
            if (Products.Count == 2)
            {
                if (ColdInlet.Contains(",") && HotInlet.Contains(","))
                {
                    //只需要针对coldinlet 做mix
                    int errorType=0;
                    string sourceDir=DirProtectedSystem+@"\temp";
                    string mixDir=DirProtectedSystem+@"\temp\ColdMixed";
                    CustomStream mixFeed=ProIIMethod.MixStream(SourceFileInfo.FileVersion,arrColdFeed.ToList(),lstColdFeed,sourceDir,mixDir,ref errorType);
                    if (mixFeed.Temperature > ColdProduct.Temperature)
                    {
                        HX.ColdInlet = HotInlet;
                        HX.ColdOutlet = HotOutlet;
                        HX.HotInlet = ColdInlet;
                        HX.HotOutlet = ColdOutlet;
                    }
                }
                else if (!ColdInlet.Contains(",") )
                {
                    if (lstColdFeed[0].Temperature > ColdProduct.Temperature)
                    {
                        HX.ColdInlet = HotInlet ;
                        HX.ColdOutlet = HotOutlet;
                        HX.HotInlet = ColdInlet;
                        HX.HotOutlet = ColdOutlet;
                    }
                }
            }

           
            HX.TubeFeedStreams = TubeFeedStreams;
            HX.ShellFeedStreams = ShellFeedStreams;
            dbHX.Add(HX, SessionProtectedSystem);
            CurrentHX = HX;
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
        private ObservableCollection<string> GetFeedStreams()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("");
            if (!string.IsNullOrEmpty(ColdInlet))
            {
                list.Add(ColdInlet);
            }
            if (!string.IsNullOrEmpty(HotInlet))
            {
                list.Add(HotInlet);
            }
            return list;
        }

        /*
        private void GetColdHotStream()
        //{
        //    string[] firstfeeds = this.ProIIHX.FirstFeed.Split(',');
        //    string[] lastfeeds = this.ProIIHX.LastFeed.Split(',');

        //    string[] firstproducts = this.ProIIHX.FirstProduct.Split(',');
        //    string[] lastproducts = this.ProIIHX.LastProduct.Split(',');

        //    if (Products.Count == 1)
        //    {
        //        //说明单侧进单侧出
        //        ColdInlet = this.ProIIHX.FeedData;
        //        ColdOutlet = this.ProIIHX.ProductData;
        //    }
        //    else
        //    {
        //        int startfeed1 = int.Parse(firstfeeds[0]);
        //        int endfeed1 = int.Parse(firstfeeds[0]);
        //        int startproduct1 = int.Parse(firstproducts[0]);
        //        int endproduct1 = int.Parse(firstproducts[0]);

        //        int startfeed2 = int.Parse(firstfeeds[1]);
        //        int endfeed2 = int.Parse(firstfeeds[1]);
        //        int startproduct2 = int.Parse(firstproducts[1]);
        //        int endproduct2 = int.Parse(firstproducts[1]);

        //        if (startfeed2 > endfeed2)
        //        {
        //            //说明也是单侧进单侧出
        //            ColdInlet = this.ProIIHX.FeedData;
        //            ColdOutlet = this.ProIIHX.ProductData;
        //        }
        //        else
        //        {
        //            //说明双侧进，双侧出。 我们需要判断那个是冷侧进，那个是热侧进
        //            int feed1count = endfeed1 - startfeed1 + 1;
        //            int feed2count = endfeed2 - startfeed2 + 1;
        //            if (feed1count == 1)
        //            {
        //                CustomStream feed1 = Feeds[0];
        //                CustomStream product1 = Products[0];
        //                if (feed1.Temperature < product1.Temperature)
        //                {
        //                    //冷侧进
        //                    ColdInlet = feed1.StreamName;
        //                    ColdOutlet = product1.StreamName;

        //                    //热侧
        //                    StringBuilder sb = new StringBuilder();
        //                    for (int i = startfeed2; i <= endfeed2; i++)
        //                    {
        //                        sb.Append(",").Append(Feeds[i - 1].StreamName);
        //                    }
        //                    HotInlet = sb.ToString().Substring(1);
        //                    HotOutlet = Products[1].StreamName;
        //                }
        //                else
        //                {
        //                    //冷测
        //                    StringBuilder sb = new StringBuilder();
        //                    for (int i = startfeed2; i <= endfeed2; i++)
        //                    {
        //                        sb.Append(",").Append(Feeds[i - 1].StreamName);
        //                    }
        //                    ColdInlet = sb.ToString().Substring(1);
        //                    ColdOutlet = Products[1].StreamName;

        //                    //热侧进
        //                    HotInlet = feed1.StreamName;
        //                    HotOutlet = product1.StreamName;
        //                }
        //            }
        //            else if (feed2count == 1)
        //            {
        //                CustomStream feed2 = Feeds[Feeds.Count - 1];
        //                CustomStream product2 = Products[1];
        //                if (feed2.Temperature < product2.Temperature)
        //                {
        //                    //冷侧进
        //                    ColdInlet = feed2.StreamName;
        //                    ColdOutlet = product2.StreamName;

        //                    StringBuilder sb = new StringBuilder();
        //                    for (int i = startfeed1; i <= endfeed1; i++)
        //                    {
        //                        sb.Append(",").Append(Feeds[i - 1].StreamName);
        //                    }
        //                    HotInlet = sb.ToString().Substring(1);
        //                    HotOutlet = Products[0].StreamName;
        //                }
        //                else
        //                {
        //                    //热侧进

        //                    StringBuilder sb = new StringBuilder();
        //                    for (int i = startfeed1; i <= endfeed1; i++)
        //                    {
        //                        sb.Append(",").Append(Feeds[i - 1].StreamName);
        //                    }
        //                    ColdInlet = sb.ToString().Substring(1);
        //                    ColdOutlet = Products[0].StreamName;

        //                    HotInlet = feed2.StreamName;
        //                    HotOutlet = product2.StreamName;
        //                }
        //            }

        //        }

        //    }
        //}
         
        */
        private void GetColdHotStream()
        {
            string dir = DirPlant + @"\" + FileName.Substring(0,FileName.Length-4);
            string[] files = Directory.GetFiles(dir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);

            int i=1;
            for (i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if((line+",").Contains("HX   UID="+HXName+","))
                {
                    break;
                }
            }
            i++;
            string row = lines[i].Trim();
            if (row.Substring(0,3)=="HOT")
            {
                GetHXStreamInfo(row, ref HotInlet, ref HotOutlet);
            }
            else if (row.Substring(0,4)=="COLD")
            {
                GetHXStreamInfo(row, ref ColdInlet, ref ColdOutlet);
            }

            i++;
            row = lines[i].Trim();
            if (row.Substring(0, 3) == "HOT")
            {
                GetHXStreamInfo(row, ref HotInlet, ref HotOutlet);
            }
            else if (row.Substring(0, 4) == "COLD")
            {
                GetHXStreamInfo(row, ref ColdInlet, ref ColdOutlet);
            }
        }

        private void GetHXStreamInfo(string line, ref string feed, ref string product)
        {
            int begin = line.IndexOf("FEED=");
            int end1 = line.IndexOf(", M=");
            feed = line.Substring(begin + 5, end1 - begin - 5);
            
            int end2 = line.IndexOf(", DP=") - 1;
            if (end2 == -2)
            {
                end2 = line.Length - 1;
            }
            product = line.Substring(end1 + 4, end2 - end1 - 3);
        }

    }
}
