using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using UOMLib;

namespace ReliefProMain
{
    public class ProIIToDefault
    {
        public static CustomStream ConvertProIIStreamToCustomStream(ProIIStreamData s)
        {
            if (s == null)
                return null;
            CustomStream cs = new CustomStream();
            cs.StreamName = s.StreamName;
            if (!string.IsNullOrEmpty(s.BulkCP))
            {
                cs.BulkCP = double.Parse(s.BulkCP);
            }
            if (!string.IsNullOrEmpty(s.BulkCPCVRatio))
            {
                cs.BulkCPCVRatio = double.Parse(s.BulkCPCVRatio);
            }
            
            if (!string.IsNullOrEmpty(s.BulkDensityAct))
            {
                cs.BulkDensityAct = double.Parse(s.BulkDensityAct);
            }
            if (!string.IsNullOrEmpty(s.BulkMwOfPhase))
            {
                cs.BulkMwOfPhase = double.Parse(s.BulkMwOfPhase);
            }
            if (!string.IsNullOrEmpty(s.BulkSurfTension))
            {
                cs.BulkSurfTension = double.Parse(s.BulkSurfTension);
            }
            if (!string.IsNullOrEmpty(s.BulkThermalCond))
            {
                cs.BulkThermalCond = double.Parse(s.BulkThermalCond);
            }
            if (!string.IsNullOrEmpty(s.BulkViscosity))
            {
                cs.BulkViscosity = double.Parse(s.BulkViscosity);
            }
            cs.ProdType = s.ProdType;
            cs.CompIn = s.CompIn;
            cs.Componentid = s.Componentid;
            cs.PrintNumber = s.PrintNumber;
            if (!string.IsNullOrEmpty(s.InertWeightEnthalpy))
            {
                cs.InertWeightEnthalpy = double.Parse(s.InertWeightEnthalpy);
            }

            if (!string.IsNullOrEmpty(s.InertWeightRate))
            {
                cs.InertWeightRate = double.Parse(s.InertWeightRate);
            }
            if (!string.IsNullOrEmpty(s.Pressure))
            {
                cs.Pressure = UnitConvert.Convert("KPA", "MPAG", double.Parse(s.Pressure));
            }
            if (!string.IsNullOrEmpty(s.Temperature))
            {
                cs.Temperature = UnitConvert.Convert("K", "C", double.Parse(s.Temperature));
            }
            cs.ProdType = s.ProdType;


            cs.TotalComposition = s.TotalComposition;
            if (!string.IsNullOrEmpty(s.TotalMolarEnthalpy))
            {
                cs.TotalMolarEnthalpy = double.Parse(s.TotalMolarEnthalpy);
            }
            if (!string.IsNullOrEmpty(s.TotalMolarRate))
            {
                cs.TotalMolarRate = double.Parse(s.TotalMolarRate);
            }
            if (!string.IsNullOrEmpty(s.Tray))
            {
                cs.Tray = Int32.Parse(s.Tray);
            }
            if (!string.IsNullOrEmpty(s.VaporFraction))
            {
                cs.VaporFraction = double.Parse(s.VaporFraction);
            }
            if (!string.IsNullOrEmpty(s.VaporZFmKVal))
            {
                cs.VaporZFmKVal = double.Parse(s.VaporZFmKVal);
            }
            //cs.WeightFlow = s.WeightFlow;
            // cs.SpEnthalpy = s.SpEnthalpy;

            double TotalMolarRate = 0;
            if (!string.IsNullOrEmpty(s.TotalMolarRate))
                TotalMolarRate = double.Parse(s.TotalMolarRate);
            string bulkmwofphase = s.BulkMwOfPhase;
            cs.WeightFlow = 0;
            if (!string.IsNullOrEmpty(bulkmwofphase))
            {
                double wf = TotalMolarRate * double.Parse(bulkmwofphase);
                cs.WeightFlow = double.Parse(string.Format("{0:0.0000}", wf * 3600));  //Kg/h
            }

            //enthalpy=TotalMolarEnthalpy*TotalMolarRate+InertWeightEnthalpy*InertWeightRate;
            double TotalMolarEnthalpy = 0;
            string strTotalMolarEnthalpy = s.TotalMolarEnthalpy;
            if (!string.IsNullOrEmpty(strTotalMolarEnthalpy))
            {
                TotalMolarEnthalpy = double.Parse(strTotalMolarEnthalpy);
            }


            double InertWeightEnthalpy = 0;
            string strInertWeightEnthalpy = s.InertWeightEnthalpy;
            if (!string.IsNullOrEmpty(strInertWeightEnthalpy))
            {
                InertWeightEnthalpy = double.Parse(strInertWeightEnthalpy);
            }

            double InertWeightRate = 0;
            string strInertWeightRate = s.InertWeightRate;
            if (!string.IsNullOrEmpty(strInertWeightRate))
            {
                InertWeightRate = double.Parse(strInertWeightRate);
            }


            double Enthalpy = TotalMolarEnthalpy * TotalMolarRate + InertWeightEnthalpy * InertWeightRate;
            //cs.Enthalpy = string.Format("{0:0.0000}", Enthalpy * 3600); //KJ


            double TotalMassRate = 0;
            if (TotalMolarRate > 0 && !string.IsNullOrEmpty(bulkmwofphase))
            {
                TotalMassRate = TotalMolarRate * double.Parse(bulkmwofphase);
            }


            double SpEnthalpy = 0;
            if (TotalMassRate + InertWeightRate > 0)
            {
                SpEnthalpy = Enthalpy / (TotalMassRate + InertWeightRate);
            }
            cs.SpEnthalpy = double.Parse(string.Format("{0:0.0000}", SpEnthalpy));  //KJ/Kg
            return cs;


        }

