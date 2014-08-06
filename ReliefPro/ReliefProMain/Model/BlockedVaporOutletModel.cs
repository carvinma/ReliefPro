using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using ReliefProModel.Towers;

namespace ReliefProMain.Model
{
    public class BlockedVaporOutletModel : ModelBase
    {
        public BlockedVaporOutlet dbmodel { get; set; }
        public Scenario dbScenario { get; set; }

        #region BlockedVaporOutlet Unit
        private string inletGasUpstreamMaxPressureUnit;
        public string InletGasUpstreamMaxPressureUnit
        {
            get { return inletGasUpstreamMaxPressureUnit; }
            set
            {
                inletGasUpstreamMaxPressureUnit = value;
                this.NotifyPropertyChanged("InletGasUpstreamMaxPressureUnit");
            }
        }

        private string inletAbsorbentUpstreamMaxPressureUnit;
        public string InletAbsorbentUpstreamMaxPressureUnit
        {
            get { return inletAbsorbentUpstreamMaxPressureUnit; }
            set
            {
                inletAbsorbentUpstreamMaxPressureUnit = value;
                this.NotifyPropertyChanged("InletAbsorbentUpstreamMaxPressureUnit");
            }
        }

        private string normalGasFeedWeightRateUnit;
        public string NormalGasFeedWeightRateUnit
        {
            get { return normalGasFeedWeightRateUnit; }
            set
            {
                normalGasFeedWeightRateUnit = value;
                this.NotifyPropertyChanged("NormalGasFeedWeightRateUnit");
            }
        }

        private string normalGasProductWeightRateUnit;
        public string NormalGasProductWeightRateUnit
        {
            get { return normalGasProductWeightRateUnit; }
            set
            {
                normalGasProductWeightRateUnit = value;
                this.NotifyPropertyChanged("NormalGasProductWeightRateUnit");
            }
        }

        #endregion

        public BlockedVaporOutletModel(BlockedVaporOutlet outletModel, Scenario sModel)
        {
            dbmodel = outletModel;
            dbScenario = sModel;

            this.InletGasUpstreamMaxPressure = outletModel.InletGasUpstreamMaxPressure;
            this.InletAbsorbentUpstreamMaxPressure = outletModel.InletAbsorbentUpstreamMaxPressure;
            this.NormalGasFeedWeightRate = outletModel.NormalGasFeedWeightRate;
            this.NormalGasProductWeightRate = outletModel.NormalGasProductWeightRate;

            this.ReliefLoad = sModel.ReliefLoad;
            this.ReliefPressure = sModel.ReliefPressure;
            this.ReliefTemperature = sModel.ReliefTemperature;
            this.ReliefCpCv = sModel.ReliefCpCv;
            this.ReliefZ = sModel.ReliefZ;
        }
        #region Value

        private double? inletGasUpstreamMaxPressure;
        public double? InletGasUpstreamMaxPressure
        {
            get { return inletGasUpstreamMaxPressure; }
            set
            {
                inletGasUpstreamMaxPressure = value;
                dbmodel.InletGasUpstreamMaxPressure = value;
                this.NotifyPropertyChanged("InletGasUpstreamMaxPressure");
            }
        }

        private double? inletAbsorbentUpstreamMaxPressure;
        public double? InletAbsorbentUpstreamMaxPressure
        {
            get { return inletAbsorbentUpstreamMaxPressure; }
            set
            {
                inletAbsorbentUpstreamMaxPressure = value;
                dbmodel.InletAbsorbentUpstreamMaxPressure = value;
                this.NotifyPropertyChanged("InletAbsorbentUpstreamMaxPressure");
            }
        }

        private double? normalGasFeedWeightRate;
        public double? NormalGasFeedWeightRate
        {
            get { return normalGasFeedWeightRate; }
            set
            {
                normalGasFeedWeightRate = value;
                dbmodel.NormalGasFeedWeightRate = value;
                this.NotifyPropertyChanged("NormalGasFeedWeightRate");
            }
        }

        private double? normalGasProductWeightRate;
        public double? NormalGasProductWeightRate
        {
            get { return normalGasProductWeightRate; }
            set
            {
                normalGasProductWeightRate = value;
                dbmodel.NormalGasProductWeightRate = value;
                this.NotifyPropertyChanged("NormalGasProductWeightRate");
            }
        }

