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
using ProII;
using ReliefProCommon.CommonLib;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopVM
    {
        public ICommand OKCMD { get; set; }
        public ICommand SimulationCMD { get; set; }
        public ICommand ImportCMD { get; set; }
        public ICommand ProcessHXAddCMD { get; set; }
        public ICommand ProcessHXDelCMD { get; set; }
        public ICommand ProcessHXAdd2CMD { get; set; }
        public ICommand ProcessHXDel2CMD { get; set; }

        public ICommand UtilityHXAddCMD { get; set; }
        public ICommand UtilityHXDelCMD { get; set; }
        public ICommand UtilityHXAdd2CMD { get; set; }
        public ICommand UtilityHXDel2CMD { get; set; }

        public ICommand MixerSplitterAddCMD { get; set; }
        public ICommand MixerSplitterDelCMD { get; set; }
        public ICommand MixerSplitterAdd2CMD { get; set; }
        public ICommand MixerSplitterDel2CMD { get; set; }

        public ICommand NetworkHXAddCMD { get; set; }
        public ICommand NetworkHXDelCMD { get; set; }
        public ICommand NetworkHXAdd2CMD { get; set; }
        public ICommand NetworkHXDel2CMD { get; set; }

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
        private string sourcePrzFile;
        ProIIEqDataDAL eqDAL = new ProIIEqDataDAL();
        ProIIStreamDataDAL streamDAL = new ProIIStreamDataDAL();
        List<ReactorLoopEqDiff> eqDiffList;
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

            ProcessHXAdd2CMD = new DelegateCommand<object>(ProcessHXAdd2);
            ProcessHXDel2CMD = new DelegateCommand<object>(ProcessHXDel2);

            UtilityHXAdd2CMD = new DelegateCommand<object>(UtilityHXAdd2);
            UtilityHXDel2CMD = new DelegateCommand<object>(UtilityHXDel2);

            NetworkHXAdd2CMD = new DelegateCommand<object>(NetworkHXAdd2);
            NetworkHXDel2CMD = new DelegateCommand<object>(NetworkHXDel2);

            MixerSplitterAdd2CMD = new DelegateCommand<object>(MixerSplitterAdd2);
            MixerSplitterDel2CMD = new DelegateCommand<object>(MixerSplitterDel2);

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
            rlt.Add(string.Empty);
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
            rlt.Add(string.Empty);
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
                d.DetailType = 2;
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
                d.DetailType = 3;
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
                    model.EffluentStreamSource = InitSource(model.EffluentStream, "Pressurized Vessel");                    
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
                    model.EffluentStream2Source = InitSource(model.EffluentStream2, "Pressurized Vessel");      
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
                    model.CompressorH2StreamSource = InitSource(model.CompressorH2Stream, "Compressor(Steam Turbine Driven)");                         
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
                    model.ColdReactorFeedStreamSource = InitSource(model.ColdReactorFeedStream, "Pump(Motor)");  
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
                    model.ColdReactorFeedStream2Source = InitSource(model.ColdReactorFeedStream2, "Pump(Motor)");  
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
                    model.InjectionWaterStreamSource = InitSource(model.InjectionWaterStream, "Pump(Motor)");  
                }
                else
                {
                    Source sr = srdal.GetModel(SessionPS, model.InjectionWaterStream);
                    model.InjectionWaterStreamSource = new SourceModel(sr);
                }
            }



        }
        private SourceModel InitSource(string streamName,string sourceType)
        {
            Source sr = new Source();
            SourceModel m = new SourceModel(sr);
            m.StreamName = streamName;
            m.SourceType = sourceType;
            m.SourceType_Color = ColorBorder.green.ToString();
            return m;
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
                model.HotHighPressureSeparator_Color = ColorBorder.green.ToString();
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

        private void ProcessHXAdd2(object obj)
        {
            for (int i = 0; i < model.ObcProcessHXSource.Count; i++)
            {
                ReactorLoopDetail d = model.ObcProcessHXSource[i];
                model.ObcProcessHX.Add(d);
                var find = model.ObcNetworkHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                model.ObcNetworkHXSource.Remove(find);                
            }
            model.ObcProcessHXSource.Clear();
        }
        private void ProcessHXDel2(object obj)
        {
            for (int i = 0; i < model.ObcProcessHX.Count; i++)
            {
                ReactorLoopDetail d = model.ObcProcessHX[i];
                model.ObcProcessHXSource.Add(d);

                ReactorLoopDetail detail3 = d;
                detail3.ReactorType = 3;
                model.ObcNetworkHXSource.Add(detail3);
            }

            model.ObcProcessHX.Clear();

        }

        private void UtilityHXAdd2(object obj)
        {
            
            for (int i = 0; i < model.ObcUtilityHXSource.Count; i++)
            {
                ReactorLoopDetail d = model.ObcUtilityHXSource[i];
                model.ObcUtilityHX.Add(d);
                var find = model.ObcNetworkHXSource.FirstOrDefault(p => p.DetailInfo == d.DetailInfo);
                model.ObcNetworkHXSource.Remove(find);
                
            }
            model.ObcUtilityHXSource.Clear();
        }
        private void UtilityHXDel2(object obj)
        {
            for (int i = 0; i < model.ObcUtilityHX.Count; i++)
            {
                ReactorLoopDetail d = model.ObcUtilityHX[i];
                model.ObcUtilityHXSource.Add(d);

                ReactorLoopDetail detail3 = d;
                detail3.ReactorType = 3;
                model.ObcNetworkHXSource.Add(detail3);
            }
            model.ObcUtilityHX.Clear();
           
        }

        private void NetworkHXAdd2(object obj)
        {            
            for (int i = 0; i < model.ObcNetworkHXSource.Count; i++)
            {
                ReactorLoopDetail d = model.ObcNetworkHXSource[i];
                model.ObcNetworkHX.Add(d);
                string str = d.DetailInfo;
                var find2 = model.ObcUtilityHXSource.FirstOrDefault(p => p.DetailInfo == str);
                if (find2 != null)
                {
                    model.ObcUtilityHXSource.Remove(find2);
                }
                var find3 = model.ObcProcessHXSource.FirstOrDefault(p => p.DetailInfo == str);
                if (find3 != null)
                {
                    model.ObcProcessHXSource.Remove(find3);
                }
                
            }
            model.ObcNetworkHXSource.Clear();
        }
        private void NetworkHXDel2(object obj)
        {            
            for (int i = 0; i < model.ObcNetworkHX.Count; i++)
            {
                ReactorLoopDetail d = model.ObcNetworkHX[i];
                string str = d.DetailInfo;
                model.ObcNetworkHXSource.Add(d);

                if (d.ReactorLoopID_Color == "1")
                {
                    ReactorLoopDetail detail3 = d;
                    detail3.ReactorType = 1;
                    model.ObcUtilityHXSource.Add(detail3);
                }
                else
                {
                    ReactorLoopDetail detail0 = d;
                    detail0.ReactorType = 0;
                    model.ObcProcessHXSource.Add(detail0);
                }
                
                
            }
            model.ObcNetworkHX.Clear();
        }

        private void MixerSplitterAdd2(object obj)
        {
            for (int i = 0; i < model.ObcMixerSplitterSource.Count; i++)
            {
                ReactorLoopDetail d = model.ObcMixerSplitterSource[i];
                model.ObcMixerSplitter.Add(d);
            }

            model.ObcMixerSplitterSource.Clear();

        }
        private void MixerSplitterDel2(object obj)
        {
            for (int i = 0; i < model.ObcMixerSplitter.Count; i++)
            {
                ReactorLoopDetail d = model.ObcMixerSplitter[i];
                model.ObcMixerSplitterSource.Add(d);
            }

            model.ObcMixerSplitter.Clear();
            
        }




        private void Import(object obj)
        {
            SelectPathView view = new SelectPathView();
            SelectPathVM vm = new SelectPathVM(SessionPF);
            view.DataContext = vm;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (view.ShowDialog() == true)
            {
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
            string sourcePrzFile = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
            string newInpFile = DirProtectedSystem + @"\myrp\myrp.inp";
            string newPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            List<string> hxList = new List<string>();
            List<string> flashList = new List<string>();
            foreach (ReactorLoopDetail d in model.ObcProcessHX)
            {
                hxList.Add(d.DetailInfo);
            }
            foreach (ReactorLoopDetail d in model.ObcUtilityHX)
            {
                hxList.Add(d.DetailInfo);
            }
            foreach (ReactorLoopDetail d in model.ObcNetworkHX)
            {
                hxList.Add(d.DetailInfo);
            }
            string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
            string[] files = Directory.GetFiles(dir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            bool IsHXOK=CheckHXData(lines,hxList);
            if (!IsHXOK)
            {
                MessageBox.Show("Error:Hot side feed temperature is Lower than cold side outlet temperature detected for HX,Please correct your model and re-import.","Message Box",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(model.ColdHighPressureSeparator))
            {
                flashList.Add(model.ColdHighPressureSeparator);
            }
            if (!string.IsNullOrEmpty(model.HotHighPressureSeparator))
            {
                flashList.Add(model.HotHighPressureSeparator);
            }
            if (System.IO.File.Exists(newPrzFile))
            {
                if (MessageBox.Show("Do you want to rewrite inp file", "Message Box", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    CreateInpFile();
                }
            }
            else
            {
                CreateInpFile();
            }

            
            ReactorLoopSimulationView v = new ReactorLoopSimulationView();           
            ReactorLoopSimulationVM vm = new ReactorLoopSimulationVM(reactorLoopID,newInpFile,sourcePrzFile,SourceFileInfo.FileVersion,hxList,flashList,SessionPS,SessionPF,model.IsSolved,model.IsMatched,eqDiffList);
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            v.DataContext = vm;
            if (v.ShowDialog() == true)
            {
                model.IsMatched = vm.IsMatched;
                model.IsSolved = vm.IsSolved;
                model.IsSolved_Color = vm.IsSolved_Color;
                model.IsMatched_Color = vm.IsMatched_Color;
                eqDiffList = vm.lst;
            }

        }
        /// <summary>
        /// 组合生成inp文件
        /// </summary>
        private void CreateInpFile()
        {
            string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
            string[] files = Directory.GetFiles(dir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);

            string newInpFile = DirProtectedSystem + @"\myrp\myrp.inp";
            streams.Clear();
            List<CustomStream> csList = new List<CustomStream>();
            List<string> eqList = new List<string>();
            List<string> processHxList = new List<string>();
            List<string> otherHxList = new List<string>();
            List<string> splitterList = new List<string>();
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
                if (d.DetailType == 3)
                {
                    splitterList.Add(d.DetailInfo);
                }
                else
                {
                    eqList.Add(d.DetailInfo);
                }
                               
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
            List<CustomStream> csList4 = GetReactorLoopStreamsFromProII(splitterList);
            csList = csList.Union(csList2).ToList();
            csList = csList.Union(csList3).ToList();
            csList = csList.Union(csList4).ToList();
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


            
            int errorTag = 0;
            string inpData = CreateReactorLoopInpData(lines, csList, eqList, processHxList, otherHxList,splitterList, ref errorTag);

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
        }

        private void Save(object obj)
        {
            string newPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            if (!System.IO.File.Exists(newPrzFile))
            {
                MessageBox.Show("Please Simulate First.","Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
            if (!model.IsSolved)
            {
                MessageBox.Show("Simulation not solved,the result can't be saved.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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
                    else
                    {
                        MessageBoxResult r = MessageBox.Show("Are you sure to rewrite all data?", "Message Box", MessageBoxButton.YesNo);
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
            if (model.EffluentStreamSource == null)
            {
                model.EffluentStreamSource = InitSource(model.EffluentStream, "Pressurized Vessel");
            }
            model.EffluentStreamSource.dbmodel.SourceType = model.EffluentStreamSource.SourceType;
            model.EffluentStreamSource.SourceType_Color = model.EffluentStreamSource.SourceType_Color;
            lstStreamSource.Add(model.EffluentStreamSource.dbmodel);

            if (model.EffluentStream2Source == null)
            {
                model.EffluentStream2Source = InitSource(model.EffluentStream2, "Pressurized Vessel");
            }
            model.EffluentStream2Source.dbmodel.SourceType = model.EffluentStream2Source.SourceType;
            model.EffluentStream2Source.SourceType_Color = model.EffluentStream2Source.SourceType_Color;
            lstStreamSource.Add(model.EffluentStream2Source.dbmodel);

            if (model.CompressorH2StreamSource == null)
            {
                model.CompressorH2StreamSource = InitSource(model.CompressorH2Stream, "Compressor(Steam Turbine Driven)");
            }
            model.CompressorH2StreamSource.dbmodel.SourceType = model.CompressorH2StreamSource.SourceType;
            model.CompressorH2StreamSource.SourceType_Color = model.CompressorH2StreamSource.SourceType_Color;
            lstStreamSource.Add(model.CompressorH2StreamSource.dbmodel);

            if (model.ColdReactorFeedStreamSource == null)
            {
                model.ColdReactorFeedStreamSource = InitSource(model.ColdReactorFeedStream, "Pump(Motor)");
            }

            model.ColdReactorFeedStreamSource.dbmodel.SourceType = model.ColdReactorFeedStreamSource.SourceType;
            model.ColdReactorFeedStreamSource.SourceType_Color = model.ColdReactorFeedStreamSource.SourceType_Color;
            lstStreamSource.Add(model.ColdReactorFeedStreamSource.dbmodel);

            if (model.ColdReactorFeedStream2Source == null)
            {
                model.ColdReactorFeedStream2Source = InitSource(model.ColdReactorFeedStream2, "Pump(Motor)");
            }
            model.ColdReactorFeedStream2Source.dbmodel.SourceType = model.ColdReactorFeedStream2Source.SourceType;
            model.ColdReactorFeedStream2Source.SourceType_Color = model.ColdReactorFeedStream2Source.SourceType_Color;
            lstStreamSource.Add(model.ColdReactorFeedStream2Source.dbmodel);

            if (model.InjectionWaterStreamSource == null)
            {
                model.InjectionWaterStreamSource = InitSource(model.InjectionWaterStream, "Pump(Motor)");
            }
            model.InjectionWaterStreamSource.dbmodel.SourceType = model.InjectionWaterStreamSource.SourceType;
            model.InjectionWaterStreamSource.SourceType_Color = model.InjectionWaterStreamSource.SourceType_Color;
            lstStreamSource.Add(model.InjectionWaterStreamSource.dbmodel);

            reactorBLL.Save(model.dbModel, allSelectedInfo, lstStreamSource,eqDiffList);
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
        /// 生成inp文件的数据
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="streamList"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public string CreateReactorLoopInpData(string[] lines, List<CustomStream> csList, List<string> eqList, List<string> processHxList, List<string> otherHxList, List<string> splitterList, ref int errorTag)
        {
            StringBuilder sb = new StringBuilder();            
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
                    sb.Append(line).Append("\r\n");
                    i++;
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
                string line = lines[i];
                if (line.Contains("UNIT OPERATIONS"))
                {
                    break;
                }
                else
                {
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

            i = unitStart;
            while (i < lines.Length)
            {
                foreach (string eq in splitterList)
                {
                    string eqinfo = FindSplitterInfo(lines, i, eq);
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
                int idx = line.IndexOf(",");
                if (idx > 0)
                    line = line.Substring(0,idx);
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

        public string FindSplitterInfo(string[] lines, int start, string eqName)
        {
            StringBuilder sb = new StringBuilder();
            string EqInfo = "SPLITTER UID=" + eqName.ToUpper();
            int i = start;
            while (i < lines.Length)
            {
                string line = lines[i];
                if ((line + ",").Contains(EqInfo + ","))
                {
                    string eqInfo = GetSplitterInfo(lines, i, eqName);
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
                int idx = line.IndexOf(",");
                if (idx > 0)
                    line = line.Substring(0, idx);
                if ((line + ",").Contains(EqInfo + ","))
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
                        if (line.Contains("COMPONENT DATA"))
                        {
                            end = i;
                        }
                        else
                        {
                            end = i + 1;
                        }
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
            bool b = false;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains(tag) && line.Substring(0, len) == tag)
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
                    if (line.Substring(0,2)=="  ")
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


        private int removeRowTagInfo2(string[] lines, int start, string tag)
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


        private double GetLMTD(ISession sessionPlant, string fileName, string eqName, out double duty, out double lmtdfactor)
        {
            double tin=0;
            double tout=0;
            double Tin=0;
            double Tout=0;
            double lmtd = -1;
            
            ProIIEqData eqData = eqDAL.GetModel(sessionPlant, fileName, eqName);
            duty = double.Parse(eqData.DutyCalc);
            lmtdfactor = double.Parse(eqData.LmtdFactorCalc);
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
                lmtd = (a - b) / c;
            }
            return lmtd;
        }

        private string GetNewHXInfo(string[] lines, int start,string hxName,ref int errorTag)
        {
            bool b = CheckHXColdHotTemperature(lines, start, hxName);            
            StringBuilder sb = new StringBuilder();
            int i = start;
            string line = lines[i];
            line = line.Replace(", ZONES(OUTPUT)=5", "");
            sb.Append(line).Append("\r\n");
            i++;
            line = lines[i];
            if (!b)
            {
                line = line.Replace("HOT  FEED", "COLD  FEED");
            }
            sb.Append(line).Append("\r\n");
            i++;
            line = lines[i];
            if (!b)
            {
                line = line.Replace("COLD FEED", "HOT  FEED");
            }
            sb.Append(line).Append("\r\n");
            
            double duty=0;
            double lmtdfactor = 0;            
            //三期再采用这个方法
            double lmtd = 0;// GetLTMD(SessionPF, FileName, hxName, out  duty);
            ProIIEqDataDAL eqDAL = new ProIIEqDataDAL();
            ProIIEqData eqData = eqDAL.GetModel(SessionPF, FileName, hxName);
            double LmtdCalc = double.Parse(eqData.LmtdCalc);
            if (LmtdCalc == 0 || LmtdCalc == 1)
            {
                errorTag = -1;
                return "";
            }
           
            if (b)
            {
                duty = double.Parse(eqData.DutyCalc);               
            }
            else
            {
                double LmtdFactorCalc = 0;
                if (!string.IsNullOrEmpty(eqData.LmtdFactorCalc))
                    LmtdFactorCalc = double.Parse(eqData.LmtdFactorCalc);
                if (LmtdFactorCalc == 0 || LmtdFactorCalc == 1)
                    lmtd = GetLMTD(SessionPF, FileName, hxName, out  duty, out lmtdfactor);
                else
                {
                    lmtd = LmtdCalc / LmtdFactorCalc;
                    if (lmtd > 200 || lmtd < 1)
                        lmtd = GetLMTD(SessionPF, FileName, hxName, out  duty, out lmtdfactor);
                }
                LmtdCalc = lmtd;
            }
            double k = 0.3;  //  KW/m2-K
            double a = duty / LmtdCalc / k;  //   m2

            if (b)
            {
                sb.Append("CONFIGURE COUNTER,  U(KW/MK)=").Append(k).Append(", AREA(M2)=").Append(a).Append("\r\n");
            }
            else
            {
                sb.Append("CONFIGURE COUNTER, FT=1, U(KW/MK)=").Append(k).Append(", AREA(M2)=").Append(a).Append("\r\n");
            }
            return sb.ToString();
            
        }


        private string GetNewOtherHXInfo(string[] lines, int start, string hxName, ref int errorTag)
        {
            bool bTemp = CheckHXColdHotTemperature(lines, start, hxName);           
            ProIIEqData eqData=eqDAL.GetModel(SessionPF,SourceFileInfo.FileName,hxName);
            StringBuilder sb = new StringBuilder();
            string line = lines[start];
            line = line.Replace(", ZONES(OUTPUT)=5", "");

            sb.Append(line).Append("\r\n");
            List<string> list = new List<string>();
            bool bOPER = false;
            //bool bCONFIGURE=false;
            string attrvalue = string.Empty;
            for (int i = start+1; i < lines.Length; i++)
            {
                line = lines[i];
                string key1 = "UID=";
                string key2 = "END";
                if (line.Contains(key1) || line.Trim()==key2)
                {
                    if (!bOPER)
                    {
                        //if (!bCONFIGURE)
                        //{
                        //    sb.Append("CONFIGURE COUNTER, FT=1");
                        //}
                        attrvalue = "OPER Duty(KW)=" + double.Parse(eqData.DutyCalc)/1e6;
                        sb.Append(attrvalue).Append("\r\n");
                    }
                    break;
                }
                else if (line.Contains("OPER "))
                {
                    bOPER = true;
                    //if (!bCONFIGURE)
                    //{
                    //    sb.Append("CONFIGURE COUNTER, FT=1");
                    //}
                    attrvalue = "OPER Duty(KW)=" + double.Parse(eqData.DutyCalc) / 1e6;
                    sb.Append(attrvalue).Append("\r\n");
                }
                //else if (line.Contains("CONFIGURE COUNTER"))
                //{
                //    bCONFIGURE = true;
                //    sb.Append("CONFIGURE COUNTER, FT=1");
                //}
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

        private string GetSplitterInfo(string[] lines, int start, string splitterName)
        {
            StringBuilder sb = new StringBuilder();
            ProIIEqData eqData = eqDAL.GetModel(SessionPF, SourceFileInfo.FileName, splitterName);
            string[] feeds = eqData.FeedData.Split(',');
            string[] products = eqData.ProductData.Split(',');
            double REFFEED = 0;
            foreach (string s in feeds)
            {
                ProIIStreamData sdata = streamDAL.GetModel(SessionPF, s, SourceFileInfo.FileName);
                REFFEED = REFFEED + double.Parse(sdata.TotalMolarRate) * double.Parse(sdata.BulkMwOfPhase);
            }
            
            int i = start;
            sb.Append(lines[i]).Append("\r\n");
            sb.Append(lines[i+1]).Append("\r\n");
            sb.Append(lines[i+2]).Append("\r\n");
            sb.Append(lines[i+3]).Append("\r\n");

            for (i = 1; i < products.Length; i++)
            {
                string s = products[i];
                ProIIStreamData sdata = streamDAL.GetModel(SessionPF, s, SourceFileInfo.FileName);
                double w = double.Parse(sdata.TotalMolarRate) * double.Parse(sdata.BulkMwOfPhase);
                double value=w/REFFEED;
                sb.Append("\t  SPEC STREAM=").Append(s).Append(", RATE(WT,KG/H),TOTAL,WET, DIVIDE, REFFEED, &\r\n");
                sb.Append("\t  RATE(WT,KG/H),WET, VALUE=").Append(value).Append("\r\n");
            }


            return sb.ToString();
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
                    if (model.EffluentStreamSource == null)
                    {
                        model.EffluentStreamSource = InitSource(model.EffluentStream, "Pressurized Vessel");
                    }
                    sm=model.EffluentStreamSource;
                    break;
                case 2:
                    streamName = model.EffluentStream2;
                    if (model.EffluentStream2Source == null)
                    {
                        model.EffluentStream2Source = InitSource(model.EffluentStream2, "Pressurized Vessel");
                    }
                    sm=model.EffluentStream2Source;
                    break;
                case 3:
                    streamName = model.CompressorH2Stream;
                    if (model.CompressorH2StreamSource == null)
                    {
                        model.CompressorH2StreamSource = InitSource(model.CompressorH2Stream, "Compressor(Steam Turbine Driven)");
                    }
                    sm=model.CompressorH2StreamSource;
                    break;
                case 4:
                    streamName = model.ColdReactorFeedStream;
                    if (model.ColdReactorFeedStreamSource == null)
                    {
                        model.ColdReactorFeedStreamSource = InitSource(model.ColdReactorFeedStream, "Pump(Motor)");
                    }
                    sm=model.ColdReactorFeedStreamSource;
                    break;
                case 5:
                    streamName = model.ColdReactorFeedStream2;
                    if (model.ColdReactorFeedStream2Source == null)
                    {
                        model.ColdReactorFeedStream2Source = InitSource(model.ColdReactorFeedStream2, "Pump(Motor)");
                    }
                    sm=model.ColdReactorFeedStream2Source;
                    break;
                case 6:
                    streamName = model.InjectionWaterStream;
                    if (model.InjectionWaterStreamSource == null)
                    {
                        model.InjectionWaterStreamSource = InitSource(model.InjectionWaterStream, "Pump(Motor)");
                    }
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
                if (v.ShowDialog() == true)
                {
                    switch (type)
                    {
                        case 1:
                            streamName = model.EffluentStream;
                            model.EffluentStreamSource.SourceType = vm.model.SourceType;
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


        private bool CheckHXColdHotTemperature(string[] lines,int start,string hx)
        {
            bool b = true;
            ProIIEqData eqdata = eqDAL.GetModel(SessionPF, SourceFileInfo.FileName, hx);
            if (eqdata.ProductData.Contains(","))
            {
                List<string> hotFeeds = new List<string>();
                List<string> coldFeeds = new List<string>();
                string hotProduct = string.Empty;
                string coldProduct = string.Empty;
                double hotFeedTemperature = 0;
                double coldFeedTemperature = 0;
                int j = start + 1;
                string line = lines[j].Trim();                
                GetHXStreamInfo(line, ref hotFeeds, ref hotProduct);                
                j++;
                line = lines[j].Trim();
                GetHXStreamInfo(line, ref coldFeeds, ref coldProduct);
                ProIIStreamData proiihotfeed = streamDAL.GetModel(SessionPF, hotFeeds[0], SourceFileInfo.FileName);
                if (hotFeeds.Count == 1)
                {
                    hotFeedTemperature = double.Parse(proiihotfeed.Temperature);
                }
                else
                {
                    // do Mixer
                    hotFeedTemperature = GetMixerTemperature(hotFeeds);                    
                }
                ProIIStreamData proiicoldfeed = streamDAL.GetModel(SessionPF, coldFeeds[0], SourceFileInfo.FileName);
                if (coldFeeds.Count == 1)
                {
                    coldFeedTemperature = double.Parse(proiicoldfeed.Temperature);
                }
                else
                {
                    // do Mixer
                    coldFeedTemperature = GetMixerTemperature(coldFeeds);
                }
                if (hotFeedTemperature < coldFeedTemperature)
                {
                    b = false;
                }


            }
            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hxList"></param>
        /// <returns></returns>
        private bool CheckHXData(string[] lines,List<string> hxList)
        {
            bool b = true;

            int i=4;
            int length=lines.Length;
            while (i < length)
            {
                if (lines[i].Contains("UNIT OPERATIONS"))
                {
                    i++;
                    break;
                }
                else
                {
                    i++;
                }
            }

            int start = i;
            foreach (string hx in hxList)
            {
                ProIIEqData eqdata = eqDAL.GetModel(SessionPF, SourceFileInfo.FileName, hx);
                if (!eqdata.ProductData.Contains(",")) 
                {
                    break;
                }
                int j = start;
                while (j < length)
                {
                    List<string> hotFeeds = new List<string>();
                    List<string> coldFeeds = new List<string>();
                    string hotProduct = string.Empty;
                    string coldProduct = string.Empty;
                    string line = lines[j];
                    if ((line + ",").Contains("HX   UID=" + hx + ","))
                    {
                        j++;
                        line = lines[j].Trim();
                        if (line.Substring(0, 3) == "HOT")
                        {
                            GetHXStreamInfo(line, ref hotFeeds, ref hotProduct);
                        }
                        j++;
                        line = lines[j].Trim();
                        GetHXStreamInfo(line, ref coldFeeds, ref coldProduct);
                        ProIIStreamData proiihotfeed = streamDAL.GetModel(SessionPF, hotFeeds[0], SourceFileInfo.FileName);
                        if (hotFeeds.Count == 1)
                        {
                            ProIIStreamData proiicoldproduct = streamDAL.GetModel(SessionPF, coldProduct, SourceFileInfo.FileName);
                            if (double.Parse(proiicoldproduct.Temperature) > double.Parse(proiihotfeed.Temperature))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // do Mixer
                            double mixTemperature = GetMixerTemperature(hotFeeds);
                            if (mixTemperature > double.Parse(proiihotfeed.Temperature))
                            {
                                return false;
                            }
                        }
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            return b;
        }

        private void GetHXStreamInfo(string line,ref List<string> feeds,ref string product)
        {            
            int begin = line.IndexOf("FEED=");
            int end1 = line.IndexOf(", M=");
            string strFeed = line.Substring(begin + 5, end1 - begin - 5);
            string[] arrFeed = strFeed.Split(',');
            foreach (string s in arrFeed)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    feeds.Add(s);
                }
            }

            int end2 = line.IndexOf(", DP=") - 1;
            if (end2 == -2)
            {
                end2 = line.Length - 1;
            }
            product = line.Substring(end1 + 4, end2 - end1 - 3);
        }

        private double GetMixerTemperature(List<string> feedList)
        {
            double temperature = 0;
            string sourceDir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
            string tempdir = DirProtectedSystem + @"\temp\";
            if (!Directory.Exists(tempdir))
            {
                Directory.CreateDirectory(tempdir);
            }
            string dirMix = tempdir + "Mix";
            if (!Directory.Exists(dirMix))
            {
                Directory.CreateDirectory(dirMix);
            }
            StringBuilder sb = new StringBuilder();
            string content= PROIIFileOperator.getUsableContent(feedList, sourceDir);

            List<CustomStream> csList=new List<CustomStream>();
            foreach(string s in feedList)
            {
                ProIIStreamData data=streamDAL.GetModel(SessionPF,s,SourceFileInfo.FileName);
                CustomStream cs=ProIIToDefault.ConvertProIIStreamToCustomStream(data);
                csList.Add(cs);
            }

            int ImportResult = 1;
            int RunResult = 1;
            IMixCalculate mixcalc = ProIIFactory.CreateMixCalculate(SourceFileInfo.FileVersion);
            string mixProduct = Guid.NewGuid().ToString().Substring(0, 6);
            string tray1_f = mixcalc.Calculate(content.ToString(),csList , mixProduct, dirMix, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIProduct = reader.GetSteamInfo(mixProduct);                    
                    reader.ReleaseProIIReader();
                    temperature = double.Parse(proIIProduct.Temperature);
                }
            }
            return temperature;
        }

        
    }



}
