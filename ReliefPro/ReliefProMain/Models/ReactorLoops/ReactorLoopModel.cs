using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.Models.ReactorLoops
{
    public class ReactorLoopModel : ModelBase
    {
        public ReactorLoop dbModel { get; set; }

        private ObservableCollection<string> _StreamSource;
        public ObservableCollection<string> StreamSource
        {
            get { return _StreamSource; }
            set
            {
                _StreamSource = value;
                NotifyPropertyChanged("StreamSource");
            }
        }
        
        private ObservableCollection<string> _SeparatorSource;
        public ObservableCollection<string> SeparatorSource
        {
            get { return _SeparatorSource; }
            set
            {
                _SeparatorSource = value;
                NotifyPropertyChanged("SeparatorSource");
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
        public string EffluentStream2
        {
            get { return dbModel.EffluentStream2; }
            set
            {
                dbModel.EffluentStream2 = value;
                NotifyPropertyChanged("EffluentStream2");
            }
        }
        public string ColdReactorFeedStream2
        {
            get { return dbModel.ColdReactorFeedStream2; }
            set
            {
                dbModel.ColdReactorFeedStream2 = value;
                NotifyPropertyChanged("ColdReactorFeedStream2");
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
        public string CompressorH2Stream
        {
            get { return dbModel.CompressorH2Stream; }
            set
            {
                dbModel.CompressorH2Stream = value;
                NotifyPropertyChanged("CompressorH2Stream");
            }
        }
        
        
        public string SourceFile
        {
            get { return dbModel.SourceFile; }
            set
            {
                dbModel.SourceFile = value;
                NotifyPropertyChanged("SourceFile");
            }
        }
        public string ReactorLoopName
        {
            get { return dbModel.ReactorLoopName; }
            set
            {
                dbModel.ReactorLoopName = value;
                NotifyPropertyChanged("ReactorLoopName");
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

        private ReactorLoopDetail selectedHXSourceModel;
        public ReactorLoopDetail SelectedHXSourceModel
        {
            get { return selectedHXSourceModel; }
            set
            {
                selectedHXSourceModel = value;
                this.NotifyPropertyChanged("SelectedHXSourceModel");
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
        private ReactorLoopDetail selectedUtilityHXSourceModel;
        public ReactorLoopDetail SelectedUtilityHXSourceModel
        {
            get { return selectedUtilityHXSourceModel; }
            set
            {
                selectedUtilityHXSourceModel = value;
                this.NotifyPropertyChanged("SelectedUtilityHXSourceModel");
            }
        }
        private ReactorLoopDetail selectedNetworkHXModel;
        public ReactorLoopDetail SelectedNetworkHXModel
        {
            get { return selectedNetworkHXModel; }
            set
            {
                selectedNetworkHXModel = value;
                this.NotifyPropertyChanged("SelectedNetworkHXModel");
            }
        }
        private ReactorLoopDetail selectedNetworkHXSourceModel;
        public ReactorLoopDetail SelectedNetworkHXSourceModel
        {
            get { return selectedNetworkHXSourceModel; }
            set
            {
                selectedNetworkHXSourceModel = value;
                this.NotifyPropertyChanged("SelectedNetworkHXSourceModel");
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
        private ReactorLoopDetail selectedMixerSourceModel;
        public ReactorLoopDetail SelectedMixerSourceModel
        {
            get { return selectedMixerSourceModel; }
            set
            {
                selectedMixerSourceModel = value;
                this.NotifyPropertyChanged("SelectedMixerSourceModel");
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
        private ObservableCollection<ReactorLoopDetail> obcNetworkHX;
        public ObservableCollection<ReactorLoopDetail> ObcNetworkHX
        {
            get { return obcNetworkHX; }
            set
            {
                obcNetworkHX = value;
                this.NotifyPropertyChanged("ObcNetworkHX");
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
        private ObservableCollection<ReactorLoopDetail> obcNetworkHXSource;
        public ObservableCollection<ReactorLoopDetail> ObcNetworkHXSource
        {
            get { return obcNetworkHXSource; }
            set
            {
                obcNetworkHXSource = value;
                this.NotifyPropertyChanged("ObcNetworkHXSource");
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
