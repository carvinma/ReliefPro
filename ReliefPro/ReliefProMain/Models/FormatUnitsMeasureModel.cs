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
    public class FormatUnitsMeasureModel : ModelBase
    {
        public delegate void SelectDefaultUnitDelegate(object SelectDefaultUnit);
        public event SelectDefaultUnitDelegate handler;
        public delegate void ChangeDefaultUnitDelegate(object ChangeDefaultUnit);
        public event ChangeDefaultUnitDelegate handlerChange;

        private bool _canUseDelButtn=true;
        public bool canUseDelButtn
        {
            get
            {
                return this._canUseDelButtn;
            }
            set
            {
                this._canUseDelButtn = value;
                NotifyPropertyChanged("canUseDelButtn");
            }
        }
        #region combobox数据源
        #region 选择项
        private BasicUnit _BasicUnitselectLocation;
        public BasicUnit BasicUnitselectLocation
        {
            get
            {
                return this._BasicUnitselectLocation;
            }
            set
            {
                if (this._BasicUnitselectLocation == value) return;
                this._BasicUnitselectLocation = value;
                if (handler != null && value != null && value.ID > 0)
                {
                    handler(_BasicUnitselectLocation);
                }
                NotifyPropertyChanged("BasicUnitselectLocation");
            }
        }

        private SystemUnit _selectLocation;
        public SystemUnit TemperatureSelectLocation
        {
            get
            {
                if (null == this._selectLocation)
                    _selectLocation = new SystemUnit();
                return this._selectLocation;
            }
            set
            {
                this._selectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("TemperatureSelectLocation");
            }
        }

        private SystemUnit _PressureselectLocation;
        public SystemUnit PressureSelectLocation
        {
            get
            {
                return this._PressureselectLocation;
            }
            set
            {
                this._PressureselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("PressureSelectLocation");
            }
        }

        private SystemUnit _WeightselectLocation;
        public SystemUnit WeightSelectLocation
        {
            get
            {
                if (null == _WeightselectLocation)
                    _WeightselectLocation = new SystemUnit();
                return this._WeightselectLocation;
            }
            set
            {
                this._WeightselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("WeightSelectLocation");
            }
        }

        private SystemUnit _MolarLocation;
        public SystemUnit MolarSelectLocation
        {
            get
            {
                return this._MolarLocation;
            }
            set
            {
                this._MolarLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("MolarSelectLocation");
            }
        }

        private SystemUnit _StandardVolumeRateselectLocation;
        public SystemUnit StandardVolumeRateSelectLocation
        {
            get
            {
                return this._StandardVolumeRateselectLocation;
            }
            set
            {
                this._StandardVolumeRateselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("StandardVolumeRateSelectLocation");
            }
        }

        private SystemUnit _ViscosityselectLocation;
        public SystemUnit ViscositySelectLocation
        {
            get
            {
                return this._ViscosityselectLocation;
            }
            set
            {
                this._ViscosityselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("ViscositySelectLocation");
            }
        }

        private SystemUnit _HeatCapacityselectLocation;
        public SystemUnit HeatCapacitySelectLocation
        {
            get
            {
                return this._HeatCapacityselectLocation;
            }
            set
            {
                this._HeatCapacityselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("HeatCapacitySelectLocation");
            }
        }

        private SystemUnit _ThermalConductivityselectLocation;
        public SystemUnit ThermalConductivitySelectLocation
        {
            get
            {
                return this._ThermalConductivityselectLocation;
            }
            set
            {
                this._ThermalConductivityselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("ThermalConductivitySelectLocation");
            }
        }

        private SystemUnit _HeatTransCoeffcientselectLocation;
        public SystemUnit HeatTransCoeffcientSelectLocation
        {
            get
            {
                return this._HeatTransCoeffcientselectLocation;
            }
            set
            {
                this._HeatTransCoeffcientselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("HeatTransCoeffcientSelectLocation");
            }
        }

        private SystemUnit _SurfaceTensionselectLocation;
        public SystemUnit SurfaceTensionSelectLocation
        {
            get
            {
                return this._SurfaceTensionselectLocation;
            }
            set
            {
                this._SurfaceTensionselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("SurfaceTensionSelectLocation");
            }
        }

        //private SystemUnit _CompositionselectLocation;
        //public SystemUnit CompositionSelectLocation
        //{
        //    get
        //    {
        //        return this._CompositionselectLocation;
        //    }
        //    set
        //    {
        //        this._CompositionselectLocation = value;
        //        if (handlerChange != null)
        //        {
        //            handlerChange(value);
        //        }
        //        NotifyPropertyChanged("CompositionSelectLocation");
        //    }
        //}

        private SystemUnit _MachineSpeedselectLocation;
        public SystemUnit MachineSpeedSelectLocation
        {
            get
            {
                return this._MachineSpeedselectLocation;
            }
            set
            {
                this._MachineSpeedselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("MachineSpeedSelectLocation");
            }
        }

        private SystemUnit _VolumeselectLocation;
        public SystemUnit VolumeSelectLocation
        {
            get
            {
                return this._VolumeselectLocation;
            }
            set
            {
                this._VolumeselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("VolumeSelectLocation");
            }
        }

        private SystemUnit _LengthselectLocation;
        public SystemUnit LengthSelectLocation
        {
            get
            {
                return this._LengthselectLocation;
            }
            set
            {
                this._LengthselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("LengthSelectLocation");
            }
        }

        private SystemUnit _AeraselectLocation;
        public SystemUnit AeraSelectLocation
        {
            get
            {
                return this._AeraselectLocation;
            }
            set
            {
                this._AeraselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("AeraSelectLocation");
            }
        }

        private SystemUnit _EnergyselectLocation;
        public SystemUnit EnergySelectLocation
        {
            get
            {
                return this._EnergyselectLocation;
            }
            set
            {
                this._EnergyselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("EnergySelectLocation");
            }
        }

        private SystemUnit _TimeselectLocation;
        public SystemUnit TimeSelectLocation
        {
            get
            {
                return this._TimeselectLocation;
            }
            set
            {
                this._TimeselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("TimeSelectLocation");
            }
        }

        private SystemUnit _FlowConductanceselectLocation;
        public SystemUnit FlowConductanceSelectLocation
        {
            get
            {
                return this._FlowConductanceselectLocation;
            }
            set
            {
                this._FlowConductanceselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("FlowConductanceSelectLocation");
            }
        }

        private SystemUnit _MassRateselectLocation;
        public SystemUnit MassRateSelectLocation
        {
            get
            {
                return this._MassRateselectLocation;
            }
            set
            {
                this._MassRateselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("MassRateSelectLocation");
            }
        }
        private SystemUnit _VolumeRateselectLocation;
        public SystemUnit VolumeRateSelectLocation
        {
            get
            {
                return this._VolumeRateselectLocation;
            }
            set
            {
                this._VolumeRateselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("VolumeRateSelectLocation");
            }
        }

        private SystemUnit _DensityselectLocation;
        public SystemUnit DensitySelectLocation
        {
            get
            {
                return this._DensityselectLocation;
            }
            set
            {
                this._DensityselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("DensitySelectLocation");
            }
        }

        private SystemUnit _SpecificEnthalpyselectLocation;
        public SystemUnit SpecificEnthalpySelectLocation
        {
            get
            {
                return this._SpecificEnthalpyselectLocation;
            }
            set
            {
                this._SpecificEnthalpyselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("SpecificEnthalpySelectLocation");
            }
        }

        private SystemUnit _EnthalpyselectLocation;
        public SystemUnit EnthalpySelectLocation
        {
            get
            {
                return this._EnthalpyselectLocation;
            }
            set
            {
                this._EnthalpyselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("EnthalpySelectLocation");
            }
        }

        private SystemUnit _FineLenthselectLocation;
        public SystemUnit FineLenthSelectLocation
        {
            get
            {
                return this._FineLenthselectLocation;
            }
            set
            {
                this._FineLenthselectLocation = value;
                if (handlerChange != null)
                {
                    handlerChange(value);
                }
                NotifyPropertyChanged("FineLenthSelectLocation");
            }
        }

        #endregion

        public ObservableCollection<BasicUnit> ObBasicUnit
        { get; set; }

        private ObservableCollection<SystemUnit> obcTemperature;
        public ObservableCollection<SystemUnit> ObcTemperature
        {
            get { return obcTemperature; }
            set
            {
                obcTemperature = value;
                this.NotifyPropertyChanged("ObcTemperature");
            }
        }

        private ObservableCollection<SystemUnit> obcPressure;
        public ObservableCollection<SystemUnit> ObcPressure
        {
            get { return obcPressure; }
            set
            {
                obcPressure = value;
                this.NotifyPropertyChanged("ObcPressure");
            }
        }

        private ObservableCollection<SystemUnit> obcWeight;
        public ObservableCollection<SystemUnit> ObcWeight
        {
            get { return obcWeight; }
            set
            {
                obcWeight = value;
                this.NotifyPropertyChanged("ObcWeight");
            }
        }

        public ObservableCollection<SystemUnit> ObcMolar
        { get; set; }
        public ObservableCollection<SystemUnit> ObcStandardVolumeRate
        { get; set; }
        public ObservableCollection<SystemUnit> ObcViscosity
        { get; set; }
        public ObservableCollection<SystemUnit> ObcHeatCapacity
        { get; set; }
        public ObservableCollection<SystemUnit> ObcThermalConductivity
        { get; set; }
        public ObservableCollection<SystemUnit> ObcHeatTransCoeffcient
        { get; set; }
        public ObservableCollection<SystemUnit> ObcSurfaceTension
        { get; set; }
        //public ObservableCollection<SystemUnit> ObcComposition
        //{ get; set; }
        public ObservableCollection<SystemUnit> ObcMachineSpeed
        { get; set; }
        public ObservableCollection<SystemUnit> ObcVolume
        { get; set; }
        public ObservableCollection<SystemUnit> ObcLength
        { get; set; }
        public ObservableCollection<SystemUnit> ObcAera
        { get; set; }
        public ObservableCollection<SystemUnit> ObcEnergy
        { get; set; }
        public ObservableCollection<SystemUnit> ObcTime
        { get; set; }
        public ObservableCollection<SystemUnit> ObcFlowConductance
        { get; set; }

        public ObservableCollection<SystemUnit> ObcMassRate
        { get; set; }

        public ObservableCollection<SystemUnit> ObcVolumeRate
        { get; set; }

        public ObservableCollection<SystemUnit> ObcDensity
        { get; set; }

        public ObservableCollection<SystemUnit> ObcSpecificEnthalpy
        { get; set; }

        public ObservableCollection<SystemUnit> ObcEnthalpy
        { get; set; }

        public ObservableCollection<SystemUnit> ObcFineLength
        { get; set; }

        #endregion




    }
}
