﻿using System;
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
        public static double GetHXArea(string ExposedToFire, string Type, double Length, double Length2, double OD)
        {
            double Area = 0;
            double D = OD;
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
                else
                {
                    Area = 3.14159 * (Length + 0.4 * D) * D;
                }
            }
            else
            {
                if (Type == "Fixed")
                {
                    Area = 3.14159 * (Length + Length2 + 0.8*D) * D;
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
                    Area = Area + 3.14159 * BootDiameter * BootHeight;
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
                    else //flat
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
                    Area = Area + 3.14159 * BootDiameter * BootHeight;
                }
                else //Spherical 
                {
                    if ((NLL + Elevation) > 7.6)
                    {
                        Area = 0.5 * 3.14159 * Math.Pow(Diameter, 2);
                    }
                    else
                    {
                        double h = 7.6 - Elevation - 0.5 * Diameter;
                        Area = 0.5 * 3.14159 * Diameter * (Diameter + 2 * h);
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
        public static double GetFullVaporW(double CpCv, double MW, double P1, double Area, double Tw,  double T1)
        {
            double result = 0;

            if (T1 >= Tw)
            {
                result = 0;
            }
            else
            {
                double C10 = 0.0395;
                double K = CpCv;
                double K2 = Math.Pow(2 / (K + 1), (K + 1) / (K - 1));
                double C = C10 * Math.Pow(K * K2, 0.5);

                double C9 = 0.2772;
                double Kd = 0.975;
                double F1 = C9 / (C * Kd);
                double F2 = Math.Pow((Tw - T1), 1.25) / Math.Pow(T1, 0.6506);
                double F = F1 * F2;
                if (F <= 182)
                {
                    double C13 = 182;
                    result = C13 * C * Area * Math.Pow(MW * P1/T1, 0.5);
                }
                else
                {
                    double C12 = 0.2772;
                    result = C12 * Math.Pow(MW * P1, 0.5) * Area * Math.Pow((Tw - T1), 1.25) / Math.Pow(T1, 1.1506);
                }
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
        public static double CalcKL(double p1, double p2, double Rmass)
        {
            double tmpP = p1 - p2;
            if (tmpP > 0)
            {
                return 2 * 490 * Math.Sqrt(tmpP * 10 * Rmass);
            }
            return 0;
        }

        public static bool CheckCritial(double P1, double P2, double K, ref double Pcf)
        {
            Pcf = 0;
            if (K == 1 || K == -1) return false;
            Pcf = P1 * Math.Pow(2 / (K + 1), K / (K - 1));
            if (P2 < Pcf) return true;

            return false;
        }
        public static double CalcWv(double d, double P1, double Rmassv, double K)
        {
            if (K == 1 || K == -1) return 0;
            return 2 * 347 * Math.Pow(d, 2) * Math.Sqrt(Rmassv * K * (P1 * 10) * Math.Pow(2 / (K + 1), (K + 1) / (K - 1)));
        }
        public static double CalcKv(double P1, double Rmassv, double K)
        {
            if (K == 1 || K == -1) return 0;
            return 2 * 347 * Math.Sqrt(Rmassv * K * (P1 * 10) * Math.Pow(2 / (K + 1), (K + 1) / (K - 1)));

        }
        public static double CalcKvSecond(double P1,double P2, double Rmassv)
        {
            double tmpP = P1 - P2;
            double Y = 1 - 0.317 * tmpP / P1;
            return 2 * 490 * Y * Math.Sqrt(tmpP * 10 * Rmassv);
        }

        /// <summary>
        /// 气相亚临界流计算
        /// </summary>
        /// <param name="d"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="Rmassv"></param>
        /// <returns></returns>
        public static double CalcWvSecond(double d, double P1, double P2, double Rmassv)
        {
            if (P1 == 0) return 0;
            double tmpP = P1 - P2;
            double Y = 1 - 0.317 * tmpP / P1;
            return 2 * 490 * Y * Math.Pow(d, 2) * Math.Sqrt(tmpP * 10 * Rmassv);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rv"></param>
        /// <param name="Kl"></param>
        /// <param name="Kv"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double CalcWH(double Rv, double Kl, double Kv, double d)
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

    /// <summary>
    /// 根据离散点进行二次的拉格朗日插值
    /// http://www.cnblogs.com/technology
    /// </summary>
    public class Lagrange
    {
        /// <summary>
        /// X各点坐标组成的数组
        /// </summary>
        public int[] x { get; set; }

        /// <summary>
        /// X各点对应的Y坐标值组成的数组
        /// </summary>
        public double[] y { get; set; }

        /// <summary>
        /// x数组或者y数组中元素的个数, 注意两个数组中的元素个数需要一样
        /// </summary>
        public int itemNum { get; set; }

        /// <summary>
        /// 初始化拉格朗日插值
        /// </summary>
        /// <param name="x">X各点坐标组成的数组</param>
        /// <param name="y">X各点对应的Y坐标值组成的数组</param>
        public Lagrange(int[] x, double[] y)
        {
            this.x = x; this.y = y;
            this.itemNum = x.Length;
        }

        /// <summary>
        /// 获得某个横坐标对应的Y坐标值
        /// </summary>
        /// <param name="xValue">x坐标值</param>
        /// <returns></returns>
        public double GetValue(int xValue)
        {
            //用于累乘数组始末下标
            int start, end;
            //返回值
            double value = 0.0;
            //如果初始的离散点为空, 返回0
            if (itemNum < 1) { return value; }
            //如果初始的离散点只有1个, 返回该点对应的Y值
            if (itemNum == 1) { value = y[0]; return value; }
            //如果初始的离散点只有2个, 进行线性插值并返回插值
            if (itemNum == 2)
            {
                value = (y[0] * (xValue - x[1]) - y[1] * (xValue - x[0])) / (x[0] - x[1]);
                return value;
            }
            //如果插值点小于第一个点X坐标, 取数组前3个点做插值
            if (xValue <= x[1]) { start = 0; end = 2; }
            //如果插值点大于等于最后一个点X坐标, 取数组最后3个点做插值
            else if (xValue >= x[itemNum - 2]) { start = itemNum - 3; end = itemNum - 1; }
            //除了上述的一些特殊情况, 通常情况如下
            else
            {
                start = 1; end = itemNum;
                int temp;
                //使用二分法决定选择哪三个点做插值
                while ((end - start) != 1)
                {
                    temp = (start + end) / 2;
                    if (xValue < x[temp - 1])
                        end = temp;
                    else
                        start = temp;
                }
                start--; end--;
                //看插值点跟哪个点比较靠近
                if (Math.Abs(xValue - x[start]) < Math.Abs(xValue - x[end]))
                    start--;
                else
                    end++;
            }
            //这时已经确定了取哪三个点做插值, 第一个点为x[start]
            double valueTemp;
            //注意是二次的插值公式
            for (int i = start; i <= end; i++)
            {
                valueTemp = 1.0;
                for (int j = start; j <= end; j++)
                    if (j != i)
                        valueTemp *= (double)(xValue - x[j]) / (double)(x[i] - x[j]);
                value += valueTemp * y[i];
            }
            return value;
        }
    }
}
