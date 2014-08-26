using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Drums;

namespace ReliefProMain.Models
{
    public class DrumDepressuringModel : ModelBase
    {
        private string initialPressureUnit;
        public string InitialPressureUnit
        {
            get { return initialPressureUnit; }
            set
            {
                initialPressureUnit = value;
                NotifyPropertyChanged("InitialPressureUnit");
            }
        }

        private string vaporDensityUnit;
        public string VaporDensityUnit
        {
            get { return vaporDensityUnit; }
            set
            {
                vaporDensityUnit = value;
                NotifyPropertyChanged("VaporDensityUnit");
            }
        }

        private string totalVaporVolumeUnit;
        public string TotalVaporVolumeUnit
        {
            get { return totalVaporVolumeUnit; }
            set
            {
                totalVaporVolumeUnit = value;
                NotifyPropertyChanged("TotalVaporVolumeUnit");
            }
        }

        private string vesseldesignpressureUnit;
        public string VesseldesignpressureUnit
        {
            get { return vesseldesignpressureUnit; }
            set
            {
                vesseldesignpressureUnit = value;
                NotifyPropertyChanged("VesseldesignpressureUnit");
            }
        }

        private string totalWettedAreaUnit;
        public string TotalWettedAreaUnit
        {
            get { return totalWettedAreaUnit; }
            set
            {
                totalWettedAreaUnit = value;
                NotifyPropertyChanged("TotalWettedAreaUnit");
            }
        }

        private string initialDepressuringRateUnit;
        public string InitialDepressuringRateUnit
        {
            get { return initialDepressuringRateUnit; }
            set
            {
                initialDepressuringRateUnit = value;
                NotifyPropertyChanged("InitialDepressuringRateUnit");
            }
        }

        private string timespecifyUnit;
        public string TimespecifyUnit
        {
            get { return timespecifyUnit; }
            set
            {
                timespecifyUnit = value;
                NotifyPropertyChanged("TimespecifyUnit");
            }
        }

        private string calculatedVesselPressureUnit;
        public string CalculatedVesselPressureUnit
        {
            get { return calculatedVesselPressureUnit; }
            set
            {
                calculatedVesselPressureUnit = value;
                NotifyPropertyChanged("CalculatedVesselPressureUnit");
            }
        }

        private string calculatedDepressuringRateUnit;
        public string CalculatedDepressuringRateUnit
        {
            get { return calculatedDepressuringRateUnit; }
            set
            {
                calculatedDepressuringRateUnit = value;
                NotifyPropertyChanged("CalculatedDepressuringRateUnit");
            }
        }

        private string detailPUnit;
        public string DetailPUnit
        {
            get { return detailPUnit; }
            set
            {
                detailPUnit = value;
                NotifyPropertyChanged("DetailPUnit");
            }
        }

        private string detailPTimeUnit;
        public string DetailPTimeUnit
        {
            get { return detailPTimeUnit; }
            set
            {
                detailPTimeUnit = value;
                NotifyPropertyChanged("DetailPTimeUnit");
            }
        }
        private string timeStepUnit;
        public string TimeStepUnit
        {
            get { return timeStepUnit; }
            set
            {
                timeStepUnit = value;
                NotifyPropertyChanged("TimeStepUnit");
            }
        }
        public DrumDepressuring dbmodel { get; set; }
        public DrumDepressuringModel(DrumDepressuring drumModel)
        {
            dbmodel = drumModel;
            this.depressuringRequirements = dbmodel.DepressuringRequirements;
            this.heatInputModel = dbmodel.HeatInputModel;
            this.valveConstantforSonicFlow = dbmodel.ValveConstantforSonicFlow;

            this.initialPressure = dbmodel.InitialPressure;
            this.vaporDensity = dbmodel.VaporDensity;
            this.totalVaporVolume = dbmodel.TotalVaporVolume;
            this.vesseldesignpressure = dbmodel.Vesseldesignpressure;
            this.totalWettedArea = dbmodel.TotalWettedArea;

            this.initialDepressuringRate = dbmodel.InitialDepressuringRate;
            this.timespecify = dbmodel.Timespecify;
            this.calculatedDepressuringRate = dbmodel.CalculatedDepressuringRate;
            this.calculatedVesselPressure = dbmodel.CalculatedVesselPressure;

            this.detailP = dbmodel.DeltaP;
            this.detailPTime = dbmodel.DeltaPTime;
            this.timeStep = dbmodel.TimeStep;
        }