        public static LatentProduct ConvertProIIStreamToLatentProduct(ProIIStreamData s)
        {
            UnitConvert unitConvert = new UnitConvert();
            LatentProduct cs = new LatentProduct();
            cs.StreamName = s.StreamName;
            if (!string.IsNullOrEmpty(s.BulkCP))
            {
                cs.BulkCP = double.Parse(s.BulkCP);
            }
            if (!string.IsNullOrEmpty(s.BulkCPCVRatio))
            {
                cs.BulkCPCVRatio = double.Parse(s.BulkCPCVRatio);
            }
            if (!string.IsNullOrEmpty(s.BulkDensityAct))
            {
                cs.BulkDensityAct = double.Parse(s.BulkDensityAct);
            }
            if (!string.IsNullOrEmpty(s.BulkMwOfPhase))
            {
                cs.BulkMwOfPhase = double.Parse(s.BulkMwOfPhase);
            }
            if (!string.IsNullOrEmpty(s.BulkSurfTension))
            {
                cs.BulkSurfTension = double.Parse(s.BulkSurfTension);
            }
            if (!string.IsNullOrEmpty(s.BulkThermalCond))
            {
                cs.BulkThermalCond = double.Parse(s.BulkThermalCond);
            }
            if (!string.IsNullOrEmpty(s.BulkViscosity))
            {
                cs.BulkViscosity = double.Parse(s.BulkViscosity);
            }
            cs.ProdType = s.ProdType;
            cs.CompIn = s.CompIn;
            cs.Componentid = s.Componentid;
            cs.PrintNumber = s.PrintNumber;
            if (!string.IsNullOrEmpty(s.InertWeightEnthalpy))
            {
                cs.InertWeightEnthalpy = double.Parse(s.InertWeightEnthalpy);
            }

            if (!string.IsNullOrEmpty(s.InertWeightRate))
            {
                cs.InertWeightRate = double.Parse(s.InertWeightRate);
            }
            if (!string.IsNullOrEmpty(s.Pressure))
            {
                cs.Pressure = UnitConvert.Convert("KPA", "MPAG", double.Parse(s.Pressure));
            }
            if (!string.IsNullOrEmpty(s.Temperature))
            {
                cs.Temperature = UnitConvert.Convert("K", "C", double.Parse(s.Temperature));
            }
            cs.ProdType = s.ProdType;


            cs.TotalComposition = s.TotalComposition;
            if (!string.IsNullOrEmpty(s.TotalMolarEnthalpy))
            {
                cs.TotalMolarEnthalpy = double.Parse(s.TotalMolarEnthalpy);
            }
            if (!string.IsNullOrEmpty(s.TotalMolarRate))
            {
                cs.TotalMolarRate = double.Parse(s.TotalMolarRate);
            }
            if (!string.IsNullOrEmpty(s.Tray))
            {
                cs.Tray = Int32.Parse(s.Tray);
            }
            if (!string.IsNullOrEmpty(s.VaporFraction))
            {
                cs.VaporFraction = double.Parse(s.VaporFraction);
            }
            if (!string.IsNullOrEmpty(s.VaporZFmKVal))
            {
                cs.VaporZFmKVal = double.Parse(s.VaporZFmKVal);
            }
            //cs.WeightFlow = s.WeightFlow;
            // cs.SpEnthalpy = s.SpEnthalpy;

            double TotalMolarRate = 0;
            if (!string.IsNullOrEmpty(s.TotalMolarRate))
                TotalMolarRate = double.Parse(s.TotalMolarRate);
            string bulkmwofphase = s.BulkMwOfPhase;
            cs.WeightFlow = 0;
            if (!string.IsNullOrEmpty(bulkmwofphase))
            {
                double wf = TotalMolarRate * double.Parse(bulkmwofphase);
                cs.WeightFlow = double.Parse(string.Format("{0:0.0000}", wf * 3600));  //Kg/h
            }

            //enthalpy=TotalMolarEnthalpy*TotalMolarRate+InertWeightEnthalpy*InertWeightRate;
            double TotalMolarEnthalpy = 0;
            string strTotalMolarEnthalpy = s.TotalMolarEnthalpy;
            if (!string.IsNullOrEmpty(strTotalMolarEnthalpy))
            {
                TotalMolarEnthalpy = double.Parse(strTotalMolarEnthalpy);
            }


            double InertWeightEnthalpy = 0;
            string strInertWeightEnthalpy = s.InertWeightEnthalpy;
            if (!string.IsNullOrEmpty(strInertWeightEnthalpy))
            {
                InertWeightEnthalpy = double.Parse(strInertWeightEnthalpy);
            }

            double InertWeightRate = 0;
            string strInertWeightRate = s.InertWeightRate;
            if (!string.IsNullOrEmpty(strInertWeightRate))
            {
                InertWeightRate = double.Parse(strInertWeightRate);
            }


            double Enthalpy = TotalMolarEnthalpy * TotalMolarRate + InertWeightEnthalpy * InertWeightRate;
            //cs.Enthalpy = string.Format("{0:0.0000}", Enthalpy * 3600); //KJ


            double TotalMassRate = 0;
            if (TotalMolarRate > 0 && !string.IsNullOrEmpty(bulkmwofphase))
            {
                TotalMassRate = TotalMolarRate * double.Parse(bulkmwofphase);
            }


            double SpEnthalpy = 0;
            if (TotalMassRate + InertWeightRate > 0)
            {
                SpEnthalpy = Enthalpy / (TotalMassRate + InertWeightRate);
            }
            cs.SpEnthalpy = double.Parse(string.Format("{0:0.0000}", SpEnthalpy));  //KJ/Kg
            return cs;


        }

