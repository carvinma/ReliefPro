using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using NHibernate;

using ReliefProMain.Models.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProDAL;
using ReliefProModel;
using ReliefProMain.View;
using ReliefProMain.ViewModel.ReactorLoops;
using ReliefProMain.View.ReactorLoops;
using System.IO;
using System.Windows.Controls;
using ReliefProBLL;

using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using ReliefProCommon.Enum;
using ReliefProMain.Models;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopVM
    {
        public ICommand OKCMD { get; set; }
        public ICommand SimulationCMD { get; set; }
        public ICommand ImportCMD { get; set; }
        public ICommand ProcessHXAddCMD { get; set; }
        public ICommand ProcessHXDelCMD { get; set; }

        public ICommand UtilityHXAddCMD { get; set; }
        public ICommand UtilityHXDelCMD { get; set; }

        public ICommand MixerSplitterAddCMD { get; set; }
        public ICommand MixerSplitterDelCMD { get; set; }

        public ICommand NetworkHXAddCMD { get; set; }
        public ICommand NetworkHXDelCMD { get; set; }

        public ICommand DetailCMD { get; set; }
        
        private ISession SessionPS;
        private ISession SessionPF;
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }
        
        private int reactorLoopID;
        public ReactorLoopModel model { get; set; }
        private ReactorLoopBLL reactorBLL;
        public SourceFile SourceFileInfo { get; set; }
        public string FileFullPath { get; set; }
        public string FileName { set; get; }

        ProIIEqDataDAL eqDAL = new ProIIEqDataDAL();
        ProIIStreamDataDAL streamDAL = new ProIIStreamDataDAL();

        public List<string> streams = new List<string>();
        int op = 1;
        private void InitCMD()
        {
            OKCMD = new DelegateCommand<object>(Save);
            SimulationCMD = new DelegateCommand<object>(Simulation);
            ImportCMD = new DelegateCommand<object>(Import);
            ProcessHXAddCMD = new DelegateCommand<object>(ProcessHXAdd);
            ProcessHXDelCMD = new DelegateCommand<object>(ProcessHXDel);

            UtilityHXAddCMD = new DelegateCommand<object>(UtilityHXAdd);
            UtilityHXDelCMD = new DelegateCommand<object>(UtilityHXDel);

            NetworkHXAddCMD = new DelegateCommand<object>(NetworkHXAdd);
            NetworkHXDelCMD = new DelegateCommand<object>(NetworkHXDel);

            MixerSplitterAddCMD = new DelegateCommand<object>(MixerSplitterAdd);
            MixerSplitterDelCMD = new DelegateCommand<object>(MixerSplitterDel);

            DetailCMD = new DelegateCommand<object>(Detail);
        }
        private void InitPage()
        {

            ObservableCollection<string> list = GetProIIStreamNames();

            model.SeparatorSource = GetProIIFlashs();
            model.StreamSource = list;
           
            if (SourceFileInfo != null)
            {
                model.ObcProcessHXSource = GetProIIHXs(0);
                model.ObcUtilityHXSource = GetProIIHXs(1);
                model.ObcNetworkHXSource = GetProIIHXs(3);
                model.ObcMixerSplitterSource = GetProIIMixers();
            }
            if (model.dbModel==null)
            {
                model.ObcProcessHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcUtilityHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcNetworkHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcMixerSplitter = new ObservableCollection<ReactorLoopDetail>();
            }
            
        }
        private ObservableCollection<string> GetProIIStreamNames()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIStreamDataDAL dal = new ProIIStreamDataDAL();
            IList<ProIIStreamData> list = dal.GetAllList(SessionPF, FileName);
            var query = from s in list
                        orderby s.StreamName
                        select s;
            foreach (ProIIStreamData s in query.ToList())
            {
                rlt.Add(s.StreamName);
            }
            return rlt;
        }
        private ObservableCollection<string> GetProIIFlashs()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, FileName, "Flash");
            foreach (ProIIEqData eq in list)
            {
                rlt.Add(eq.EqName);
            }
            return rlt;
        }
        private ObservableCollection<ReactorLoopDetail> GetProIIHXs(int reactorType)
        {
            ObservableCollection<ReactorLoopDetail> rlt = new ObservableCollection<ReactorLoopDetail>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, SourceFileInfo.FileName, "Hx");
            var query = from s in list
                        orderby s.EqName
                        select s;
            foreach (ProIIEqData eq in query.ToList())
            {
                if (reactorType == 0 || reactorType == 3)
                {
                    string[] arr1 = eq.FeedData.Split(',');
                    string[] arr2 = eq.ProductData.Split(',');
                    if (arr2.Length == 2)
                    {
                        ReactorLoopDetail d = new ReactorLoopDetail();
                        d.DetailInfo = eq.EqName;
                        d.ReactorType = reactorType;
                        d.ReactorLoopID_Color = "0";
                        rlt.Add(d);
                    }
                }
                
                if (reactorType == 1 || reactorType == 3)
                {
                    string[] arr1 = eq.FeedData.Split(',');
                    string[] arr2 = eq.ProductData.Split(',');
                    if (arr2.Length == 1)
                    {
                        ReactorLoopDetail d = new ReactorLoopDetail();
                        d.DetailInfo = eq.EqName;
                        d.ReactorType = reactorType;
                        d.ReactorLoopID_Color = "1";
                        rlt.Add(d);
                    }
                }
                
            }
            return rlt;
        }
        private ObservableCollection<ReactorLoopDetail> GetProIIMixers()
        {
            ObservableCollection<ReactorLoopDetail> rlt = new ObservableCollection<ReactorLoopDetail>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, SourceFileInfo.FileName, "Mixer");
            var query = from s in list
                        orderby s.EqName
                        select s;
            foreach (ProIIEqData eq in query.ToList())
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            IList<ProIIEqData> list2 = dal.GetAllList(SessionPF, SourceFileInfo.FileName, "Splitter");
            var query2 = from s in list2
                        orderby s.EqName
                        select s;
            foreach (ProIIEqData eq in query2.ToList())
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            return rlt;
        }

        private void InitReactorLoopSource()
        {
            SourceDAL srdal = new SourceDAL();
            if (!string.IsNullOrEmpty(model.EffluentStream))
            {
                if (model.EffluentStreamSource == null)
                {
                    Source sr = new Source();
                    model.EffluentStreamSource = new SourceModel(sr);
                    model.EffluentStreamSource.StreamName = model.EffluentStream;
                    model.EffluentStreamSource.SourceType = "Pressurized Vessel";
                    model.EffluentStreamSource.SourceType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    Source sr = srdal.GetModel(SessionPS, model.EffluentStream);
                    model.EffluentStreamSource = new SourceModel(sr);
                }
            }
            if (!string.IsNullOrEmpty(model.EffluentStream2))
            {
                if (model.EffluentStream2Source == null)
                {
                    Source sr = new Source();
                    model.EffluentStream2Source = new SourceModel(sr);
                    model.EffluentStream2Source.StreamName = model.EffluentStream2;
                    model.EffluentStream2Source.SourceType = "Pressurized Vessel";
                    model.EffluentStream2Source.SourceType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    Source sr = srdal.GetModel(SessionPS, model.EffluentStream2);
                    model.EffluentStream2Source = new SourceModel(sr);
                }
            }
            if (!string.IsNullOrEmpty(model.CompressorH2Stream))
            {
                if (model.CompressorH2StreamSource == null)
                {
                    Source sr = new Source();
                    model.CompressorH2StreamSource = new SourceModel(sr);
                    model.CompressorH2StreamSource.StreamName = model.CompressorH2Stream;
                    model.CompressorH2StreamSource.SourceType = "Compressor(Steam Turbine Driven)";
                    model.CompressorH2StreamSource.SourceType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    Source sr = srdal.GetModel(SessionPS, model.CompressorH2Stream);
                    model.CompressorH2StreamSource = new SourceModel(sr);
                }
            }

            if ( !string.IsNullOrEmpty(model.ColdReactorFeedStream))
            {
                if (model.ColdReactorFeedStreamSource == null)
                {
                    Source sr = new Source();
                    model.ColdReactorFeedStreamSource = new SourceModel(sr);
                    model.ColdReactorFeedStreamSource.StreamName = model.ColdReactorFeedStream;
                    model.ColdReactorFeedStreamSource.SourceType = "Pump(Motor)";
                    model.ColdReactorFeedStreamSource.SourceType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    Source sr = srdal.GetModel(SessionPS, model.ColdReactorFeedStream);
                    model.ColdReactorFeedStreamSource = new SourceModel(sr);
                }
            }
            if (!string.IsNullOrEmpty(model.ColdReactorFeedStream2))
            {
                if (model.ColdReactorFeedStream2Source == null)
                {
                    Source sr = new Source();
                    model.ColdReactorFeedStream2Source = new SourceModel(sr);
                    model.ColdReactorFeedStream2Source.StreamName = model.ColdReactorFeedStream2;
                    model.ColdReactorFeedStream2Source.SourceType = "Pump(Motor)";
                    model.ColdReactorFeedStream2Source.SourceType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    Source sr = srdal.GetModel(SessionPS, model.ColdReactorFeedStream2);
                    model.ColdReactorFeedStream2Source = new SourceModel(sr);
                }
            }

            if (model.InjectionWaterStreamSource == null && !string.IsNullOrEmpty(model.InjectionWaterStream))
            {
                if (model.InjectionWaterStreamSource == null)
                {
                    Source sr = new Source();
                    model.InjectionWaterStreamSource = new SourceModel(sr);
                    model.InjectionWaterStreamSource.StreamName = model.InjectionWaterStream;
                    model.InjectionWaterStreamSource.SourceType = "Pump(Motor)";
                    model.InjectionWaterStreamSource.SourceType_Color = ColorBorder.green.ToString();
                }
                else
                {
                    Source sr = srdal.GetModel(SessionPS, model.InjectionWaterStream);
                    model.InjectionWaterStreamSource = new SourceModel(sr);
                }
            }



        }
        public ReactorLoopVM( ISession SessionPF, ISession SessionPS, string dirPlant, string dirProtectedSystem)
        {
            model = new ReactorLoopModel();
            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var RLModel = reactorBLL.GetReactorLoopModel();
            model.dbModel = RLModel;
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            
            InitCMD();
            InitPage();

            if (RLModel.ID > 0)
            {
                reactorLoopID = RLModel.ID;
                model.ObcProcessHX = reactorBLL.GetProcessHX(reactorLoopID);
                model.ObcUtilityHX = reactorBLL.GetUtilityHX(reactorLoopID);
                model.ObcNetworkHX = reactorBLL.GetNetworkHX(reactorLoopID);
                model.ObcMixerSplitter = reactorBLL.GetMixerSplitter(reactorLoopID);

                SourceFileBLL sfbll = new SourceFileBLL(SessionPF);
                SourceFileInfo = sfbll.GetSourceFileInfo(model.dbModel.SourceFile);
                FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
                FileName = SourceFileInfo.FileName;
                InitPage();
                InitReactorLoopSource();
                op = 1;
            }
            else
            {
                model.ObcProcessHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcUtilityHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcNetworkHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcMixerSplitter = new ObservableCollection<ReactorLoopDetail>();
                model.ReactorLoopName_Color = ColorBorder.blue.ToString();
                model.EffluentStream_Color = ColorBorder.red.ToString();
                model.EffluentStream2_Color = ColorBorder.green.ToString();
                model.ColdReactorFeedStream_Color = ColorBorder.red.ToString();
                model.ColdReactorFeedStream2_Color = ColorBorder.green.ToString();
                model.HotHighPressureSeparator_Color = ColorBorder.red.ToString();
                model.ColdHighPressureSeparator_Color = ColorBorder.red.ToString();
                model.HXNetworkColdStream_Color = ColorBorder.red.ToString();
                model.InjectionWaterStream_Color = ColorBorder.red.ToString();
                model.CompressorH2Stream_Color = ColorBorder.red.ToString();
                model.ReactorLoopName = "ReactorLoop1";

                InitReactorLoopSource();
                op = 0;
            }
            foreach (ReactorLoopDetail d in model.ObcProcessHX)
            {
                var find = model.ObcProcessHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find != null)
                {
                    model.ObcProcessHXSource.Remove(find);
                }

                var find2 = model.ObcNetworkHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find2 != null)
                {
                    model.ObcNetworkHXSource.Remove(find2);
                }
            }
            foreach (ReactorLoopDetail d in model.ObcUtilityHX)
            {
                var find = model.ObcUtilityHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find != null)
                {
                    model.ObcUtilityHXSource.Remove(find);
                }
                var find2 = model.ObcNetworkHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find2 != null)
                {
                    model.ObcNetworkHXSource.Remove(find2);
                }
            }

            foreach (ReactorLoopDetail d in model.ObcNetworkHX)
            {
                var find = model.ObcNetworkHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find != null)
                {
                    model.ObcNetworkHXSource.Remove(find);
                }
                var find1 = model.ObcUtilityHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find1 != null)
                {
                    model.ObcUtilityHXSource.Remove(find1);
                }

                var find2 = model.ObcProcessHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find2 != null)
                {
                    model.ObcProcessHXSource.Remove(find2);
                }
            }

            foreach (ReactorLoopDetail d in model.ObcMixerSplitter)
            {
                var find = model.ObcMixerSplitterSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                if (find != null)
                {
                    model.ObcMixerSplitterSource.Remove(find);
                }
               
            }
        }
        private void ProcessHXAdd(object obj)
        {
            if (model.SelectedHXSourceModel != null)
            {
                string str = model.SelectedHXSourceModel.DetailInfo;
                model.ObcProcessHX.Add(model.SelectedHXSourceModel);
                
                var find2 = model.ObcNetworkHXSource.FirstOrDefault(p => p.DetailInfo == str );
                model.ObcNetworkHXSource.Remove(find2);

                model.ObcProcessHXSource.Remove(model.SelectedHXSourceModel);
                
            }
        }
        private void ProcessHXDel(object obj)
        {
            if (model.SelectedHXModel != null)
            {
                string str = model.SelectedHXModel.DetailInfo;
                model.ObcProcessHXSource.Add(model.SelectedHXModel);
                ReactorLoopDetail detail3 = model.SelectedHXModel;

                detail3.ReactorType = 3;
                model.ObcNetworkHXSource.Add(detail3);

                var find = model.ObcProcessHX.FirstOrDefault(p => p.DetailInfo == str);
                model.ObcProcessHX.Remove(find);
            }
        }
        private void UtilityHXAdd(object obj)
        {
            if (model.SelectedUtilityHXSourceModel != null)
            {
                string str = model.SelectedUtilityHXSourceModel.DetailInfo;
                model.ObcUtilityHX.Add(model.SelectedUtilityHXSourceModel);

                model.ObcUtilityHXSource.Remove(model.SelectedUtilityHXSourceModel);
               
                var find2 = model.ObcNetworkHXSource.FirstOrDefault(p => p.DetailInfo == str );
                model.ObcNetworkHXSource.Remove(find2);
            }
        }
        private void UtilityHXDel(object obj)
        {
            if (model.SelectedUtilityHXModel != null)
            {
                string str = model.SelectedUtilityHXModel.DetailInfo;
                model.ObcUtilityHXSource.Add(model.SelectedUtilityHXModel);

                ReactorLoopDetail network = model.SelectedUtilityHXModel;
                network.ReactorType = 3;
                model.ObcNetworkHXSource.Add(network);

                var find = model.ObcUtilityHX.FirstOrDefault(p => p.DetailInfo == str);
                model.ObcUtilityHX.Remove(find);

            }
        }

        private void NetworkHXAdd(object obj)
        {
            if (model.SelectedNetworkHXSourceModel != null)
            {
                string str = model.SelectedNetworkHXSourceModel.DetailInfo;
                model.ObcNetworkHX.Add(model.SelectedNetworkHXSourceModel);
                                
                var find2 = model.ObcUtilityHXSource.FirstOrDefault(p => p.DetailInfo == str );
                if (find2 != null)
                {
                    model.ObcUtilityHXSource.Remove(find2);
                }
                var find3 = model.ObcProcessHXSource.FirstOrDefault(p => p.DetailInfo == str );
                if (find3 != null)
                {
                    model.ObcProcessHXSource.Remove(find3);
                }
                model.ObcNetworkHXSource.Remove(model.SelectedNetworkHXSourceModel);
            }
        }
        private void NetworkHXDel(object obj)
        {
            if (model.SelectedNetworkHXModel != null)
            {
                string str = model.SelectedNetworkHXModel.DetailInfo;
                model.ObcNetworkHXSource.Add(model.SelectedNetworkHXModel);

                if (model.SelectedNetworkHXModel.ReactorLoopID_Color == "1")
                {
                    ReactorLoopDetail detail3 = model.SelectedNetworkHXModel;
                    detail3.ReactorType = 1;
                    model.ObcUtilityHXSource.Add(detail3);
                }
                else
                {
                    ReactorLoopDetail detail0 = model.SelectedNetworkHXModel;
                    detail0.ReactorType = 0;
                    model.ObcProcessHXSource.Add(detail0);
                }
                var find = model.ObcNetworkHX.FirstOrDefault(p => p.DetailInfo == str );
                model.ObcNetworkHX.Remove(find);
            }
        }


        private void MixerSplitterAdd(object obj)
        {
            if (model.SelectedMixerSourceModel != null)
            {
                model.ObcMixerSplitter.Add(model.SelectedMixerSourceModel);
                var find = model.ObcMixerSplitterSource.FirstOrDefault(p => p.DetailInfo == model.SelectedMixerSourceModel.DetailInfo );
                model.ObcMixerSplitterSource.Remove(find);
            }
        }
        private void MixerSplitterDel(object obj)
        {
            if (model.SelectedMixerModel != null)
            {
                model.ObcMixerSplitterSource.Add(model.SelectedMixerModel);
                var find = model.ObcMixerSplitter.FirstOrDefault(p => p.DetailInfo == model.SelectedMixerModel.DetailInfo );
                model.ObcMixerSplitter.Remove(find);
            }
        }
        private void Import(object obj)
        {
            SelectPathView view = new SelectPathView();
            SelectPathVM vm = new SelectPathVM(SessionPF);
            view.DataContext = vm;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (view.ShowDialog() == true)
            {
                if (op == 1)
                    op = 2;
                FileName = vm.SelectedFile;
                SourceFileInfo = vm.SourceFileInfo;
                InitPage();
                model.ObcNetworkHX.Clear();
                model.ObcProcessHX.Clear();
                model.ObcUtilityHX.Clear();
                model.ObcMixerSplitter.Clear();
            }
        }
        /// <summary>
        /// 模拟ReactorLoop，生成ProII的keyword 文件。
        /// </summary>
        /// <param name="obj"></param>
        private void Simulation(object obj)
        {            
            streams.Clear();
            List<CustomStream> csList = new List<CustomStream>();
            List<string> eqList = new List<string>();
            List<string> processHxList = new List<string>();
            List<string> otherHxList = new List<string>();
             
            foreach (ReactorLoopDetail d in model.ObcProcessHX)
            {
                processHxList.Add(d.DetailInfo);
            }
            foreach (ReactorLoopDetail d in model.ObcUtilityHX)
            {
                otherHxList.Add(d.DetailInfo);
            }
            foreach (ReactorLoopDetail d in model.ObcNetworkHX)
            {
                otherHxList.Add(d.DetailInfo);
            }
            foreach (ReactorLoopDetail d in model.ObcMixerSplitter)
            {
                eqList.Add(d.DetailInfo);
            }
            if (!string.IsNullOrEmpty(model.ColdHighPressureSeparator))
            {
                eqList.Add(model.ColdHighPressureSeparator);
            }
            if (!string.IsNullOrEmpty(model.HotHighPressureSeparator))
            {
                eqList.Add(model.HotHighPressureSeparator);
            }
            csList = GetReactorLoopStreamsFromProII(eqList);
            List<CustomStream> csList2 = GetReactorLoopStreamsFromProII(processHxList);
            List<CustomStream> csList3 = GetReactorLoopStreamsFromProII(otherHxList);
            csList=csList.Union(csList2).ToList();
            csList = csList.Union(csList3).ToList();
            if (!string.IsNullOrEmpty(model.ColdReactorFeedStream) && !streams.Contains(model.ColdReactorFeedStream))
            {               
                if (!streams.Contains(model.ColdReactorFeedStream))
                {                   
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.ColdReactorFeedStream);
                    csList.Add(cs);
                    streams.Add(model.ColdReactorFeedStream);
                }
            }
            if (!string.IsNullOrEmpty(model.ColdReactorFeedStream2) && !streams.Contains(model.ColdReactorFeedStream2))
            {
                if (!streams.Contains(model.ColdReactorFeedStream2))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.ColdReactorFeedStream2);
                    csList.Add(cs);
                    streams.Add(model.ColdReactorFeedStream2);
                }
            }
            if (!string.IsNullOrEmpty(model.InjectionWaterStream) && !streams.Contains(model.InjectionWaterStream))
            {
                if (!streams.Contains(model.InjectionWaterStream))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.InjectionWaterStream);
                    csList.Add(cs);
                    streams.Add(model.InjectionWaterStream);
                }
            }
            if (!string.IsNullOrEmpty(model.CompressorH2Stream) && !streams.Contains(model.CompressorH2Stream))
            {
                if (!streams.Contains(model.CompressorH2Stream))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.CompressorH2Stream);
                    csList.Add(cs);
                    streams.Add(model.CompressorH2Stream);
                }
            }
            if (!string.IsNullOrEmpty(model.EffluentStream) && !streams.Contains(model.EffluentStream))
            {
                if (!streams.Contains(model.EffluentStream))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.EffluentStream);
                    csList.Add(cs);
                    streams.Add(model.EffluentStream);
                }
            }
            if (!string.IsNullOrEmpty(model.EffluentStream2) && !streams.Contains(model.EffluentStream2))
            {
                if (!streams.Contains(model.EffluentStream2))
                {
                    CustomStream cs = GetReactorLoopStreamInfoFromProII(model.EffluentStream2);
                    csList.Add(cs);
                    streams.Add(model.EffluentStream2);
                }
            }


            string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;

            int errorTag = 0;
            string inpData = CreateReactorLoopInpData(dir, csList, eqList,processHxList,otherHxList,ref errorTag);
            
            string newInpFile = DirProtectedSystem + @"\myrp\myrp.inp";
            string sourcePrzFile = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;

            string newInpDir = DirProtectedSystem + @"\myrp";
            if (!Directory.Exists(newInpDir))
            {
                Directory.CreateDirectory(newInpDir);
            }
            if (System.IO.File.Exists(newInpFile))
                File.Delete(newInpFile);
            File.Create(newInpFile).Close();
            File.WriteAllText(newInpFile, inpData);
            if (errorTag == -1)
            {
                MessageBoxResult r = MessageBox.Show("New inp file has error,do you want to investigate it? ", "Message Box", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();
                    //设置要调用的外部程序名
                    procInfo.FileName = "notepad.exe";
                    //设置外部程序的启动参数（命令行参数）为1.txt
                    procInfo.Arguments = newInpFile;

                    //设置外部程序工作目录为 C:\
                    //procInfo.WorkingDirectory = "C:\\";
                    System.Diagnostics.Process.Start(procInfo);
                }
            }
            ReactorLoopSimulationView v = new ReactorLoopSimulationView();           
            ReactorLoopSimulationVM vm = new ReactorLoopSimulationVM(reactorLoopID,newInpFile,sourcePrzFile,SourceFileInfo.FileVersion,processHxList,SessionPS,SessionPF);
            v.DataContext = vm;
            v.ShowDialog();


        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    if (op == 0)
                    {
                        ReImportBLL reimportbll = new ReImportBLL(SessionPS);
                        reimportbll.DeleteAllData();
                        Create();
                    }
                    else if (op == 1)
                    {
                        Create();
                    }
                    else
                    {
                        MessageBoxResult r = MessageBox.Show("Are you sure to reimport all data?", "Message Box", MessageBoxButton.YesNo);
                        if (r == MessageBoxResult.Yes)
                        {
                            ReImportBLL reimportbll = new ReImportBLL(SessionPS);
                            reimportbll.DeleteAllData();
                            Create();
                        }
                    }

                    wd.DialogResult = true;
                }
            }
        }
        private void Create()
        {
            model.SourceFile = FileName;
            var allSelectedInfo = new ObservableCollection<ReactorLoopDetail>();
            if (model.ObcProcessHX != null)
            {
                foreach (var hx in model.ObcProcessHX)
                {
                    hx.ID = 0;
                    allSelectedInfo.Add(hx);
                }
            }
            if (model.ObcUtilityHX != null)
            {
                foreach (var hx in model.ObcUtilityHX)
                {
                    hx.ID = 0;
                    allSelectedInfo.Add(hx);
                }
            }
            if (model.ObcNetworkHX != null)
            {
                foreach (var hx in model.ObcNetworkHX)
                {
                    hx.ID = 0;
                    allSelectedInfo.Add(hx);
                }
            }
            if (model.ObcMixerSplitter != null)
            {
                foreach (var hx in model.ObcMixerSplitter)
                {
                    hx.ID = 0;
                    allSelectedInfo.Add(hx);
                }
            }

            List<Source> lstStreamSource = new List<Source>();
            if (model.EffluentStreamSource!=null)
            {
                model.EffluentStreamSource.dbmodel.SourceType = model.EffluentStreamSource.SourceType;
                model.EffluentStreamSource.SourceType_Color = model.EffluentStreamSource.SourceType_Color;
                lstStreamSource.Add(model.EffluentStreamSource.dbmodel);
            }
            if (model.EffluentStream2Source != null)
            {
                model.EffluentStream2Source.dbmodel.SourceType = model.EffluentStream2Source.SourceType;
                model.EffluentStream2Source.SourceType_Color = model.EffluentStream2Source.SourceType_Color;
                lstStreamSource.Add(model.EffluentStream2Source.dbmodel);
            }
            if (model.CompressorH2StreamSource != null)
            {
                model.CompressorH2StreamSource.dbmodel.SourceType = model.CompressorH2StreamSource.SourceType;
                model.CompressorH2StreamSource.SourceType_Color = model.CompressorH2StreamSource.SourceType_Color;
                lstStreamSource.Add(model.CompressorH2StreamSource.dbmodel);
            }
            if (model.ColdReactorFeedStreamSource != null)
            {
                model.ColdReactorFeedStreamSource.dbmodel.SourceType = model.ColdReactorFeedStreamSource.SourceType;
                model.ColdReactorFeedStreamSource.SourceType_Color = model.ColdReactorFeedStreamSource.SourceType_Color;
                lstStreamSource.Add(model.ColdReactorFeedStreamSource.dbmodel);
            }
            if (model.ColdReactorFeedStream2Source != null)
            {
                model.ColdReactorFeedStream2Source.dbmodel.SourceType = model.ColdReactorFeedStream2Source.SourceType;
                model.ColdReactorFeedStream2Source.SourceType_Color = model.ColdReactorFeedStream2Source.SourceType_Color;
                lstStreamSource.Add(model.ColdReactorFeedStream2Source.dbmodel);
            }
            if (model.InjectionWaterStreamSource != null)
            {
                model.InjectionWaterStreamSource.dbmodel.SourceType = model.InjectionWaterStreamSource.SourceType;
                model.InjectionWaterStreamSource.SourceType_Color = model.InjectionWaterStreamSource.SourceType_Color;
                lstStreamSource.Add(model.InjectionWaterStreamSource.dbmodel);
            }
            if (allSelectedInfo.Count > 0)
                reactorBLL.Save(model.dbModel, allSelectedInfo, lstStreamSource);
            if (op != 1)
            {
                ProtectedSystemDAL psDAL = new ProtectedSystemDAL();
                ProtectedSystem ps = new ProtectedSystem();
                ps.PSType = 6;
                psDAL.Add(ps, SessionPS);
            }
            SourceFileDAL sfdal = new SourceFileDAL();
            SourceFileInfo = sfdal.GetModel(model.SourceFile, SessionPF);
            SessionPS.Flush();
        }

        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((Window)sender).DragMove();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="streamList"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public string CreateReactorLoopInpData(string rootDir, List<CustomStream> csList, List<string> eqList, List<string> processHxList,List<string> otherHxList,ref int errorTag)
        {
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(rootDir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            List<int> list = new List<int>();
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("  SEQUENCE"))
                {
                    i = removeTagInfo(lines, i, "  SEQUENCE");
                    break;
                }
                else
                {
                    sb.Append(line).Append("\r\n");
                    i++;
                }
            }
            while (i < lines.Length)
            {
                string line = lines[i];                
                if (line.Contains("STREAM DATA"))
                {
                    sb.Append(line).Append("\r\n");
                    i++;
                    break;
                }
                else
                {
                    i++;
                    sb.Append(line).Append("\r\n");
                }
            }

            List<string> lvStreams = new List<string>();
            string assayLVs = GetAssayLVStreamInfo(lines, i,ref lvStreams);
            sb.Append(assayLVs);
            
            foreach (CustomStream cs in csList)
            {
                if (cs != null && !lvStreams.Contains(cs.StreamName))
                {
                   string s= getStreamData(cs);
                   sb.Append(s);
                }
            }
            
            while (i < lines.Length)
            {
                i = removeTagInfo(lines, i, "  OUTPUT ");
                i = removeRowTagInfo(lines, i, "RXDATA");
                i = removeRowTagInfo(lines, i, "  RXSET ID=");
                i = removeRowTagInfo(lines, i, "    REACTION ID=");
                int end = removeRowTagInfo(lines, i, "      HORX ");
                string line = lines[i];
                if (line.Contains("UNIT OPERATIONS"))
                {
                    break;
                }
                else
                {
                    i = end;
                    if (i < lines.Length)
                    {
                        i++;
                    }
                }
                
            }

            int unitStart = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("UNIT OPERATIONS"))
                {
                    unitStart = i;
                    sb.Append("UNIT OPERATIONS").Append("\r\n");
                    foreach (string eq in eqList)
                    {
                        string eqinfo = CopyEqInfo(lines, eq);
                        sb.Append(eqinfo).Append("\r\n");
                    }

                    break;
                }
                else
                {
                    i++;
                }
            }

            i = unitStart;
            while (i < lines.Length)
            {
                foreach (string eq in otherHxList)
                {
                    string eqinfo = FindOtherHxInfo(lines, i, eq, ref errorTag);
                    sb.Append(eqinfo).Append("\r\n");
                }
                break;
            }

            i = unitStart;
            while (i < lines.Length)
            {
                foreach (string eq in processHxList)
                {
                    string eqinfo = FindProcessHxInfo(lines, i, eq, ref errorTag);
                    sb.Append(eqinfo).Append("\r\n");                   
                }
                break;
            }
            sb.Append("END");

            return sb.ToString();
        }

        public string[] FindStreamInfo(string[] lines, CustomStream cs, ref string newStreamInfo)
        {
            List<string> list = new List<string>();
            string PropStream = "PROPERTY STREAM=" + cs.StreamName.ToUpper();
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(PropStream+","))
                {
                    i++;
                    break;
                }
                else
                {
                    list.Add(line);
                    i++;
                }
            }

            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("  PROP") || line.Substring(0, 2) != "  ")
                {
                    break;
                }
                else
                {
                    i++;
                }
            }
            while (i < lines.Length)
            {
                string line = lines[i];
                list.Add(line);
                i++;
            }
            newStreamInfo = getStreamData(cs);
            string[] newLines = new string[list.Count];
            newLines = list.ToArray();
            return newLines;
        }

        public string GetAssayLVStreamInfo(string[] lines,int start,ref List<string> lvStreams)
        {
            StringBuilder sb = new StringBuilder();
            int i = start;
            while (i < lines.Length-1)
            {
                string line = lines[i];
                string line2 = lines[i + 1];
                if (line.Contains(" STREAM=") && line2.Contains("ASSAY=LV"))
                {
                    int sbegin = line.IndexOf("=");
                    int send = line.IndexOf(",");
                    string streamName = line.Substring(sbegin + 1, send - sbegin-1);
                    lvStreams.Add(streamName);
                    sb.Append(line).Append("\r\n");
                    sb.Append(line2).Append("\r\n");
                    i = i + 2;
                    line = lines[i];
                    while (line.Contains("    "))
                    {
                        sb.Append(line).Append("\r\n");
                        i++;
                        line = lines[i];
                    }
                }
                else
                {
                    i = i + 2;
                    if (line.Contains("UNIT OPERATIONS") || line2.Contains("UNIT OPERATIONS"))
                    {
                        break;
                    }
                }
               
            }
            return sb.ToString();
            
        }

        public string CopyEqInfo(string[] lines, string eqName)
        {
            StringBuilder sb = new StringBuilder();
            string EqInfo = "UID=" + eqName.ToUpper();
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if ((line+",").Contains(EqInfo+","))
                {
                    i++;
                    sb.Append(line).Append("\r\n");
                    break;
                }
                else
                {
                    i++;
                }
            }

            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("     "))
                {
                    sb.Append(line).Append("\r\n");
                    i++;
                }
                else
                {
                    break;
                }
            }

            return sb.ToString();

        }

        public string FindProcessHxInfo(string[] lines, int start, string eqName, ref int errorTag)
        {
            StringBuilder sb = new StringBuilder();
            string EqInfo = "HX   UID=" + eqName.ToUpper();
            int i = start;
            while (i < lines.Length)
            {
                string line = lines[i];
                if ((line+",").Contains(EqInfo+","))
                {
                    string eqInfo = GetNewHXInfo(lines, i, eqName, ref errorTag);
                    sb.Append(eqInfo);
                    break;
                }
                else
                {
                    i++;
                }
            }
            return sb.ToString();

        }

        public string FindOtherHxInfo(string[] lines, int start, string eqName, ref int errorTag)
        {
            StringBuilder sb = new StringBuilder();
            string EqInfo = "HX   UID=" + eqName.ToUpper();
            int i = start;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Trim() == EqInfo)
                {
                    string eqInfo = GetNewOtherHXInfo(lines, i, eqName, ref errorTag);
                    sb.Append(eqInfo);
                    break;
                }
                else
                {
                    i++;
                }
            }
            return sb.ToString();

        }

        private string getStreamData(CustomStream stream)
        {
            StringBuilder data1 = new StringBuilder();
            string streamName = stream.StreamName;
            data1.Append("\tPROPERTY STREAM=").Append(streamName.ToUpper()).Append(",&\r\n");
            if (stream.Pressure < 0)
                stream.Pressure = 1e-6;
            data1.Append("\t PRESSURE(MPAG)=").Append(stream.Pressure).Append(",&\r\n");
            if (stream.Temperature < 0)
                stream.Temperature = 1e-6;
            data1.Append("\t TEMPERATURE(C)=").Append(stream.Temperature).Append(",&\r\n");
            double rate = stream.TotalMolarRate;
            if (rate == 0)
                rate = 1;
            data1.Append("\t RATE(KGM/S)=").Append(rate).Append(",&\r\n");
            string com = stream.TotalComposition;
            string Componentid = stream.Componentid;
            string CompIn = stream.CompIn;
            string PrintNumber = stream.PrintNumber;
            Dictionary<string, string> compdict = new Dictionary<string, string>();
            data1.Append("\t COMP=&\r\n");
            string[] coms = com.Split(',');
            string[] PrintNumbers = PrintNumber.Split(',');
            StringBuilder sbCom = new StringBuilder();
            string[] Componentids = Componentid.Split(',');
            string[] CompIns = CompIn.Split(',');
            int comCount = coms.Length;
            for (int i = 0; i < comCount; i++)
            {
                compdict.Add(Componentids[i], coms[i]);
            }
            for (int i = 0; i < comCount; i++)
            {
                //string s = CompIns[i];
                sbCom.Append("/&\r\n").Append(PrintNumbers[i]).Append(",").Append(coms[i]);
            }
            data1.Append("\t").Append(sbCom.Remove(0, 2)).Append("\r\n");
            return data1.ToString();
        }

        private int removeTagInfo(string[] lines, int start, string tag)
        {
            int end = start;
            int i = start;
            bool b = false;
            int len = tag.Trim().Length;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(tag) && line.Trim().Substring(0,len)==tag.Trim())
                {
                    b = true;
                    break;
                }
                else
                {
                    i++;
                }
            }
            if (b)
            {
                i = i + 1;
                while (i < lines.Length)
                {
                    string line = lines[i];
                    if (line.Contains("&"))
                    {
                        i++;
                    }
                    else
                    {
                        end = i;
                        break;
                    }

                }
            }
            return end;

        }

        private int removeRowTagInfo(string[] lines, int start, string tag)
        {
            int end = start;
            int i = start;
            int len = tag.Length;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(tag) && line.Substring(0, len) == tag)
                {
                    end = i+1;
                    break;
                }
                else
                {
                    i++;
                }
            }
            
            return end;

        }

        private List<CustomStream> GetReactorLoopStreamsFromProII(List<string> eqList)
        {
            List<CustomStream> list = new List<CustomStream>();
            ProIIEqDataDAL eqdal = new ProIIEqDataDAL();
            ProIIStreamDataDAL streamdal = new ProIIStreamDataDAL();
            foreach (string eq in eqList)
            {
                ProIIEqData eqData = eqdal.GetModel(SessionPF, SourceFileInfo.FileName, eq);
                string feeddata = eqData.FeedData;
                if (!string.IsNullOrEmpty(feeddata))
                {
                    string[] feeds = feeddata.Split(',');
                    foreach (string s in feeds)
                    {
                        if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                        {
                            streams.Add(s);
                            ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
                            CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                            if (cs != null)
                            {
                                list.Add(cs);
                            }
                        }
                    }
                }
                //string productdata = eqData.ProductData;
                //if (!string.IsNullOrEmpty(productdata))
                //{
                //    string[] products = productdata.Split(',');
                //    foreach (string s in products)
                //    {
                //        if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                //        {
                //            streams.Add(s);
                //            ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
                //            CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                //            if (cs != null)
                //            {
                //                list.Add(cs);
                //            }
                //        }
                //    }
                //}
            }
            return list;
        }

        private List<CustomStream> GetReactorLoopStreamsFromProII(string eqName)
        {
            ProIIEqDataDAL eqdal = new ProIIEqDataDAL();
            ProIIStreamDataDAL streamdal = new ProIIStreamDataDAL();
            List<CustomStream> list = new List<CustomStream>();
            ProIIEqData eqData = eqdal.GetModel(SessionPF, SourceFileInfo.FileName, eqName);
            string feeddata = eqData.FeedData;
            string[] feeds = feeddata.Split(',');
            foreach (string s in feeds)
            {
                if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                {
                    streams.Add(s);
                    ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                    list.Add(cs);
                }
            }
            string productdata = eqData.ProductData;
            string[] products = productdata.Split(',');
            foreach (string s in products)
            {
                if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
                {
                    streams.Add(s);
                    ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);
                    list.Add(cs);
                }
            }
            return list;
        }

        private CustomStream GetReactorLoopStreamInfoFromProII(string streamName)
        {
            CustomStream cs = null;
            string s = streamName;
            if (!streams.Contains(s) && !string.IsNullOrEmpty(s))
            {
                ProIIStreamDataDAL streamdal = new ProIIStreamDataDAL();
                streams.Add(s);
                ProIIStreamData piis = streamdal.GetModel(SessionPF, s, SourceFileInfo.FileName);
                cs = ProIIToDefault.ConvertProIIStreamToCustomStream(piis);

            }
            return cs;


        }

        //get eq duty,ltmd, 然后讲HX的值进行替换。


        private double GetLTMD(ISession sessionPlant, string fileName, string eqName, out double duty, out double ltmdfactor)
        {
            double tin=0;
            double tout=0;
            double Tin=0;
            double Tout=0;
            double ltmd = -1;
            
            ProIIEqData eqData = eqDAL.GetModel(sessionPlant, fileName, eqName);
            duty = double.Parse(eqData.DutyCalc);
            ltmdfactor = double.Parse(eqData.LmtdFactorCalc);
            string feeddata = eqData.FeedData;
            string[] feeds = feeddata.Split(',');
             string productdata = eqData.ProductData;
            string[] products = productdata.Split(',');
            if (feeds.Length >= 2 && products.Length >=2)
            {
                ProIIStreamData prostream = streamDAL.GetModel(sessionPlant, feeds[0], fileName);
                if(!string.IsNullOrEmpty(prostream.Temperature))
                {
                    tin = double.Parse(prostream.Temperature);
                    Tin = tin;
                }
                prostream = streamDAL.GetModel(sessionPlant, feeds[1], fileName);
                if (!string.IsNullOrEmpty(prostream.Temperature))
                {
                    double t = double.Parse(prostream.Temperature);
                    if (t > tin)
                        Tin = t;
                    else
                        tin = t;
                }

                prostream = streamDAL.GetModel(sessionPlant, products[0], fileName);
                if (!string.IsNullOrEmpty(prostream.Temperature))
                {
                    tout = double.Parse(prostream.Temperature);
                    Tout = tout;
                }
                prostream = streamDAL.GetModel(sessionPlant, products[1], fileName);
                if (!string.IsNullOrEmpty(prostream.Temperature))
                {
                    double t = double.Parse(prostream.Temperature);
                    if (t > tout)
                        Tout = t;
                    else
                        tout = t;
                }
                
                double a = Tin - tout;
                double b = Tout - tin;
                double c = Math.Log(a / b);
                ltmd = (a - b) / c;
            }
            return ltmd;
        }

        private string GetNewHXInfo(string[] lines, int start,string hxName,ref int errorTag)
        {
            StringBuilder sb = new StringBuilder();
            int i = start;
            string line = lines[i];
            line = line.Replace(", ZONES(OUTPUT)=5", "");
            sb.Append(line).Append("\r\n");
            i++;
            line = lines[i];
            sb.Append(line).Append("\r\n");
            i++;
            line = lines[i];
            sb.Append(line).Append("\r\n");
            i++;
            line = lines[i];
            sb.Append(line);
            double duty=0;
            //double ltmdfactor = 0;
            //double ltmd = GetLTMD(SessionPF, FileName, hxName, out  duty,out ltmdfactor);

            //三期再采用这个方法
            double ltmd = 0;// GetLTMD(SessionPF, FileName, hxName, out  duty);
            ProIIEqDataDAL eqDAL = new ProIIEqDataDAL();
            ProIIEqData eqData = eqDAL.GetModel(SessionPF, FileName, hxName);
            double LmtdCalc = double.Parse(eqData.LmtdCalc);
            if (LmtdCalc == 0 || LmtdCalc == 1)
            {
                errorTag = -1;
                return "";
            }
            ////////double LmtdFactorCalc = 0;
            ////////if (!string.IsNullOrEmpty(eqData.LmtdFactorCalc))
            ////////    LmtdFactorCalc=double.Parse(eqData.LmtdFactorCalc);
            ////////if (LmtdFactorCalc == 0 || LmtdFactorCalc == 1)
            ////////    ltmd = GetLTMD(SessionPF, FileName, hxName, out  duty, out ltmdfactor);
            ////////else
            ////////{
            ////////    //ltmd = LmtdCalc / LmtdFactorCalc;
            ////////    if(ltmd>200 || ltmd<1)
            ////////        ltmd = GetLTMD(SessionPF, FileName, hxName, out  duty, out ltmdfactor);
            ////////}
            duty = double.Parse(eqData.DutyCalc);
            double k = 0.3;  //  KW/m2-K
            double a = duty / LmtdCalc / k;  //   m2
            //if (ltmdfactor != 0)
            //    a = a / ltmdfactor;
            sb.Append(" ,U(KW/MK)=").Append(k).Append(",AREA(M2)=").Append(a).Append("\r\n");
            
            return sb.ToString();
            
        }


        private string GetNewOtherHXInfo(string[] lines, int start, string hxName, ref int errorTag)
        {
            ProIIEqData eqData=eqDAL.GetModel(SessionPF,SourceFileInfo.FileName,hxName);
            StringBuilder sb = new StringBuilder();
            string line = lines[start];
            line = line.Replace(", ZONES(OUTPUT)=5", "");
            sb.Append(line).Append("\r\n");
            List<string> list = new List<string>();
            bool b = false;
            string attrvalue = string.Empty;
            for (int i = start+1; i < lines.Length; i++)
            {
                line = lines[i];
                string key1 = "UID=";
                string key2 = "END";
                if (line.Contains(key1) || line.Trim()==key2)
                {
                    
                    if (!b)
                    {
                        attrvalue = "OPER Duty(KW)=" + double.Parse(eqData.DutyCalc)/1e6;
                        sb.Append(attrvalue).Append("\r\n");
                    }
                    break;
                }
                else if (line.Contains("OPER "))
                {
                    b = true;
                    attrvalue = "OPER Duty(KW)=" + double.Parse(eqData.DutyCalc) / 1e6;
                    sb.Append(attrvalue).Append("\r\n");
                }
                else
                {
                    sb.Append(line).Append("\r\n");
                }
            }


            
            return sb.ToString();

        }


        private string[] ReplaceStreamInfo(string[] lines, int start, string streamName, double temp)
        {
            List<string> list = new List<string>();
            int i = start;
            string temperKey="TEMPERATURE";
            string key = "PROPERTY STREAM=" + streamName.ToUpper();
            while (i < lines.Length)
            {
                string line = lines[i];
                if ((line+",").Contains(key+",") && line.Contains(temperKey))
                {
                    string oldValue=string.Empty;
                    string newValue=temperKey+temp;

                    int s=line.IndexOf(temperKey);
                    string sub=line.Substring(s);
                    s=sub.IndexOf(",");
                     oldValue=sub.Substring(0,s);
                    string newLine=line.Replace(oldValue,newValue);
                    list.Add(newLine);
                    break;
                }
            }
            while (i < lines.Length)
            {
                string line = lines[i];
                list.Add(line);
                i++;
            }
            return list.ToArray();
        }


        private void Detail(object o)
        {
            int type = int.Parse(o.ToString());
            string streamName = string.Empty;
            SourceModel sm=null;
            ReactorLoopSourceVM vm=null;
            switch (type)
            {
                case 1:
                    streamName = model.EffluentStream;
                    sm=model.EffluentStreamSource;
                    break;
                case 2:
                    streamName = model.EffluentStream2;
                    sm=model.EffluentStream2Source;
                    break;
                case 3:
                    streamName = model.CompressorH2Stream;
                    sm=model.CompressorH2StreamSource;
                    break;
                case 4:
                    streamName = model.ColdReactorFeedStream;
                    sm=model.ColdReactorFeedStreamSource;
                    break;
                case 5:
                    streamName = model.ColdReactorFeedStream2;
                    sm=model.ColdReactorFeedStream2Source;
                    break;
                case 6:
                    streamName = model.InjectionWaterStream;
                    sm=model.InjectionWaterStreamSource;
                    break;

            }
            if(string.IsNullOrEmpty(streamName))
            {
                return ;
            }
             vm = new ReactorLoopSourceVM(type, sm, SessionPF, SessionPS);
            if (!string.IsNullOrEmpty(streamName))
            {
                ReactorLoopSourceView v = new ReactorLoopSourceView();
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
               if( v.ShowDialog()==true)
               {
                switch (type)
                {
                    case 1:
                        streamName = model.EffluentStream;
                        model.EffluentStreamSource.SourceType=vm.model.SourceType;
                        break;
                    case 2:
                        streamName = model.EffluentStream2;
                        model.EffluentStream2Source.SourceType = vm.model.SourceType;
                        break;
                    case 3:
                        streamName = model.CompressorH2Stream;
                        model.CompressorH2StreamSource.SourceType = vm.model.SourceType;
                        break;
                    case 4:
                        streamName = model.ColdReactorFeedStream;
                        model.ColdReactorFeedStreamSource.SourceType = vm.model.SourceType;
                        break;
                    case 5:
                        streamName = model.ColdReactorFeedStream2;
                        model.ColdReactorFeedStream2Source.SourceType = vm.model.SourceType;
                        break;
                    case 6:
                        streamName = model.InjectionWaterStream;
                        model.InjectionWaterStreamSource.SourceType = vm.model.SourceType;
                        break;
                }
               }
            }
        }
    }



}
