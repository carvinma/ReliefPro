using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

namespace ReliefProMain.Models
{
    public class TowerScenarioHXModel:ModelBase
    {
        public TowerScenarioHX model;
        public TowerScenarioHXModel(TowerScenarioHX m)
        {
            model = m;
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
        public int DetailID
        {
            get
            {
                return model.DetailID;
            }
            set
            {
                if (model.DetailID != value)
                {
                    model.DetailID = value;
                    NotifyPropertyChanged("DetailID");
                }
            }
        }
        public string DetailName
        {
            get
            {
                return model.DetailName;
            }
            set
            {
                if (model.DetailName != value)
                {
                    model.DetailName = value;
                    NotifyPropertyChanged("DetailName");
                }
            }
        }
        public double DutyCalcFactor
        {
            get
            {
                return model.DutyCalcFactor;
            }
            set
            {
                if (model.DutyCalcFactor != value)
                {
                    model.DutyCalcFactor = value;
                    NotifyPropertyChanged("DutyCalcFactor");
                }
            }
        }
        public bool DutyLost
        {
            get
            {
                return model.DutyLost;
            }
            set
            {
                if (model.DutyLost != value)
                {
                    model.DutyLost = value;
                    NotifyPropertyChanged("DutyLost");
                }
            }
        }
        public int HeaterType
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
        public string Medium
        {
            get
            {
                return model.Medium;
            }
            set
            {
                if (model.Medium != value)
                {
                    model.Medium = value;
                    NotifyPropertyChanged("Medium");
                }
            }
        }
        public double PinchFactor
        {
            get
            {
                return model.PinchFactor;
            }
            set
            {
                if (model.PinchFactor != value)
                {
                    model.PinchFactor = value;
                    NotifyPropertyChanged("PinchFactor");
                }
            }
        }
        public bool IsPinch
        {
            get
            {
                return model.IsPinch;
            }
            set
            {
                if (model.IsPinch != value)
                {
                    model.IsPinch = value;
                    NotifyPropertyChanged("IsPinch");
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
    }
}
