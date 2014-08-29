using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ReliefProMain.CustomControl
{
    public delegate void ChangeUnitDelegate(object colInfo, object OrginUnit, object TargetUnit);
    public class GVColumnHeader : GridViewColumnHeader
    {
        public ChangeUnitDelegate ChangeUnitEvent
        {
            get { return GetValue(ChangeUnitEventProperty) as ChangeUnitDelegate; }
            set { SetValue(ChangeUnitEventProperty, value); }
        }

        public string ColInfo
        {
            get { return GetValue(ColInfoProperty).ToString(); }
            set { SetValue(ColInfoProperty, value); }
        }
        public string UOrigin
        {
            get { return GetValue(UOriginProperty).ToString(); }
            set { SetValue(UOriginProperty, value); }
        }
        public static readonly DependencyProperty UOriginProperty =
           DependencyProperty.Register("UOrigin", typeof(string), typeof(GVColumnHeader), new PropertyMetadata());

        public static readonly DependencyProperty ChangeUnitEventProperty =
           DependencyProperty.Register("ChangeUnitEvent", typeof(ChangeUnitDelegate), typeof(GVColumnHeader), new PropertyMetadata());

        public static readonly DependencyProperty ColInfoProperty =
          DependencyProperty.Register("ColInfo", typeof(string), typeof(GVColumnHeader), new PropertyMetadata());


        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(UOrigin))
            {
                string tmpUorigin = UOrigin;
                UnitConvertCommonView unitConvertCommonView = new UnitConvertCommonView(UOrigin, 0);
                unitConvertCommonView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (unitConvertCommonView.ShowDialog() == true)
                {
                    this.UOrigin = unitConvertCommonView.TargetUnit;
                    if (ChangeUnitEvent != null)
                    {
                        ChangeUnitEvent(ColInfo, tmpUorigin, this.UOrigin);
                    }
                }

            }
        }
    }
}
