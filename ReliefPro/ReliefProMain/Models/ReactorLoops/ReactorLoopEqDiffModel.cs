using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.Models.ReactorLoops
{
    public class ReactorLoopEqDiffModel : ModelBase
    {

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                this.NotifyPropertyChanged("ID");
            }
        }
        private int _ReactorLoopID;
        public int ReactorLoopID
        {
            get { return _ReactorLoopID; }
            set
            {
                _ReactorLoopID = value;
                this.NotifyPropertyChanged("ReactorLoopID");
            }
        }


        private string _EqType;
        public string EqType
        {
            get { return _EqType; }
            set
            {
                _EqType = value;
                this.NotifyPropertyChanged("EqType");
            }
        }
        private string _EqName;
        public string EqName
        {
            get { return _EqName; }
            set
            {
                _EqName = value;
                this.NotifyPropertyChanged("EqName");
            }
        }

       

        public ReactorLoopEqDiff dbmodel { get; set; }
        public ReactorLoopEqDiffModel(ReactorLoopEqDiff model)
        {
            dbmodel = model;
            this.ReactorLoopID = model.ReactorLoopID;
            this.EqName = model.EqName;
            this.EqType = model.EqType;
            this.ID = model.ID;
            this.OrginDuty = model.OrginDuty;
            this.CurrentDuty = model.CurrentDuty;
            this.Diff = model.Diff;
        }


        private double _OrginDuty;
        public double OrginDuty
        {
            get { return _OrginDuty; }
            set
            {
                _OrginDuty = value;
                this.NotifyPropertyChanged("OrginDuty");
            }
        }
        private double _CurrentDuty;
        public double CurrentDuty
        {
            get { return _CurrentDuty; }
            set
            {
                _CurrentDuty = value;
                this.NotifyPropertyChanged("CurrentDuty");
            }
        }

        private double _Diff;
        public double Diff
        {
            get { return _Diff; }
            set
            {
                _Diff = value;
                this.NotifyPropertyChanged("Diff");
            }
        }

        

        

    }
}
