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
using System.IO;
using NHibernate;
using ReliefProBLL;
using ReliefProCommon;
using ReliefProCommon.CommonLib;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProModel;
using ProII91;
using ImportLib;
namespace ReliefProMain.View
{
    /// <summary>
    /// PSV.xaml 的交互逻辑
    /// </summary>
    public partial class PSVView : Window
    {
        public string dbProtectedSystemFile;
        public string dbPlantFile;
        public string przFile;
        public string currentEqName;
        public ImportDB importdb;
        public int op = 0;
        PSV model = new PSV();
        public PSVView()
        {
            InitializeComponent();
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.ToString().Trim() == string.Empty)
            {
                MessageBox.Show("PSV Name could not be empty", "Message Box");
                return;
            }
            string pressure = txtPress.Text.ToString().Trim();
            if (pressure == string.Empty)
            {
                MessageBox.Show("Pressure  could not be empty", "Message Box");
                return;
            }
            else if (isNumber(pressure) == false)
            {
                MessageBox.Show("Pressure  must be a number", "Message Box");
                return;
            }

            string relief = txtPrelief.Text.ToString().Trim();
            if (relief == string.Empty)
            {
                MessageBox.Show("Prelief Pressure Factor could not be empty", "Message Box");
                return;
            }
            else if (isNumber(pressure) == false)
            {
                MessageBox.Show("Prelief Pressure Factor must be a number", "Message Box");
                return;
            }
            try
            {


                UnitConvert unitConvert = new UnitConvert();
                string tempdir = System.IO.Path.GetDirectoryName(dbProtectedSystemFile) + @"\temp\";
                if (!Directory.Exists(tempdir))
                    Directory.CreateDirectory(tempdir);

                string dirPhase = tempdir + "Phase";
                if (!Directory.Exists(dirPhase))
                    Directory.CreateDirectory(dirPhase);
                string dirLatent = tempdir + "Latent";
                if (!Directory.Exists(dirLatent))
                    Directory.CreateDirectory(dirLatent);


                importdb = new ImportDB(dbProtectedSystemFile);
                ProIIReader picker = new ProIIReader();
                picker.InitProIIPicker(przFile);
                DataTable dtStream = importdb.GetDataByTableName("tbStream", "1=0");
                picker.CopyStream(currentEqName, 1, 2, 1, ref dtStream);
                picker.ReleaseProIIPicker();

                string strTray1Pressure = dtStream.Rows[0]["pressure"].ToString();
                strTray1Pressure = unitConvert.Convert("KPA", "MPAG", double.Parse(strTray1Pressure)).ToString();
                string strTray1Temperature = dtStream.Rows[0]["Temperature"].ToString();
                strTray1Temperature = unitConvert.Convert("K", "C", double.Parse(strTray1Temperature)).ToString();
                string tray1_s = dtStream.Rows[0]["streamname"].ToString().ToUpper();
                string gd = Guid.NewGuid().ToString();
                string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();

                PRIIFileOperator.DecompressProIIFile(przFile, tempdir);
                string content = PRIIFileOperator.getUsableContent(dtStream.Rows[0]["streamname"].ToString(), tempdir);

                
                double ReliefPressure = double.Parse(this.txtPrelief.Text) * double.Parse(this.txtPress.Text);
                CustomStream stream = new CustomStream();
                stream.Temperature = strTray1Temperature;
                stream.Pressure = strTray1Pressure;
                stream.CompIn = dtStream.Rows[0]["CompIn"].ToString();
                stream.Componentid = dtStream.Rows[0]["Componentid"].ToString();
                stream.StreamName = tray1_s;
                stream.TotalComposition = dtStream.Rows[0]["TotalComposition"].ToString();
                stream.TotalMolarRate = dtStream.Rows[0]["TotalMolarRate"].ToString();

                PHASECalculation PhaseCalc = new PHASECalculation();
                string PH="PH"+Guid.NewGuid().ToString().Substring(0,4);
                string phasef = PhaseCalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH,dirPhase);
               
                ProIIReader picker1 = new ProIIReader();
                picker1.InitProIIPicker(phasef);
                string criticalPress = picker1.GetCriticalPressure(PH);               
                picker1.ReleaseProIIPicker();
                criticalPress = UnitConverter.unitConv(criticalPress, "KPA", "MPAG", "{0:0.0000}");
                


                FlashCalculation fcalc = new FlashCalculation();
                string tray1_f = fcalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, vapor, liquid, dirLatent);

