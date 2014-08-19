using ReliefProCommon;
using ReliefProCommon.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReliefProMain.Model
{
    public abstract class ModelBase : DataErrorInfoBase
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //public void NotifyPropertyChanged(string propertyName)
        //{
        //    if (this.PropertyChanged != null)
        //    {
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
                MessageBox.Show(sb.ToString(), "Message Box");
                return false;
            }
            return true;
        }

        
    }
}
