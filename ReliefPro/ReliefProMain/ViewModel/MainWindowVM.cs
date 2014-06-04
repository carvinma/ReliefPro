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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Data;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Resources;
using System.Collections;
using System.Configuration;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.AvalonDock;


using ReliefProDAL;
using ProII;
using ReliefProCommon.CommonLib;
using ReliefProBLL.Common;
using ReliefProModel;
using AxMicrosoft.Office.Interop.VisOcx;
using Vo = Microsoft.Office.Interop.VisOcx;
using Visio = Microsoft.Office.Interop.Visio;
using ReliefProMain.View;
using ReliefProMain.ViewModel;


namespace ReliefProMain.ViewModel
{
    public class MainWindowVM:ViewModelBase
    {
        //版本信息
        string version;
        string defaultReliefProDir;
        string tempReliefProWorkDir;
        string currentPlantWorkFolder;
        string currentPlantFile;
        string currentPlantName;
        AxDrawingControl visioControl = new AxDrawingControl();
        public void OnloadUnitOfMeasure(object sender, RoutedEventArgs e)
        {
            FormatUnitsMeasure fum = new FormatUnitsMeasure();
            if (fum.ShowDialog() == true)
            {

            }
        }


        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)e.OriginalSource;
                switch (item.Header.ToString())
                {
                    case "Open Plant":
                        OpenPlant();
                        break;
                    case "Exit":
                        //this.Close();
                        break;
                    case "New Plant":
                        CreatePlant();
                        break;
                    case "Save Plant":
                        SavePlant();
                        break;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Action");
            }
        }

        public void lvGeneral_MouseMove(object sender, MouseEventArgs e)
        {
            Image lvi = (Image)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Visio.Page currentPage = visioControl.Document.Pages[1];

                if (lvi.Source.ToString().Contains("tower"))
                {
                    Visio.Document myCurrentStencil = visioControl.Document.Application.Documents.OpenEx(System.Environment.CurrentDirectory + @"/Template/Tower.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                    Visio.Master visioRectMaster = myCurrentStencil.Masters.get_ItemU(@"Dis");
                    DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                    myCurrentStencil.Close();
                    //openProperty();
                    foreach (Visio.Shape shape in visioControl.Window.Selection)
                    {
                        shape.Cells["EventDblClick"].Formula = "=0";
                    }
                    visioControl.Window.DeselectAll();
                }
                if (lvi.Source.ToString().Contains("drum"))
                {
                    Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                    Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Column");
                    DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                    foreach (Visio.Shape shape in visioControl.Window.Selection)
                    {
                        shape.Cells["EventDblClick"].Formula = "=0";
                    }
                    visioControl.Window.DeselectAll();
                }

            }
        }




        private void initTower()
        {
            ObservableCollection<ListViewItemData> collections = new ObservableCollection<ListViewItemData>();
            collections.Add(new ListViewItemData { Name = "Distillation", Pic = "/images/tower.ico" });
            collections.Add(new ListViewItemData { Name = "Drum", Pic = "/images/drum.ico" });
            //this.lvTower.ItemsSource = collections;
        }



        private void SavePlant()
        {
            ReliefProCommon.CommonLib.CSharpZip.CompressZipFile(currentPlantWorkFolder, currentPlantFile);
        }

        private void OpenPlant()
        {
           
        }

        private void CreatePlant()
        {
            

        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button item = (Button)sender;
                switch (item.ToolTip.ToString())
                {
                    case "Open Plant":
                        OpenPlant();
                        break;
                    case "New Plant":
                        CreatePlant();
                        break;
                    case "Save Plant":
                        SavePlant();
                        break;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Action");
            }
        }

      


    }
    public class ListViewItemData
    {
        public string Name { get; set; }
        public string Pic { get; set; }
    }


}
