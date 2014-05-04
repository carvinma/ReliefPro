using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.CommonLib
{
   public class BaseDataUtil
    {

       public static IList<KVM> kvmlist = new List<KVM>();

       public static string GetItemNameByModuleidAndItemid(String Moduleid, String Itemid){
         var p = kvmlist.FirstOrDefault(w => w.K.Equals(Itemid) && w.M.Equals(Moduleid));
            if (p != null) return p.V;
            else return "";
       }
    }

   [Serializable]
   public class KVM {

       public String K { get; set; }

       public String V { get; set; }

       public String M { get; set; }
   }
}