                ImportDB importdb2 = new ImportDB(dbPlantFile);
                //DataTable dtCritical = importdb2.GetDataByTableName("tbCritical", "1=0");
                //DataRow drCritical = dtCritical.NewRow();
                //drCritical["CriticalPressure"] = criticalPress;
                //dtCritical.Rows.Add(drCritical);
               // importdb2.SaveDataByTableName(dtCritical);

                DataTable dtEqType = importdb2.GetDataByTableName("tbproiieqtype", "");
                DataTable dtEqList = importdb2.GetDataByTableName("tbproiieqdata", "1=0");
                DataTable dtStream2 = importdb2.GetDataByTableName("tbproiistreamdata", "1=0");
                ProIIReader picker2 = new ProIIReader();
                picker2.InitProIIPicker(tray1_f);

                picker2.getDataFromFile(ref dtEqType, ref dtEqList, ref dtStream2);
                while (dtStream.Rows[0]["temperature"].ToString() == "")
                {
                    picker2.getDataFromFile(ref dtEqType, ref dtEqList, ref dtStream2);
                }
                picker2.ReleaseProIIPicker();


                //string[] feedH = importdb2.computeH(dtStream2, tray1_s.ToUpper());
                string[] vaporH = importdb2.computeH(dtStream2, vapor);
                string[] liqidH = importdb2.computeH(dtStream2, liquid);

                DataTable dtLatentProduct = importdb.GetDataByTableName("tbLatentProduct", "1=0");

                foreach (DataRow dr in dtStream2.Rows)
                {
                    string name = dr["streamname"].ToString();
                    DataRow drLatentProduct = dtLatentProduct.NewRow();
                    foreach (DataColumn dc in dtStream2.Columns)
                    {
                        if (dc.ColumnName != "SourceFile")
                        {
                            drLatentProduct[dc.ColumnName] = dr[dc];
                        }
                    }

                    dtLatentProduct.Rows.Add(drLatentProduct);
                }
                importdb.SaveDataByTableName(dtLatentProduct);
                double latentH = double.Parse(vaporH[1]) - double.Parse(liqidH[1]);
                double ReliefTemperature = double.Parse(vaporH[3]);


                DataTable dtStreamOut = importdb.GetDataByTableName("tbstream", "isproduct=true");
                DataTable dtFlashResult = importdb.GetDataByTableName("flashresult", "1=0");
                int count = dtStreamOut.Rows.Count;
                for (int i = 1; i <= count; i++)
                {
                    DataRow dr = dtStreamOut.Rows[i - 1];
                    if (dr["TotalMolarRate"].ToString() != "0")
                    {
                        FlashCalculation fc = new FlashCalculation();
                        string l = string.Empty;
                        string v = string.Empty;
                        string prodtype = dr["prodtype"].ToString();
                        string tray = dr["tray"].ToString();
                        string streamname = dr["streamname"].ToString();
                        string strPressure = dr["pressure"].ToString();
                        string strTemperature = dr["Temperature"].ToString();
                        string f = string.Empty;

                        CustomStream sm = new CustomStream();
                        sm.Temperature = strTemperature;
                        sm.Pressure = strPressure;
                        sm.CompIn = dr["CompIn"].ToString();
                        sm.Componentid = dr["Componentid"].ToString();
                        sm.StreamName = dr["streamname"].ToString();
                        sm.TotalComposition = dr["TotalComposition"].ToString();
                        sm.TotalMolarRate = dr["TotalMolarRate"].ToString();

                        string dirflash = tempdir + sm.StreamName;
                        if (!Directory.Exists(dirflash))
                            Directory.CreateDirectory(dirflash);
                        double prodpressure = 0;
                        if (strPressure != "")
                        {
                            prodpressure = double.Parse(strPressure);
                        }
                        string usablecontent = PRIIFileOperator.getUsableContent(dr["streamname"].ToString(), tempdir);

                        if (prodtype == "4" || (prodtype == "2" && tray == "1")) // 2个条件是等同含义，后者是有气有液
                        {
                            f = fc.Calculate(usablecontent, 1, ReliefPressure.ToString(), 4, "", sm, vapor, liquid, dirflash);
                        }

                        else if (prodtype == "6" || prodtype == "3") //3 气相  6 沉积水 
                        {
                            f = fc.Calculate(usablecontent, 2, ReliefTemperature.ToString(), 4, "", sm, vapor, liquid, dirflash);
                        }
                        else
                        {
                            double p = ReliefPressure + (double.Parse(dr["pressure"].ToString()) - double.Parse(strTray1Pressure));
                            f = fc.Calculate(usablecontent, 1, p.ToString(), 3, "", sm, vapor, liquid, dirflash);
                        }
                        DataRow drFlash = dtFlashResult.NewRow();
                        drFlash["przfile"] = f;
                        drFlash["liquid"] = liquid;
                        drFlash["vapor"] = vapor;
                        drFlash["stream"] = streamname;
                        drFlash["prodtype"] = prodtype;
                        drFlash["tray"] = tray;

                        dtFlashResult.Rows.Add(drFlash);



                    }
                }

