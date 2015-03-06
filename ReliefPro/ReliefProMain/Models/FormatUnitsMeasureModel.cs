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
        private systbBasicUnit _BasicUnitselectLocation;
        public systbBasicUnit BasicUnitselectLocation
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

        private systbSystemUnit _selectLocation;
        public systbSystemUnit TemperatureSelectLocation
        {
            get
            {
                if (null == this._selectLocation)
                    _selectLocation = new systbSystemUnit();
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

        private systbSystemUnit _PressureselectLocation;
        public systbSystemUnit PressureSelectLocation
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

        private systbSystemUnit _WeightselectLocation;
        public systbSystemUnit WeightSelectLocation
        {
            get
            {
                if (null == _WeightselectLocation)
                    _WeightselectLocation = new systbSystemUnit();
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

        private systbSystemUnit _MolarLocation;
        public systbSystemUnit MolarSelectLocation
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

        private systbSystemUnit _StandardVolumeRateselectLocation;
        public systbSystemUnit StandardVolumeRateSelectLocation
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

        private systbSystemUnit _ViscosityselectLocation;
        public systbSystemUnit ViscositySelectLocation
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

        private systbSystemUnit _HeatCapacityselectLocation;
        public systbSystemUnit HeatCapacitySelectLocation
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

        private systbSystemUnit _ThermalConductivityselectLocation;
        public systbSystemUnit ThermalConductivitySelectLocation
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

        private systbSystemUnit _HeatTransCoeffcientselectLocation;
        public systbSystemUnit HeatTransCoeffcientSelectLocation
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

        private systbSystemUnit _SurfaceTensionselectLocation;
        public systbSystemUnit SurfaceTensionSelectLocation
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


        private systbSystemUnit _MachineSpeedselectLocation;
        public systbSystemUnit MachineSpeedSelectLocation
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

        private systbSystemUnit _VolumeselectLocation;
        public systbSystemUnit VolumeSelectLocation
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

        private systbSystemUnit _LengthselectLocation;
        public systbSystemUnit LengthSelectLocation
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

        private systbSystemUnit _AeraselectLocation;
        public systbSystemUnit AeraSelectLocation
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

        private systbSystemUnit _EnergyselectLocation;
        public systbSystemUnit EnergySelectLocation
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

        private systbSystemUnit _TimeselectLocation;
        public systbSystemUnit TimeSelectLocation
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

        private systbSystemUnit _FlowConductanceselectLocation;
        public systbSystemUnit FlowConductanceSelectLocation
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

        private systbSystemUnit _MassRateselectLocation;
        public systbSystemUnit MassRateSelectLocation
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
        private systbSystemUnit _VolumeRateselectLocation;
        public systbSystemUnit VolumeRateSelectLocation
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

        private systbSystemUnit _DensityselectLocation;
        public systbSystemUnit DensitySelectLocation
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

        private systbSystemUnit _SpecificEnthalpyselectLocation;
        public systbSystemUnit SpecificEnthalpySelectLocation
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

        private systbSystemUnit _EnthalpyselectLocation;
        public systbSystemUnit EnthalpySelectLocation
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

        private systbSystemUnit _FineLenthselectLocation;
        public systbSystemUnit FineLenthSelectLocation
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

        public ObservableCollection<systbBasicUnit> ObBasicUnit
        { get; set; }

        private ObservableCollection<systbSystemUnit> obcTemperature;
        public ObservableCollection<systbSystemUnit> ObcTemperature
        {
            get { return obcTemperature; }
            set
            {
                obcTemperature = value;
                this.NotifyPropertyChanged("ObcTemperature");
            }
        }

        private ObservableCollection<systbSystemUnit> obcPressure;
        public ObservableCollection<systbSystemUnit> ObcPressure
        {
            get { return obcPressure; }
            set
            {
                obcPressure = value;
                this.NotifyPropertyChanged("ObcPressure");
            }
        }

        private ObservableCollection<systbSystemUnit> obcWeight;
        public ObservableCollection<systbSystemUnit> ObcWeight
        {
            get { return obcWeight; }
            set
            {
                obcWeight = value;
                this.NotifyPropertyChanged("ObcWeight");
            }
        }

        public ObservableCollection<systbSystemUnit> ObcMolar
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcStandardVolumeRate
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcViscosity
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcHeatCapacity
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcThermalConductivity
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcHeatTransCoeffcient
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcSurfaceTension
        { get; set; }
        //public ObservableCollection<SystemUnit> ObcComposition
        //{ get; set; }
        public ObservableCollection<systbSystemUnit> ObcMachineSpeed
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcVolume
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcLength
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcAera
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcEnergy
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcTime
        { get; set; }
        public ObservableCollection<systbSystemUnit> ObcFlowConductance
        { get; set; }

        public ObservableCollection<systbSystemUnit> ObcMassRate
        { get; set; }

        public ObservableCollection<systbSystemUnit> ObcVolumeRate
        { get; set; }

        public ObservableCollection<systbSystemUnit> ObcDensity
        { get; set; }

        public ObservableCollection<systbSystemUnit> ObcSpecificEnthalpy
        { get; set; }

        public ObservableCollection<systbSystemUnit> ObcEnthalpy
        { get; set; }

        public ObservableCollection<systbSystemUnit> ObcFineLength
        { get; set; }

        #endregion




    }
}
