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
    /// TowerScenarioFeed.xaml 的交互逻辑
    /// </summary>
    public partial class TowerScenarioFeedView : Window
    {
        public List<TowerScenarioStream> TowerTowerScenarioStreams { get; set; }
        public string dbProtectedSystemFile;
        public string dbPlantFile;
        public int ScenarioID = 0;
        public TowerScenarioStream stream = new TowerScenarioStream();
        public TowerScenarioFeedView()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                dbTowerScenarioStream db = new dbTowerScenarioStream();
                var Session=helper.GetCurrentSession();
                IList<TowerScenarioStream> list = db.GetAllList(Session, ScenarioID,false);               
                myGrid.ItemsSource = list;
                
            }
        }

        private void myGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            FrameworkElement element_FlowStop = myGrid.Columns[4].GetCellContent(e.Row);
            if (element_FlowStop.GetType() == typeof(CheckBox))
            {
                bool value = ((CheckBox)element_FlowStop).IsChecked??false;
                this.stream.FlowStop = value;
            }
        }

        private void myGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stream = myGrid.SelectedItem as TowerScenarioStream;
        }

        private void myGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (stream != null)
            {
                if (stream.ID > 0)
                {
                    using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                    {
                        var Session = helper.GetCurrentSession();
                        dbTowerScenarioStream db = new dbTowerScenarioStream();
                        TowerScenarioStream tsSteam = db.GetModel(Session, stream.ID);
                        tsSteam.FlowStop = stream.FlowStop;
                        db.Update(tsSteam, Session);

                        //List<TowerScenarioStream> prodcuts=db.GetAllList(Session,ScenarioID,true).ToList();



                    }

                }
            }
                
        }
    }
}
