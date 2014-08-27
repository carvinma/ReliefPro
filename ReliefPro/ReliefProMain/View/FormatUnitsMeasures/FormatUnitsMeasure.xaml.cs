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
using ReliefProMain.ViewModel;

namespace ReliefProMain.View
{
    /// <summary>
    /// FormatUnitsMeasure.xaml 的交互逻辑
    /// </summary>
    public partial class FormatUnitsMeasure : Window
    {
        private FormatUnitsMeasureVM viewModel = new FormatUnitsMeasureVM();
        public FormatUnitsMeasure()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            UOMLib.UOMEnum.UnitFormFlag = true;
        }
    }
}
