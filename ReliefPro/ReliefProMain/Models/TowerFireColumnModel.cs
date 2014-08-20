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
        private ObservableCollection<TowerFireColumnDetail> _Details;
        public ObservableCollection<TowerFireColumnDetail> Details
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


        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public int NumberOfSegment
        {
            get
            {
                return this.Instance.NumberOfSegment;
            }
            set
            {
                this.Instance.NumberOfSegment = value;
                _Details.Clear();
                int count =this.Instance.NumberOfSegment;
                for (int i = 0; i < count; i++)
                {
                    TowerFireColumnDetail detail=new TowerFireColumnDetail();
                    detail.Segment = i + 1;
                    detail.ColumnID = Instance.ID;
                    _Details.Add(detail);
                }
                Instance.NumberOfSegment = this.Instance.NumberOfSegment;
                NotifyPropertyChanged("NumberOfSegment");
                NotifyPropertyChanged("Details");

            }
        }

        private double? _Elevation;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? Elevation
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

        private double? _BNLL;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? BNLL
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

        private double? _PipingContingency;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double? PipingContingency
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

        private TowerFireColumn _Instance;
        public TowerFireColumn Instance
        {
            get
            {
                return this._Instance;
            }
            set
            {
                this._Instance = value;
                NotifyPropertyChanged("Instance");
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