        private string depressuringRequirements;
        public string DepressuringRequirements
        {
            get { return depressuringRequirements; }
            set
            {
                depressuringRequirements = value;
                NotifyPropertyChanged("DepressuringRequirements");
            }
        }

        private string heatInputModel;
        public string HeatInputModel
        {
            get { return heatInputModel; }
            set
            {
                heatInputModel = value;
                NotifyPropertyChanged("HeatInputModel");
            }
        }

        private double valveConstantforSonicFlow;
        public double ValveConstantforSonicFlow
        {
            get { return valveConstantforSonicFlow; }
            set
            {
                valveConstantforSonicFlow = value;
                NotifyPropertyChanged("ValveConstantforSonicFlow");
            }
        }

        private double initialPressure;
        public double InitialPressure
        {
            get { return initialPressure; }
            set
            {
                initialPressure = value;
                NotifyPropertyChanged("InitialPressure");
            }
        }

        private double vaporDensity;
        public double VaporDensity
        {
            get { return vaporDensity; }
            set
            {
                vaporDensity = value;
                NotifyPropertyChanged("VaporDensity");
            }
        }

        private double totalVaporVolume;
        public double TotalVaporVolume
        {
            get { return totalVaporVolume; }
            set
            {
                totalVaporVolume = value;
                NotifyPropertyChanged("TotalVaporVolume");
            }
        }

        private double vesseldesignpressure;
        public double Vesseldesignpressure
        {
            get { return vesseldesignpressure; }
            set
            {
                vesseldesignpressure = value;
                NotifyPropertyChanged("Vesseldesignpressure");
            }
        }

        private double totalWettedArea;
        public double TotalWettedArea
        {
            get { return totalWettedArea; }
            set
            {
                totalWettedArea = value;
                NotifyPropertyChanged("TotalWettedArea");
            }
        }

        private double initialDepressuringRate;
        public double InitialDepressuringRate
        {
            get { return initialDepressuringRate; }
            set
            {
                initialDepressuringRate = value;
                NotifyPropertyChanged("InitialDepressuringRate");
            }
        }

        private double timespecify;
        public double Timespecify
        {
            get { return timespecify; }
            set
            {
                timespecify = value;
                NotifyPropertyChanged("Timespecify");
            }
        }

        private double calculatedVesselPressure;
        public double CalculatedVesselPressure
        {
            get { return calculatedVesselPressure; }
            set
            {
                calculatedVesselPressure = value;
                NotifyPropertyChanged("CalculatedVesselPressure");
            }
        }

        private double calculatedDepressuringRate;
        public double CalculatedDepressuringRate
        {
            get { return calculatedDepressuringRate; }
            set
            {
                calculatedDepressuringRate = value;
                NotifyPropertyChanged("CalculatedDepressuringRate");
            }
        }
        private double detailP;
        public double DetailP
        {
            get { return detailP; }
            set
            {
                detailP = value;
                NotifyPropertyChanged("DetailP");
            }
        }

        private double detailPTime;
        public double DetailPTime
        {
            get { return detailPTime; }
            set
            {
                detailPTime = value;
                NotifyPropertyChanged("DetailPTime");
            }
        }
        private double timeStep;
        public double TimeStep
        {
            get { return timeStep; }
            set
            {
                timeStep = value;
                NotifyPropertyChanged("TimeStep");
            }
        }
    }
}
