using ReliefProMain.Models;
using ReliefProMain.ViewModel.TowerFires;
using ReliefProModel;
using System;
using System.Collections.Generic;
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

namespace ReliefProMain.View.TowerFires
{
    /// <summary>
    /// TowerFireInfo.xaml 的交互逻辑
    /// </summary>
    public partial class TowerFireColumnView : Window
    {
        public TowerFireColumnView()
        {
            InitializeComponent();
        }



        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                
                TowerFireColumnVM vm = (TowerFireColumnVM)this.DataContext;
                TowerFireColumnModel model = vm.model;
                if (!string.IsNullOrEmpty(txtNumberOfSegment.Text))
                {
                    model.NumberOfSegment = int.Parse(txtNumberOfSegment.Text);
                }
                int count = model.Details.Count;
                if (model.NumberOfSegment > model.Details.Count)
                {
                    for (int i = count; i < model.NumberOfSegment; i++)
                    {
                        TowerFireColumnDetail d = new TowerFireColumnDetail();
                        TowerFireColumnDetailModel detail = new TowerFireColumnDetailModel(d);
                        detail.Segment = i + 1;
                        detail.Internal = "Trayed";
                        detail.ColumnID = model.dbmodel.ID;
                        model.Details.Add(detail);
                    }
                }
                else
                {
                    for (int i = count - 1; i >= model.NumberOfSegment; i--)
                    {
                        model.Details.RemoveAt(i);
                    }
                }
            }
        }
    
    }
}
