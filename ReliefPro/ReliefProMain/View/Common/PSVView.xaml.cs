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
using ProII;
using UOMLib;

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
        private int op = 0;
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
                using (var helper = new NHibernateHelper(dbProtectedSystemFile))
                {
                    var Session = helper.GetCurrentSession();
                    dbPSV db = new dbPSV();
                    PSV psv = db.GetModel(Session);
                    if (psv == null)
                    {

                        model.PSVName = txtName.Text.Trim();
                        model.ReliefPressureFactor = this.txtPrelief.Text;
                        model.Pressure = txtPress.Text;
                        model.DrumPSVName = txtDrumPSVName.Text;
                        model.DrumPressure = txtDrumPressure.Text;
                        db.Add(model, Session);
                    }
                    else
                    {
                        model = psv;
                        model.PSVName = txtName.Text;
                        model.ReliefPressureFactor = txtPrelief.Text;
                        model.Pressure = txtPress.Text;
                        model.DrumPSVName = txtDrumPSVName.Text;
                        model.DrumPressure = txtDrumPressure.Text;
                        db.Update(model, Session);
                        Session.Flush();
                    }


                    dbTower dbtower = new dbTower();
                    IList<Tower> list = dbtower.GetAllList(Session);
                    if (list.Count > 0)
                    {
                        CreateTowerPSV();
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
                dbCritical dbcritical = new dbCritical();
                IList<PSV> list = db.GetAllList(Session);
                if (list.Count > 0)
                {
                    op = 1;
                    model = list[0];

                    this.txtName.Text = model.PSVName;
                    txtPrelief.Text = model.ReliefPressureFactor;
                    txtPress.Text = model.Pressure;

                    Critical c = dbcritical.GetModel(Session);
                    //txtCritical.Text = c.CriticalPressure;

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

        public void CreateTowerPSV()
        {
            //判断压力是否更改，relief pressure 是否更改。  （drum的是否修改，会影响到火灾的计算）

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

            string rootDir = System.IO.Path.GetDirectoryName(dbPlantFile);

            
            string version = ProIIFactory.GetProIIVerison(przFile, rootDir);
            IProIIReader reader = ProIIFactory.CreateReader(version);
            reader.InitProIIReader(przFile);
            ProIIStreamData proIITray1StreamData =reader.CopyStream(currentEqName, 1, 2, 1);
            reader.ReleaseProIIReader();
            CustomStream stream = ProIIToDefault.ConvertProIIStreamToCustomStream(proIITray1StreamData);
           

            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();

            PRIIFileOperator.DecompressProIIFile(przFile, tempdir);
            string content = PRIIFileOperator.getUsableContent(stream.StreamName, tempdir);


            double ReliefPressure = double.Parse(this.txtPrelief.Text) * double.Parse(this.txtPress.Text);
            
           

            //IPHASECalculate PhaseCalc = ProIIFactory.CreatePHASECalculate(version);
            //string PH = "PH" + Guid.NewGuid().ToString().Substring(0, 4);
            // string phasef = PhaseCalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH, dirPhase);

            // reader = ProIIFactory.CreateReader(version);
            // reader.InitProIIReader(phasef);
            //  string criticalPress = reader.GetCriticalPressure(PH);
            //   reader.ReleaseProIIReader();
            //  criticalPress = unitConvert.Convert("KPA", "MPAG", double.Parse(criticalPress)).ToString();



            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(version);
            string tray1_f = fcalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, vapor, liquid, dirLatent);


            reader = ProIIFactory.CreateReader(version);
            reader.InitProIIReader(tray1_f);
            ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
            ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
            reader.ReleaseProIIReader();

            CustomStream latentVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
            CustomStream latentLiquid = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);

            double latentEnthalpy = double.Parse(latentVapor.SpEnthalpy) - double.Parse(latentLiquid.SpEnthalpy);
            double ReliefTemperature = double.Parse(latentVapor.Temperature);

            IList<CustomStream> products = null;
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbCustomStream dbstream = new dbCustomStream();
                products = dbstream.GetAllList(Session, true);
            }

            List<FlashResult> listFlashResult = new List<FlashResult>();
            int count = products.Count;
            for (int i = 1; i <= count; i++)
            {
                CustomStream p = products[i - 1];
                if (p.TotalMolarRate != "0")
                {
                    IFlashCalculate fc = ProIIFactory.CreateFlashCalculate(version);
                    string l = string.Empty;
                    string v = string.Empty;
                    string prodtype = p.ProdType;
                    string tray = p.ProdType;
                    string streamname = p.StreamName;
                    string strPressure = p.Pressure;
                    string strTemperature = p.Temperature;
                    string f = string.Empty;


                    string dirflash = tempdir + p.StreamName;
                    if (!Directory.Exists(dirflash))
                        Directory.CreateDirectory(dirflash);
                    double prodpressure = 0;
                    if (strPressure != "")
                    {
                        prodpressure = double.Parse(strPressure);
                    }
                    string usablecontent = PRIIFileOperator.getUsableContent(p.StreamName, tempdir);

                    if (prodtype == "4" || (prodtype == "2" && tray == "1")) // 2个条件是等同含义，后者是有气有液
                    {
                        f = fc.Calculate(usablecontent, 1, ReliefPressure.ToString(), 4, "", p, vapor, liquid, dirflash);
                    }

                    else if (prodtype == "6" || prodtype == "3") //3 气相  6 沉积水 
                    {
                        f = fc.Calculate(usablecontent, 2, ReliefTemperature.ToString(), 4, "", p, vapor, liquid, dirflash);
                    }
                    else
                    {
                        double press = ReliefPressure + (double.Parse(p.Pressure) - double.Parse(stream.Pressure));
                        f = fc.Calculate(usablecontent, 1, press.ToString(), 3, "", p, vapor, liquid, dirflash);
                    }
                    FlashResult fr = new FlashResult();
                    fr.LiquidName = liquid;
                    fr.VaporName = vapor;
                    fr.StreamName = streamname;
                    fr.PrzFile = f;
                    fr.Tray = tray;
                    fr.ProdType = prodtype;
                    listFlashResult.Add(fr);
                }
            }

            List<TowerFlashProduct> listFlashProduct = new List<TowerFlashProduct>();
            count = listFlashResult.Count;
            for (int i = 1; i <= count; i++)
            {
                FlashResult fr = listFlashResult[i - 1];
                string prodtype = fr.ProdType;
                string tray = fr.Tray;
                if (fr.PrzFile != "")
                {
                    CustomStream cs = null;
                    reader = ProIIFactory.CreateReader(version);
                    reader.InitProIIReader(fr.PrzFile);
                    TowerFlashProduct product = new TowerFlashProduct();
                    if (prodtype == "4" || (prodtype == "2" && tray == "1") || prodtype == "3" || prodtype == "6")
                    {
                        ProIIStreamData data = reader.GetSteamInfo(fr.VaporName);
                        cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
                    }
                    else
                    {
                        ProIIStreamData data = reader.GetSteamInfo(fr.LiquidName);
                        cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);

                    }
                    reader.ReleaseProIIReader();
                    product.SpEnthalpy = cs.SpEnthalpy;
                    product.StreamName = fr.StreamName;
                    product.WeightFlow = cs.WeightFlow;
                    product.ProdType = fr.ProdType;
                    product.Tray = tray;
                    listFlashProduct.Add(product);
                }
            }


            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();

                dbLatent dblatent = new dbLatent();
                //dbLatentProduct dblatentproduct = new dbLatentProduct();
                dbTowerFlashProduct dbFlashProduct = new dbTowerFlashProduct();

                if (op == 0)
                {

                    // Critical c = new Critical();
                    // c.CriticalPressure = criticalPress;
                    // dbCritical dbcritical = new dbCritical();
                    // dbcritical.Add(c, Session);



                    Latent latent = new Latent();
                    latent.LatentEnthalpy = latentEnthalpy.ToString();
                    latent.ReliefTemperature = ReliefTemperature.ToString();
                    latent.ReliefOHWeightFlow = latentVapor.BulkMwOfPhase;
                    latent.ReliefPressure = (double.Parse(model.Pressure) * double.Parse(model.ReliefPressureFactor)).ToString();
                    dblatent.Add(latent, Session);

                    foreach (TowerFlashProduct p in listFlashProduct)
                    {
                        dbFlashProduct.Add(p, Session);
                    }
                }
                else
                {
                    //直接删除再新增



                }
            }
        }

    }
}
