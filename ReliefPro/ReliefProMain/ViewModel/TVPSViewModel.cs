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
using System.IO;

namespace ReliefProMain.ViewModel
{
    public class TVPSViewModel : TreeViewItemViewModel
    {
        public ISession SessionPlant { set; get; }
        public TVPS tvPS { set; get; }
        public TVPSViewModel(TVPS tv,TVUnitViewModel tvUnitVM)
            : base(tvUnitVM, true)
        {
            tvPS = tv;
            this.IsExpanded = true;
            SessionPlant = tvUnitVM.SessionPlant;
        }

        public string Name
        {
            get { return tvPS.Name; }
            set
            {
                tvPS.Name = value;
                tvPS.FullPath=Directory.GetParent(tvPS.FullPath).FullName+@"\"+tvPS.Name;
                
                this.OnPropertyChanged("Name");
                
                foreach (TreeViewItemViewModel m in this.Children)
                {
                    TVFileViewModel f =  m as TVFileViewModel;
                    f.Name = tvPS.Name;
                    f.tvFile.FullPath = tvPS.FullPath + @"\design.vsd";
                    f.tvFile.Name = f.Name;
                    f.tvFile.dbPlantFile = tvPS.dbPlantFile;
                    f.tvFile.dbProtectedSystemFile = tvPS.FullPath + @"\ProtectedSystem.mdb";
                                        
                }
            }
        }

        protected override void LoadChildren()
        {
            TVFile tvfile = new TVFile();
            tvfile.dbPlantFile = tvPS.dbPlantFile;
            tvfile.FullPath = tvPS.FullPath+@"\design.vsd";
            tvfile.Name = tvfile.Name;
            tvfile.dbPlantFile = tvPS.dbPlantFile;
            tvfile.dbProtectedSystemFile = tvPS.FullPath + @"\ProtectedSystem.mdb";
            tvfile.Name = tvPS.Name;
            base.Children.Add(new TVFileViewModel(tvfile,this));
        }
    }
}