        #endregion

        #region Color
        private string inletGasUpstreamMaxPressure_Color;
        public string InletGasUpstreamMaxPressure_Color
        {
            get { return inletGasUpstreamMaxPressure_Color; }
            set
            {
                inletGasUpstreamMaxPressure_Color = value;
                dbmodel.InletGasUpstreamMaxPressure_Color = value;
                this.NotifyPropertyChanged("InletGasUpstreamMaxPressure_Color");
            }
        }

        private string inletAbsorbentUpstreamMaxPressure_Color;
        public string InletAbsorbentUpstreamMaxPressure_Color
        {
            get { return inletAbsorbentUpstreamMaxPressure_Color; }
            set
            {
                inletAbsorbentUpstreamMaxPressure_Color = value;
                dbmodel.InletAbsorbentUpstreamMaxPressure_Color = value;
                this.NotifyPropertyChanged("InletAbsorbentUpstreamMaxPressure_Color");
            }
        }

        private string normalGasFeedWeightRate_Color;
        public string NormalGasFeedWeightRate_Color
        {
            get { return normalGasFeedWeightRate_Color; }
            set
            {
                normalGasFeedWeightRate_Color = value;
                dbmodel.NormalGasFeedWeightRate_Color = value;
                this.NotifyPropertyChanged("NormalGasFeedWeightRate_Color");
            }
        }

        private string normalGasProductWeightRate_Color;
        public string NormalGasProductWeightRate_Color
        {
            get { return normalGasProductWeightRate_Color; }
            set
            {
                normalGasProductWeightRate_Color = value;
                dbmodel.NormalGasProductWeightRate_Color = value;
                this.NotifyPropertyChanged("NormalGasProductWeightRate_Color");
            }
        }
        #endregion

        #region Scenario Unit
        private string reliefLoadUnit;
        public string ReliefLoadUnit
        {
            get { return reliefLoadUnit; }
            set
            {
                reliefLoadUnit = value;
                NotifyPropertyChanged("ReliefLoadUnit");
            }
        }

        private string reliefPressureUnit;
        public string ReliefPressureUnit
        {
            get { return reliefPressureUnit; }
            set
            {
                reliefPressureUnit = value;
                NotifyPropertyChanged("ReliefPressureUnit");
            }
        }

        private string reliefTemperatureUnit;
        public string ReliefTemperatureUnit
        {
            get { return reliefTemperatureUnit; }
            set
            {
                reliefTemperatureUnit = value;
                NotifyPropertyChanged("ReliefTemperatureUnit");
            }
        }
        #endregion

        #region Scenario Value

        private double? reliefLoad;
        public double? ReliefLoad
        {
            get { return reliefLoad; }
            set
            {
                reliefLoad = value;
                dbScenario.ReliefLoad = reliefLoad;
                this.NotifyPropertyChanged("ReliefLoad");
            }
        }
        private double? reliefTemperatur;
        public double? ReliefTemperature
        {
            get { return reliefTemperatur; }
            set
            {
                reliefTemperatur = value;
                dbScenario.ReliefTemperature = reliefTemperatur;
                this.NotifyPropertyChanged("ReliefTemperature");
            }
        }
        private double? reliefPressure;
        public double? ReliefPressure
        {
            get { return reliefPressure; }
            set
            {
                reliefPressure = value;
                dbScenario.ReliefPressure = reliefPressure;
                this.NotifyPropertyChanged("ReliefPressure");
            }
        }
        private double? reliefMW;
        public double? ReliefMW
        {
            get { return reliefMW; }
            set
            {
                reliefMW = value;
                dbScenario.ReliefMW = reliefMW;
                this.NotifyPropertyChanged("ReliefMW");
            }
        }
        private double? reliefCpCv;
        public double? ReliefCpCv
        {
            get { return reliefCpCv; }
            set
            {
                reliefCpCv = value;
                dbScenario.ReliefCpCv = reliefCpCv;
                this.NotifyPropertyChanged("ReliefCpCv");
            }
        }
        private double? reliefZ;
        public double? ReliefZ
        {
            get { return reliefZ; }
            set
            {
                reliefZ = value;
                dbScenario.ReliefZ = reliefZ;
                this.NotifyPropertyChanged("ReliefZ");
            }
        }
        #endregion

    }
}
