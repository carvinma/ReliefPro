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
using NHibernate;
using ReliefProMain.View;

namespace ReliefProMain.ViewModel
{
   public class TowerVM:ViewModelBase
    {      
       public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        private string przFile;
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
        private string _StageNumber;
        public string StageNumber
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

        private int op = 0; //0 :新增或者发生变化 1 未更改
        public TowerVM(string towerName, string dbPSFile, string dbPFile)
        {
            SideColumns = new List<SideColumn>();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            TowerName = towerName;
            if (!string.IsNullOrEmpty(TowerName))
            {
                op = 0;
                using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                {
                    var Session = helper.GetCurrentSession();
                    dbTower dbtower = new dbTower();
                    Tower model = dbtower.GetModel(Session);
                    TowerName = model.TowerName;
                    Desciption = model.Description;
                    StageNumber = model.StageNumber;
                    Feeds = GetStreams(Session, false);
                    Products = GetStreams(Session, true);
                    Condensers = GetHeaters(Session, 1);
                    HxCondensers = GetHeaters(Session, 2);
                    Reboilers = GetHeaters(Session, 3);
                    HxReboilers = GetHeaters(Session, 4);
                }
            }
            else
            {
                
            }
        }

        private ObservableCollection<CustomStream> GetStreams(ISession Session,bool IsProduct)
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

