using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReliefProBLL;

namespace ReliefProMain.View
{
    /// <summary>
    /// BasicUnitInfo.xaml 的交互逻辑
    /// </summary>
    public partial class BasicUnitInfo : Window
    {
        public BasicUnitInfo()
        {
            InitializeComponent();
        }
        public string BasicNewName
        {
            get { return this.txtName.Text.Trim(); }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.BasicNewName))
            {
                UnitInfo unitInfo = new UnitInfo();
                int iCount = unitInfo.GetBasicUnit().Where(p => p.UnitName.ToLower() == this.BasicNewName.ToLower()).ToList().Count();
                if (iCount > 0)
                {
                    this.lblWarning.Visibility = Visibility.Visible;
                }
                else
                {
                    this.DialogResult = true;
                }
            }
        }

       
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.lblWarning.Visibility = Visibility.Hidden;
        }
    }
}
