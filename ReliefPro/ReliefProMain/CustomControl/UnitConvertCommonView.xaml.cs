using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ReliefProBLL;
using ReliefProModel;
using UOMLib;

namespace ReliefProMain.CustomControl
{
    /// <summary>
    /// UnitConvertCommonView.xaml
    /// </summary>
    public partial class UnitConvertCommonView : Window
    {
        private ILookup<int, SystemUnit> lkpSystemUnit;
        private ILookup<string, UnitType> lkpUnitType;
        private UnitInfo unitInfo;
        private int UnitTypeID;
        private string OriginUnit;
        public string TargetUnit;
        private double Value;
        public double ResultValue { get; set; }
        public UnitConvertCommonView()
        {
            InitializeComponent();
        }
        public UnitConvertCommonView(string OriginUnit, double value)
            : this()
        {
            this.OriginUnit = OriginUnit;
            this.Value = value;
            InitInfo(OriginUnit);
            InitListBox();
        }
        private void InitListBox()
        {
            try
            {
                this.ltboxUnit.ItemsSource = lkpSystemUnit[this.UnitTypeID].ToList();
                this.ltboxUnit.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void InitInfo(string Unit)
        {
            try
            {
                unitInfo = new UnitInfo();
                var tmpSystemUnit = unitInfo.GetSystemUnit();
                if (null != tmpSystemUnit)
                {
                    var systemUnit = tmpSystemUnit.Where(p => p.Name.ToLower() == Unit.ToLower()).FirstOrDefault();
                    if (systemUnit != null)
                    {
                        UnitTypeID = systemUnit.UnitType;
                    }
                    lkpSystemUnit = tmpSystemUnit.ToLookup(p => p.UnitType);
                    if (tmpSystemUnit.Count > 0 && UnitTypeID > 0)
                    {
                        TargetUnit = lkpSystemUnit[UnitTypeID].First().Name;
                        this.lblInfo.Content = string.Format("Change {0} To {1}", Unit, TargetUnit);
                    }
                }
                var tmpUnitType = unitInfo.GetUnitType();
                if (null != tmpUnitType)
                    lkpUnitType = tmpUnitType.ToLookup(p => p.ShortName.ToLower());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ltboxUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ltboxUnit.SelectedIndex == -1)
                return;
            var selectedObject = ltboxUnit.SelectedItem as SystemUnit;
            if (selectedObject != null)
            {
                TargetUnit = selectedObject.Name;
                string targetUnit = TargetUnit;
                if (this.chkLong.IsChecked.HasValue && this.chkLong.IsChecked.Value)
                {
                    targetUnit = selectedObject.Description;
                }
                this.lblInfo.Content = string.Format("Change {0} To {1}", OriginUnit, targetUnit);
            }
        }

        private void chkLong_Checked(object sender, RoutedEventArgs e)
        {
            this.ltboxUnit.DisplayMemberPath = "Description";
        }

        private void chkLong_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ltboxUnit.DisplayMemberPath = "Name";
        }


        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            //var sw = Stopwatch.StartNew();
            // long t1 = sw.ElapsedMilliseconds;
            //ResultValue = unitConvert.Convert(UnitType, OriginUnit, TargetUnit, Value);
            ResultValue = UnitConvert.Convert(OriginUnit, TargetUnit, Value);
            this.DialogResult = true;
        }
    }
}
