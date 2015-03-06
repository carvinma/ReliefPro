using ReliefProModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOMLib
{
    public class PlantInfo
    {
        /// <summary>
        /// plantID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// plantName
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Plant的DataContext
        /// </summary>
        public ORDesignerPlantDataContext DataContext  {get;set;}
        /// <summary>
        /// Plant的单位制信息
        /// </summary>
        public UOMEnum UnitInfo { get; set; }
    }
}
