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
    public class TVFileViewModel : TreeViewItemViewModel
    {
        public ISession SessionPlant { set; get; }
        public ISession SessionPS { set; get; }
        public TVFile tvFile { set; get; }
        public TVFileViewModel(TVFile tv,TVPSViewModel tvPSVM)
            : base(null, false)
        {
            tvFile = tv;
            SessionPlant = tvPSVM.SessionPlant;
            NHibernateHelper helperProtectedSystem = new NHibernateHelper(tvFile.dbProtectedSystemFile);
            SessionPS = helperProtectedSystem.GetCurrentSession();
        }

        public string Name
        {
            get { return tvFile.Name; }
            set { 
                tvFile.Name = value;
            this.OnPropertyChanged("Name");
            }
        }

        
    }
}
