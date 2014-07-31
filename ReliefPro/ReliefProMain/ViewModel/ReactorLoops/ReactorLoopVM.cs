using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProDAL;
using ReliefProModel;
using ReliefProMain.View;

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

        private ISession SessionPS;
        private ISession SessionPF;
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }
        public string PrzFile { set; get; }
        public string PrzVersion { set; get; }

        private int reactorLoopID;
        public ReactorLoopModel model { get; set; }
        private ReactorLoopBLL reactorBLL;

        private void InitCMD()
        {
            OKCMD = new DelegateCommand<object>(Save);
            SimulationCMD = new DelegateCommand<object>(Simulation);
            ImportCMD = new DelegateCommand<object>(Import);
            ProcessHXAddCMD = new DelegateCommand<object>(ProcessHXAdd);
            ProcessHXDelCMD = new DelegateCommand<object>(ProcessHXDel);

            UtilityHXAddCMD = new DelegateCommand<object>(UtilityHXAdd);
            UtilityHXDelCMD = new DelegateCommand<object>(UtilityHXDel);

            MixerSplitterAddCMD = new DelegateCommand<object>(MixerSplitterAdd);
            MixerSplitterDelCMD = new DelegateCommand<object>(MixerSplitterDel);
        }
        private void InitPage()
        {
            if (!string.IsNullOrEmpty(PrzFile))
            {
                ObservableCollection<string> list = GetProIIStreamNames();
                
                model.ColdHighPressureSeparatorSource = GetProIIFlashs();
                model.HotHighPressureSeparatorSource = GetProIIFlashs();
                model.EffluentStreamSource = list;
                model.ColdReactorFeedStreamSource = list;
                model.HXNetworkColdStreamSource = list;
                model.InjectionWaterStreamSource = list;
                model.ObcProcessHXSource = GetProIIHXs(0);
                model.ObcUtilityHXSource = GetProIIHXs(1);
                model.ObcMixerSplitterSource = GetProIIMixers();

                model.ObcProcessHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcUtilityHX = new ObservableCollection<ReactorLoopDetail>();
                model.ObcMixerSplitter = new ObservableCollection<ReactorLoopDetail>();

            
            }
        }
        private ObservableCollection<string> GetProIIStreamNames()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIStreamDataDAL dal = new ProIIStreamDataDAL();
            IList<ProIIStreamData> list = dal.GetAllList(SessionPF,PrzFile);
            foreach(ProIIStreamData s in list)
            {
                rlt.Add(s.StreamName);
            }
            return rlt;
        }
        private ObservableCollection<string> GetProIIFlashs()
        {
            ObservableCollection<string> rlt = new ObservableCollection<string>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, PrzFile, "Flash");
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
            IList<ProIIEqData> list = dal.GetAllList(SessionPF,PrzFile,"Hx");
            foreach (ProIIEqData eq in list)
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = reactorType;
                rlt.Add(d);
            }
            return rlt;
        }
        private ObservableCollection<ReactorLoopDetail> GetProIIMixers()
        {
            ObservableCollection<ReactorLoopDetail> rlt = new ObservableCollection<ReactorLoopDetail>();
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IList<ProIIEqData> list = dal.GetAllList(SessionPF, PrzFile, "Mixer");
            foreach (ProIIEqData eq in list)
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            IList<ProIIEqData> list2 = dal.GetAllList(SessionPF, PrzFile, "Spliter");
            foreach (ProIIEqData eq in list2)
            {
                ReactorLoopDetail d = new ReactorLoopDetail();
                d.DetailInfo = eq.EqName;
                d.ReactorType = 2;
                rlt.Add(d);
            }
            return rlt;
        }

        public ReactorLoopVM(string przFile,string przVersion, ISession SessionPF,ISession SessionPS, string dirPlant,string dirProtectedSystem)
        {
            model = new ReactorLoopModel();
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            PrzFile = przFile;
            PrzVersion = przVersion;
            InitCMD();
            InitPage();

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var RLModel = reactorBLL.GetReactorLoopModel();

            model.dbModel = RLModel;
            if (RLModel.ID > 0)
            {
                reactorLoopID = RLModel.ID;
                model.ObcProcessHX = reactorBLL.GetProcessHX(reactorLoopID);
                model.ObcUtilityHX = reactorBLL.GetUtilityHX(reactorLoopID);
                model.ObcMixerSplitter = reactorBLL.GetMixerSplitter(reactorLoopID);
            }
            else
            {
                
            }
        }
        private void ProcessHXAdd(object obj)
        {
            model.ObcProcessHX.Add(model.SelectedHXModel);
            var find = model.ObcProcessHXSource.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
            model.ObcProcessHXSource.Remove(find);
        }
        private void ProcessHXDel(object obj)
        {
            model.ObcProcessHXSource.Add(model.SelectedHXModel);
            var find = model.ObcProcessHX.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
            model.ObcProcessHX.Remove(find);
        }
        private void UtilityHXAdd(object obj)
        {
            model.ObcUtilityHX.Add(model.SelectedHXModel);
            var find = model.ObcUtilityHXSource.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
            model.ObcUtilityHXSource.Remove(find);
        }
        private void UtilityHXDel(object obj)
        {
            model.ObcUtilityHXSource.Add(model.SelectedHXModel);
            var find = model.ObcUtilityHX.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
            model.ObcUtilityHX.Remove(find);
        }
        private void MixerSplitterAdd(object obj)
        {
            model.ObcMixerSplitter.Add(model.SelectedHXModel);
            var find = model.ObcMixerSplitterSource.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
            model.ObcMixerSplitter.Remove(find);
        }
        private void MixerSplitterDel(object obj)
        {
            model.ObcMixerSplitterSource.Add(model.SelectedHXModel);
            var find = model.ObcMixerSplitter.FirstOrDefault(p => p.DetailInfo == model.SelectedHXModel.DetailInfo && p.ReactorType == 0);
            model.ObcMixerSplitter.Remove(find);
        }
        private void Import(object obj)
        {
            SelectPathView view = new SelectPathView();
            SelectPathVM vm = new SelectPathVM(DirPlant);
            view.DataContext = vm;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (view.ShowDialog() == true)
            {
                PrzFile = vm.SelectedFile+".prz";
                InitPage();
            }
        }
        private void Simulation(object obj)
        {
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    var allSelectedInfo = new ObservableCollection<ReactorLoopDetail>();
                    foreach (var hx in model.ObcProcessHX)
                    {
                        hx.ID = 0;
                        allSelectedInfo.Add(hx);
                    }
                    foreach (var hx in model.ObcUtilityHX)
                    {
                        hx.ID = 0;
                        allSelectedInfo.Add(hx);
                    }
                    foreach (var hx in model.ObcMixerSplitter)
                    {
                        hx.ID = 0;
                        allSelectedInfo.Add(hx);
                    }
                    reactorBLL.Save(model.dbModel, allSelectedInfo);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
