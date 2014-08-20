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

namespace ReliefProMain.Models
{     
    public class AbnormalHeaterDetailModel : ModelBase
    {
        public AbnormalHeaterDetail model;        
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
       
        public string HeaterName
        {
            get
            {
                return model.HeaterName;
            }
            set
            {
                if (model.HeaterName != value)
                {                    
                    model.HeaterName = value;
                    NotifyPropertyChanged("HeaterName");
                }
            }
        }
        public string HeaterType
        {
            get
            {
                return model.HeaterType;
            }
            set
            {
                if (model.HeaterType != value)
                {
                    model.HeaterType = value;
                    NotifyPropertyChanged("HeaterType");
                }
            }
        }
        public int AbnormalType
        {
            get
            {
                return model.AbnormalType;
            }
            set
            {
                if (model.AbnormalType != value)
                {
                    model.AbnormalType = value;
                    NotifyPropertyChanged("AbnormalType");
                }
            }
        }

        public double? Duty
        {
            get
            {
                return model.Duty;
            }
            set
            {
                if (model.Duty != value)
                {
                    model.Duty = value;
                    NotifyPropertyChanged("Duty");
                }
            }
        }
        public double? DutyFactor
        {
            get
            {
                return model.DutyFactor;
            }
            set
            {
                if (model.DutyFactor != value)
                {
                    model.DutyFactor = value;
                    NotifyPropertyChanged("DutyFactor");
                }
            }
        }
        public int HeaterID
        {
            get
            {
                return model.HeaterID;
            }
            set
            {
                if (model.HeaterID != value)
                {
                    model.HeaterID = value;
                    NotifyPropertyChanged("HeaterID");
                }
            }
        }
        public int ScenarioID
        {
            get
            {
                return model.ScenarioID;
            }
            set
            {
                if (model.ScenarioID != value)
                {
                    model.ScenarioID = value;
                    NotifyPropertyChanged("ScenarioID");
                }
            }
        }

        public AbnormalHeaterDetailModel(AbnormalHeaterDetail m)
        {
            model = m;
        }
       
    }
}
