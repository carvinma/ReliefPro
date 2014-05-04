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
using System.Data.OleDb;
using System.Data;
using ImportLib;

namespace ReliefProMain.View
{
    /// <summary>
    /// Stream.xaml 的交互逻辑
    /// </summary>
    public partial class CustomStreamView : Window
    {
        public CustomStreamView()
        {
            InitializeComponent();
        }

        public string dbProtectedSystemFile;
        private int op = 0;
        private DataTable dt = new DataTable();
        private DataRow dr;
        public string streamName = string.Empty;
        private int loadstatus = 0;
        ImportDB db;

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = this.txtName.Text;
                string temp = this.txtTemp.Text;
                string press = this.txtPres.Text;
                string wf = this.txtWf.Text;
                string enthalpy = this.txtH.Text;
                string spenthalpy = this.txtSph.Text;
                string vf = this.txtVabFrac.Text;

                dr["streamname"] = name;
                dr["Temperature"] = temp;
                dr["Pressure"] = press;
                dr["Enthalpy"] = enthalpy;
                dr["SpEnthalpy"] = spenthalpy;
                dr["VaporFraction"] = vf;
                dr["WeightFlow"] = wf;
                dr["Description"] = txtDescription.Text;
                
                getColor(ref dr, "streamname_color", txtName);
                getColor(ref dr, "Temperature_color", txtTemp);
                getColor(ref dr, "Pressure_color", txtPres);
                getColor(ref dr, "VaporFraction_color", txtVabFrac);
                getColor(ref dr, "WeightFlow_color", txtWf);
                getColor(ref dr, "Enthalpy_color", txtH);
                getColor(ref dr, "SpEnthalpy_color", txtSph);

                if (op == 0)
                    db.InsertDataByRow(dr);
                else
                    db.UpdateDataByRow(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Action");
            }
            this.DialogResult = true;
        }


        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                FocusManager.SetFocusedElement(grid1, txtName);
                db = new ImportDB(dbProtectedSystemFile);
                string Name = txtName.Text;
                dt = db.GetDataByTableName("tbstream", "streamname='" + Name + "'");
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        op = 1;
                        dr = dt.Rows[0];

                        txtName.Text = Name;
                        txtDescription.Text = dr["description"].ToString();
                        txtTemp.Text = dr["Temperature"].ToString();
                        txtPres.Text = dr["Pressure"].ToString();
                        txtVabFrac.Text = dr["VaporFraction"].ToString();
                        txtWf.Text = dr["WeightFlow"].ToString();
                        txtH.Text = dr["Enthalpy"].ToString();
                        txtSph.Text = dr["SpEnthalpy"].ToString();

                        setColor(dr, "streamname_color", txtName);
                        setColor(dr, "Temperature_color", txtTemp);
                        setColor(dr, "Pressure_color", txtPres);
                        setColor(dr, "VaporFraction_color", txtVabFrac);
                        setColor(dr, "WeightFlow_color", txtWf);
                        setColor(dr, "Enthalpy_color", txtH);
                        setColor(dr, "SpEnthalpy_color", txtSph);
                        loadstatus = 1;
                    }
                    else
                    {
                        op = 1;
                        dt = db.GetDataByTableName("frmproduct", "streamname='" + Name + "'");
                        dr = dt.Rows[0];

                        txtName.Text = Name;
                        txtDescription.Text = dr["description"].ToString();
                        txtTemp.Text = dr["Temperature"].ToString();
                        txtPres.Text = dr["Pressure"].ToString();
                        txtVabFrac.Text = dr["VaporFraction"].ToString();
                        txtWf.Text = dr["WeightFlow"].ToString();
                        txtH.Text = dr["Enthalpy"].ToString();
                        txtSph.Text = dr["SpEnthalpy"].ToString();

                        setColor(dr, "streamname_color", txtName);
                        setColor(dr, "Temperature_color", txtTemp);
                        setColor(dr, "Pressure_color", txtPres);
                        setColor(dr, "VaporFraction_color", txtVabFrac);
                        setColor(dr, "WeightFlow_color", txtWf);
                        setColor(dr, "Enthalpy_color", txtH);
                        setColor(dr, "SpEnthalpy_color", txtSph);
                        loadstatus = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Action");
            }

        }

       

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void getColor(ref DataRow dr, string dcName, TextBox txtBox)
        {
            if (txtBox.BorderBrush == Brushes.Green)
            {
                dr[dcName] = "green";
            }
            else
            {
                dr[dcName] = "blue";
            }
        }

        private void getColor(ref DataRow dr, string dcName, CheckBox chkBox)
        {
            if (chkBox.BorderBrush == Brushes.Green)
            {
                dr[dcName] = "green";
            }
            else
            {
                dr[dcName] = "blue";
            }
        }

        private void setColor(DataRow dr, string dcName, TextBox txtBox)
        {
            string color = dr[dcName].ToString();
            if (color == "blue")
            {
                txtBox.BorderBrush = Brushes.Blue;
                txtBox.BorderThickness = new Thickness(2, 2, 2, 2);
            }
            else
            {
                txtBox.BorderBrush = Brushes.Green;
                txtBox.BorderThickness = new Thickness(2, 2, 2, 2);
            }
        }

        private void setColor(DataRow dr, string dcName, CheckBox chkBox)
        {
            string color = dr[dcName].ToString();
            if (color == "blue")
            {
                chkBox.BorderBrush = Brushes.Blue;
                chkBox.BorderThickness = new Thickness(2, 2, 2, 2);
            }
            else
            {
                chkBox.BorderBrush = Brushes.Green;
                chkBox.BorderThickness = new Thickness(2, 2, 2, 2);
            }
        }

        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loadstatus == 1)
            {
                TextBox txtBox = (TextBox)sender;
                if (txtBox.Text.Trim() == string.Empty)
                {
                    txtBox.BorderBrush = Brushes.Red;
                }
                else
                {
                    txtBox.BorderBrush = Brushes.Blue;
                }
                txtBox.BorderThickness = new Thickness(2, 2, 2, 2);
            }
        }      
        
    }
}
