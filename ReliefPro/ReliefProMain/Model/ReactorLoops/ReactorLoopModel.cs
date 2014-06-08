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
        public ReactorLoop dbModel;

        public List<string> EffluentStreamSource { get; set; }
        public List<string> ColdReactorFeedStreamSource { get; set; }
        public List<string> HotHighPressureSeparatorSource { get; set; }
        public List<string> ColdHighPressureSeparatorSource { get; set; }
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

        public ObservableCollection<ReactorLoopDetail> ObcProcessHX { get; set; }
        public ObservableCollection<ReactorLoopDetail> ObcUtilityHX { get; set; }
        public ObservableCollection<ReactorLoopDetail> ObcMixerSplitter { get; set; }

        public ObservableCollection<ReactorLoopDetail> ObcProcessHXSource { get; set; }
        public ObservableCollection<ReactorLoopDetail> ObcUtilityHXSource { get; set; }
        public ObservableCollection<ReactorLoopDetail> ObcMixerSplitterSource { get; set; }
    }
}
