using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;

namespace ReliefProMain.Model
{     
    public class HeatSourceModel : ModelBase
    {
        public HeatSource model;

        private int _SeqNumber;
        public int SeqNumber
        {
            get
            {
                return _SeqNumber;
            }
            set
            {

                _SeqNumber = value;
                NotifyPropertyChanged("SeqNumber");

            }
        }
        public int ID
        {
            get
            {
                return model.ID;
            }
            set
            {
                if (model.ID != value)
                {
                    model.ID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
       
        public string HeatSourceName
        {
            get
            {
                return model.HeatSourceName;
            }
            set
            {
                if (model.HeatSourceName != value)
                {
                    model.HeatSourceName = value;
                    NotifyPropertyChanged("HeatSourceName");
                }
            }
        }

        public string HeatSourceType
        {
            get
            {
                return model.HeatSourceType;
            }
            set
            {
                if (model.HeatSourceType != value)
                {
                    model.HeatSourceType = value;
                    NotifyPropertyChanged("HeatSourceType");
                }
            }
        }
        public int SourceID
        {
            get
            {
                return model.SourceID;
            }
            set
            {
                if (model.SourceID != value)
                {
                    model.SourceID = value;
                    NotifyPropertyChanged("SourceID");
                }
            }
        }

        public HeatSourceModel(HeatSource m)
        {
            model = m;
        }
       
    }
}
