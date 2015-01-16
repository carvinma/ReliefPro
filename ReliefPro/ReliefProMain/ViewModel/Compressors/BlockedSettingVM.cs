using ReliefProMain.Commands;
using ReliefProMain.Models.Compressors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReliefProMain.ViewModel.Compressors
{
    public class BlockedSettingVM:ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public CentrifugalBlockedOutletModel model { get; set; }
        public  BlockedSettingVM(CentrifugalBlockedOutletModel m)
        {
            model = m;
            OKCMD = new DelegateCommand<object>(Save);
        }
        private void Save(object obj)
        {
            if (model.KNormal >= 0)
            {
                MessageBox.Show("K Normal must be lesser than zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (model.DeltPowY <= 0)
            {
                MessageBox.Show("Pow Y must be greater than zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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
