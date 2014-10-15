using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReliefProMain
{
    /// <summary>
    /// SplashScreen.xaml 的交互逻辑
    /// </summary>
    public partial class SplashScreen : Window
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register
          ("Message", typeof(string), typeof(SplashScreen));

        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register
          ("ProgressValue", typeof(double), typeof(SplashScreen));

        public static readonly DependencyProperty SProgressValueProperty = DependencyProperty.Register
         ("SProgressValue", typeof(double), typeof(SplashScreen));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public double ProgressValue
        {
            get { return (double)this.GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        public double SProgressValue
        {
            get { return (double)this.GetValue(SProgressValueProperty); }
            set { SetValue(SProgressValueProperty, value); }
        }

        public SplashScreen()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
