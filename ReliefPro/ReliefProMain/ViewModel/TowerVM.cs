/*
 * 塔基础信息界面
 * 该文件主要是实现塔基础信息的导入与信息展示。
 * 信息包括了塔的名称，塔层数以及塔的feed,product,condenser,reboiler
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProCommon.CommonLib;
using NHibernate;
using ReliefProMain.View;

namespace ReliefProMain.ViewModel
{
   public class TowerVM:ViewModelBase
    {
       public ISession SessionPlant { set; get; }
        public ISession SessionProtectedSystem { set; get; }
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }        
        public SourceFile SourceFileInfo { set; get; }
        public string SourceFileName { set; get; }
        private string _TowerName;
        public string TowerName
        {
            get
            {
                return this._TowerName;
            }
            set
            {
                this._TowerName = value;
                OnPropertyChanged("TowerName");
            }
        }
        private string _TowerType;
        public string TowerType
        {
            get
            {
                return this._TowerType;
            }
            set
            {
                this._TowerType = value;
                OnPropertyChanged("TowerType");
            }
        }
        private string _Desciption;
        public string Desciption
        {
            get
            {
                return this._Desciption;
            }
            set
            {
                this._Desciption = value;
                OnPropertyChanged("Desciption");
            }
        }
        private int _StageNumber;
        public int StageNumber
        {
            get
            {
                return this._StageNumber;
            }
            set
            {
                this._StageNumber = value;
                OnPropertyChanged("StageNumber");
            }
        }
        private ObservableCollection<string> _TowerTypes;
        public ObservableCollection<string> TowerTypes
        {
            get
            {
                return this._TowerTypes;
            }
            set
            {
                this._TowerTypes = value;
                OnPropertyChanged("TowerTypes");
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




        TowerDAL dbtower;
        Tower tower;
        int op = 1;
        public TowerVM(string towerName, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            ColorImport = "Gray";
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            SideColumns = new List<SideColumn>();
            TowerName = towerName;
            TowerTypes = GetTowerTypes();
            TowerType = TowerTypes[0];
            if (!string.IsNullOrEmpty(TowerName))
            {
                dbtower= new TowerDAL();
                tower = dbtower.GetModel(SessionProtectedSystem);
                TowerName = tower.TowerName;
                TowerType = tower.TowerType;
                Desciption = tower.Description;
                StageNumber = tower.StageNumber;
                TowerType = tower.TowerType;
                Feeds = GetStreams(SessionProtectedSystem, false);
                Products = GetStreams(SessionProtectedSystem, true);
                Condensers = GetHeaters(SessionProtectedSystem, 1);
                HxCondensers = GetHeaters(SessionProtectedSystem, 2);
                Reboilers = GetHeaters(SessionProtectedSystem, 3);
                HxReboilers = GetHeaters(SessionProtectedSystem, 4);
            }
           
        }

        private ObservableCollection<CustomStream> GetStreams(ISession Session,bool IsProduct)
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

        private ObservableCollection<TowerHX> GetHeaters(ISession Session, int HeaterType)
        {
            ObservableCollection<TowerHX> list = new ObservableCollection<TowerHX>();
            TowerHXDAL db = new TowerHXDAL();
            IList<TowerHX> lt = db.GetAllList(Session, HeaterType);
            foreach (TowerHX c in lt)
            {
                list.Add(c);
            }

            return list;
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
        private ObservableCollection<TowerHX> _Reboilers;
        public ObservableCollection<TowerHX> Reboilers
        {
            get { return _Reboilers; }
            set
            {
                _Reboilers = value;
                OnPropertyChanged("Reboilers");
            }
        }
        private ObservableCollection<TowerHX> _HxReboilers;
        public ObservableCollection<TowerHX> HxReboilers
        {
            get { return _HxReboilers; }
            set
            {
                _HxReboilers = value;
                OnPropertyChanged("HxReboilers");
            }
        }

        private ObservableCollection<TowerHX> _Condensers;
        public ObservableCollection<TowerHX> Condensers
        {
            get { return _Condensers; }
            set
            {
                _Condensers = value;
                OnPropertyChanged("Condensers");
            }
        }
        private ObservableCollection<TowerHX> _HxCondensers;
        public ObservableCollection<TowerHX> HxCondensers
        {
            get { return _HxCondensers; }
            set
            {
                _HxCondensers = value;
                OnPropertyChanged("HxCondensers");
            }
        }

        private List<SideColumn> SideColumns=new List<SideColumn>();
        private List<ProIIEqData> SideColumnList=new List<ProIIEqData>();
        Dictionary<string, string> sideColumnTray = new Dictionary<string, string>();
        Dictionary<string, string> sideColumnProdType = new Dictionary<string, string>();
        private ProIIEqData CurrentTower;
        Dictionary<string, string> dicFeeds = new Dictionary<string, string>();
        Dictionary<string, string> dicProducts = new Dictionary<string, string>();
        Dictionary<string, string> dicProdTypes = new Dictionary<string, string>();
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
            SelectEquipmentVM vm = new SelectEquipmentVM("Column", SessionPlant);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                op = 0;
                if (!string.IsNullOrEmpty(vm.SelectedEq))
                {
                    SideColumnList.Clear();
                    SideColumns.Clear();
                    sideColumnProdType.Clear();
                    sideColumnTray.Clear();
                    dicFeeds.Clear();
                    dicProdTypes.Clear();
                    dicProducts.Clear();
                    //根据设该设备名称来获取对应的物流线信息和其他信息。                    
                    ProIIEqDataDAL dbEq = new ProIIEqDataDAL();
                    SourceFileName = vm.SelectedFile;
                    CurrentTower = dbEq.GetModel(SessionPlant, SourceFileName, vm.SelectedEq, "Column");

                    TowerName = CurrentTower.EqName;
                    StageNumber = int.Parse(CurrentTower.NumberOfTrays);
                    string[] products = CurrentTower.ProductData.Split(',');
                    IList<ProIIEqData> allSideColumnList = dbEq.GetAllList(SessionPlant, SourceFileName, "SideColumn");
                    foreach(ProIIEqData d in allSideColumnList)
                    {
                        if (isRelatedSideColumn(d, products))
                        {
                            SideColumn sc = new SideColumn();
                            sc.EqName = d.EqName;
                            SideColumns.Add(sc);

                            SideColumnList.Add(d);
                        }
                    }
                  
                    Feeds = new ObservableCollection<CustomStream>();
                    Products = new ObservableCollection<CustomStream>();
                    Reboilers = new ObservableCollection<TowerHX>();
                    HxReboilers = new ObservableCollection<TowerHX>();
                    Condensers = new ObservableCollection<TowerHX>();
                    HxCondensers = new ObservableCollection<TowerHX>();
                    GetHeaters(CurrentTower);
                    GetMaincolumnRealFeedProduct(ref dicFeeds, ref dicProducts);
                    ProIIStreamDataDAL dbStreamData = new ProIIStreamDataDAL();
                    foreach (KeyValuePair<string, string> k in dicFeeds)
                    {
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k.Key, SourceFileName);
                        CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                        cstream.Tray = int.Parse(k.Value);
                        cstream.IsProduct = false;

                        Feeds.Add(cstream);
                    }
                    foreach (KeyValuePair<string, string> k in dicProducts)
                    {
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k.Key, SourceFileName);
                        if (d.TotalMolarRate != "0")
                        {
                            CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                            cstream.Tray = int.Parse(k.Value);
                            cstream.IsProduct = true;
                            if (dicProdTypes.Keys.Contains(k.Key))
                            {
                                cstream.ProdType = dicProdTypes[k.Key];
                            }

                            Products.Add(cstream);
                        }
                    }




                }

            }           
        }




        private void GetHeaters(ProIIEqData data)
        {
            string heaterNames = data.HeaterNames;
            string heaterDuties = data.HeaterDuties;
            string heaterTrayLoc = data.HeaterTrayLoc;
            string[] arrHeaterNames = heaterNames.Split(',');
            string[] arrHeaterDuties = heaterDuties.Split(',');
            string[] arrHeaterTrayLoc = heaterTrayLoc.Split(',');
            for (int i = 0; i < arrHeaterNames.Length; i++)
            {
                double duty = double.Parse(arrHeaterDuties[i]) * 3600;  //KJ/hr
                if (arrHeaterNames[i] == "CONDENSER")
                {
                    TowerHX hx = new TowerHX();
                    hx.HeaterName=arrHeaterNames[i];
                    hx.HeaterDuty=duty;
                    hx.HeaterType=1;
                    Condensers.Add(hx);

                }
                else if (arrHeaterNames[i] == "REBOILER")
                {

                    TowerHX hx = new TowerHX();
                    hx.HeaterName=arrHeaterNames[i];
                    hx.HeaterDuty=duty;
                    hx.HeaterType = 3;
                    this.Reboilers.Add(hx);

                }
                else if (double.Parse(arrHeaterDuties[i]) <= 0 && arrHeaterNames[i] != "CONDENSER")
                {
                    if (arrHeaterTrayLoc[i] == "1")
                    {
                        TowerHX hx = new TowerHX();
                        hx.HeaterName = arrHeaterNames[i];
                        hx.HeaterDuty = duty;
                        hx.HeaterType = 1;
                        Condensers.Add(hx);
                    }
                    else
                    {
                        TowerHX hx = new TowerHX();
                        hx.HeaterName = arrHeaterNames[i];
                        hx.HeaterDuty = duty;
                        hx.HeaterType = 2;
                        HxCondensers.Add(hx);
                    }
                }
                else if (double.Parse(arrHeaterDuties[i]) > 0 && arrHeaterNames[i] != "REBOILER")
                {
                    if (arrHeaterTrayLoc[i] == data.NumberOfTrays)
                    {
                        TowerHX hx = new TowerHX();
                        hx.HeaterName = arrHeaterNames[i];
                        hx.HeaterDuty = duty;
                        hx.HeaterType = 3;
                        Reboilers.Add(hx);
                    }
                    else
                    {
                        TowerHX hx = new TowerHX();
                        hx.HeaterName = arrHeaterNames[i];
                        hx.HeaterDuty = duty;
                        hx.HeaterType = 4;
                        HxReboilers.Add(hx);
                    }
                }
            }
        }

        public void GetEqFeedProduct(ProIIEqData data, ref Dictionary<string, string> cFeeds, ref Dictionary<string, string> cProducts,ref Dictionary<string,string> cProdTypes)
        {
            string feeddata = data.FeedData;
            string productdata = data.ProductData;
            string prodtype = data.ProdType;
            string feedtrays = data.FeedTrays;
            string prodtrays = data.ProdTrays;
            string[] arrFeeds = feeddata.Split(',');
            string[] arrProducts = productdata.Split(',');
            string[] arrProdTypes = prodtype.Split(',');
            string[] arrFeedtrays = feedtrays.Split(',');
            string[] arrProdtrays = prodtrays.Split(',');
            for (int i = 0; i < arrFeeds.Length; i++)
            {
                cFeeds.Add(arrFeeds[i], arrFeedtrays[i]);
            }
            for (int i = 0; i < arrProducts.Length; i++)
            {
                cProducts.Add(arrProducts[i], arrProdtrays[i]);
            }
            for (int i = 0; i < arrProdTypes.Length; i++)
            {
                cProdTypes.Add(arrProducts[i], arrProdTypes[i]);
            }
        }

        public void GetAllSideColumnFeedProductData(ref Dictionary<string, string[]> dictFeed, ref Dictionary<string, string[]> dictProdcut)
        {
            foreach (ProIIEqData d in SideColumnList)
            {
                string key = d.EqName;
                string feeddata = d.FeedData;
                string productdata = d.ProductData;
                string[] feeds = feeddata.Split(',');
                string[] products = productdata.Split(',');
                dictFeed.Add(key, feeds);
                dictProdcut.Add(key, products);
            }
               
        }

        private string[] GetSideColumnTrayAndProdType(string[] sideColumnFeeds, Dictionary<string, string> dicColumnProducts,Dictionary<string,string> dicColumnProdTypes )
        {
            string[] result = new string[2];
            foreach (string feed in sideColumnFeeds)
            {
                if (dicColumnProducts.Keys.Contains(feed))
                {
                    result[0] = dicColumnProducts[feed];
                    result[1] = dicColumnProdTypes[feed];
                    break;
                }
            }
            return result;
        }

        private void GetMaincolumnRealFeedProduct(ref Dictionary<string, string> dicFeeds, ref Dictionary<string, string> dicProducts)
        {
            Dictionary<string, string> tempFeeds = new Dictionary<string, string>();
            Dictionary<string, string> tempProducts = new Dictionary<string, string>();
            Dictionary<string, string> tempProdTypes = new Dictionary<string, string>();
            Dictionary<string, string[]> sideColumnFeeds = new Dictionary<string, string[]>();
            Dictionary<string, string[]> sideColumnProducts = new Dictionary<string, string[]>();
            GetEqFeedProduct(CurrentTower, ref tempFeeds, ref tempProducts, ref tempProdTypes);
            GetAllSideColumnFeedProductData(ref sideColumnFeeds, ref sideColumnProducts);
            foreach (KeyValuePair<string, string> feed in tempFeeds)
            {
                bool isInternal = false;
                foreach (KeyValuePair<string, string[]> p in sideColumnProducts)
                {
                    if (p.Value.Contains(feed.Key))
                    {
                        isInternal = true;
                        break;
                    }
                }
                if (!isInternal)
                {
                    dicFeeds.Add(feed.Key, feed.Value);
                }
            }
            foreach (KeyValuePair<string, string> product in tempProducts)
            {
                bool isInternal = false;
                foreach (KeyValuePair<string, string[]> p in sideColumnFeeds)
                {
                    if (p.Value.Contains(product.Key))
                    {
                        isInternal = true;
                        break;
                    }
                }
                if (!isInternal)
                {
                    dicProducts.Add(product.Key, product.Value);
                    dicProdTypes.Add(product.Key, tempProdTypes[product.Key]);
                }
            }


            foreach (KeyValuePair<string, string[]> p in sideColumnFeeds)
            {
                string[] trayandprodtype = GetSideColumnTrayAndProdType(p.Value, tempProducts, tempProdTypes);

                sideColumnTray.Add(p.Key, trayandprodtype[0]);
                sideColumnProdType.Add(p.Key, trayandprodtype[1]);
                foreach (string feed in p.Value)
                {
                    if (feed != string.Empty)
                    {
                        bool isInternal = false;
                        if (tempProducts.Keys.Contains(feed))
                        {
                            isInternal = true;
                        }
                        if (!isInternal)
                        {
                            if (dicFeeds.Keys.Contains(feed) == false)
                            {
                                dicFeeds.Add(feed, trayandprodtype[0]);
                            }
                        }
                    }
                }


            }

            foreach (KeyValuePair<string, string[]> p in sideColumnProducts)
            {
                string tray = sideColumnTray[p.Key];
                string prodtype = sideColumnProdType[p.Key];
                if (tray != string.Empty)
                {
                    foreach (string product in p.Value)
                    {
                        if (product != string.Empty)
                        {
                            bool isInternal = false;
                            if (tempFeeds.Keys.Contains(product))
                            {
                                isInternal = true;
                            }
                            if (!isInternal)
                            {
                                if (dicProducts.Keys.Contains(product) == false)
                                {
                                    dicProducts.Add(product, tray);
                                    dicProdTypes.Add(product, prodtype);
                                }
                            }
                        }
                    }
                }

            }
        }

        private bool IsSteam(CustomStream s)
        {
            bool b=false;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] Componentids = s.Componentid.Split(',');
            string[] Coms = s.TotalComposition.Split(',');
            int comCount = Coms.Length;
            for (int i = 0; i < comCount; i++)
            {
                dic.Add(Componentids[i], Coms[i]);
            }
            string H2O = "H2O";
            if (dic.Keys.Contains(H2O))
            {
                if (dic[H2O] == "1" &&s.VaporFraction==1)
                    b = true;
            }
            return b;
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
            if (string.IsNullOrEmpty(TowerName))
            {
                MessageBox.Show("You must Import Data first.", "Message Box");
                ColorImport = "red";
                return;
            }
            if (Feeds.Count == 0)
            {
                return;
            }
            if (op == 0)
            {
                try
                {
                    
                    TowerHXDAL dbHx = new TowerHXDAL();
                    TowerHXDetailDAL dbDetail = new TowerHXDetailDAL();
                    AccumulatorDAL dbAc = new AccumulatorDAL();
                    SideColumnDAL dbSC = new SideColumnDAL();
                    CustomStreamDAL dbCS = new CustomStreamDAL();
                    SourceDAL dbsr = new SourceDAL();
                    TowerDAL dbtower = new TowerDAL();
                    SinkDAL dbsink = new SinkDAL();

                    IList<Accumulator> listAccumulator = dbAc.GetAllList(SessionProtectedSystem);
                    foreach (Accumulator m in listAccumulator)
                    {
                        dbAc.Delete(m, SessionProtectedSystem);
                    }

                    IList<CustomStream> listCustomStream = dbCS.GetAllList(SessionProtectedSystem);
                    foreach (CustomStream m in listCustomStream)
                    {
                        dbCS.Delete(m, SessionProtectedSystem);
                    }


                    IList<Source> listSource = dbsr.GetAllList(SessionProtectedSystem);
                    foreach (Source m in listSource)
                    {
                        dbsr.Delete(m, SessionProtectedSystem);
                    }

                    IList<Sink> listSink = dbsink.GetAllList(SessionProtectedSystem);
                    foreach (Sink m in listSink)
                    {
                        dbsink.Delete(m, SessionProtectedSystem);
                    }


                    IList<SideColumn> listSideColumn = dbSC.GetAllList(SessionProtectedSystem);
                    foreach (SideColumn m in listSideColumn)
                    {
                        dbSC.Delete(m, SessionProtectedSystem);
                    }

                    IList<Tower> listTower = dbtower.GetAllList(SessionProtectedSystem);
                    foreach (Tower m in listTower)
                    {
                        dbtower.Delete(m, SessionProtectedSystem);
                    }
                    IList<TowerHX> listTowerHX = dbHx.GetAllList(SessionProtectedSystem);
                    foreach (TowerHX m in listTowerHX)
                    {
                        dbHx.Delete(m, SessionProtectedSystem);
                    }
                    IList<TowerHXDetail> listTowerHXDetail = dbDetail.GetAllList(SessionProtectedSystem);
                    foreach (TowerHXDetail m in listTowerHXDetail)
                    {
                        dbDetail.Delete(m, SessionProtectedSystem);
                    }



                    foreach (TowerHX hx in Condensers)
                    {
                        dbHx.Add(hx, SessionProtectedSystem);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = 100;
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Cooling Water";
                        detail.MediumSideFlowSource = "Supply Header";
                        dbDetail.Add(detail, SessionProtectedSystem);
                    }
                    foreach (TowerHX hx in HxCondensers)
                    {
                        dbHx.Add(hx, SessionProtectedSystem);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = 100;
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Process Stream";
                        detail.MediumSideFlowSource = "Pump(Motor)";
                        dbDetail.Add(detail, SessionProtectedSystem);
                    }
                    foreach (TowerHX hx in Reboilers)
                    {
                        dbHx.Add(hx, SessionProtectedSystem);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = 100;
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Steam";
                        detail.MediumSideFlowSource = "Supply Header";
                        dbDetail.Add(detail, SessionProtectedSystem);
                    }
                    foreach (TowerHX hx in HxReboilers)
                    {
                        dbHx.Add(hx, SessionProtectedSystem);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = 100;
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Steam";
                        detail.MediumSideFlowSource = "Supply Header";
                        dbDetail.Add(detail, SessionProtectedSystem);
                    }

                    Accumulator ac = new Accumulator();
                    ac.AccumulatorName = "AC1";
                    dbAc.Add(ac, SessionProtectedSystem);


                    if (SideColumns != null)
                    {
                        foreach (SideColumn sc in SideColumns)
                        {
                            dbSC.Add(sc, SessionProtectedSystem);
                        }
                    }


                    foreach (CustomStream cs in Feeds)
                    {
                        Source sr = new Source();
                        sr.MaxPossiblePressure = cs.Pressure;
                        sr.StreamName = cs.StreamName;
                        sr.SourceName = cs.StreamName + "_Source";
                        sr.SourceType = "Pump(Motor)";
                        sr.StreamName = cs.StreamName;
                        sr.IsSteam = IsSteam(cs);
                        sr.IsHeatSource = false;
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

                        dbsink.Add(sink, SessionProtectedSystem);
                        dbCS.Add(cs, SessionProtectedSystem);
                    }


                    Tower tower = new Tower();
                    tower.TowerName = TowerName;
                    tower.StageNumber = StageNumber;
                    tower.SourceFile = SourceFileName;
                    dbtower.Add(tower, SessionProtectedSystem);

                    ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
                    ProtectedSystem ps = new ProtectedSystem();
                    ps.PSType = 1;
                    psDAL.Add(ps, SessionProtectedSystem);

                    SourceFileDAL sfdal=new SourceFileDAL();
                    SourceFileInfo = sfdal.GetModel(tower.SourceFile, SessionPlant);
                    SessionProtectedSystem.Flush();
                }
                catch (Exception ex)
                {

                }


                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
            else
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = false;
                }
            }
        }

       /// <summary>
       /// 获得塔类型列表
       /// </summary>
       /// <returns></returns>
        private ObservableCollection<string> GetTowerTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Distillation");
            list.Add("Absorber");
            list.Add("Absorbent Regenerator");
            return list;
        }

        private bool isRelatedSideColumn(ProIIEqData eq,string[] products)
        {
            bool b = false;
            string[] streams=eq.FeedData.Split(',');
            foreach (string s in streams)
            {
                if(!string.IsNullOrEmpty(s))
                {
                    if (products.Contains(s))
                    {
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }
    }

  
}
