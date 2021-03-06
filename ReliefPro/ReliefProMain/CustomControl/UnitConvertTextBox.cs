﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ReliefProMain.CustomControl
{
    public class UnitConvertTextBox : TextBox
    {
        private string format = "";
        public string Format
        {
            get { return format; }
            set { format = value; }
        }
        private string preUnit = string.Empty;
        public string UnitOrigin
        {
            get { return GetValue(UnitOriginProperty).ToString(); }
            set { SetValue(UnitOriginProperty, value); }
        }
        private string dbFirstValue = string.Empty;
        private object dbFirstColor = null;
        private bool isFirst = true;
        public static readonly DependencyProperty UnitOriginProperty =
           DependencyProperty.Register("UnitOrigin", typeof(string), typeof(UnitConvertTextBox), new PropertyMetadata());
        public UnitConvertTextBox()
        {
        }
        private object GetVale(object obj, List<string> propertys)
        {
            Type t = obj.GetType();
            string current = propertys[0];
            propertys.RemoveAt(0);
            if (propertys.Count > 0)
                return GetVale(t.GetProperty(current).GetValue(obj, null), propertys);
            else
                return t.GetProperty(current).GetValue(obj, null);
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            //BindingExpression expresson = this.GetBindingExpression(TextBox.TextProperty);
            //expresson.UpdateSource();
            if (isFirst)
            {
                if (BindingOperations.IsDataBound(this, TextBox.TextProperty))
                {
                    dbFirstValue = this.Text.Trim();
                    dbFirstColor = this.GetValue(TextBox.BorderBrushProperty);
                    isFirst = false;
                }
            }
            if (this.GetValue(UnitConvertTextBox.UnitOriginProperty) != null)
            {
                if (!string.IsNullOrEmpty(preUnit) && preUnit != this.UnitOrigin)
                {
                    preUnit = string.Empty;
                }
                else
                {
                    if (!this.IsReadOnly)
                    {
                        if (dbFirstValue == this.Text.Trim())
                            this.SetValue(TextBox.BorderBrushProperty, dbFirstColor);
                        else
                            this.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Blue));
                    }
                }
            }
        }

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(UnitOrigin))
            {
                preUnit = UnitOrigin;
                double UnitValue;
                if (BindingOperations.IsDataBound(this, TextBox.TextProperty))
                {
                    //this.DataContext = obj;
                    //Binding b = new Binding(property);
                    //b.Mode = BindingMode.TwoWay;
                    //b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    //BindingOperations.SetBinding(textBox, textProp, b);
                }

                BindingExpression bexp = this.GetBindingExpression(TextBox.TextProperty);
                //bexp.UpdateSource();
                var data = bexp.DataItem;
                Binding b = bexp.ParentBinding;
                string[] str = b.Path.Path.Split('.');

                object txtValue = GetVale(data, str.ToList());

                if (double.TryParse(this.Text.Trim(), out UnitValue))
                {
                    UnitConvertCommonView unitConvertCommonView = new UnitConvertCommonView(UnitOrigin, UnitValue);
                    unitConvertCommonView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    if (unitConvertCommonView.ShowDialog() == true)
                    {
                        this.UnitOrigin = unitConvertCommonView.TargetUnit;
                        if (!string.IsNullOrEmpty(format))
                            this.Text = unitConvertCommonView.ResultValue.ToString(format);
                        else
                            this.Text = unitConvertCommonView.ResultValue.ToString();
                    }
                }
                else
                {
                    UnitConvertCommonView unitConvertCommonView = new UnitConvertCommonView(UnitOrigin, 0);
                    unitConvertCommonView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    if (unitConvertCommonView.ShowDialog() == true)
                    {
                        this.UnitOrigin = unitConvertCommonView.TargetUnit;
                    }
                }
            }
            return;
        }

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
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)//减号 
            {
                if (this.Text.IndexOf('-') == -1 && this.SelectionStart == 0)
                { e.Handled = false; return; }
            }
            e.Handled = true;
        }

    }
}
