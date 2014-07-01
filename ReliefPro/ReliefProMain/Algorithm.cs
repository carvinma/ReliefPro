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
            // L1:NLL   L2:Elevation
            double Area = 0;

            if (Elevation > 7.6)
            {
                Area = 0;
            }
            else
            {
                if (Orientation == "Horizontal")
                {

                    if (Diameter <= 1.9)
                    {
                        if (HeadType == "Eclipse")
                        {
                            Area = (Length + 0.8 * Diameter) * 3.14159 * Diameter;
                        }
                        else
                        {
                            Area = (Length + Diameter) * 3.14159 * Diameter;
                        }
                    }
                    else if ((0.5 * Diameter + Elevation) > 7.6)
                    {
                        Area = 0.5 * (Length + 0.8 * Diameter) * 3.14159 * Diameter;
                    }
                    else if ((NLL + Elevation) > 7.6)
                    {
                        double hfire = 7.6 - Elevation;
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
                        if ((NLL + Elevation) <= 7.6)
                        {
                            Area = (NLL + 0.4 * Diameter) * 3.14159 * Diameter;
                        }
                        else
                        {
                            Area = (7.6 - Elevation + 0.4 * Diameter) * 3.14159 * Diameter;
                        }
                    }
                    else
                    {
                        if ((NLL + Elevation) <= 7.6)
                        {
                            Area = (NLL + 0.25 * Diameter) * 3.14159 * Diameter;
                        }
                        else
                        {
                            Area = (7.6 - Elevation + 0.25 * Diameter) * 3.14159 * Diameter;
                        }
                    }
                }
                else
                {
                    if ((NLL + Elevation) > 7.6)
                    {
                        Area = 0.5 * 3.14159 * Math.Pow(Diameter, 2);
                    }
                    else
                    {
                        double h = 7.6 - Elevation - 0.5*Diameter;
                        Area = 0.5 * 3.14159 * Diameter * (Diameter+2*h);
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
            if (L != 0 && M != 0)
            {
                double Q = 0;
                if (A < 18.6) { Q = 63150 * A; }
                else if (A >= 18.6 && A < 93) { Q = 224200 * Math.Pow(A, 0.566); }
                else if (A >= 93 && A < 260) { Q = 630400 * Math.Pow(A, 0.338); }
                else if (A >= 260)
                {
                    if (P > 0.07 && P <= 1.034) { Q = 43200 * Math.Pow(A, 0.82); }
                    else if (P <= 0.07) { Q = 4129700; }
                }
                return Q * 3.6;
            }
            return 0;
        }

        public static double CalcWL(double d, double p1, double p2, double Rmass)
        {
            double tmpP = p1 - p2;
            if (tmpP > 0)
            {
                return 2 * 490 * Math.Pow(d, 2) * Math.Sqrt(tmpP * 10 * Rmass);
            }
            return 0;
        }
        public static bool CheckCritial(double P1, double P2, double K)
        {
            if (K == 1 || K == -1) return false;
            double Pcf = P1 * Math.Pow(2 / (K + 1), K / (K - 1));
            if (P2 < Pcf) return true;
            return false;
        }
        public static double CalcWv(double d, double P1, double Rmassv, double K)
        {
            if (K == 1 || K == -1) return 0;
            return 2 * 347 * Math.Pow(d, 2) * Math.Sqrt(Rmassv * K * (P1 / 10) * Math.Pow(2 / (K + 1), (K + 1) / (K - 1)));
        }
        public static double CalcWvSecond(double d, double P1, double P2, double Rmassv)
        {
            if (P1 == 0) return 0;
            double tmpP = P1 - P2;
            double Y = 1 - 0.317 * tmpP / P1;
            return 2 * 490 * Y * Math.Pow(d, 2) * Math.Sqrt(tmpP * 10 * Rmassv);
        }
        public static double CalcWvc(double Rv, double Kl, double Kv, double d)
        {
            double tmpResult = Rv * Kl + (1 - Rv) * Kv;
            if (tmpResult == 0) return 0;
            return Rv * Kl * Kv * Math.Pow(d, 2) / tmpResult;
        }
        public static double CalcBolckedOutlet(double Q, double λ, double T1, double Tbp, double t1, double t2)
        {
            double Tav = (t1 + t2) / 2;
            if (T1 == Tav) return 0;
            return Q / λ * ((T1 - Tbp) / (T1 - Tav));
        }
    }
}
