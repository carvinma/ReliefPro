﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProMain.Models
{
    public class TowerScenarioStreamModel:ModelBase
    {
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
        private int _ScenarioID;
        public int ScenarioID
        {
            get
            {
                return this._ScenarioID;
            }
            set
            {
                this._ScenarioID = value;
                NotifyPropertyChanged("ScenarioID");
            }
        }
        private string _StreamName;
        public string StreamName
        {
            get
            {
                return this._StreamName;
            }
            set
            {
                this._StreamName = value;
                NotifyPropertyChanged("StreamName");
            }
        }
        private double? _FlowCalcFactor;
        public double? FlowCalcFactor
        {
            get
            {
                return this._FlowCalcFactor;
            }
            set
            {
                this._FlowCalcFactor = value;
                NotifyPropertyChanged("FlowCalcFactor");
            }
        }
        private string _SourceType;
        public string SourceType
        {
            get
            {
                return this._SourceType;
            }
            set
            {
                this._SourceType = value;
                NotifyPropertyChanged("SourceType");
            }
        }
        private bool _FlowStop;
        public bool FlowStop
        {
            get
            {
                return this._FlowStop;
            }
            set
            {
                this._FlowStop = value;
                NotifyPropertyChanged("FlowStop");
            }
        }
    }
}
