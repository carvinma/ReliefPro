﻿using System;
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
using System.IO.Compression;
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
using ReliefProMain.View.Reports;
using ReliefProMain.ViewModel.Reports;
using ReliefProMain.Models;
using NHibernate;
using System.Threading.Tasks;
using System.Threading;
using ReliefProCommon.Logging;
using ReliefProMain.View.Common;

namespace ReliefProMain
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        //版本信息
        //string version;
        //string defaultReliefProDir;
        //string tempReliefProWorkDir;
        //string currentPlantWorkFolder;
        //string currentPlantFile;
        //string currentPlantName;
        //string currentProtectedSystemFile;
        AxDrawingControl visioControl = new AxDrawingControl();
        //public ISession SessionPlant { set; get; }
        //public ISession SessionProtectedSystem { set; get; }
        //public List<TreeViewItemData> treeList;


        public MainWindow()
        {
            InitializeComponent();
        }



        #region TestBackground

        /// <summary>
        /// TestBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty TestBackgroundProperty =
            DependencyProperty.Register("TestBackground", typeof(Brush), typeof(MainWindow),
                new FrameworkPropertyMetadata((Brush)null));

        /// <summary>
        /// Gets or sets the TestBackground property.  This dependency property 
        /// indicates a randomly changing brush (just for testing).
        /// </summary>
        public Brush TestBackground
        {
            get { return (Brush)GetValue(TestBackgroundProperty); }
            set { SetValue(TestBackgroundProperty, value); }
        }

        #endregion


        #region FocusedElement

        /// <summary>
        /// FocusedElement Dependency Property
        /// </summary>
        public static readonly DependencyProperty FocusedElementProperty =
            DependencyProperty.Register("FocusedElement", typeof(string), typeof(MainWindow),
                new FrameworkPropertyMetadata((IInputElement)null));

        /// <summary>
        /// Gets or sets the FocusedElement property.  This dependency property 
        /// indicates ....
        /// </summary>
        public string FocusedElement
        {
            get { return (string)GetValue(FocusedElementProperty); }
            set { SetValue(FocusedElementProperty, value); }
        }

        #endregion

        private void OnLayoutRootPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var activeContent = ((LayoutRoot)sender).ActiveContent;
            if (e.PropertyName == "ActiveContent")
            {
                Debug.WriteLine(string.Format("ActiveContent-> {0}", activeContent));
            }
        }

        private void OnLoadLayout(object sender, RoutedEventArgs e)
        {
            var currentContentsList = dockManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();

            string fileName = (sender as MenuItem).Header.ToString();
            var serializer = new XmlLayoutSerializer(dockManager);
            //serializer.LayoutSerializationCallback += (s, args) =>
            //    {
            //        var prevContent = currentContentsList.FirstOrDefault(c => c.ContentId == args.Model.ContentId);
            //        if (prevContent != null)
            //            args.Content = prevContent.Content;
            //    };
            using (var stream = new StreamReader(string.Format(@".\AvalonDock_{0}.config", fileName)))
                serializer.Deserialize(stream);
        }

        private void OnSaveLayout(object sender, RoutedEventArgs e)
        {
            string fileName = (sender as MenuItem).Header.ToString();
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter(string.Format(@".\AvalonDock_{0}.config", fileName)))
                serializer.Serialize(stream);
        }

        private void OnShowWinformsWindow(object sender, RoutedEventArgs e)
        {
            var winFormsWindow = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "WinFormsWindow");
            if (winFormsWindow.IsHidden)
                winFormsWindow.Show();
            else if (winFormsWindow.IsVisible)
                winFormsWindow.IsActive = true;
            else
                winFormsWindow.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }

        private void AddTwoDocuments_click(object sender, RoutedEventArgs e)
        {
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                LayoutDocument doc = new LayoutDocument();
                doc.Title = "Test1";
                firstDocumentPane.Children.Add(doc);

                LayoutDocument doc2 = new LayoutDocument();
                doc2.Title = "Test2";
                firstDocumentPane.Children.Add(doc2);
            }

            var leftAnchorGroup = dockManager.Layout.LeftSide.Children.FirstOrDefault();
            if (leftAnchorGroup == null)
            {
                leftAnchorGroup = new LayoutAnchorGroup();
                dockManager.Layout.LeftSide.Children.Add(leftAnchorGroup);
            }

            leftAnchorGroup.Children.Add(new LayoutAnchorable() { Title = "New Anchorable" });

        }

        private void OnShowPlantExplorer(object sender, RoutedEventArgs e)
        {
            var toolWindow1 = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "Navigation");
            if (toolWindow1.IsHidden)
                toolWindow1.Show();
            else if (toolWindow1.IsVisible)
                toolWindow1.IsActive = true;
            else
                toolWindow1.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }
        private void OnShowICON(object sender, RoutedEventArgs e)
        {
            var toolWindow1 = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "toolWindow");
            if (toolWindow1.IsHidden)
                toolWindow1.Show();
            else if (toolWindow1.IsVisible)
                toolWindow1.IsActive = true;
            else
                toolWindow1.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }

        private void dockManager_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            try
            {

                //Application.Current.FindResource
                MessageBoxResult r = MessageBox.Show("Are you sure you want to save the document?", "Message Box", MessageBoxButton.YesNoCancel);
                if (r == MessageBoxResult.Yes)
                {
                    string vsdFile = visioControl.Src;
                    visioControl.Document.SaveAs(vsdFile);
                }
                else if (r == MessageBoxResult.No)
                {

                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void OnDumpToConsole(object sender, RoutedEventArgs e)
        {
            // Uncomment when TRACE is activated on AvalonDock project
            // dockManager.Layout.ConsoleDump(0);
        }

        private void OnReloadManager(object sender, RoutedEventArgs e)
        {
        }

        private void OnUnloadManager(object sender, RoutedEventArgs e)
        {
            if (layoutRoot.Children.Contains(dockManager))
                layoutRoot.Children.Remove(dockManager);
        }

        private void OnLoadManager(object sender, RoutedEventArgs e)
        {
            if (!layoutRoot.Children.Contains(dockManager))
                layoutRoot.Children.Add(dockManager);
        }

        private void OnToolWindow1Hiding(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to hide this tool?", "AvalonDock", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void OnloadUnitOfMeasure(object sender, RoutedEventArgs e)
        {
            OpenUOM();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)e.OriginalSource;
                switch (item.Header.ToString())
                {
                    case "Exit":
                        this.Close();
                        break;
                    case "Save Plant":
                        SavePlant();
                        break;
                    case "Close Plant":
                        ClosePlant();
                        break;
                    case "Save As":
                        SaveAsPlant();
                        break;
                    case "Import Simulation":
                        ImportExtraData();
                        break;
                    case "Help":
                        Help();
                        break;
                    case "About SimTechRelief":
                        About();
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Action");
            }
        }

        private void Help()
        {
            string helpPath=Environment.CurrentDirectory+@"\Manual\";
            System.Diagnostics.Process.Start(helpPath);
        }
        private void About()
        {
            AboutUsView view = new AboutUsView();
            AboutUsVM vm = new AboutUsVM();
            view.DataContext = vm;
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.ShowDialog();
        }

        private void lvGeneral_MouseMove(object sender, MouseEventArgs e)
        {
            Image lvi = (Image)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    if (firstDocumentPane.Children.Count > 0)
                    {
                        Visio.Page currentPage = visioControl.Document.Pages[1];

                        if (lvi.Source.ToString().ToLower().Contains("tower"))
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
                        }
                        if (lvi.Source.ToString().ToLower().Contains("drum"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Column");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                        }

                        if (lvi.Source.ToString().ToLower().Contains("storagetank"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Tank");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                        }
                        if (lvi.Source.ToString().ToLower().Contains("heatexchanger"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Heat exchanger1");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                        }
                        if (lvi.Source.ToString().ToLower().Contains("compressor"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEPUMP_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Selectable Compressor1");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                        }
                        if (lvi.Source.ToString().ToLower().Contains("reactorloop"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Reaction vessel");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                        }
                        foreach (LayoutDocument doc in firstDocumentPane.Children)
                        {
                            UCDrawingControl uc = doc.Content as UCDrawingControl;
                            uc.ShapeDoubleClick();
                        }
                        visioControl.Window.DeselectAll();
                    }
                }

            }
        }


        private void initIcon()
        {
            ObservableCollection<ListViewItemData> collections1 = new ObservableCollection<ListViewItemData>();
            collections1.Add(new ListViewItemData { Name = "Tower", Pic = "/images/tower.ico" });

            ObservableCollection<ListViewItemData> collections2 = new ObservableCollection<ListViewItemData>();
            collections2.Add(new ListViewItemData { Name = "Drum", Pic = "/images/drum.ico" });

            ObservableCollection<ListViewItemData> collections3 = new ObservableCollection<ListViewItemData>();
            collections3.Add(new ListViewItemData { Name = "Compressor", Pic = "/images/compressor.ico" });

            ObservableCollection<ListViewItemData> collections4 = new ObservableCollection<ListViewItemData>();
            collections4.Add(new ListViewItemData { Name = "Heat Exchanger", Pic = "/images/HeatExchanger.ico" });

            ObservableCollection<ListViewItemData> collections5 = new ObservableCollection<ListViewItemData>();
            collections5.Add(new ListViewItemData { Name = "Reactor Loop", Pic = "/images/ReactorLoop.ico" });

            ObservableCollection<ListViewItemData> collections6 = new ObservableCollection<ListViewItemData>();
            collections6.Add(new ListViewItemData { Name = "Storage Tank", Pic = "/images/StorageTank.ico" });
            //this.lvTower.ItemsSource = collections;
            this.icon1.ItemsSource = collections1;
            this.icon2.ItemsSource = collections2;
            this.icon3.ItemsSource = collections3;
            this.icon4.ItemsSource = collections4;
            this.icon5.ItemsSource = collections5;
            this.icon6.ItemsSource = collections6;
        }


        private void OpenGloadDefalut()
        {
            string PlantPath = GetPlantPath();
            if (!string.IsNullOrEmpty(PlantPath))
            {
                string DirPlant = System.IO.Path.GetDirectoryName(PlantPath);
                ReliefProMain.View.GlobalDefault.GlobalDefaultView view = new View.GlobalDefault.GlobalDefaultView();
                GlobalDefaultVM vm = new GlobalDefaultVM(UOMLib.UOMSingle.UomEnums.First(p => p.SessionDBPath.Contains(PlantPath)).SessionPlant, DirPlant);
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                view.DataContext = vm;
                view.ShowDialog();
            }
        }
        private void OpenReport()
        {
            //List<string> ReportPath = new List<string>();
            //ReportPath.Add(@"C:\Users\Administrator\AppData\Local\Relief 1.0\testtank\plant.mdb");
            //ReportPath.Add(@"C:\Users\Administrator\AppData\Local\Relief 1.0\testtank\Unit1\ProtectedSystem1\protectedsystem.mdb");
            //PUsummaryView view = new PUsummaryView();
            //PUsummaryVM vm = new PUsummaryVM(1, ReportPath);
            //view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //view.DataContext = vm;
            //view.ShowDialog();
            string currentPlantName = string.Empty;
            string currentPlantWorkFolder = string.Empty;
            ObservableCollection<TVPlantViewModel> list = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
            if (list.Count == 0)
            {
                MessageBox.Show("Please open one Plant", "Message Box");
                return;
            }
            TVPlantViewModel p = GetCurrentPlant();
            if (p == null)
            {
                MessageBox.Show("Please select one Plant", "Message Box");
                return;
            }
            currentPlantName = p.Name;
            currentPlantWorkFolder = p.tvPlant.FullPath;


            ReportTreeView view = new ReportTreeView();
            ReportTreeVM vm = new ReportTreeVM(currentPlantName, currentPlantWorkFolder);
            view.DataContext = vm;

            view.ShowDialog();
        }
        private void OpenUOM()
        {
            string PlantPath = GetPlantPath();
            if (!string.IsNullOrEmpty(PlantPath))
            {
                FormatUnitsMeasure view = new FormatUnitsMeasure();
                FormatUnitsMeasureVM vm = new FormatUnitsMeasureVM(UOMLib.UOMSingle.UomEnums.First(p => p.SessionDBPath.Contains(PlantPath)).SessionPlant, PlantPath);
                view.DataContext = vm;
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                view.ShowDialog();
            }
        }

        private string GetPlantPath()
        {
            var treeSelectedItem = NavigationTreeView.SelectedItem;
            if ((treeSelectedItem as TVPlantViewModel) != null)
            {
                return (treeSelectedItem as TVPlantViewModel).tvPlant.dbPlantFile;
            }
            if ((treeSelectedItem as TVUnitViewModel) != null)
            {
                return (treeSelectedItem as TVUnitViewModel).tvUnit.dbPlantFile;
            }
            if ((treeSelectedItem as TVPSViewModel) != null)
            {
                return (treeSelectedItem as TVPSViewModel).tvPS.dbPlantFile;
            }
            if ((treeSelectedItem as TVFileViewModel) != null)
            {
                return (treeSelectedItem as TVFileViewModel).tvFile.dbPlantFile;
            }
            ObservableCollection<TVPlantViewModel> list = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
            if (list.Count > 0)
                return list[0].tvPlant.dbPlantFile;
            return string.Empty;
        }

        private void ClosePlant()
        {
            ObservableCollection<TVPlantViewModel> list = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
            if (list.Count > 0)
            {
                MessageBoxResult r = MessageBox.Show("Are you sure you want to save all plants ?", "", MessageBoxButton.YesNoCancel);
                if (r == MessageBoxResult.Yes)
                {
                    var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                    if (firstDocumentPane != null)
                    {
                        if (firstDocumentPane.Children.Count > 0)
                        {
                            MessageBox.Show("Please close all documents first!", "Message Box");
                            return;
                        }
                    }
                    SavePlant();

                    list.Clear();
                    btnReport.IsEnabled = false;
                    btnUOM.IsEnabled = false;
                    btnClosePlant.IsEnabled = false;
                    btnGlobalDefault.IsEnabled = false;
                    btnImport.IsEnabled = false;
                    btnSavePlant.IsEnabled = false;

                    itemClosePlant.IsEnabled = false;
                    itemImport.IsEnabled = false;
                    itemSavePlant.IsEnabled = false;
                    itemSavePlantAs.IsEnabled = false;
                    itemUOM.IsEnabled = false;
                }
                else if (r == MessageBoxResult.No)
                {
                    var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                    if (firstDocumentPane != null)
                    {
                        if (firstDocumentPane.Children.Count > 0)
                        {
                            MessageBox.Show("Please close all documents first!", "Message Box");
                            return;
                        }
                    }                    
                    list.Clear();
                    btnReport.IsEnabled = false;
                    btnUOM.IsEnabled = false;
                    btnClosePlant.IsEnabled = false;
                    btnGlobalDefault.IsEnabled = false;
                    btnImport.IsEnabled = false;
                    btnSavePlant.IsEnabled = false;

                    itemClosePlant.IsEnabled = false;
                    itemImport.IsEnabled = false;
                    itemSavePlant.IsEnabled = false;
                    itemSavePlantAs.IsEnabled = false;
                    itemUOM.IsEnabled = false;
                }
            }
        }

        private void SavePlant()
        {
            try
            {
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    if (firstDocumentPane.Children.Count > 0)
                    {
                        foreach (LayoutContent c in firstDocumentPane.Children)
                        {
                            UCDrawingControl uc = c.Content as UCDrawingControl;
                            uc.visioControl.Document.SaveAs(uc.visioControl.Src);
                        }
                    }
                }

                ObservableCollection<TVPlantViewModel> list = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
                foreach (TVPlantViewModel p in list)
                {
                    string currentPlantWorkFolder = p.tvPlant.FullPath;
                    string currentPlantFile = p.tvPlant.FullRefPath;
                    string winTempDir = Environment.GetEnvironmentVariable("Temp") +@"\" +Guid.NewGuid().ToString();

                    try
                    {
                        if (Directory.Exists(winTempDir))
                        {
                            Directory.Delete(winTempDir, true);
                        }
                        Directory.CreateDirectory(winTempDir);
                        string winTempFile = winTempDir + @"\" + p.Name + ".ref";
                        //ReliefProCommon.CommonLib.CSharpZip.CompressZipFile(currentPlantWorkFolder, currentPlantFile);
                        ZipFile.CreateFromDirectory(currentPlantWorkFolder, winTempFile);
                        File.Copy(winTempFile, currentPlantFile, true);
                    }
                    catch (Exception ex2)
                    {
                    }
                    finally
                    {
                        if (Directory.Exists(winTempDir))
                        {
                            Directory.Delete(winTempDir, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SaveAsPlant()
        {
            try
            {
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    if (firstDocumentPane.Children.Count > 0)
                    {
                        foreach (LayoutContent c in firstDocumentPane.Children)
                        {
                            UCDrawingControl uc = c.Content as UCDrawingControl;
                            uc.visioControl.Document.SaveAs(uc.visioControl.Src);
                        }
                    }
                }
                Microsoft.Win32.SaveFileDialog dlgSaveDiagram = new Microsoft.Win32.SaveFileDialog();
                dlgSaveDiagram.Filter = "ReliefPro|*.ref;";
                dlgSaveDiagram.FileName = string.Empty;
                if (dlgSaveDiagram.ShowDialog() == true)
                {
                    if (dlgSaveDiagram.FileName.Trim().Contains(" "))
                    {
                        MessageBox.Show("Plant Name could not contain space", "Message Box");
                        return;
                    }
                    ObservableCollection<TVPlantViewModel> list = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
                    if (list.Count > 0)
                    {
                        TVPlantViewModel p = list[0];
                        string currentPlantWorkFolder = p.tvPlant.FullPath;
                        string newPlantFile = dlgSaveDiagram.FileName;
                        //ReliefProCommon.CommonLib.CSharpZip.CompressZipFile(currentPlantWorkFolder, dlgSaveDiagram.FileName);
                        if (File.Exists(newPlantFile))
                        {
                            File.Delete(newPlantFile);
                        }
                        ZipFile.CreateFromDirectory(currentPlantWorkFolder, newPlantFile);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void MainWindowApp_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                initIcon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private static ManualResetEvent BusinessDone = new ManualResetEvent(false);
        private void NavigationTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TVFileViewModel tvi = NavigationTreeView.SelectedItem as TVFileViewModel;
                if (NavigationTreeView.SelectedItem == null || tvi == null)
                    return;
                //TVFileViewModel tvi = (TVFileViewModel)NavigationTreeView.SelectedItem;
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {

                    bool b = false;
                    foreach (LayoutDocument d in firstDocumentPane.Children)
                    {
                        if (d.Description == tvi.tvFile.FullPath)
                        {
                            b = true;
                            d.IsActive = true;
                        }
                        else
                            d.IsActive = false;
                    }
                    if (!b)
                    {
                        //this.busyCtrl.IsBusy = true;
                        //this.busyCtrl.Text = "Loading Content...";
                        Task.Factory.StartNew(() =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                this.busyCtrl.IsBusy = true;
                                //this.busyCtrl.Text = "Loading Content...";
                            }));


                        }).ContinueWith((t) =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                LayoutDocument doc = new LayoutDocument();
                                doc.Title = tvi.Name;
                                doc.Description = tvi.tvFile.FullPath;
                                doc.IsActive = true;
                                UCDrawingControl ucDrawingControl = new UCDrawingControl();
                                ucDrawingControl.ownerWindow = this;
                                doc.Content = ucDrawingControl;
                                ucDrawingControl.Tag = tvi;

                                firstDocumentPane.Children.Add(doc);
                                visioControl = ucDrawingControl.visioControl;

                            }));
                        }).ContinueWith((t) =>
                        {

                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                this.busyCtrl.IsBusy = false;
                            }));
                        });
                        // BusinessDone.WaitOne();
                    }


                }
            }
            catch (Exception ex)
            {
            }
        }

        private void NavigationTreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;

            if (treeViewItem != null)
            {
                //treeViewItem.Focus();
                e.Handled = true;


                ContextMenu rmenu = (ContextMenu)this.Resources["RightContextMenu"];


                if (treeViewItem.Header.GetType() == typeof(TVPlantViewModel))
                {
                    for (int i = 1; i < 3; i++)
                    {
                        MenuItem item = (MenuItem)rmenu.Items[i];
                        item.IsEnabled = true;
                    }
                    MenuItem item2 = (MenuItem)rmenu.Items[3];
                    item2.IsEnabled = false;
                    MenuItem item1 = (MenuItem)rmenu.Items[0];
                    item1.IsEnabled = false;

                    MenuItem item4 = (MenuItem)rmenu.Items[4];
                    item4.IsEnabled = false;
                }


                else if (treeViewItem.Header.GetType() == typeof(TVUnitViewModel))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        MenuItem item = (MenuItem)rmenu.Items[i];
                        item.IsEnabled = false;
                    }
                    MenuItem item2 = (MenuItem)rmenu.Items[3];
                    item2.IsEnabled = true;
                    MenuItem item1 = (MenuItem)rmenu.Items[0];
                    item1.IsEnabled = true;
                    MenuItem item4 = (MenuItem)rmenu.Items[4];
                    item4.IsEnabled = true;
                }
                else if (treeViewItem.Header.GetType() == typeof(TVPSViewModel))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        MenuItem item = (MenuItem)rmenu.Items[i];
                        item.IsEnabled = false;
                    }
                    MenuItem item1 = (MenuItem)rmenu.Items[0];
                    item1.IsEnabled = true;

                    MenuItem item4 = (MenuItem)rmenu.Items[4];
                    item4.IsEnabled = true;
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MenuItem item = (MenuItem)rmenu.Items[i];
                        item.IsEnabled = false;
                    }

                }




            }
        }
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        public void ImportDataFromOther(object sender, RoutedEventArgs e)
        {
            ImportExtraData();
        }

        private void ImportExtraData()
        {
            TVPlantViewModel tvi = null;
            ObservableCollection<TVPlantViewModel> Plants = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
            if (Plants.Count == 0)
            {
                MessageBox.Show("Please create a plant or open a plant first.", "Message Box");
                return;
            }
            if (Plants.Count == 1)
            {
                tvi = Plants[0];
            }
            else if (Plants.Count > 1)
            {
                if (NavigationTreeView.SelectedItem == null || NavigationTreeView.SelectedItem.GetType() != typeof(TVPlantViewModel))
                {
                    MessageBox.Show("Please select a plant first.", "Message Box");
                    return;
                }
                else
                {
                    tvi = (TVPlantViewModel)NavigationTreeView.SelectedItem;

                }

            }
            ImportDataView imptdata = new ImportDataView();

            imptdata.dirInfo = tvi.tvPlant.FullPath;
            imptdata.SessionPlant = tvi.SessionPlant;
            imptdata.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            imptdata.Owner = this;
            imptdata.ShowDialog();
        }


        public void CreateUnit(object sender, RoutedEventArgs e)
        {
            if (NavigationTreeView.SelectedItem == null)
                return;
            if (NavigationTreeView.SelectedItem.GetType() == typeof(TVPlantViewModel))
            {
                TVPlantViewModel p = NavigationTreeView.SelectedItem as TVPlantViewModel;
                CreateUnitView v = new CreateUnitView();
                CreateUnitVM vm = new CreateUnitVM(p.SessionPlant, p.tvPlant.FullPath);
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                v.Owner = this;
                if (v.ShowDialog() == true)
                {
                    TVUnit tvUnit = vm.tvUnit;
                    tvUnit.dbPlantFile = p.tvPlant.dbPlantFile;
                    TVUnitViewModel unit = new TVUnitViewModel(tvUnit, p);
                    p.Children.Add(unit);
                }
            }

        }

        public void CreateProtectedSystem(object sender, RoutedEventArgs e)
        {
            if (NavigationTreeView.SelectedItem == null)
                return;
            if (NavigationTreeView.SelectedItem.GetType() == typeof(TVUnitViewModel))
            {
                TVUnitViewModel p = NavigationTreeView.SelectedItem as TVUnitViewModel;
                CreateProtectedSystemView v = new CreateProtectedSystemView();
                CreateProtectedSystemVM vm = new CreateProtectedSystemVM(p.SessionPlant, p.tvUnit.ID, p.tvUnit.FullPath);
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                v.Owner = this;
                if (v.ShowDialog() == true)
                {
                    TVPS tvPS = vm.tvPS;
                    tvPS.dbPlantFile = p.tvUnit.dbPlantFile;
                    TVPSViewModel ps = new TVPSViewModel(tvPS, p);
                    p.Children.Add(ps);
                }
            }



        }

        public void ReName(object sender, RoutedEventArgs e)
        {
            if (NavigationTreeView.SelectedItem == null)
                return;
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                if (firstDocumentPane.Children.Count > 0)
                {
                    MessageBox.Show("Please Close all documents", "Message Box");
                    return;
                }



            }

            ReNameView v = new ReNameView();
            ReNameVM vm = null;
            if (NavigationTreeView.SelectedItem.GetType() == typeof(TVUnitViewModel))
            {
                TVUnitViewModel u = NavigationTreeView.SelectedItem as TVUnitViewModel;
                vm = new ReNameVM(u.Name, u.tvUnit.FullPath, 1);
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (v.ShowDialog() == true)
                {
                    u.Name = vm.Name;
                    TreeUnitDAL treeUnitDal = new TreeUnitDAL();
                    TreeUnit treeU = treeUnitDal.GetModel(u.SessionPlant, vm.OldName);
                    treeU.UnitName = u.Name;
                    treeUnitDal.Update(treeU, u.SessionPlant);
                    u.SessionPlant.Flush();
                }
            }
            else if (NavigationTreeView.SelectedItem.GetType() == typeof(TVPSViewModel))
            {
                TVPSViewModel ps = NavigationTreeView.SelectedItem as TVPSViewModel;
                vm = new ReNameVM(ps.Name, ps.tvPS.FullPath, 2);
                v.DataContext = vm;
                v.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                DirectoryInfo unitDI = Directory.GetParent(ps.tvPS.FullPath);
                TreeUnitDAL treeUnitDal = new TreeUnitDAL();
                TreeUnit treeU = treeUnitDal.GetModel(ps.SessionPlant, unitDI.Name);

                if (v.ShowDialog() == true)
                {
                    ps.Name = vm.Name;
                    TreePSDAL treePSDal = new TreePSDAL();
                    TreePS treep = treePSDal.GetModel(ps.SessionPlant, treeU.ID, vm.OldName);
                    treep.PSName = ps.Name;
                    treePSDal.Update(treep, ps.SessionPlant);
                    ps.SessionPlant.Flush();
                }
            }
            //重命名时，需要改
            if (!UOMSingle.plantsInfo.Exists(o => o.Name == currentPlantName))
            {
                PlantInfo uomPlant = new PlantInfo();
                uomPlant.Name = currentPlantName;
                if (p.FullPath.Contains("plant.mdb"))
                    uomPlant.DataContext = new ORDesignerPlantDataContext(p.FullPath);
                else
                    uomPlant.DataContext = new ORDesignerPlantDataContext(p.FullPath + @"\plant.mdb");
                UOMSingle.currentPlantContext = uomPlant.DataContext;
                uomPlant.UnitInfo = new UOMEnum();
                UOMSingle.plantsInfo.
            }





        }

        public void RemoveNode(object sender, RoutedEventArgs e)
        {
            if (NavigationTreeView.SelectedItem == null)
                return;
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                if (firstDocumentPane.Children.Count > 0)
                {
                    MessageBox.Show("Please Close all documents", "Message Box");
                    return;
                }
            }

            ReNameView v = new ReNameView();
            ReNameVM vm = null;
            if (NavigationTreeView.SelectedItem.GetType() == typeof(TVUnitViewModel))
            {
                TVUnitViewModel u = NavigationTreeView.SelectedItem as TVUnitViewModel;
                MessageBoxResult result = MessageBox.Show("Are your sure remove this Unit and its Protected System?", "Message Box", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    TVPlantViewModel p = u.Parent as TVPlantViewModel;
                    p.Children.Remove(u);   //从集合中删除

                    TreeUnitDAL treeUnitDal = new TreeUnitDAL();
                    TreeUnit treeU = treeUnitDal.GetModel(u.tvUnit.ID, u.SessionPlant);
                    foreach (TreeViewItemViewModel m in u.Children)
                    {
                        TVPSViewModel ps = m as TVPSViewModel;
                        TreePSDAL treePSDal = new TreePSDAL();
                        TreePS treep = treePSDal.GetModel(u.SessionPlant, treeU.ID, ps.Name);
                        treePSDal.Delete(treep, ps.SessionPlant);        //db删除unit下的ps                
                    }
                    treeUnitDal.Delete(treeU, u.SessionPlant); //db删除unit
                    Directory.Delete(u.tvUnit.FullPath, true);//删除unit对应的文件夹

                }
            }
            else if (NavigationTreeView.SelectedItem.GetType() == typeof(TVPSViewModel))
            {
                TVPSViewModel ps = NavigationTreeView.SelectedItem as TVPSViewModel;
                DirectoryInfo unitDI = Directory.GetParent(ps.tvPS.FullPath);
                TreeUnitDAL treeUnitDal = new TreeUnitDAL();
                TreeUnit treeU = treeUnitDal.GetModel(ps.SessionPlant, unitDI.Name);
                MessageBoxResult result = MessageBox.Show("Are your sure remove this Protected System?", "Message Box", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    TVUnitViewModel u = ps.Parent as TVUnitViewModel;
                    u.Children.Remove(ps);

                    TreePSDAL treePSDal = new TreePSDAL();
                    TreePS treep = treePSDal.GetModel(ps.SessionPlant, treeU.ID, ps.Name);
                    treePSDal.Delete(treep, ps.SessionPlant);
                    Directory.Delete(ps.tvPS.FullPath, true);
                }

            }







        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button item = (Button)sender;
                switch (item.ToolTip.ToString())
                {
                    case "Global Default":
                        OpenGloadDefalut();
                        break;
                    case "Report":
                        OpenReport();
                        break;
                    case "UOM":
                        OpenUOM();
                        break;
                    case "Save Plant":
                        SavePlant();
                        break;
                    case "Close Plant":
                        ClosePlant();
                        break;
                    case "Import Simulation":
                        ImportExtraData();
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Action");
            }
        }

        private void MainWindowApp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ObservableCollection<TVPlantViewModel> list = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
                if (list.Count > 0)
                {
                    MessageBoxResult r = MessageBox.Show("Are you sure you want to save all plants?", "", MessageBoxButton.YesNoCancel);
                    if (r == MessageBoxResult.Yes)
                    {
                        try
                        {
                            SavePlant();
                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                    }
                    else if (r == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
                
            }
            catch (Exception ex)
            {
                Logging.Debug(ex.Message);
                MessageBox.Show(ex.ToString());
            }
            finally
            {
               
                //Application.Current.Shutdown(-1);    
                //Environment.Exit(0);
            }

        }

        private TVPlantViewModel GetCurrentPlant()
        {
            TVPlantViewModel p = null;
            ObservableCollection<TVPlantViewModel> list = NavigationTreeView.ItemsSource as ObservableCollection<TVPlantViewModel>;
            if (list.Count == 1)
            {
                p = list[0];

            }
            else if (NavigationTreeView.SelectedItem == null)
            {
                p = null;
            }
            else if (NavigationTreeView.SelectedItem.GetType() == typeof(TVPlantViewModel))
            {
                p = NavigationTreeView.SelectedItem as TVPlantViewModel;
            }
            else if (NavigationTreeView.SelectedItem.GetType() == typeof(TVUnitViewModel))
            {
                TVUnitViewModel u = NavigationTreeView.SelectedItem as TVUnitViewModel;
                p = u.Parent as TVPlantViewModel;
            }
            else if (NavigationTreeView.SelectedItem.GetType() == typeof(TVPSViewModel))
            {
                TVPSViewModel ps = NavigationTreeView.SelectedItem as TVPSViewModel;
                p = ps.Parent.Parent as TVPlantViewModel;
            }
            else if (NavigationTreeView.SelectedItem.GetType() == typeof(TVFileViewModel))
            {
                TVFileViewModel f = NavigationTreeView.SelectedItem as TVFileViewModel;
                p = f.Parent.Parent.Parent as TVPlantViewModel;
            }
            return p;
        }

        private void ClosePlant(object obj)
        {
            ObservableCollection<TVPlantViewModel> Plants = (ObservableCollection<TVPlantViewModel>)NavigationTreeView.ItemsSource;
            if (Plants.Count == 0)
            {
                return;
            }
            
            MessageBoxResult result = MessageBox.Show("Are your sure clear all plants and save all documents?", "Message Box", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Plants.Clear();
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    if (firstDocumentPane.Children.Count > 0)
                    {
                        foreach (LayoutDocument doc in firstDocumentPane.Children)
                        {
                            UCDrawingControl uc = doc.Content as UCDrawingControl;
                            uc.visioControl.Document.SaveAs(uc.visioControl.Src);
                        }
                        
                    }
                }
            }
        }

        private void Icon_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Image item = (Image)sender;
                
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    if (firstDocumentPane.Children.Count > 0)
                    {
                        Visio.Page currentPage = visioControl.Document.Pages[1];

                        if (item.ToolTip.ToString().ToLower().Contains("tower"))
                        {
                            Visio.Document myCurrentStencil = visioControl.Document.Application.Documents.OpenEx(System.Environment.CurrentDirectory + @"/Template/Tower.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = myCurrentStencil.Masters.get_ItemU(@"Dis");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(item, visioRectMaster, DragDropEffects.All);
                            myCurrentStencil.Close();
                            //openProperty();
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                            visioControl.Window.DeselectAll();
                        }
                        if (item.ToolTip.ToString().ToLower().Contains("drum"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Column");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(item, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                            visioControl.Window.DeselectAll();
                        }

                        if (item.ToolTip.ToString().ToLower().Contains("storagetank"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Tank");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(item, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                            visioControl.Window.DeselectAll();
                        }
                        if (item.ToolTip.ToString().ToLower().Contains("hx"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Heat exchanger2");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(item, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                            visioControl.Window.DeselectAll();
                        }
                        if (item.ToolTip.ToString().ToLower().Contains("compressor"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEPUMP_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Selectable Compressor1");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(item, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                            visioControl.Window.DeselectAll();
                        }
                        if (item.ToolTip.ToString().ToLower().Contains("reactorloop"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEVESS_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Reaction vessel");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(item, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                            visioControl.Window.DeselectAll();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Message Box");
            }
        }

        private bool checkAdobeReader()
        {
            Microsoft.Win32.RegistryKey uninstallNode = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string subKeyName in uninstallNode.GetSubKeyNames())
            {
                Microsoft.Win32.RegistryKey subKey = uninstallNode.OpenSubKey(subKeyName);
                object displayName = subKey.GetValue("DisplayName");
                if (displayName != null)
                {
                    if (displayName.ToString().Contains("Adobe Reader"))
                    {
                        return true;
                        // MessageBox.Show(displayName.ToString());   
                    }
                }
            }
            return false;
        } 
    
    }



}
