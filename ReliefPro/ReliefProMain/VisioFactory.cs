using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomVisio;

namespace ReliefProMain
{
    public class VisioFactory
    {
        public static IVisio CreateVisio(string version)
        {
            IVisio curVisio = null;
            if (version == "2003")
            {
                curVisio = new Visio2003.VisioImp();
            }
            else if (version == "2010")
            {
                curVisio = new Visio2010.VisioImp();
            }
            
            return curVisio;
        }
        
    }
}
