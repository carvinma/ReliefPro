using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.ReactorLoops;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopVM
    {
        public ICommand OKCMD { get; set; }
        public ICommand SimulationCMD { get; set; }

        public ICommand ProcessHXAddCMD { get; set; }
        public ICommand ProcessHXDelCMD { get; set; }

        public ICommand UtilityHXAddCMD { get; set; }
        public ICommand UtilityHXDelCMD { get; set; }

        public ICommand MixerSplitterAddCMD { get; set; }
        public ICommand MixerSplitterDelCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;

        private int reactorLoopID;
        public ReactorLoopModel model { get; set; }
        private ReactorLoopBLL reactorBLL;

        private void InitCMD()
        {
            OKCMD = new DelegateCommand<object>(Save);
            SimulationCMD = new DelegateCommand<object>(Simulation);

            ProcessHXAddCMD = new DelegateCommand<object>(ProcessHXAdd);
            ProcessHXDelCMD = new DelegateCommand<object>(ProcessHXDel);

            UtilityHXAddCMD = new DelegateCommand<object>(UtilityHXAdd);
            UtilityHXDelCMD = new DelegateCommand<object>(UtilityHXDel);

            MixerSplitterAddCMD = new DelegateCommand<object>(MixerSplitterAdd);
            MixerSplitterDelCMD = new DelegateCommand<object>(MixerSplitterDel);
        }
        private void InitPage()
        {
            model.EffluentStreamSource = new List<string> { };
            model.ColdHighPressureSeparatorSource = new List<string> { };
            model.HotHighPressureSeparatorSource = new List<string> { };
            model.ColdReactorFeedStreamSource = new List<string> { };
            model.HXNetworkColdStreamSource = new List<string> { };
            model.InjectionWaterStreamSource = new List<string> { };

            model.ObcProcessHXSource = new ObservableCollection<ReactorLoopDetail> { 
                new ReactorLoopDetail { ID=0, ReactorType=0, ReactorLoopID=reactorLoopID,DetailInfo="" },
            };
            model.ObcUtilityHXSource = new ObservableCollection<ReactorLoopDetail> { 
                 new ReactorLoopDetail { ID=0, ReactorType=1, ReactorLoopID=reactorLoopID,DetailInfo="" },
            };
            model.ObcMixerSplitterSource = new ObservableCollection<ReactorLoopDetail> { 
                 new ReactorLoopDetail { ID=0, ReactorType=2, ReactorLoopID=reactorLoopID,DetailInfo="" },
            };
        }

        public ReactorLoopVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            model = new ReactorLoopModel();
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            InitCMD();
            InitPage();

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var RLModel = reactorBLL.GetReactorLoopModel(ScenarioID);

            model.dbModel = RLModel;
            if (RLModel.ID > 0)
            {
                reactorLoopID = RLModel.ID;
                model.ObcProcessHX = reactorBLL.GetProcessHX(reactorLoopID);
                model.ObcUtilityHX = reactorBLL.GetUtilityHX(reactorLoopID);
                model.ObcMixerSplitter = reactorBLL.GetMixerSplitter(reactorLoopID);
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
