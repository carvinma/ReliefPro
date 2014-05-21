using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using ReliefProMain.Commands;
using ReliefProMain.View;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumFireVM
    {
        public ICommand FluidCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private int ScenarioID;
        public DrumFireVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.ScenarioID = ScenarioID;
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            FluidCMD = new DelegateCommand<object>(OpenFluidWin);
            OKCMD = new DelegateCommand<object>(Save);
        }
        private void OpenFluidWin(object obj)
        {
            Drum_fireFluid win = new Drum_fireFluid();
            DrumFireFluidVM vm = new DrumFireFluidVM(ScenarioID, SessionPS, SessionPF);
            win.DataContext = vm;
            if (win.ShowDialog() == true)
            {
            }
        }
        private void Save(object obj)
        { }
    }
}
