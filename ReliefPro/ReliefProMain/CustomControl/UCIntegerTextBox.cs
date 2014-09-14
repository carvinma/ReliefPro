using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ReliefProMain.CustomControl
{
    public class UCIntegerTextBox : TextBox
    {
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {           
            //屏蔽非法按键
            if ((e.Key >= Key.D0 && e.Key <= Key.D9)//数字0-9键 
                || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)//小键盘数字0-9 
            {
                e.Handled = true;
                return;
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)//减号 
            {
                if (this.Text.IndexOf('-') == -1 && this.SelectionStart == 0)
                { 
                    e.Handled = false; 
                    return; 
                }
                else
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = false;
        }

       
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            this.Text = int.Parse(this.Text).ToString();
        }
    }

}
