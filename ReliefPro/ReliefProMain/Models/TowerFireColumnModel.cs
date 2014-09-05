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
    public class TowerFireColumnModel : ModelBase
    {
        private ObservableCollection<TowerFireColumnDetailModel> _Details;
        public ObservableCollection<TowerFireColumnDetailModel> Details
        {
            get
            {
                return this._Details;
            }
            set
            {
                this._Details = value;
                NotifyPropertyChanged("Details");
            }
        }

        private int _NumberOfSegment;
        [ReliefProMain.Util.Required(ErrorMessage = "NumberOfSegmentWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public int NumberOfSegment
        {
            get
            {
                return _NumberOfSegment;
            }
            set
            {
                _NumberOfSegment = value;
                if (_NumberOfSegment > _Details.Count)
                {
                    for (int i = _Details.Count; i < _NumberOfSegment; i++)
                    {
                        TowerFireColumnDetail d = new TowerFireColumnDetail();
                        TowerFireColumnDetailModel detail = new TowerFireColumnDetailModel(d);
                        detail.Segment = i + 1;
                        detail.Internal = "Trayed";
                        detail.ColumnID = dbmodel.ID;
                        Details.Add(detail);
                    }
                }
                else
                {
                    int count = _Details.Count;
                    for (int i =count-1 ; i>=_NumberOfSegment; i--)
                    {
                        Details.RemoveAt(i);
                    }
                }
                
                NotifyPropertyChanged("NumberOfSegment");                
            }
        }

        private double _Elevation;
        [ReliefProMain.Util.Required(ErrorMessage = "ElevationWarning")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double Elevation
        {
            get
            {
                return this._Elevation;
            }
            set
            {
                this._Elevation = value;
                NotifyPropertyChanged("Elevation");
            }
        }

        private double _BNLL;
        
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double BNLL
        {
            get
            {
                return this._BNLL;
            }
            set
            {
                this._BNLL = value;
                NotifyPropertyChanged("BNLL");
            }
        }

        private double _PipingContingency;
        
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double PipingContingency
        {
            get
            {
                return this._PipingContingency;
            }
            set
            {
                this._PipingContingency = value;
                NotifyPropertyChanged("PipingContingency");
            }
        }


        public TowerFireColumn dbmodel;
        
        public TowerFireColumnModel(TowerFireColumn model)
        {
            dbmodel = model;
            this.ID = model.ID;
            this.EqID = model.EqID;
            this._BNLL = model.BNLL;
            this._Elevation = model.Elevation;
            this._NumberOfSegment = model.NumberOfSegment;
            this._PipingContingency = model.PipingContingency;
            this.bNLL_Color = model.BNLL_Color;
            this.elevation_Color = model.Elevation_Color;
            this.numberOfSegment_Color = model.NumberOfSegment_Color;
            this.pipingContingency_Color = model.PipingContingency_Color;
        }
        private int _EqID;
        public int EqID
        {
            get
            {
                return this._EqID;
            }
            set
            {
                this._EqID = value;
                NotifyPropertyChanged("EqID");

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


        private string numberOfSegment_Color;
        public string NumberOfSegment_Color
        {
            get
            {
                return this.numberOfSegment_Color;
            }
            set
            {
                this.numberOfSegment_Color = value;
                NotifyPropertyChanged("NumberOfSegment_Color");

            }
        }

        private string elevation_Color;
        public string Elevation_Color
        {
            get
            {
                return this.elevation_Color;
            }
            set
            {
                this.elevation_Color = value;
                NotifyPropertyChanged("Elevation_Color");

            }
        }

        private string bNLL_Color;
        public string BNLL_Color
        {
            get
            {
                return this.bNLL_Color;
            }
            set
            {
                this.bNLL_Color = value;
                NotifyPropertyChanged("BNLL_Color");

            }
        }

        private string pipingContingency_Color;
        public string PipingContingency_Color
        {
            get
            {
                return this.pipingContingency_Color;
            }
            set
            {
                this.pipingContingency_Color = value;
                NotifyPropertyChanged("PipingContingency_Color");

            }
        }
    }
}
