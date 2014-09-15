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
    public class UCNumberTextBox:TextBox
    {
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            //可输入：数字0-9,小数点,减号
            int PPos = this.Text.IndexOf('.');   //获取当前小数点位置 
            if ((e.Key >= Key.D0 && e.Key <= Key.D9)//数字0-9键 
                || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)//小键盘数字0-9 
            {
                if (PPos != -1 && this.SelectionStart > PPos)//可输入小数，且输入点在小数点后 
                {
                    e.Handled = false; return;
                }
                else { e.Handled = false; return; }
            }
            else if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)//小数点 
            {
                if (PPos == -1)//未输入过小数点,且允许输入小数 
                {
                    if (this.SelectionStart == 0) { this.Text = "0."; }
                    e.Handled = false; return;
                }
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)//减号 不允许是负数
            {
                if (this.Text.IndexOf('-') == -1 && this.SelectionStart == 0)
                { e.Handled = true; return; }
            }
            e.Handled = true;
        }


        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            this.Text = int.Parse(this.Text).ToString();
        }
    }
}
