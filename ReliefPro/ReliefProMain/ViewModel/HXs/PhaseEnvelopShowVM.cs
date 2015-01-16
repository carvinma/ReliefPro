using ReliefProMain.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReliefProMain.ViewModel.HXs
{
    public class PhaseEnvelopShowVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        private bool _IsSuperCritical;
        public bool IsSuperCritical
        {
            get { return _IsSuperCritical; }
            set
            {
                _IsSuperCritical = value;
                this.OnPropertyChanged("IsSuperCritical");
            }
        }

        private bool _IsReverseCondensation;
        public bool IsReverseCondensation
        {
            get { return _IsReverseCondensation; }
            set
            {
                _IsReverseCondensation = value;
                this.OnPropertyChanged("IsReverseCondensation");
            }
        }

        public PhaseEnvelopShowVM()
        {
            OKCMD = new DelegateCommand<object>(OK);
            IsSuperCritical = true;
            IsReverseCondensation = !IsSuperCritical;
        }

        private void OK(object obj)
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
