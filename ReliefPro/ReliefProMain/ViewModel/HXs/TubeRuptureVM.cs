using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Model.HXs;
using UOMLib;

namespace ReliefProMain.ViewModel.HXs
{
    public class TubeRuptureVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private string PrzFile;
        private string PrzVersion;

        public TubeRuptureVM(int ScenarioID, string przFile, string version, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.PrzFile = przFile;
            this.PrzVersion = version;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            CalcCMD = new DelegateCommand<object>(CalcResult);
            OKCMD = new DelegateCommand<object>(Save);
        }

        public void CalcResult(object obj)
        {
            
        }
        /// <summary>
        /// calcType  0全液相 L 1 全气相 V  2  混合 V/L
        /// </summary>
        /// <param name="calcType"></param>
        private void Calc(int calcType)
        {
            double d=0;
            double p1=0;
            double p2=0;
            double rmass=0;
            double k=0;
            bool b = false;

            switch (calcType)
            {
                case 0:
                    Algorithm.CalcWL(d, p1, p2, rmass);
                    break;
                case 1:
                    b = Algorithm.CheckCritial(p1, p2, k);
                    Algorithm.CalcWv(d, p1, rmass, k);
                    break;
                case 2:
                    b = Algorithm.CheckCritial(p1, p2, k);
                    Algorithm.CalcWv(d, p1, rmass, k);
                    break;

            }

        }

        private void Save(object obj)
        {
            
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
        }
    }
}