        private ObservableCollection<TowerHX> GetHeaters(ISession Session, int HeaterType)
        {
            ObservableCollection<TowerHX> list = new ObservableCollection<TowerHX>();
            dbTowerHX db = new dbTowerHX();
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

        private List<SideColumn> SideColumns;
        private IList<ProIIEqData> SideColumnList;
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
            SelectEquipmentVM vm = new SelectEquipmentVM("Column", dbProtectedSystemFile, dbPlantFile);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                op = 0;
                if (!string.IsNullOrEmpty(vm.SelectedEq))
                {
                    //根据设该设备名称来获取对应的物流线信息和其他信息。

                    using (var helper = new NHibernateHelper(dbPlantFile))
                    {
                        var Session = helper.GetCurrentSession();
                        dbProIIEqData dbEq = new dbProIIEqData();
                        przFile = vm.SelectedFile + ".prz";
                        CurrentTower = dbEq.GetModel(Session, przFile, vm.SelectedEq, "Column");

                        TowerName = CurrentTower.EqName;
                        StageNumber = CurrentTower.NumberOfTrays;

                        SideColumnList = dbEq.GetAllList(Session, przFile, "SideColumn");
                        foreach (ProIIEqData d in SideColumnList)
                        {
                            SideColumn sc = new SideColumn();
                            sc.EqName = d.EqName;
                            SideColumns.Add(sc);
                        }
                        Feeds = new ObservableCollection<CustomStream>();
                        Products = new ObservableCollection<CustomStream>();
                        Reboilers = new ObservableCollection<TowerHX>();
                        HxReboilers = new ObservableCollection<TowerHX>();
                        Condensers = new ObservableCollection<TowerHX>();
                        HxCondensers = new ObservableCollection<TowerHX>();
                        GetHeaters(CurrentTower);
                        GetMaincolumnRealFeedProduct(ref dicFeeds, ref dicProducts);
                        dbProIIStreamData dbStreamData=new dbProIIStreamData();
                        foreach (KeyValuePair<string,string> k in dicFeeds)
                        {
                            ProIIStreamData d = dbStreamData.GetModel(Session, k.Key);
                            CustomStream cstream =ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                            cstream.Tray = k.Value;
                            cstream.IsProduct = false;
                            
                            Feeds.Add(cstream);
                        }
                        foreach (KeyValuePair<string, string> k in dicProducts)
                        {
                            ProIIStreamData d = dbStreamData.GetModel(Session, k.Key);
                            CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                            cstream.Tray = k.Value;
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
                decimal duty = decimal.Parse(arrHeaterDuties[i]) * 3600;  //KJ/hr
                if (arrHeaterNames[i] == "CONDENSER")
                {
                    TowerHX hx = new TowerHX();
                    hx.HeaterName=arrHeaterNames[i];
                    hx.HeaterDuty=duty.ToString();
                    hx.HeaterType=1;
                    Condensers.Add(hx);

                }
                else if (arrHeaterNames[i] == "REBOILER")
                {

                    TowerHX hx = new TowerHX();
                    hx.HeaterName=arrHeaterNames[i];
                    hx.HeaterDuty=duty.ToString();
                    hx.HeaterType = 3;
                    this.Reboilers.Add(hx);

                }
                else if (double.Parse(arrHeaterDuties[i]) <= 0 && arrHeaterNames[i] != "CONDENSER")
                {
                    if (arrHeaterTrayLoc[i] == "1")
                    {
                        TowerHX hx = new TowerHX();
                        hx.HeaterName = arrHeaterNames[i];
                        hx.HeaterDuty = duty.ToString();
                        hx.HeaterType = 1;
                        Condensers.Add(hx);
                    }
                    else
                    {
                        TowerHX hx = new TowerHX();
                        hx.HeaterName = arrHeaterNames[i];
                        hx.HeaterDuty = duty.ToString();
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
                        hx.HeaterDuty = duty.ToString();
                        hx.HeaterType = 3;
                        Reboilers.Add(hx);
                    }
                    else
                    {
                        TowerHX hx = new TowerHX();
                        hx.HeaterName = arrHeaterNames[i];
                        hx.HeaterDuty = duty.ToString();
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
            if (dic["H2O"] == "1")
                b = true;
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
            if (Feeds.Count == 0)
            {
                return;
            }
            if (op == 0)
            {
                using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                {
                    var Session = helper.GetCurrentSession();
                    dbTowerHX dbHx = new dbTowerHX();
                    dbTowerHXDetail dbDetail = new dbTowerHXDetail();
                    dbAccumulator dbAc = new dbAccumulator();                    
                    dbSideColumn dbSC = new dbSideColumn();
                    dbCustomStream dbCS = new dbCustomStream();
                    dbSource dbsr = new dbSource();
                    dbTower dbtower = new dbTower();


                    IList<Accumulator> listAccumulator = dbAc.GetAllList(Session);
                    foreach (Accumulator m in listAccumulator)
                    {
                        dbAc.Delete(m, Session);
                    }

                    IList<CustomStream> listCustomStream = dbCS.GetAllList(Session);
                    foreach (CustomStream m in listCustomStream)
                    {
                        dbCS.Delete(m, Session);
                    }


                    IList<Source> listSource = dbsr.GetAllList(Session);
                    foreach (Source m in listSource)
                    {
                        dbsr.Delete(m, Session);
                    }
                    IList<SideColumn> listSideColumn = dbSC.GetAllList(Session);
                    foreach (SideColumn m in listSideColumn)
                    {
                        dbSC.Delete(m, Session);
                    }

                    IList<Tower> listTower = dbtower.GetAllList(Session);
                    foreach (Tower m in listTower)
                    {
                        dbtower.Delete(m, Session);
                    }
                    IList<TowerHX> listTowerHX = dbHx.GetAllList(Session);
                    foreach (TowerHX m in listTowerHX)
                    {
                        dbHx.Delete(m, Session);
                    }
                    IList<TowerHXDetail> listTowerHXDetail = dbDetail.GetAllList(Session);
                    foreach (TowerHXDetail m in listTowerHXDetail)
                    {
                        dbDetail.Delete(m, Session);
                    }



                    foreach (TowerHX hx in Condensers)
                    {
                        dbHx.Add(hx, Session);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = "100";
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Cooling Water";
                        detail.MediumSideFlowSource = "Supply Header";
                        dbDetail.Add(detail, Session);
                    }
                    foreach (TowerHX hx in HxCondensers)
                    {
                        dbHx.Add(hx, Session);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = "100";
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Process Stream";
                        detail.MediumSideFlowSource = "Pump(Motor)";
                        dbDetail.Add(detail, Session);
                    }
                    foreach (TowerHX hx in Reboilers)
                    {
                        dbHx.Add(hx, Session);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = "100";
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Steam";
                        detail.MediumSideFlowSource = "Supply Header";
                        dbDetail.Add(detail, Session);
                    }
                    foreach (TowerHX hx in HxReboilers)
                    {
                        dbHx.Add(hx, Session);
                        TowerHXDetail detail = new TowerHXDetail();
                        detail.DetailName = hx.HeaterName + "_1";
                        detail.Duty = hx.HeaterDuty;
                        detail.DutyPercentage = "100";
                        detail.HXID = hx.ID;
                        detail.ProcessSideFlowSource = "Pressure Driven";
                        detail.Medium = "Steam";
                        detail.MediumSideFlowSource = "Supply Header";
                        dbDetail.Add(detail, Session);
                    }

                    Accumulator ac = new Accumulator();
                    ac.AccumulatorName = "AC1";
                    dbAc.Add(ac, Session);

                    
                    if (SideColumns != null)
                    {
                        foreach (SideColumn sc in SideColumns)
                        {
                            dbSC.Add(sc, Session);
                        }
                    }

                   
                    foreach (CustomStream cs in Feeds)
                    {
                        Source sr = new Source();
                        sr.MaxPossiblePressure = cs.Pressure;
                        sr.StreamName = cs.StreamName;
                        sr.SourceName = cs.StreamName + "_Source";
                        sr.SourceType = "Compressor(Motor)";
                        sr.StreamName = cs.StreamName;
                        sr.IsSteam = IsSteam(cs);
                        sr.IsHeatSource = false;
                        dbsr.Add(sr, Session);


                        dbCS.Add(cs, Session);
                    }


                    foreach (CustomStream cs in Products)
                    {

                        dbCS.Add(cs, Session);
                    }

                    
                    Tower tower = new Tower();
                    tower.TowerName = TowerName;
                    tower.StageNumber = StageNumber;
                    tower.PrzFile = przFile;
                    dbtower.Add(tower, Session);


                    Session.Flush();
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
    }

  
}
