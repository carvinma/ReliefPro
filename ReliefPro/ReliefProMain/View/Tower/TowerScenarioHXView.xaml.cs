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
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProModel;

namespace ReliefProMain.View
{
    /// <summary>
    /// TowerHX.xaml 的交互逻辑
    /// </summary>
    public partial class TowerScenarioHXView : Window
    {
        public string dbProtectedSystemFile;
        public string dbPlantFile;
        public List<TowerScenarioHX> Employees { get; set; }
        public List<string> ProcessSideFlowSources { get; set; }
        public int ScenarioID;
        public int HeaterType = 1;
        TowerScenarioHX sHX = new TowerScenarioHX();
        public TowerScenarioHXView()
        {
            InitializeComponent();            
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            //{
            //    dbTowerScenarioHX db = new dbTowerScenarioHX();
            //    var Session = helper.GetCurrentSession();
            //    IList<TowerScenarioHX> list = db.GetAllList(Session, ScenarioID,HeaterType);
            //    //myGrid.ItemsSource = list;
                

            //}
        }

        //private void myGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    FrameworkElement element_FlowStop = myGrid.Columns[4].GetCellContent(e.Row);
        //    if (element_FlowStop.GetType() == typeof(CheckBox))
        //    {
        //        bool value = ((CheckBox)element_FlowStop).IsChecked ?? false;
        //        this.sHX.DutyLost = value;
        //    }
        //}

        //private void myGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    if (sHX != null)
        //    {
        //        if (sHX.ID > 0)
        //        {
        //            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
        //            {
        //                var Session = helper.GetCurrentSession();
        //                dbTowerScenarioHX db = new dbTowerScenarioHX();
        //                TowerScenarioHX tsHX = db.GetModel(Session, sHX.ID);
        //                tsHX.DutyLost = sHX.DutyLost;
        //                Session.Update(tsHX);
        //                //db.Update(tsHX, Session);
                        
        //                //List<TowerScenarioStream> prodcuts=db.GetAllList(Session,ScenarioID,true).ToList();
        //            }
        //        }
        //    }
        //}

        //private void myGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    sHX = myGrid.SelectedItem as TowerScenarioHX;
        //}
        

    }
   
}
