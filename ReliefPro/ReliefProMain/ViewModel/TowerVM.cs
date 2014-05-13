using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
       
        public TowerVM(string towerName, string dbPSFile, string dbPFile)
        {
            SideColumns = new List<SideColumn>();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            TowerName = TowerName;
            if (!string.IsNullOrEmpty(TowerName))
            {
                using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                {
                    var Session = helper.GetCurrentSession();
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
        private ProIIEqData CurrentTower;
        Dictionary<string, string> dicFeeds = new Dictionary<string, string>();
        Dictionary<string, string> dicProducts = new Dictionary<string, string>();

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
                            CustomStream cstream = ConvertProIIStreamToCustomStream(d);
                            cstream.Tray = k.Value;
                            cstream.IsProduct = false;
                            Feeds.Add(cstream);
                        }
                        foreach (KeyValuePair<string, string> k in dicProducts)
                        {
                            ProIIStreamData d = dbStreamData.GetModel(Session, k.Key);
                            CustomStream cstream = ConvertProIIStreamToCustomStream(d);
                            cstream.Tray = k.Value;
                            cstream.IsProduct = false;
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

        public void GetEqFeedProduct(ProIIEqData data, ref Dictionary<string, string> dicFeeds, ref Dictionary<string, string> dicProducts)
        {
            string feeddata = data.FeedData;
            string productdata = data.ProductData;
            string feedtrays = data.FeedTrays;
            string prodtrays = data.ProdTrays;
            string[] arrFeeds = feeddata.Split(',');
            string[] arrProducts = productdata.Split(',');
            string[] arrFeedtrays = feedtrays.Split(',');
            string[] arrProdtrays = prodtrays.Split(',');
            for (int i = 0; i < arrFeeds.Length; i++)
            {
                dicFeeds.Add(arrFeeds[i], arrFeedtrays[i]);
            }
            for (int i = 0; i < arrProducts.Length; i++)
            {
                dicProducts.Add(arrProducts[i], arrProdtrays[i]);
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

        private string GetSideColumnTray(string[] sideColumnFeeds, Dictionary<string, string> dicProducts)
        {
            string result = string.Empty;
            foreach (string feed in sideColumnFeeds)
            {
                if (dicProducts.Keys.Contains(feed))
                {
                    result = dicProducts[feed];
                    break;
                }
            }
            return result;
        }

        private void GetMaincolumnRealFeedProduct(ref Dictionary<string, string> dicFeeds, ref Dictionary<string, string> dicProducts)
        {
            Dictionary<string, string> tempFeeds = new Dictionary<string, string>();
            Dictionary<string, string> tempProducts = new Dictionary<string, string>();
            Dictionary<string, string[]> sideColumnFeeds = new Dictionary<string, string[]>();
            Dictionary<string, string[]> sideColumnProducts = new Dictionary<string, string[]>();
            GetEqFeedProduct(CurrentTower, ref tempFeeds, ref tempProducts);
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

            Dictionary<string, string> sideColumnTray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string[]> p in sideColumnFeeds)
            {
                string tray = GetSideColumnTray(p.Value, tempProducts);
                sideColumnTray.Add(p.Key, tray);
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
                                dicFeeds.Add(feed, tray);
                            }
                        }
                    }
                }

            }

            foreach (KeyValuePair<string, string[]> p in sideColumnProducts)
            {
                string tray = sideColumnTray[p.Key];
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
                            }
                        }
                    }
                }

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
            if (string.IsNullOrEmpty(s.Pressure))
            {
                cs.Pressure = UnitConverter.unitConv(s.Pressure, "KPA", "MPAG", "{0:0.0000}");
            }
            if (string.IsNullOrEmpty(s.Temperature))
            {
                cs.Temperature = UnitConverter.unitConv(s.Temperature, "K", "C", "{0:0.0000}");
            }
            cs.ProdType = s.ProdType;
           
            
            cs.TotalComposition = s.TotalComposition;
            cs.TotalMolarEnthalpy = s.TotalMolarEnthalpy;
            cs.TotalMolarRate = s.TotalMolarRate;
            cs.Tray = s.Tray;
            cs.VaporFraction = s.VaporFraction;
            cs.VaporZFmKVal = s.VaporZFmKVal;
            //cs.WeightFlow = s.WeightFlow;
            // cs.SpEnthalpy = s.SpEnthalpy;

            double TotalMolarRate = 0;
            if (!string.IsNullOrEmpty(s.TotalMolarRate))
                TotalMolarRate = double.Parse(s.TotalMolarRate);
            string bulkmwofphase = s.BulkMwOfPhase;
            if (!string.IsNullOrEmpty(bulkmwofphase))
            {
                double wf = TotalMolarRate * double.Parse(bulkmwofphase);
                cs.WeightFlow = string.Format("{0:0.0000}", wf * 3600);  //Kg/h
            }

            //enthalpy=TotalMolarEnthalpy*TotalMolarRate+InertWeightEnthalpy*InertWeightRate;
            double TotalMolarEnthalpy = 0;
            string strTotalMolarEnthalpy = s.TotalMolarEnthalpy;
            if (!string.IsNullOrEmpty(strTotalMolarEnthalpy))
            {
                TotalMolarEnthalpy = double.Parse(strTotalMolarEnthalpy);
            }


            double InertWeightEnthalpy = 0;
            string strInertWeightEnthalpy = s.InertWeightEnthalpy;
            if (strInertWeightEnthalpy != "")
            {
                InertWeightEnthalpy = double.Parse(strInertWeightEnthalpy);
            }

            double InertWeightRate = 0;
            string strInertWeightRate = s.InertWeightRate;
            if (strInertWeightRate != "")
            {
                InertWeightRate = double.Parse(strInertWeightRate);
            }


            double Enthalpy = TotalMolarEnthalpy * TotalMolarRate + InertWeightEnthalpy * InertWeightRate;
            //cs.Enthalpy = string.Format("{0:0.0000}", Enthalpy * 3600); //KJ


            double TotalMassRate = 0;
            if (TotalMolarRate > 0 && bulkmwofphase != "")
            {
                TotalMassRate = TotalMolarRate * double.Parse(bulkmwofphase);
            }


            double SpEnthalpy = 0;
            if (TotalMolarRate + InertWeightRate > 0)
            {
                SpEnthalpy = Enthalpy / (TotalMassRate + InertWeightRate);
            }
            cs.SpEnthalpy = string.Format("{0:0.0000}", SpEnthalpy);  //KJ/Kg
            return cs;


        }



        public void Save(object obj)
        {
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTowerHX dbHx = new dbTowerHX();
                dbTowerHXDetail dbDetail = new dbTowerHXDetail();
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

                dbAccumulator dbAc = new dbAccumulator();
                Accumulator ac = new Accumulator();
                ac.AccumulatorName = "AC1";
                dbAc.Add(ac, Session);

                dbSideColumn dbSC = new dbSideColumn();
                if (SideColumns != null)
                {
                    foreach (SideColumn sc in SideColumns)
                    {
                        dbSC.Add(sc, Session);
                    }
                }

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

                dbTower dbtower = new dbTower();
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
    }

   public class UnitConverter
   {
       public static string unitConv(string param, string sourcetype, string targetype, string format)
       {
           //string.Format("{0:000.000}", 12.2);
           string result = param;
           if (sourcetype.ToUpper() == "K" && targetype == "C")
           {
               double temp = double.Parse(param) - 273.15;
               result = string.Format(format, temp);
           }
           if (sourcetype.ToUpper() == "KPA" && targetype == "MPAG")
           {
               double temp = double.Parse(param) / 1000 - 0.10135;
               result = string.Format(format, temp);
           }
           if (sourcetype.ToUpper() == "KG/SEC" && targetype == " KG/HR")
           {
               double temp = double.Parse(param) / 3600;
               result = string.Format(format, temp);
           }
           if (sourcetype.ToUpper() == "M3/SEC" && targetype == "M3/HR")
           {
               double temp = double.Parse(param) / 3600;
               result = string.Format(format, temp);
           }

           if (sourcetype.ToUpper() == "PAS" && targetype == "CP")
           {
               //1P=0.1PaS=100CP=100mPaS
               double temp = double.Parse(param) * 1000;
               result = string.Format(format, temp);
           }
           return result;
       }

       public static string convertData(object obj)
       {
           string rs = string.Empty;
           if (obj is Array)
           {
               object[] objdata = (System.Object[])obj;
               foreach (object s in objdata)
               {
                   if (s.ToString() != string.Empty)
                   {
                       rs = rs + "," + s;
                   }
               }
               rs = rs.Substring(1);
           }
           else if (obj == null)
           {
               rs = "";
           }
           else
               rs = obj.ToString();

           return rs;
       }
   }
}
