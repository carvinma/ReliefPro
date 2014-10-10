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
using ReliefProModel.Compressors;
using ReliefProDAL.Compressors;
using ReliefProMain.Models.Compressors;
using ReliefProBLL;
using ReliefProCommon.Enum;

namespace ReliefProMain.ViewModel
{
    public class CompressorVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { set; get; }
        public string FileName { set; get; }
        private ProIIEqData ProIICompressor;
        public int op=1;
        public CompressorModel model { set; get; }
        List<string> dicFeeds = new List<string>();
        List<string> dicProducts = new List<string>();
        List<string> dicProductTypes = new List<string>();

        CustomStreamBLL csbll;
        CompressorBLL compressorbll;
        SourceFileBLL sourcebll;
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
        public CompressorVM(string CompressorName, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            csbll = new CustomStreamBLL( sessionPlant,sessionProtectedSystem);
            compressorbll = new CompressorBLL(sessionPlant, sessionProtectedSystem);
            sourcebll = new SourceFileBLL(sessionPlant);

            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            
            if (!string.IsNullOrEmpty(CompressorName))
            {
                ObservableCollection<CustomStream>  Feeds = csbll.GetStreams(SessionProtectedSystem, false);
                ObservableCollection<CustomStream>  Products = csbll.GetStreams(SessionProtectedSystem, true);
                Compressor compressor = compressorbll.GetModel();
                model = new CompressorModel(compressor,Feeds,Products);
               
                SourceFileDAL sfdal = new SourceFileDAL();
                SourceFileInfo = sfdal.GetModel(compressor.SourceFile, SessionPlant);
                FileName = compressor.SourceFile;
                ColorImport = ColorBorder.blue.ToString();
                op = 1;
            }
            else
            {
                ColorImport = ColorBorder.red.ToString();
                ObservableCollection<CustomStream> Feeds = new ObservableCollection<CustomStream>();
                ObservableCollection<CustomStream> Products = new ObservableCollection<CustomStream>();
                Compressor compressor = new Compressor();
                compressor.CompressorType = "Centrifugal";
                model = new CompressorModel(compressor, Feeds, Products);
                op = 0;
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
            SelectEquipmentVM vm = new SelectEquipmentVM("Compressor",  SessionPlant);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(vm.SelectedEq))
                {
                    if (op==1)
                    {
                        op = 2;
                    }
                    //根据设该设备名称来获取对应的物流线信息和其他信息。
                    ProIIEqDataDAL dbEq = new ProIIEqDataDAL();
                    FileName = vm.SelectedFile;
                    model.SourceFile = FileName;
                    ProIICompressor = dbEq.GetModel(SessionPlant, FileName, vm.SelectedEq, "Compressor");                   
                    model.CompressorType = model.CompressorTypes[0];
                    model.CompressorName = ProIICompressor.EqName;
                    dicFeeds = new List<string>();
                    dicProducts = new List<string>();
                    dicProductTypes = new List<string>();
                    model.Feeds = new ObservableCollection<CustomStream>();
                    model.Products = new ObservableCollection<CustomStream>();
                    GetEqFeedProduct(ProIICompressor, ref dicFeeds, ref dicProducts, ref dicProductTypes);
                    ProIIStreamDataDAL dbStreamData = new ProIIStreamDataDAL();

                    foreach (string k in dicFeeds)
                    {
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k, FileName);
                        CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                        cstream.IsProduct = false;
                        model.Feeds.Add(cstream);
                    }
                    for (int i = 0; i < dicProducts.Count; i++)
                    {
                        string k = dicProducts[i];
                        ProIIStreamData d = dbStreamData.GetModel(SessionPlant, k, FileName);
                        CustomStream cstream = ProIIToDefault.ConvertProIIStreamToCustomStream(d);
                        cstream.IsProduct = true;
                        cstream.ProdType = dicProductTypes[i];
                        model.Products.Add(cstream);
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
            if (string.IsNullOrEmpty(model.CompressorName))
            {
                MessageBox.Show("You must Import Data first.", "Message Box");
                ColorImport = ColorBorder.red.ToString();
                return;
            }
            if (op == 2)
            {
                MessageBoxResult r = MessageBox.Show("Are you sure to reimport all data?", "Message Box", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    ReImportBLL reimportbll = new ReImportBLL(SessionProtectedSystem);
                    reimportbll.DeleteAllData();
                }
            }


            ProtectedSystem ps = new ProtectedSystem();
            ps.PSType = 3;           
            compressorbll.Save(model.dbmodel, model.Feeds, model.Products, ps);
            SourceFileInfo = sourcebll.GetSourceFileInfo(model.SourceFile);

            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        
    }
}