                DataTable dtfrmcase_product_single = importdb.GetDataByTableName("tbTowerFlashProduct", "1=0");
                count = dtFlashResult.Rows.Count;
                for (int i = 1; i <= count; i++)
                {
                    DataRow dr = dtFlashResult.Rows[i - 1];
                    string prodtype = dr["prodtype"].ToString();
                    string tray = dr["tray"].ToString();
                    if (dr["przfile"].ToString() != "")
                    {
                        dtEqList.Rows.Clear();
                        dtStream2.Rows.Clear();
                        ProIIReader picker3 = new ProIIReader();
                        picker3.InitProIIPicker(dr["przfile"].ToString());
                        picker3.getDataFromFile(ref dtEqType, ref dtEqList, ref dtStream2);
                        picker3.ReleaseProIIPicker();
                        DataRow drout = dtfrmcase_product_single.NewRow();


                        if (prodtype == "4" || (prodtype == "2" && tray == "1") || prodtype == "3" || prodtype == "6")
                        {
                            string[] liquidH2 = importdb.computeH(dtStream2, dr["vapor"].ToString().ToUpper());
                            drout["SpEnthalpy"] = liquidH2[1];
                        }
                        else
                        {
                            string[] liquidH2 = importdb.computeH(dtStream2, dr["liquid"].ToString().ToUpper());
                            drout["SpEnthalpy"] = liquidH2[1];
                        }


                        drout["StreamName"] = dr["stream"].ToString().ToUpper();
                        drout["weightflow"] = 0;
                        foreach (DataRow dro in dtStreamOut.Rows)
                        {
                            if (drout["streamname"].ToString() == dro["streamname"].ToString())
                            {
                                drout["weightflow"] = dro["weightflow"].ToString();
                                drout["ProdType"] = dro["ProdType"].ToString();
                            }
                        }
                        drout["tray"] = tray;

                        dtfrmcase_product_single.Rows.Add(drout);


                    }
                }
                importdb.SaveDataByTableName(dtfrmcase_product_single);

                using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                {
                    var Session = helper.GetCurrentSession();
                    dbPSV db = new dbPSV();
                    dbLatent dblatent = new dbLatent();
                    dbLatentProduct dblatentproduct = new dbLatentProduct();
                    if (op == 0)
                    {
                        model.PSVName = txtName.Text.Trim();
                        model.ReliefPressureFactor = this.txtPrelief.Text;
                        model.Pressure = txtPress.Text;
                        db.Add(model, Session);

                        Latent latent = new Latent();
                        latent.LatentEnthalpy = latentH.ToString();
                        latent.ReliefTemperature = ReliefTemperature.ToString();
                        latent.ReliefOHWeightFlow = liqidH[2];
                        latent.ReliefPressure = (double.Parse(model.Pressure) * double.Parse(model.ReliefPressureFactor)).ToString();
                        dblatent.Add(latent, Session);
                    }
                    else
                    {

                        PSV psv = db.GetModel(Session);
                        if (psv != null)
                        {
                            op = 1;
                            model = psv;
                            model.PSVName = txtName.Text;
                            model.ReliefPressureFactor = txtPrelief.Text;
                            model.Pressure = txtPress.Text;
                        }
                        db.Update(model, Session);
                        Session.Flush();

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

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbPSV db = new dbPSV();
                IList<PSV> list = db.GetAllList(Session);
                if (list.Count > 0)
                {
                    op = 1;
                    model = list[0];

                    this.txtName.Text = model.PSVName;
                    txtPrelief.Text = model.ReliefPressureFactor;
                    txtPress.Text = model.Pressure;

                }
                dbTower dbt = new dbTower();
                IList<Tower> listTower = dbt.GetAllList(Session);
                if (listTower.Count > 0)
                {
                    string dir = System.IO.Path.GetDirectoryName(dbPlantFile);
                    przFile = dir + @"\" + listTower[0].PrzFile;
                    currentEqName = listTower[0].TowerName;
                }
                else
                {

                }

            }

        }

        public bool isNumber(string strNumber)
        {
            try
            {
                decimal d = decimal.Parse(strNumber);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
