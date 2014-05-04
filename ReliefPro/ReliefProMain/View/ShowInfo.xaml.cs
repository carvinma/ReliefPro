using System.Windows;
using ReliefProMain.ViewModel;

namespace ReliefProMain.View
{
    /// <summary>
    /// ShowInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ShowInfo : Window
    {
        private ListDemoViewModel demoViewModel = new ListDemoViewModel();
        public ShowInfo()
        {
            InitializeComponent();
            this.DataContext = demoViewModel;
        }
    }
}
