using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        public string UnitOrigin
        {
            get { return GetValue(UnitOriginProperty).ToString(); }
            set { SetValue(UnitOriginProperty, value); }
        }
        public static readonly DependencyProperty UnitOriginProperty =
           DependencyProperty.Register("UnitOrigin", typeof(string), typeof(UnitConvertTextBox), new PropertyMetadata());
        public UnitConvertTextBox()
        {
        }
        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(UnitOrigin))
            {
                double UnitValue;
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
            }
            return;
        }

    }
}
