using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using UOMLib;
using NHibernate;
using ReliefProMain.View;
using System.Windows;
using ReliefProMain.Models;
using ReliefProCommon.Enum;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class ReactorLoopSourceVM : ViewModelBase
    {
        public ISession SessionPlant { set; get; }
        public ISession SessionProtectedSystem { set; get; }
        public SourceModel model { get; set; }
        public SourceDAL sourcedal = new SourceDAL();

        UOMLib.UOMEnum uomEnum;
        public ReactorLoopSourceVM(int type,SourceModel sm, ISession sessionPlant, ISession sessionProtectedSystem)
        {           
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            model=sm;
            Source source = null;
            if (model != null)
            {
                source = sourcedal.GetModel(SessionProtectedSystem, model.SourceName);
            }
            if (source == null)
            {
                source=new Source();
                source.SourceName = model.SourceName;
                source.SourceType = model.SourceType;
            }
            if (type == 3)
            {
                model.SourceTypes.Clear();
                model.SourceTypes.Add("Compressor(Motor)");
                model.SourceTypes.Add("Compressor(Steam Turbine Driven)");
            }
            else if (type == 6)
            {
                model.SourceTypes.Remove("Compressor(Motor)");
                model.SourceTypes.Remove("Compressor(Steam Turbine Driven)");
            }
        }

        private ICommand _Update;

        public ICommand Update
        {
            get { return _Update ?? (_Update = new RelayCommand(OKClick)); }
        }


        private void OKClick(object window)
        {
            System.Windows.Window wd = window as System.Windows.Window;
            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

       
       
        
    }
}
