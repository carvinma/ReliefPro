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
        //public string dbProtectedSystemFile;
        //public string dbPlantFile;
        //public string przFile;
        //public string currentEqName;
        //private int op = 0;
        //PSV model = new PSV();
        public PSVView()
        {
            InitializeComponent();
        }
        //private void btnOK_Click(object sender, RoutedEventArgs e)
        //{
        //    if (txtName.Text.ToString().Trim() == string.Empty)
        //    {
        //        MessageBox.Show("PSV Name could not be empty", "Message Box");
        //        return;
        //    }
        //    string pressure = txtPress.Text.ToString().Trim();
        //    if (pressure == string.Empty)
        //    {
        //        MessageBox.Show("Pressure  could not be empty", "Message Box");
        //        return;
        //    }
        //    else if (isNumber(pressure) == false)
        //    {
        //        MessageBox.Show("Pressure  must be a number", "Message Box");
        //        return;
        //    }

        //    string relief = txtPrelief.Text.ToString().Trim();
        //    if (relief == string.Empty)
        //    {
        //        MessageBox.Show("Prelief Pressure Factor could not be empty", "Message Box");
        //        return;
        //    }
        //    else if (isNumber(pressure) == false)
        //    {
        //        MessageBox.Show("Prelief Pressure Factor must be a number", "Message Box");
        //        return;
        //    }
        //    try
        //    {
        //        using (var helper = new NHibernateHelper(dbProtectedSystemFile))
        //        {
        //            var Session = helper.GetCurrentSession();
        //            PSVDAL db = new PSVDAL();
        //            PSV psv = db.GetModel(Session);
        //            if (psv == null)
        //            {

        //                model.PSVName = txtName.Text.Trim();
        //                model.ReliefPressureFactor = this.txtPrelief.Text;
        //                model.Pressure = txtPress.Text;
        //                model.DrumPSVName = txtDrumPSVName.Text;
        //                model.DrumPressure = txtDrumPressure.Text;
        //                db.Add(model, Session);
        //            }
        //            else
        //            {
        //                model = psv;
        //                model.PSVName = txtName.Text;
        //                model.ReliefPressureFactor = txtPrelief.Text;
        //                model.Pressure = txtPress.Text;
        //                model.DrumPSVName = txtDrumPSVName.Text;
        //                model.DrumPressure = txtDrumPressure.Text;
        //                db.Update(model, Session);
        //                Session.Flush();
        //            }


        //            TowerDAL dbtower = new TowerDAL();
        //            IList<Tower> list = dbtower.GetAllList(Session);
        //            if (list.Count > 0)
        //            {
        //                //CreateTowerPSV();
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    this.DialogResult = true;

        //}

        

        //private void Window_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    using (var helper = new NHibernateHelper(dbProtectedSystemFile))
        //    {
        //        var Session = helper.GetCurrentSession();
        //        PSVDAL db = new PSVDAL();
        //        CriticalDAL dbcritical = new CriticalDAL();
        //        IList<PSV> list = db.GetAllList(Session);
        //        if (list.Count > 0)
        //        {
        //            op = 1;
        //            model = list[0];

        //            this.txtName.Text = model.PSVName;
        //            txtPrelief.Text = model.ReliefPressureFactor;
        //            txtPress.Text = model.Pressure;

        //            Critical c = dbcritical.GetModel(Session);
        //            txtCritical.Text = c.CriticalPressure;

        //        }
        //        TowerDAL dbt = new TowerDAL();
        //        IList<Tower> listTower = dbt.GetAllList(Session);
        //        if (listTower.Count > 0)
        //        {
        //            string dir = System.IO.Path.GetDirectoryName(dbPlantFile);
        //            przFile = dir + @"\" + listTower[0].PrzFile;
        //            currentEqName = listTower[0].TowerName;
        //        }
        //        else
        //        {

        //        }

        //    }

        //}

        //public bool isNumber(string strNumber)
        //{
        //    try
        //    {
        //        decimal d = decimal.Parse(strNumber);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

       

    }
}
