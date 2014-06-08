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
        public ObservableCollection<ReactorLoopDetail> ObcProcessHX;
        public ObservableCollection<ReactorLoopDetail> ObcUtilityHX;
        public ObservableCollection<ReactorLoopDetail> ObcMixerSplitter;
    }
}
