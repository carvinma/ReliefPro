using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProMain.Models;
using ReliefProDAL;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ReliefProBLL.Common;
using NHibernate;

namespace ReliefProMain.ViewModel
{
    public class TVPlantViewModel: TreeViewItemViewModel
    {
        public ISession SessionPlant{set;get;}
        public TVPlant tvPlant { set; get; }
        public TVPlantViewModel(TVPlant tv) 
            : base(null, true)
        {
            tvPlant = tv;
            tv.dbPlantFile = tv.FullPath + @"\plant.mdb";
            NHibernateHelper helperProtectedSystem = new NHibernateHelper(tvPlant.dbPlantFile);
            SessionPlant = helperProtectedSystem.GetCurrentSession();
            this.IsExpanded = true;
        }

        public string Name
        {
            get { return tvPlant.Name; }
            set
            {
                tvPlant.Name = value;
                this.OnPropertyChanged("Name");
                //修改unit，ps，file等路径
            }
        }

        protected override void LoadChildren()
        {

            TreeUnitDAL dal = new TreeUnitDAL();
            IList<TreeUnit> list = dal.GetAllList(SessionPlant);
            foreach (TreeUnit u in list)
            {
                TVUnit tvu = new TVUnit();
                tvu.ID = u.ID;
                tvu.Name = u.UnitName;
                tvu.dbPlantFile = tvPlant.dbPlantFile;
                tvu.FullPath = tvPlant.FullPath + @"\" + tvu.Name;
                base.Children.Add(new TVUnitViewModel(tvu, this));
            }
        }
    }
}
