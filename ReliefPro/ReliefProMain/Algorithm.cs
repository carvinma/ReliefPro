using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProMain
{
    public static class Algorithm
    {
        public static double GetQ(double C1, double F, double Area)
        {
            return C1 * F * Math.Pow(Area, 0.82) * 3.6; //机算完后转换为KJ/hr
        }

        public static double GetColumnArea(string Internal, int Trays, double L1, double L2, double L3, double Diameter)
        {
            double Area = 0;

            if (Internal == "Trayed")
            {
                if ((L1 + L2 + L3) <= 7.6)
                {
                    Area = (L1 + L2 + 0.4 * Diameter) * 3.14159 * Diameter;
                }
                else
                {
                    Area = (7.6 - L3 + 0.4 * Diameter) * 3.14159 * Diameter;
                }
            }
            else
            {
                Area = (7.6 - L3 + 0.4 * Diameter) * 3.14159 * Diameter;
            }
            if (Area < 0)
                Area = 0;
            return Area;
        }
        public static double GetHXArea(string ExposedToFire, string Type, double Length, double OD, double D)
        {
            double Area = 0;

            if (ExposedToFire == "Shell")
            {
                if (Type == "Fixed")
                {
                    Area = 3.14159 * Length * D;
                }
                else if (Type == "U-Tube")
                {
                    Area = 3.14159 * (Length + 0.4 * D) * D;
                }
                else if (Type == "Floating head")
                {
                    Area = 3.14159 * (Length + 0.4 * D) * D;
                }
            }
            else
            {
                if (Type == "Fixed")
                {
                    Area = 3.14159 * Length * D;
                }
                else if (Type == "U-Tube")
                {
                    Area = 3.14159 * (Length + 0.4 * D) * D;
                }
            }
            if (Area < 0)
                Area = 0;
            return Area;
        }
        public static double GetDrumArea(string Orientation, string HeadType, double Elevation, double Diameter, double Length, double NLL, double BootHeight, double BootDiameter)
        {
            double Area = 0;

            if (Orientation == "Horiz")
            {
                if (Elevation > 7.6)
                {
                    Area = 0;
                }
                else if (Diameter <= 1.9)
                {
                    Area = (Length + 0.8 * Diameter) * 3.14159 * Math.Pow(Diameter, 2);
                }
                else if ((0.5 * Diameter + Elevation) > 7.6)
                {
                    Area = 0.5 * (Length + 0.8 * Diameter) * 3.14159 * Math.Pow(Diameter, 2);
                }
                else if ((NLL + Elevation) > 7.6)
                {
                    double hfire = 7.6 - NLL;
                    Area = 2 * (Math.Acos(1 - 2 * hfire / Diameter) * 180 / 3.14159) / 360 * 3.14159 * Diameter * Length + 0.5 * 3.14159 * Math.Pow(Diameter, 2);
                }
                else
                {
                    double hfire = NLL;
                    Area = 2 * (Math.Acos(1 - 2 * hfire / Diameter) * 180 / 3.14159) / 360 * 3.14159 * Diameter * Length + 0.5 * 3.14159 * Math.Pow(Diameter, 2);
                }
            }
            else if (Orientation == "Vertical")
            {

                if (HeadType == "Eclipse")
                {
                    if (BootHeight + Elevation <= 7.6)
                    {
                        Area = (Elevation + 0.4 * Diameter) * 3.14159 * Diameter;
                    }
                    else
                    {
                        Area = (7.6 - BootHeight + 0.4 * Diameter) * 3.14159 * Diameter;
                    }
                }
                else
                {
                    if (BootHeight + Elevation <= 7.6)
                    {
                        Area = (Elevation + 0.25 * Diameter) * 3.14159 * Diameter;
                    }
                    else
                    {
                        Area = (7.6 - BootHeight + 0.25 * Diameter) * 3.14159 * Diameter;
                    }
                }
            }
            if (Area < 0)
                Area = 0;
            return Area;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MW">分子量</param>
        /// <param name="P1">泄放压力  unit: psia</param>
        /// <param name="Area">面积  ft2</param>
        /// <param name="Tw">温度  R</param>
        /// <param name="Pn"></param>
        /// <param name="Tn"></param>
        /// <returns></returns>
        public static double GetFullVaporW(double MW, double P1, double Area, double Tw, double Pn, double Tn, ref double T1)
        {
            double result = 0;
            if (Pn != 0)
            {
                T1 = Tn * P1 / Pn;
                result = 0.1406 * Math.Pow(MW * P1, 0.5) * Area * Math.Pow((Tw - T1), 1.25) / Math.Pow(T1, 1.1506);
            }
            return result;
        }

        public static double CalcStorageTankLoad(double A, double P, double F, double L, double T, double M)
        {
            double Q = 0;
            if (A < 18.6) { Q = 63150 * A; }
            else if (A >= 18.6 && A < 93) { Q = 224200 * Math.Pow(A, 0.566); }
            else if (A >= 93 && A < 260) { Q = 630400 * Math.Pow(A, 0.338); }
            else if (A >= 260)
            {
                if (P >= 0.07 && P <= 1.034) { Q = 43200 * Math.Pow(A, 0.82); }
                else if (P <= 0.07) { Q = 4129700; }
            }
            return 881.55 * ((Q * F) / L) * Math.Pow(T / M, 0.5);
        }



    }
}
