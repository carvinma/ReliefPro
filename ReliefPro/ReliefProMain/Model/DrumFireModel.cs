using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Drum;

namespace ReliefProMain.Model
{
    public class DrumFireModel : ModelBase
    {
        private string wettedAreaUnit;
        public string WettedAreaUnit
        {
            get { return wettedAreaUnit; }
            set
            {
                wettedAreaUnit = value;
                NotifyPropertyChanged("WettedAreaUnit");
            }
        }

        private string latentHeatUnit;
        public string LatentHeatUnit
        {
            get { return latentHeatUnit; }
            set
            {
                latentHeatUnit = value;
                NotifyPropertyChanged("LatentHeatUnit");
            }
        }

        private string crackingHeatUnit;
        public string CrackingHeatUnit
        {
            get { return crackingHeatUnit; }
            set
            {
                crackingHeatUnit = value;
                NotifyPropertyChanged("CrackingHeatUnit");
            }
        }

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

        public DrumFireCalc dbmodel { get; set; }
        public DrumFireModel(DrumFireCalc fireModel)
        {
            dbmodel = fireModel;
            this.wettedArea = dbmodel.WettedArea;
            this.latentHeat = dbmodel.LatentHeat;
            this.crackingHeat = dbmodel.CrackingHeat;
            this.reliefLoad = dbmodel.ReliefLoad;
            this.reliefPressure = dbmodel.ReliefPressure;
            this.reliefTemperature = dbmodel.ReliefTemperature;
            this.ReliefMW = dbmodel.ReliefMW;
            this.reliefCpCv = dbmodel.ReliefCpCv;
            this.reliefZ = dbmodel.ReliefZ;

            this.heavyOilFluid = dbmodel.HeavyOilFluid;
            this.allGas = dbmodel.AllGas;
            this.equipmentExist = dbmodel.EquipmentExist;
        }


        private double wettedArea;
        public double WettedArea
        {
            get { return wettedArea; }
            set
            {
                wettedArea = value;
                NotifyPropertyChanged("WettedArea");
            }
        }

        private double latentHeat;
        public double LatentHeat
        {
            get { return latentHeat; }
            set
            {
                latentHeat = value;
                NotifyPropertyChanged("LatentHeat");
            }
        }

        private double crackingHeat;
        public double CrackingHeat
        {
            get { return crackingHeat; }
            set
            {
                crackingHeat = value;
                NotifyPropertyChanged("CrackingHeat");
            }
        }

        private double reliefLoad;
        public double ReliefLoad
        {
            get { return reliefLoad; }
            set
            {
                reliefLoad = value;
                NotifyPropertyChanged("ReliefLoad");
            }
        }

        private double reliefPressure;
        public double ReliefPressure
        {
            get { return reliefPressure; }
            set
            {
                reliefPressure = value;
                NotifyPropertyChanged("ReliefPressure");
            }
        }

        private double reliefTemperature;
        public double ReliefTemperature
        {
            get { return reliefTemperature; }
            set
            {
                reliefTemperature = value;
                NotifyPropertyChanged("ReliefTemperature");
            }
        }

        private double reliefMW;
        public double ReliefMW
        {
            get { return reliefMW; }
            set
            {
                reliefMW = value;
                NotifyPropertyChanged("ReliefMW");
            }
        }

        private double reliefCpCv;
        public double ReliefCpCv
        {
            get { return reliefCpCv; }
            set
            {
                reliefCpCv = value;
                NotifyPropertyChanged("ReliefCpCv");
            }
        }

        private double reliefZ;
        public double ReliefZ
        {
            get { return reliefZ; }
            set
            {
                reliefZ = value;
                NotifyPropertyChanged("ReliefZ");
            }
        }

        private bool heavyOilFluid;
        public bool HeavyOilFluid
        {
            get { return heavyOilFluid; }
            set
            {
                heavyOilFluid = value;
                NotifyPropertyChanged("HeavyOilFluid");
            }
        }
        private bool allGas;
        public bool AllGas
        {
            get { return allGas; }
            set
            {
                allGas = value;
                NoneAllGas = !value;
                NotifyPropertyChanged("AllGas");
            }
        }

        private bool noneallGas = false;
        public bool NoneAllGas
        {
            get { return noneallGas; }
            set
            {
                noneallGas = value;
                NotifyPropertyChanged("NoneAllGas");
            }
        }
        private bool equipmentExist;
        public bool EquipmentExist
        {
            get { return equipmentExist; }
            set
            {
                equipmentExist = value;
                NotifyPropertyChanged("EquipmentExist");
            }
        }


    }
}
