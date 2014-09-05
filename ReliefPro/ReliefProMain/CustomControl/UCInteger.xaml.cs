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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReliefProMain.CustomControl
{
    /// <summary>
    /// UCInteger.xaml 的交互逻辑
    /// </summary>
    public partial class UCInteger : UserControl
    {
        public UCInteger()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            //屏蔽非法按键
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = true;
                return;
            }
            e.Handled = false;
        }

        private void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {           
            TextBox txt = sender as TextBox;
            txt.Text = int.Parse(txt.Text).ToString();
        }
    }
}
