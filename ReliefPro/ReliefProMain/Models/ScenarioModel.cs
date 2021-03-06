﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProMain.Models
{
    public class ScenarioModel : ModelBase
    {
        private int _SeqNumber;
        public int SeqNumber
        {
            get
            {
                return this._SeqNumber;
            }
            set
            {
                this._SeqNumber = value;
                NotifyPropertyChanged("SeqNumber");
            }
        }
        private int _ID;
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
                NotifyPropertyChanged("ID");
            }
        }
        private string _ScenarioName;
        public string ScenarioName
        {
            get
            {
                return this._ScenarioName;
            }
            set
            {
                this._ScenarioName = value;
                NotifyPropertyChanged("ScenarioName");
            }
        }
        private double _ReliefLoad;
        public double ReliefLoad
        {
            get
            {
                return this._ReliefLoad;
            }
            set
            {
                this._ReliefLoad = value;
                NotifyPropertyChanged("ReliefLoad");
            }
        }
        private double _ReliefTemperature;
        public double ReliefTemperature
        {
            get
            {
                return this._ReliefTemperature;
            }
            set
            {
                this._ReliefTemperature = value;
                NotifyPropertyChanged("ReliefTemperature");
            }
        }
        private double _ReliefPressure;
        public double ReliefPressure
        {
            get
            {
                return this._ReliefPressure;
            }
            set
            {
                this._ReliefPressure = value;
                NotifyPropertyChanged("ReliefPressure");
            }
        }
        private double _ReliefMW;
        public double ReliefMW
        {
            get
            {
                return this._ReliefMW;
            }
            set
            {
                this._ReliefMW = value;
                NotifyPropertyChanged("ReliefMW");
            }
        }


    }

}
