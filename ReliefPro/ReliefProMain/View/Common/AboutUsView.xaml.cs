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

namespace ReliefProMain.View.Common
{
    /// <summary>
    /// AboutUs.xaml 的交互逻辑
    /// </summary>
    public partial class AboutUsView : Window
    {
        public AboutUsView()
        {
            InitializeComponent();
        }

        private void myWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string imagepath = Environment.CurrentDirectory + @"\images\app.png";
                img1.Source = new BitmapImage(new Uri(imagepath, UriKind.Absolute));
            }
            catch (Exception ex)
            {
            }
        }
    }
}
