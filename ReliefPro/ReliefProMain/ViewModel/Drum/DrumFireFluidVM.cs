using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumFireFluidVM
    {
        public ICommand OKCMD { get; set; }
        public DrumFireFluidVM()
        {
            OKCMD = new DelegateCommand<object>(Save);
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
