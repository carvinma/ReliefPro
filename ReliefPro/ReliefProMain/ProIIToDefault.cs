using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;

namespace ReliefProMain
{
    public class ProIIToDefault
    {
        public static CustomStream ConvertProIIStreamToCustomStream(ProIIStreamData s)
        {
            UnitConvert unitConvert = new UnitConvert();
            CustomStream cs = new CustomStream();
            cs.StreamName = s.StreamName;
            cs.BulkCP = s.BulkCP;
            cs.BulkCPCVRatio = cs.BulkCPCVRatio;
            cs.BulkDensityAct = s.BulkDensityAct;
            cs.BulkMwOfPhase = s.BulkMwOfPhase;
            cs.BulkSurfTension = s.BulkSurfTension;
            cs.BulkThermalCond = s.BulkThermalCond;
            cs.BulkViscosity = s.BulkViscosity;
            cs.CompIn = s.CompIn;
            cs.Componentid = s.Componentid;
            cs.InertWeightEnthalpy = s.InertWeightEnthalpy;
            cs.InertWeightRate = s.InertWeightRate;
            if (!string.IsNullOrEmpty(s.Pressure))
            {
                cs.Pressure = unitConvert.Convert("KPA", "MPAG", double.Parse(s.Pressure)).ToString();
            }
            if (!string.IsNullOrEmpty(s.Temperature))
            {
                cs.Temperature = unitConvert.Convert("K", "C", double.Parse(s.Temperature)).ToString();
            }
            cs.ProdType = s.ProdType;


            cs.TotalComposition = s.TotalComposition;
            cs.TotalMolarEnthalpy = s.TotalMolarEnthalpy;
            cs.TotalMolarRate = s.TotalMolarRate;
            cs.Tray = s.Tray;
            cs.VaporFraction = s.VaporFraction;
            cs.VaporZFmKVal = s.VaporZFmKVal;
            //cs.WeightFlow = s.WeightFlow;
            // cs.SpEnthalpy = s.SpEnthalpy;

            double TotalMolarRate = 0;
            if (!string.IsNullOrEmpty(s.TotalMolarRate))
                TotalMolarRate = double.Parse(s.TotalMolarRate);
            string bulkmwofphase = s.BulkMwOfPhase;
            if (!string.IsNullOrEmpty(bulkmwofphase))
            {
                double wf = TotalMolarRate * double.Parse(bulkmwofphase);
                cs.WeightFlow = string.Format("{0:0.0000}", wf * 3600);  //Kg/h
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
            if (TotalMolarRate > 0 && bulkmwofphase != "")
            {
                TotalMassRate = TotalMolarRate * double.Parse(bulkmwofphase);
            }


            double SpEnthalpy = 0;
            if (TotalMolarRate + InertWeightRate > 0)
            {
                SpEnthalpy = Enthalpy / (TotalMassRate + InertWeightRate);
            }
            cs.SpEnthalpy = string.Format("{0:0.0000}", SpEnthalpy);  //KJ/Kg
            return cs;


        }


    }
}
