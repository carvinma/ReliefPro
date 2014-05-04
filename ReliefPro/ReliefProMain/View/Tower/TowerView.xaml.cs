using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using ImportLib;

namespace ReliefProMain.View
{
    /// <summary>
    /// Tower.xaml 的交互逻辑
    /// </summary>
    public partial class TowerView : Window
    {
        public TowerView()
        {
            InitializeComponent();
        }
        public string dbPlantFile;
        public string dbProtectSystemFile;
        public string eqType;
        public string towerName;
        public string przFile;
        public string vsdFile;
        public int op=0;
        public DataTable dtFeed=new DataTable();
        public DataTable dtProd=new DataTable();
        public DataTable dtCondenser = new DataTable();
        public DataTable dtHxCondenser = new DataTable();
        public DataTable dtReboiler = new DataTable();
        public DataTable dtHxReboiler = new DataTable();
        public DataTable dtTower = new DataTable();
        public DataSet dsTower=new DataSet();
        public DataTable dtSource = new DataTable();
        public DataTable dtSink = new DataTable();
        public DataTable dtAccumulator = new DataTable();
        public DataTable dtSideColumn = new DataTable();

        public DataTable dtFeed_init = new DataTable();
        public DataTable dtProd_init = new DataTable();
        public DataTable dtCondenser_init = new DataTable();
        public DataTable dtHxCondenser_init = new DataTable();
        public DataTable dtReboiler_init = new DataTable();
        public DataTable dtHxReboiler_init = new DataTable();
        public DataTable dtAccumulator_init = new DataTable();
        public DataTable dtSideColumn_init = new DataTable();
        public bool IsImportedData = false;
        private int loadstatus = 0;
        ImportDB dbPlant;
        ImportDB dbProtectedSystem;

