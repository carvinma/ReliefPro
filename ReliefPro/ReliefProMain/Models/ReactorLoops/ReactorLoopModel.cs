using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ReliefProCommon.Enum;
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
        public int ID
        {
            get { return dbModel.ID; }
            set
            {
                dbModel.ID = value;
                NotifyPropertyChanged("ID");
            }
        }

        public string EffluentStream
        {
            get { return dbModel.EffluentStream; }
            set
            {
                if (dbModel.EffluentStream != value)
                {
                    this.EffluentStream_Color = ColorBorder.blue.ToString();
                }
                dbModel.EffluentStream = value;
                NotifyPropertyChanged("EffluentStream");
            }
        }
        public string ColdReactorFeedStream
        {
            get { return dbModel.ColdReactorFeedStream; }
            set
            {
                if (dbModel.ColdReactorFeedStream != value)
                {
                    this.ColdReactorFeedStream_Color = ColorBorder.blue.ToString();
                }
                dbModel.ColdReactorFeedStream = value;
                NotifyPropertyChanged("ColdReactorFeedStream");
            }
        }
        public string ColdReactorFeedStream2
        {
            get { return dbModel.ColdReactorFeedStream2; }
            set
            {
                if (dbModel.ColdReactorFeedStream2 != value)
                {
                    this.ColdReactorFeedStream2_Color = ColorBorder.blue.ToString();
                }
                dbModel.ColdReactorFeedStream2 = value;
                NotifyPropertyChanged("ColdReactorFeedStream2");
            }
        }
        public string HotHighPressureSeparator
        {
            get { return dbModel.HotHighPressureSeparator; }
            set
            {
                if (dbModel.HotHighPressureSeparator != value)
                {
                    HotHighPressureSeparator_Color = ColorBorder.blue.ToString();
                }
                dbModel.HotHighPressureSeparator = value;
                NotifyPropertyChanged("HotHighPressureSeparator");
            }
        }
        public string ColdHighPressureSeparator
        {
            get { return dbModel.ColdHighPressureSeparator; }
            set
            {
                if (dbModel.ColdHighPressureSeparator != value)
                {
                    this.ColdHighPressureSeparator_Color = ColorBorder.blue.ToString();
                }
                dbModel.ColdHighPressureSeparator = value;
                NotifyPropertyChanged("ColdHighPressureSeparator");
            }
        }
        public string HXNetworkColdStream
        {
            get { return dbModel.HXNetworkColdStream; }
            set
            {
                if (dbModel.HXNetworkColdStream != value)
                {
                    this.HXNetworkColdStream_Color = ColorBorder.blue.ToString();
                }
                dbModel.HXNetworkColdStream = value;
                NotifyPropertyChanged("HXNetworkColdStream");
            }
        }
        public string InjectionWaterStream
        {
            get { return dbModel.InjectionWaterStream; }
            set
            {
                if (dbModel.InjectionWaterStream != value)
                {
                    this.InjectionWaterStream_Color = ColorBorder.blue.ToString();
                }
                dbModel.InjectionWaterStream = value;
                NotifyPropertyChanged("InjectionWaterStream");
            }
        }
        public string CompressorH2Stream
        {
            get { return dbModel.CompressorH2Stream; }
            set
            {
                if (dbModel.CompressorH2Stream != value)
                {
                    this.CompressorH2Stream_Color = ColorBorder.blue.ToString();
                }
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
                if (dbModel.ReactorLoopName != value)
                {
                    this.ReactorLoopName_Color = ColorBorder.blue.ToString();
                }
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

        #region Color
        public string ReactorLoopName_Color { 
            get { return dbModel.ReactorLoopName_Color; }
            set {
                dbModel.ReactorLoopName_Color = value;
                this.NotifyPropertyChanged("ReactorLoopName_Color");
            }
        }

        public string EffluentStream_Color
        {
            get { return dbModel.EffluentStream_Color; }
            set
            {
                dbModel.EffluentStream_Color = value;
                this.NotifyPropertyChanged("EffluentStream_Color");
            }
        }

        public string ColdReactorFeedStream_Color
        {
            get { return dbModel.ColdReactorFeedStream_Color; }
            set
            {
                dbModel.ColdReactorFeedStream_Color = value;
                this.NotifyPropertyChanged("ColdReactorFeedStream_Color");
            }
        }

        public string EffluentStream2_Color
        {
            get { return dbModel.EffluentStream2_Color; }
            set
            {
                dbModel.EffluentStream2_Color = value;
                this.NotifyPropertyChanged("EffluentStream2_Color");
            }
        }

        public string ColdReactorFeedStream2_Color
        {
            get { return dbModel.ColdReactorFeedStream2_Color; }
            set
            {
                dbModel.ColdReactorFeedStream2_Color = value;
                this.NotifyPropertyChanged("ColdReactorFeedStream2_Color");
            }
        }

        public string HotHighPressureSeparator_Color
        {
            get { return dbModel.HotHighPressureSeparator_Color; }
            set
            {
                dbModel.HotHighPressureSeparator_Color = value;
                this.NotifyPropertyChanged("HotHighPressureSeparator_Color");
            }
        }

        public string ColdHighPressureSeparator_Color
        {
            get { return dbModel.ColdHighPressureSeparator_Color; }
            set
            {
                dbModel.ColdHighPressureSeparator_Color = value;
                this.NotifyPropertyChanged("ColdHighPressureSeparator_Color");
            }
        }

        public string HXNetworkColdStream_Color
        {
            get { return dbModel.HXNetworkColdStream_Color; }
            set
            {
                dbModel.HXNetworkColdStream_Color = value;
                this.NotifyPropertyChanged("HXNetworkColdStream_Color");
            }
        }

        public string InjectionWaterStream_Color
        {
            get { return dbModel.InjectionWaterStream_Color; }
            set
            {
                dbModel.InjectionWaterStream_Color = value;
                this.NotifyPropertyChanged("InjectionWaterStream_Color");
            }
        }

        public string CompressorH2Stream_Color
        {
            get { return dbModel.CompressorH2Stream_Color; }
            set
            {
                dbModel.CompressorH2Stream_Color = value;
                this.NotifyPropertyChanged("CompressorH2Stream_Color");
            }
        }
        #endregion
    }
}
