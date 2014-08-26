using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Drums;

namespace ReliefProMain.Models
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
        private string designPressureUnit;
        public string DesignPressureUnit
        {
            get { return designPressureUnit; }
            set
            {
                designPressureUnit = value;
                NotifyPropertyChanged("DesignPressureUnit");
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
            this.designPressure = dbmodel.DesignPressure;
            this.HeavyOilFluid = dbmodel.HeavyOilFluid;
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
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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

        private double designPressure;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
        public double DesignPressure
        {
            get { return designPressure; }
            set
            {
                designPressure = value;
                NotifyPropertyChanged("DesignPressure");
            }
        }

        private bool heavyOilFluid;
        public bool HeavyOilFluid
        {
            get { return heavyOilFluid; }
            set
            {
                heavyOilFluid = value;
                if (heavyOilFluid == true)
                {
                    EnabledCrack = true;
                    if (crackingHeat == 0)
                        CrackingHeat = 210 * 4.187;

                }
                else
                {
                    EnabledCrack = false;
                }
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

        
        private bool enabledCrack = false;
        public bool EnabledCrack
        {
            get { return enabledCrack; }
            set
            {
                enabledCrack = value;
                NotifyPropertyChanged("EnabledCrack");
            }
        }





        private string wettedArea_Color;
        public string WettedArea_Color
        {
            get { return wettedArea_Color; }
            set
            {
                wettedArea_Color = value;
                NotifyPropertyChanged("WettedArea_Color");
            }
        }

        private string latentHeat_Color;
        public string LatentHeat_Color
        {
            get { return latentHeat_Color; }
            set
            {
                latentHeat_Color = value;
                NotifyPropertyChanged("LatentHeat_Color");
            }
        }

        private string crackingHeat_Color;
        public string CrackingHeat_Color
        {
            get { return crackingHeat_Color; }
            set
            {
                crackingHeat_Color = value;
                NotifyPropertyChanged("CrackingHeat_Color");
            }
        }

        private string reliefLoad_Color;
        public string ReliefLoad_Color
        {
            get { return reliefLoad_Color; }
            set
            {
                reliefLoad_Color = value;
                NotifyPropertyChanged("ReliefLoad_Color");
            }
        }

        private string reliefPressure_Color;
        public string ReliefPressure_Color
        {
            get { return reliefPressure_Color; }
            set
            {
                reliefPressure_Color = value;
                NotifyPropertyChanged("ReliefPressure_Color");
            }
        }

        private string reliefTemperature_Color;
        public string ReliefTemperature_Color
        {
            get { return reliefTemperature_Color; }
            set
            {
                reliefTemperature_Color = value;
                NotifyPropertyChanged("ReliefTemperature_Color");
            }
        }

        private string reliefMW_Color;
        public string ReliefMW_Color
        {
            get { return reliefMW_Color; }
            set
            {
                reliefMW_Color = value;
                NotifyPropertyChanged("ReliefMW_Color");
            }
        }

        private string reliefCpCv_Color;
        public string ReliefCpCv_Color
        {
            get { return reliefCpCv_Color; }
            set
            {
                reliefCpCv_Color = value;
                NotifyPropertyChanged("ReliefCpCv_Color");
            }
        }

        private string reliefZ_Color;
        public string ReliefZ_Color
        {
            get { return reliefZ_Color; }
            set
            {
                reliefZ_Color = value;
                NotifyPropertyChanged("ReliefZ_Color");
            }
        }

        private string designPressure_Color;
        public string DesignPressure_Color
        {
            get { return designPressure_Color; }
            set
            {
                designPressure_Color = value;
                NotifyPropertyChanged("DesignPressure_Color");
            }
        }

        private string heavyOilFluid_Color;
        public string HeavyOilFluid_Color
        {
            get { return heavyOilFluid_Color; }
            set
            {
                heavyOilFluid_Color = value;

                NotifyPropertyChanged("HeavyOilFluid_Color");
            }
        }
        private string allGas_Color;
        public string AllGas_Color
        {
            get { return allGas_Color; }
            set
            {
                allGas_Color = value;
                NotifyPropertyChanged("AllGas_Color");
            }
        }

        private string noneallGas_Color;
        public string NoneAllGas_Color
        {
            get { return noneallGas_Color; }
            set
            {
                noneallGas_Color = value;
                NotifyPropertyChanged("NoneAllGas_Color");
            }
        }
        private string equipmentExist_Color;
        public string EquipmentExist_Color
        {
            get { return equipmentExist_Color; }
            set
            {
                equipmentExist_Color = value;
                NotifyPropertyChanged("EquipmentExist_Color");
            }
        }


        private string enabledCrack_Color;
        public string EnabledCrack_Color
        {
            get { return enabledCrack_Color; }
            set
            {
                enabledCrack_Color = value;
                NotifyPropertyChanged("EnabledCrack_Color");
            }
        }
    }
}
