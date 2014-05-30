using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.CommonLib
{
   public class NumberToHanzi
    {

       public static string GetHanzi(int Num) {
           string str ="";
           switch (Num) {
               case 1: str = "一"; break;
               case 2: str = "二"; break;
               case 3: str = "三"; break;
               case 4: str = "四"; break;
               case 5: str = "五"; break;
               case 6: str = "六"; break;
               case 7: str = "七"; break;
               case 8: str = "八"; break;
               case 9: str = "九"; break;
               case 10: str = "十"; break;
               default: str = "十"; break;
           }

           return str;
       }
    }
}
