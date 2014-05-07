using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProMain
{
    public static class Algorithm
    {
        public static double GetTowerQ(double C1, double F, double Area)
        {
            return C1 * F * Math.Pow(Area, 0.82) * 3.6; //机算完后转换为KJ/hr
        }

        public static double GetColumnArea(string Internal,int Trays,double L1,double L2,double L3,double Diameter)
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
        public static double GetHXArea(string ExposedToFire,string Type, double Length,  double OD, double D)
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
        public static double GetDrumArea(string Orientation,string HeadType, double Elevation, double Diameter, double Length, double NLL, double BootHeight, double BootDiameter)
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
    }
}