        UnitConvert unitConvert = new UnitConvert();
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            SelectEquipmentView frm = new SelectEquipmentView();
            frm.Owner = this;            
            frm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            frm.dbPlantFile = dbPlantFile;
            frm.eqType = eqType;
            if (frm.ShowDialog() == true)
            {
                op = 0;
                dtFeed.Rows.Clear();
                dtProd.Rows.Clear();
                dtSource.Rows.Clear();
                dtSink.Rows.Clear();
                dtCondenser.Rows.Clear();
                dtHxCondenser.Rows.Clear();
                dtReboiler.Clear();
                dtHxReboiler.Rows.Clear();
                dtTower.Rows.Clear();
                przFile = frm.przFile;
                dsTower = (DataSet)Application.Current.Properties["eqInfo"];
                Application.Current.Properties.Remove("eqInfo");
                bindTowerInfo();
                bindCombox();
                txtName.BorderBrush = Brushes.Green;
                txtName.BorderThickness = new Thickness(2, 2, 2, 2);
                txtStageNumber.BorderBrush = Brushes.Green;
                txtStageNumber.BorderThickness = new Thickness(2, 2, 2, 2);  
            }
        }
        private void bindCombox()
        {
            lvFeed.ItemsSource = dtFeed.DefaultView;
            lvProd.ItemsSource = dtProd.DefaultView;
            lvCondenser.ItemsSource = dtCondenser.DefaultView;
            lvHxCondenser.ItemsSource = dtHxCondenser.DefaultView;
            lvReboiler.ItemsSource = dtReboiler.DefaultView;
            lvHxReboiler.ItemsSource = dtHxReboiler.DefaultView;
        }
        private void bindTowerInfo()
        {
            if (dsTower.Tables.Count > 0 && dsTower.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dsTower.Tables[0].Rows[0];
                txtName.Text = dr["eqname"].ToString();
                //txtDescription.Text = dr["description"].ToString();
                txtStageNumber.Text = dr["numberoftrays"].ToString();
                Dictionary<string, string> dicFeeds = new Dictionary<string, string>();
                Dictionary<string, string> dicProducts = new Dictionary<string, string>();
                this.dbPlant.GetMaincolumnRealFeedProduct(dr["eqname"].ToString(), ref dicFeeds, ref dicProducts);

                foreach (KeyValuePair<string, string> feed in dicFeeds)
                {
                    DataRow r = dtFeed.NewRow();
                    r["streamname"] = feed.Key;
                    r["tray"] = feed.Value;
                    r["isproduct"] = false;
                    dbPlant.GetAndConvertStreamInfo(feed.Key, ref r);
                    dtFeed.Rows.Add(r);

                    DataRow rsource = dtSource.NewRow();
                    rsource["streamname"] = feed.Key;
                    rsource["sourcename"] = feed.Key + "_Source";
                    
                    rsource["ismaintained"] = false;
                    rsource["sourcetype"] = "Pump（Motor）";
                    rsource["maxpossiblepressure"] = r["pressure"].ToString();
                    rsource["maxpossiblepressure_color"] = "green";
                    dtSource.Rows.Add(rsource);

                }

                foreach (KeyValuePair<string, string> prodcut in dicProducts)
                {
                    DataRow r = dtProd.NewRow();
                    r["streamname"] = prodcut.Key;
                    r["tray"] = prodcut.Value;
                    r["isproduct"] = true;
                    dbPlant.GetAndConvertStreamInfo(prodcut.Key, ref r);
                    dtProd.Rows.Add(r);

                    DataRow rsink = dtSink.NewRow();
                    rsink["streamname"] = prodcut.Key;
                    rsink["sinkname"] = prodcut.Key + "_Sink";
                   
                    rsink["ismaintained"] = false;
                    rsink["sinktype"] = "Pump（Motor）";
                    rsink["maxpossiblepressure"] = r["pressure"].ToString();
                    rsink["maxpossiblepressure_color"] = "green";

                    dtSink.Rows.Add(rsink);

                }

                string heaterNames = dr["HeaterNames"].ToString();
                string heaterDuties = dr["HeaterDuties"].ToString();
                string heaterTrayLoc = dr["HeaterTrayLoc"].ToString();
                string[] arrHeaterNames = heaterNames.Split(',');
                string[] arrHeaterDuties = heaterDuties.Split(',');
                string[] arrHeaterTrayLoc = heaterTrayLoc.Split(',');
                for (int i = 0; i < arrHeaterNames.Length; i++)
                {
                    decimal duty = decimal.Parse(arrHeaterDuties[i])*3600 ;  //KJ/hr
                    duty = decimal.Round(duty, 4);
                    if (arrHeaterNames[i] == "CONDENSER")
                    {
                        DataRow r = dtCondenser.NewRow();
                        r["heatername"] = arrHeaterNames[i];
                        r["heaterduty"] = duty;
                       
                        r["heatertype"] = 1;
                        
                        dtCondenser.Rows.Add(r);

                    }
                    else if (arrHeaterNames[i] == "REBOILER")
                    {

                        DataRow r = dtReboiler.NewRow();
                        r["heatername"] = arrHeaterNames[i];
                        r["heaterduty"] = duty;

                        r["heatertype"] = 3;
                        //r["iscontinued"] = false;


                        dtReboiler.Rows.Add(r);

                    }
                    else if (double.Parse(arrHeaterDuties[i]) <= 0 && arrHeaterNames[i] != "CONDENSER")
                    {
                        if (arrHeaterTrayLoc[i] == "1")
                        {
                            DataRow r = dtCondenser.NewRow();
                            r["heatername"] = arrHeaterNames[i];
                            r["heaterduty"] = duty;

                            r["heatertype"] = 1;
                            dtCondenser.Rows.Add(r);
                        }
                        else
                        {
                            DataRow r = dtHxCondenser.NewRow();
                            r["heatername"] = arrHeaterNames[i];
                            r["heaterduty"] = duty;
                            r["heatertype"] = 2;
                            dtHxCondenser.Rows.Add(r);
                        }
                    }
                    else if (double.Parse(arrHeaterDuties[i]) > 0 && arrHeaterNames[i] != "REBOILER")
                    {
                        if (arrHeaterTrayLoc[i] == dr["numberoftrays"].ToString())
                        {
                            DataRow r = dtReboiler.NewRow();
                            r["heatername"] = arrHeaterNames[i];
                            r["heaterduty"] = duty;

                            r["heatertype"] = 3;
                            dtReboiler.Rows.Add(r);
                        }
                        else
                        {
                            DataRow r = dtHxReboiler.NewRow();
                            r["heatername"] = arrHeaterNames[i];
                            r["heaterduty"] = duty;

                            r["heatertype"] = 4;



                            dtHxReboiler.Rows.Add(r);
                        }
                    }
                }


            }
            dtFeed_init = dtFeed.Copy();
            dtProd_init = dtProd.Copy();
            dtCondenser_init = dtCondenser.Copy();
            dtHxCondenser_init = dtHxCondenser.Copy();
            dtReboiler_init = dtReboiler.Copy();
            dtHxReboiler_init = dtHxReboiler.Copy();
        }
        private void btnDeleteFeed_Click(object sender, RoutedEventArgs e)
        {
            int idx = lvFeed.SelectedIndex;
            if ( idx> -1)
            {
                DataView dv = (DataView)lvFeed.ItemsSource;
                DataTable dt = dv.Table;
                dt.Rows.RemoveAt(idx);
                lvFeed.ItemsSource = dt.DefaultView;
                
            }
        }

        

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            dbPlant = new ImportDB(dbPlantFile);
            dbProtectedSystem = new ImportDB(dbProtectSystemFile);
            InitDataTable();
            getTowerInfo();
            bindTowerInfo();
            loadstatus = 1;
        }
        public void getTowerInfo()
        {
            try
            {
                dtTower = this.dbProtectedSystem.GetDataByTableName("tbtower", "");
                if (dtTower.Rows.Count != 0)
                {
                    IsImportedData = true;
                    op = 1;
                    DataRow dr = dtTower.Rows[0];
                    txtName.Text = dr["towername"].ToString();
                    this.txtStageNumber.Text = dr["StageNumber"].ToString();
                    this.txtDescription.Text = dr["Description"].ToString();

                    string towercolor = dr["towername_color"].ToString();
                    if (towercolor == "blue")
                    {
                        txtName.BorderBrush = Brushes.Blue;
                        txtName.BorderThickness = new Thickness(2, 2, 2, 2);
                    }
                    else
                    {
                        txtName.BorderBrush = Brushes.Green;
                        txtName.BorderThickness = new Thickness(2, 2, 2, 2);
                    }

                    if (dr["StageNumber_color"].ToString() == "blue")
                    {
                        txtStageNumber.BorderBrush = Brushes.Blue;
                        txtStageNumber.BorderThickness = new Thickness(2, 2, 2, 2);
                    }
                    else
                    {
                        txtStageNumber.BorderBrush = Brushes.Green;
                        txtStageNumber.BorderThickness = new Thickness(2, 2, 2, 2);
                    }


                }
                DataTable dt = new DataTable();
                dt = this.dbProtectedSystem.GetDataByTableName("tbstream", "isproduct=false");
                if (dt.Rows.Count != 0)
                {
                    dtFeed = dt;
                }
                dt = dbProtectedSystem.GetDataByTableName("tbstream", " isproduct=true");
                if (dt.Rows.Count != 0)
                {
                    dtProd = dt;
                }
                dt = dbProtectedSystem.GetDataByTableName("tbtowerhx", " heatertype=1");
                if (dt.Rows.Count != 0)
                {
                    this.dtCondenser = dt;
                }
                dt = dbProtectedSystem.GetDataByTableName("tbtowerhx", " heatertype=2");
                if (dt.Rows.Count != 0)
                {
                    this.dtHxCondenser = dt;
                }
                dt = dbProtectedSystem.GetDataByTableName("tbtowerhx", " heatertype=3");
                if (dt.Rows.Count != 0)
                {
                    this.dtReboiler = dt;
                }
                dt = dbProtectedSystem.GetDataByTableName("tbtowerhx", " heatertype=4");
                if (dt.Rows.Count != 0)
                {
                    this.dtHxReboiler = dt;
                }

                bindCombox();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        public void InitDataTable()
        {
            dtFeed = dbProtectedSystem.GetDataByTableName("tbstream", "1=0");
            dtProd = dtFeed.Clone();
            dtCondenser = dbProtectedSystem.GetDataByTableName("tbtowerhx", "1=0");
            dtReboiler = dtCondenser.Clone();
            dtHxReboiler = dtReboiler.Clone();
            dtHxCondenser = dtCondenser.Clone();

            dtSource = dbProtectedSystem.GetDataByTableName("tbsource", "1=0");
            dtSink = dbProtectedSystem.GetDataByTableName("tbsink", "1=0");
            dtAccumulator = dbProtectedSystem.GetDataByTableName("tbAccumulator", "1=0");
            dtSideColumn = dbProtectedSystem.GetDataByTableName("tbSideColumn", "1=0");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtFeed.Rows.Count == 0)
                {
                    MessageBox.Show("Please import data from database.");
                    return;
                }
                DataRow dr = dtTower.NewRow();
                if (op == 0)
                {
                    Application.Current.Properties.Add("FeedData", dtFeed);
                    Application.Current.Properties.Add("Condenser", dtCondenser);
                    Application.Current.Properties.Add("HxCondenser", dtHxCondenser);
                    Application.Current.Properties.Add("Reboiler", dtReboiler);
                    Application.Current.Properties.Add("HxReboiler", dtHxReboiler);
                    Application.Current.Properties.Add("ProdData", dtProd);


                    dtTower.Rows.Add(dr);
                    dr["towername"] = txtName.Text;
                    dr["stagenumber"] = this.txtStageNumber.Text;
                    dr["description"] = this.txtDescription.Text;

                    if (!string.IsNullOrEmpty(przFile))
                    {
                        dr["przfile"] = przFile;
                    }
                    DataTable dtCondenserTemp = dtCondenser.Clone();
                    dtCondenserTemp.Merge(dtCondenser);
                    dtCondenserTemp.Merge(dtHxCondenser);

                    dtCondenserTemp.Merge(dtReboiler);
                    dtCondenserTemp.Merge(dtHxReboiler);

                    DataTable dtFeedTemp = dtFeed.Clone();
                    dtFeedTemp.Merge(dtFeed);
                    dtFeedTemp.Merge(dtProd);


                    this.dbProtectedSystem.SaveDataByTableName(dtFeedTemp);

                    DataRow drAccumulator = dtAccumulator.NewRow();
                    drAccumulator["Orientation"] = false;
                    dtAccumulator.Rows.Add(drAccumulator);

                    List<string> listSideColumn = dbPlant.GetAllSideColumn();

                    foreach (string sc in listSideColumn)
                    {
                        DataRow drSideColumn = dtSideColumn.NewRow();
                        drSideColumn["EqName"] = sc;
                        dtSideColumn.Rows.Add(drSideColumn);
                    }

                    dbProtectedSystem.SaveDataByTableName(dtSource);
                    dbProtectedSystem.SaveDataByTableName(dtSink);
                    dbProtectedSystem.SaveDataByTableName(dtCondenserTemp);                   
                    dbProtectedSystem.SaveDataByTableName(dtTower);
                    dbProtectedSystem.SaveDataByTableName(dtAccumulator);
                    dbProtectedSystem.SaveDataByTableName(dtSideColumn);
                }
                else
                {
                    Application.Current.Properties.Add("FeedData", dtFeed);
                    Application.Current.Properties.Add("Condenser", dtCondenser);
                    Application.Current.Properties.Add("HxCondenser", dtHxCondenser);
                    Application.Current.Properties.Add("Reboiler", dtReboiler);
                    Application.Current.Properties.Add("HxReboiler", dtHxReboiler);
                    Application.Current.Properties.Add("ProdData", dtProd);

                    dr = dtTower.Rows[0];
                    dr["towername"] = txtName.Text;
                    dr["stagenumber"] = this.txtStageNumber.Text;
                    dr["description"] = this.txtDescription.Text;

                    if (!string.IsNullOrEmpty(przFile))
                    {
                        dr["przfile"] = przFile;
                    }
                    if (txtName.BorderBrush == Brushes.Green)
                    {
                        dr["towername_color"] = "green";
                    }
                    else
                    {
                        dr["towername_color"] = "blue";
                    }
                    if (txtStageNumber.BorderBrush == Brushes.Green)
                    {
                        dr["stagenumber_color"] = "green";
                    }
                    else
                    {
                        dr["stagenumber_color"] = "blue";
                    }

                }
            }
            catch (Exception ex)
            {
            }
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool compareInfo()
        {
            bool result = false;

            if (dtFeed.Rows.Count == dtFeed_init.Rows.Count)
            {
                foreach (DataRow dr in dtFeed.Rows)
                {
                    string name = dr["streamname"].ToString();
                    if (dtFeed_init.Select("streamname='" + name + "'").Length == 0)
                    {
                        result = true;
                        break;
                    }
                }
                if (result)
                    return true;
            }
            else
            {
                return true;
            }

            if (this.dtProd.Rows.Count == dtProd_init.Rows.Count)
            {
                foreach (DataRow dr in dtProd.Rows)
                {
                    string name = dr["streamname"].ToString();
                    if (dtProd_init.Select("streamname='" + name + "'").Length == 0)
                    {
                        result = true;
                        break;
                    }
                }
                if (result)
                    return true;
            }
            else
            {
                return true;
            }

            if (this.dtHxCondenser.Rows.Count == dtHxCondenser_init.Rows.Count)
            {
                foreach (DataRow dr in dtHxCondenser.Rows)
                {
                    string name = dr["hxcondensername"].ToString();
                    if (dtHxCondenser_init.Select("hxcondensername='" + name + "'").Length == 0)
                    {
                        result = true;
                        break;
                    }
                }
                if (result)
                    return true;
            }
            else
            {
                return true;
            }

            if (this.dtCondenser.Rows.Count == dtCondenser_init.Rows.Count)
            {
                foreach (DataRow dr in dtCondenser.Rows)
                {
                    string name = dr["condensername"].ToString();
                    if (dtCondenser_init.Select("condensername='" + name + "'").Length == 0)
                    {
                        result = true;
                        break;
                    }
                }
                if (result)
                    return true;
            }
            else
            {
                return true;
            }


            if (this.dtReboiler.Rows.Count == dtReboiler_init.Rows.Count)
            {
                foreach (DataRow dr in dtReboiler.Rows)
                {
                    string name = dr["reboilername"].ToString();
                    if (dtReboiler_init.Select("reboilername='" + name + "'").Length == 0)
                    {
                        result = true;
                        break;
                    }
                }
                if (result)
                    return true;
            }
            else
            {
                return true;
            }

            if (this.dtHxReboiler.Rows.Count == dtHxReboiler_init.Rows.Count)
            {
                foreach (DataRow dr in dtHxReboiler.Rows)
                {
                    string name = dr["hxreboilername"].ToString();
                    if (dtHxReboiler_init.Select("hxreboilername='" + name + "'").Length == 0)
                    {
                        result = true;
                        break;
                    }
                }
                if (result)
                    return true;
            }
            else
            {
                return true;
            }

            return result;
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loadstatus == 1)
            {
                txtName.BorderBrush = Brushes.Blue;
                txtName.BorderThickness = new Thickness(2,2,2,2);
            }
        }

        private void txtStageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loadstatus == 1)
            {
                txtStageNumber.BorderBrush = Brushes.Blue;
                txtStageNumber.BorderThickness = new Thickness(2,2,2,2);
            }
        }
    }
}