        public static LatentProduct ConvertCustomStreamToLatentProduct(CustomStream s)
        {
            LatentProduct cs = new LatentProduct();
            cs.StreamName = s.StreamName;
            cs.BulkCP = s.BulkCP;            
            cs.BulkCPCVRatio = s.BulkCPCVRatio;
            cs.BulkDensityAct = s.BulkDensityAct;
            cs.BulkMwOfPhase = s.BulkMwOfPhase;
            cs.BulkSurfTension = s.BulkSurfTension;
            cs.BulkThermalCond = s.BulkThermalCond;
            cs.BulkViscosity = s.BulkViscosity;            
            cs.ProdType = s.ProdType;
            cs.CompIn = s.CompIn;
            cs.Componentid = s.Componentid;
            cs.PrintNumber = s.PrintNumber;
            cs.InertWeightEnthalpy = s.InertWeightEnthalpy;
            cs.InertWeightRate = s.InertWeightRate;
            cs.Pressure = s.Pressure;
            cs.Temperature = s.Temperature;
            cs.ProdType = s.ProdType;
            cs.TotalComposition = s.TotalComposition;
            cs.TotalMolarEnthalpy = s.TotalMolarEnthalpy;
            cs.TotalMolarRate = s.TotalMolarRate;
            cs.Tray = s.Tray;
            cs.VaporFraction = s.VaporFraction;
            cs.VaporZFmKVal = s.VaporZFmKVal;
            cs.WeightFlow = s.WeightFlow;
            cs.SpEnthalpy = s.SpEnthalpy;
            cs.TotalMolarRate = s.TotalMolarRate;
            cs.BulkMwOfPhase = s.BulkMwOfPhase;
            cs.TotalMolarEnthalpy = s.TotalMolarEnthalpy;
            cs.InertWeightEnthalpy = s.InertWeightEnthalpy;
            cs.InertWeightRate = s.InertWeightRate;
            return cs;
        }
         
    }
}
