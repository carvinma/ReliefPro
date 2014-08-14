using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ReliefProCommon;
using ReliefProCommon.Enum;

namespace ReliefProMain.ViewModel
{
    //public abstract class ViewModelBase
    //{
    //    public ViewModelBase() { }
    //}

    public abstract class ViewModelBase : DataErrorInfoBase
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;

        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        public const string GreaterThanZero = @"^(?!0(\.0+)?$)([1-9][0-9]*|0)(\.[0-9]+)?$";
        public const string IsNum = @"^[+-]?/d*[.]?/d*$";
        public bool CheckData()
        {
            foreach (var pInfo in this.GetType().GetProperties())
            {
                if (pInfo.Name.Contains("_Color"))
                {
                    pInfo.SetValue(this, ColorBorder.blue.ToString(), null);
                }
            }
            if (!this.Validate())
            {
                string sb = string.Empty;
                foreach (KeyValuePair<string, string> kvp in this.DataErrors)
                {
                    if (!sb.Contains(kvp.Value))
                        sb = sb + kvp.Value + "\r\n";
                    var pInfo = this.GetType().GetProperty(kvp.Key + "_Color");
                    pInfo.SetValue(this, ColorBorder.red.ToString(), null);
                }
                MessageBox.Show(sb.ToString());
                return false;
            }
            return true;
        }
    }
}
