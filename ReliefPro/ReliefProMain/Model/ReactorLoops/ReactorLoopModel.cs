using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.Model.ReactorLoops
{
    public class ReactorLoopModel : ModelBase
    {
        public ReactorLoop dbModel { get; set; }

        private ObservableCollection<string> _EffluentStreamSource;
        public ObservableCollection<string> EffluentStreamSource
        {
            get { return _EffluentStreamSource; }
            set
            {
                _EffluentStreamSource = value;
                NotifyPropertyChanged("EffluentStreamSource");
            }
        }
        private ObservableCollection<string> _ColdReactorFeedStreamSource;
        public ObservableCollection<string> ColdReactorFeedStreamSource
        {
            get { return _ColdReactorFeedStreamSource; }
            set
            {
                _ColdReactorFeedStreamSource = value;
                NotifyPropertyChanged("ColdReactorFeedStreamSource");
            }
        }
        private ObservableCollection<string> _HotHighPressureSeparatorSource;
        public ObservableCollection<string> HotHighPressureSeparatorSource
        {
            get { return _HotHighPressureSeparatorSource; }
            set
            {
                _HotHighPressureSeparatorSource = value;
                NotifyPropertyChanged("HotHighPressureSeparatorSource");
            }
        }
        private ObservableCollection<string> _ColdHighPressureSeparatorSource;
        public ObservableCollection<string> ColdHighPressureSeparatorSource
        {
            get { return _ColdHighPressureSeparatorSource; }
            set
            {
                _ColdHighPressureSeparatorSource = value;
                NotifyPropertyChanged("ColdHighPressureSeparatorSource");
            }
        }
        public ObservableCollection<string> _HXNetworkColdStreamSource;
        public ObservableCollection<string> HXNetworkColdStreamSource
        {
            get { return _HXNetworkColdStreamSource; }
            set
            {
                _HXNetworkColdStreamSource = value;
                NotifyPropertyChanged("HXNetworkColdStreamSource");
            }
        }
        public ObservableCollection<string> _InjectionWaterStreamSource;
        public ObservableCollection<string> InjectionWaterStreamSource
        {
            get { return _InjectionWaterStreamSource; }
            set
            {
                _InjectionWaterStreamSource = value;
                NotifyPropertyChanged("InjectionWaterStreamSource");
            }
        }

        public string EffluentStream
        {
            get { return dbModel.EffluentStream; }
            set
            {
                dbModel.EffluentStream = value;
                NotifyPropertyChanged("EffluentStream");
            }
        }
        public string ColdReactorFeedStream
        {
            get { return dbModel.ColdReactorFeedStream; }
            set
            {
                dbModel.ColdReactorFeedStream = value;
                NotifyPropertyChanged("ColdReactorFeedStream");
            }
        }
        public string HotHighPressureSeparator
        {
            get { return dbModel.HotHighPressureSeparator; }
            set
            {
                dbModel.HotHighPressureSeparator = value;
                NotifyPropertyChanged("HotHighPressureSeparator");
            }
        }
        public string ColdHighPressureSeparator
        {
            get { return dbModel.ColdHighPressureSeparator; }
            set
            {
                dbModel.ColdHighPressureSeparator = value;
                NotifyPropertyChanged("ColdHighPressureSeparator");
            }
        }
        public string HXNetworkColdStream
        {
            get { return dbModel.HXNetworkColdStream; }
            set
            {
                dbModel.HXNetworkColdStream = value;
                NotifyPropertyChanged("HXNetworkColdStream");
            }
        }
        public string InjectionWaterStream
        {
            get { return dbModel.InjectionWaterStream; }
            set
            {
                dbModel.InjectionWaterStream = value;
                NotifyPropertyChanged("InjectionWaterStream");
            }
        }

        public string PSFile
        {
            get { return dbModel.PSFile; }
            set
            {
                dbModel.PSFile = value;
                NotifyPropertyChanged("PSFile");
            }
        }

        public string PSVersion
        {
            get { return dbModel.PSVersion; }
            set
            {
                dbModel.PSVersion = value;
                NotifyPropertyChanged("PSVersion");
            }
        }
        private ReactorLoopDetail selectedHXModel;
        public ReactorLoopDetail SelectedHXModel
        {
            get { return selectedHXModel; }
            set
            {
                selectedHXModel = value;
                this.NotifyPropertyChanged("SelectedHXModel");
            }
        }

        private ReactorLoopDetail selectedUtilityHXModel;
        public ReactorLoopDetail SelectedUtilityHXModel
        {
            get { return selectedUtilityHXModel; }
            set
            {
                selectedUtilityHXModel = value;
                this.NotifyPropertyChanged("SelectedUtilityHXModel");
            }
        }

        private ReactorLoopDetail selectedMixerModel;
        public ReactorLoopDetail SelectedMixerModel
        {
            get { return selectedMixerModel; }
            set
            {
                selectedMixerModel = value;
                this.NotifyPropertyChanged("SelectedMixerModel");
            }
        }

        private ObservableCollection<ReactorLoopDetail> obcProcessHX;
        public ObservableCollection<ReactorLoopDetail> ObcProcessHX
        {
            get { return obcProcessHX; }
            set
            {
                obcProcessHX = value;
                this.NotifyPropertyChanged("ObcProcessHX");
            }
        }

        private ObservableCollection<ReactorLoopDetail> obcUtilityHX;
        public ObservableCollection<ReactorLoopDetail> ObcUtilityHX
        {
            get { return obcUtilityHX; }
            set
            {
                obcUtilityHX = value;
                this.NotifyPropertyChanged("ObcUtilityHX");
            }
        }

        private ObservableCollection<ReactorLoopDetail> obcMixerSplitter;
        public ObservableCollection<ReactorLoopDetail> ObcMixerSplitter
        {
            get { return obcMixerSplitter; }
            set
            {
                obcMixerSplitter = value;
                this.NotifyPropertyChanged("ObcMixerSplitter");
            }
        }

        private ObservableCollection<ReactorLoopDetail> obcProcessHXSource;
        public ObservableCollection<ReactorLoopDetail> ObcProcessHXSource
        {
            get { return obcProcessHXSource; }
            set
            {
                obcProcessHXSource = value;
                this.NotifyPropertyChanged("ObcProcessHXSource");
            }
        }

        private ObservableCollection<ReactorLoopDetail> obcUtilityHXSource;
        public ObservableCollection<ReactorLoopDetail> ObcUtilityHXSource
        {
            get { return obcUtilityHXSource; }
            set
            {
                obcUtilityHXSource = value;
                this.NotifyPropertyChanged("ObcUtilityHXSource");
            }
        }

        private ObservableCollection<ReactorLoopDetail> obcMixerSplitterSource;
        public ObservableCollection<ReactorLoopDetail> ObcMixerSplitterSource
        {
            get { return obcMixerSplitterSource; }
            set
            {
                obcMixerSplitterSource = value;
                this.NotifyPropertyChanged("ObcMixerSplitterSource");
            }
        }
    }
}
