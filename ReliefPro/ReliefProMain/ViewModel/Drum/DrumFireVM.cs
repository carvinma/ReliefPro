using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;
using ReliefProMain.View;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumFireVM
    {
        public ICommand FluidCMD { get; set; }
        public ICommand OKCMD { get; set; }
        public DrumFireVM(int ScenarioID, string dbPSFile, string dbPlantFile)
        {
            FluidCMD = new DelegateCommand<object>(OpenFluidWin);
            OKCMD = new DelegateCommand<object>(Save);
        }
        private void OpenFluidWin(object obj)
        {
            Drum_fireFluid win = new Drum_fireFluid();
            DrumFireFluidVM vm = new DrumFireFluidVM();
            win.DataContext = vm;
            if (win.ShowDialog() == true)
            {
            }
        }
        private void Save(object obj)
        { }
    }
}
