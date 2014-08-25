using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReliefProMain.Models;
using ReliefProDAL;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ReliefProBLL.Common;
using NHibernate;

namespace ReliefProMain.ViewModel
{
    public class TVUnitViewModel : TreeViewItemViewModel
    {
        public ISession SessionPlant { set; get; }
        public TVUnit tvUnit { set; get; }
        public TVUnitViewModel(TVUnit tv,TVPlantViewModel tvPlantVM)
            : base(tvPlantVM, true)
        {
            tvUnit = tv;
            SessionPlant = tvPlantVM.SessionPlant;
            this.IsExpanded = true;
        }

        public string Name
        {
            get { return tvUnit.Name; }
            set { tvUnit.Name = value;
            tvUnit.FullPath = Directory.GetParent(tvUnit.FullPath).FullName + @"\" + tvUnit.Name;           
            this.OnPropertyChanged("Name");

            foreach (TreeViewItemViewModel item in this.Children)
            {
                TVPSViewModel p = item as TVPSViewModel;
                p.tvPS.FullPath = tvUnit.FullPath + @"\" + p.Name;


                foreach (TVFileViewModel m in item.Children)
                {
                    TVFileViewModel f = m as TVFileViewModel;
                    f.Name = p.tvPS.Name;
                    f.tvFile.FullPath = p.tvPS.FullPath + @"\design.vsd";
                    f.tvFile.Name = f.Name;
                    f.tvFile.dbPlantFile = p.tvPS.dbPlantFile;
                    f.tvFile.dbProtectedSystemFile = p.tvPS.FullPath + @"\ProtectedSystem.mdb";
                }
            }
            }
        }

        protected override void LoadChildren()
        {

            TreePSDAL dal = new TreePSDAL();
            IList<TreePS> list = dal.GetAllList(tvUnit.ID, SessionPlant);
            foreach (TreePS ps in list)
            {
                TVPS tvps = new TVPS();
                tvps.Name = ps.PSName;
                tvps.dbPlantFile = tvUnit.dbPlantFile;
                tvps.FullPath = tvUnit.FullPath + @"\" + ps.PSName;                
                base.Children.Add(new TVPSViewModel(tvps, this));
            }
        }
    }
}
