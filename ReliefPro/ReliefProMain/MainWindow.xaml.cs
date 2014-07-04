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
using ReliefProMain.View.Reports;
using ReliefProMain.ViewModel.Reports;

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
        string version;
        string defaultReliefProDir;
        string tempReliefProWorkDir;
        string currentPlantWorkFolder;
        string currentPlantFile;
        string currentPlantName;
        //string currentProtectedSystemFile;
        AxDrawingControl visioControl = new AxDrawingControl();

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

        private void OnShowToolWindow1(object sender, RoutedEventArgs e)
        {
            var toolWindow1 = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "toolWindow1");
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
            FormatUnitsMeasure fum = new FormatUnitsMeasure();
            if (fum.ShowDialog() == true)
            {

            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
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
                        this.Close();
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
                            visioControl.Window.DeselectAll();
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
                            visioControl.Window.DeselectAll();
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
                            visioControl.Window.DeselectAll();
                        }
                        if (lvi.Source.ToString().ToLower().Contains("heatexchanger"))
                        {
                            Visio.Document currentStencil = visioControl.Document.Application.Documents.OpenEx("PEHEAT_M.vss", (short)Visio.VisOpenSaveArgs.visAddHidden);
                            Visio.Master visioRectMaster = currentStencil.Masters.get_ItemU(@"Heat exchanger2");
                            DragDropEffects dde1 = DragDrop.DoDragDrop(lvi, visioRectMaster, DragDropEffects.All);
                            foreach (Visio.Shape shape in visioControl.Window.Selection)
                            {
                                shape.Cells["EventDblClick"].Formula = "=0";
                            }
                            visioControl.Window.DeselectAll();
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
                            visioControl.Window.DeselectAll();
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
                            visioControl.Window.DeselectAll();
                        }
                    }
                }

            }
        }




        private void initTower()
        {
            ObservableCollection<ListViewItemData> collections = new ObservableCollection<ListViewItemData>();
            collections.Add(new ListViewItemData { Name = "Distillation", Pic = "/images/tower.ico" });
            collections.Add(new ListViewItemData { Name = "Drum", Pic = "/images/drum.ico" });
            collections.Add(new ListViewItemData { Name = "Compressor", Pic = "/images/compressor.ico" });
            collections.Add(new ListViewItemData { Name = "Heat Exchanger", Pic = "/images/HeatExchanger.ico" });
            collections.Add(new ListViewItemData { Name = "Reactor Loop", Pic = "/images/ReactorLoop.ico" });
            collections.Add(new ListViewItemData { Name = "Storage Tank", Pic = "/images/StorageTank.ico" });
            this.lvTower.ItemsSource = collections;
        }


        private void OpenGloadDefalut()
        {
            ReliefProMain.View.GlobalDefault.GlobalDefaultView view = new View.GlobalDefault.GlobalDefaultView();
            GlobalDefaultVM vm = new GlobalDefaultVM();
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.DataContext = vm;
            view.ShowDialog();
        }
        private void OpenReport()
        {
            List<string> ReportPath = new List<string>();
            ReportPath.Add(@"C:\Users\Administrator\AppData\Local\Relief 1.0\testtank\Unit1\ProtectedSystem1\protectedsystem.mdb");
            PUsummaryView view = new PUsummaryView();
            PUsummaryVM vm = new PUsummaryVM(ReportPath);
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.DataContext = vm;
            view.ShowDialog();
        }
        private void SavePlant()
        {
            ReliefProCommon.CommonLib.CSharpZip.CompressZipFile(currentPlantWorkFolder, currentPlantFile);
        }

        private void OpenPlant()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlgOpenDiagram = new Microsoft.Win32.OpenFileDialog();
                dlgOpenDiagram.Filter = "Relief(*.ref) |*.ref";
                if (dlgOpenDiagram.ShowDialog() == true)
                {
                    currentPlantFile = dlgOpenDiagram.FileName;
                    currentPlantName = System.IO.Path.GetFileNameWithoutExtension(currentPlantFile);
                    currentPlantWorkFolder = tempReliefProWorkDir + currentPlantName;

                    if (Directory.Exists(currentPlantWorkFolder))
                    {
                        Directory.Delete(currentPlantWorkFolder, true);
                    }

                    ReliefProCommon.CommonLib.CSharpZip.ExtractZipFile(currentPlantFile, "1", currentPlantWorkFolder);
                    string dbPlant_target = currentPlantWorkFolder + @"\plant.mdb";

                    TreeViewItem item = GetTreeViewItem(currentPlantName, currentPlantWorkFolder, 1, "images/plant.ico", dbPlant_target, null);
                    DirectoryInfo dirPlant = new DirectoryInfo(currentPlantWorkFolder);

                    foreach (DirectoryInfo device in dirPlant.GetDirectories())
                    {
                        TreeViewItem itemDevice = GetTreeViewItem(device.Name, device.FullName, 2, "images/plant.ico", dbPlant_target, null);
                        item.Items.Add(itemDevice);
                        foreach (DirectoryInfo dirProtectedSystem in device.GetDirectories())
                        {
                            string dbProtectSystem_target = dirProtectedSystem.FullName + @"\protectedsystem.mdb";
                            TreeViewItem itemProtectSystem = GetTreeViewItem(dirProtectedSystem.Name, dirProtectedSystem.FullName, 3, "images/plant.ico", dbPlant_target, dbProtectSystem_target);
                            itemDevice.Items.Add(itemProtectSystem);

                            TreeViewItem itemProtectSystemfile = GetTreeViewItem(dirProtectedSystem.Name, dirProtectedSystem.FullName + @"\design.vsd", 4, "images/project.ico", dbPlant_target, dbProtectSystem_target);
                            itemProtectSystem.Items.Add(itemProtectSystemfile);
                        }

                    }

                    NavigationTreeView.Items.Add(item);
                    item.ExpandSubtree();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void CreatePlant()
        {
            try
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog1.Filter = "ref files (*.ref)|*.ref";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.Title = "New Plant";
                saveFileDialog1.InitialDirectory = defaultReliefProDir;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    currentPlantFile = saveFileDialog1.FileName;
                    currentPlantName = System.IO.Path.GetFileNameWithoutExtension(currentPlantFile);
                    currentPlantWorkFolder = tempReliefProWorkDir + currentPlantName;
                    if (Directory.Exists(currentPlantWorkFolder))
                    {
                        Directory.Delete(currentPlantWorkFolder, true);
                    }
                    Directory.CreateDirectory(currentPlantWorkFolder);
                    string dbPlant = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\plant.mdb";
                    string dbPlant_target = currentPlantWorkFolder + @"\plant.mdb";
                    System.IO.File.Copy(dbPlant, dbPlant_target, true);

                    string unit1 = currentPlantWorkFolder + @"\Unit1";
                    Directory.CreateDirectory(unit1);
                    string protectedsystem1 = unit1 + @"\ProtectedSystem1";
                    Directory.CreateDirectory(protectedsystem1);
                    string dbProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.mdb";
                    string dbProtectedSystem_target = protectedsystem1 + @"\protectedsystem.mdb";
                    System.IO.File.Copy(dbProtectedSystem, dbProtectedSystem_target, true);
                    string visioProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.vsd";
                    string visioProtectedSystem_target = protectedsystem1 + @"\design.vsd";
                    System.IO.File.Copy(visioProtectedSystem, visioProtectedSystem_target, true);


                    ReliefProCommon.CommonLib.CSharpZip.CompressZipFile(currentPlantWorkFolder, currentPlantFile);

                    TreeViewItem item = GetTreeViewItem(currentPlantName, currentPlantWorkFolder, 1, "images/plant.ico", dbPlant_target, null);

                    TreeViewItem itemUnit = GetTreeViewItem("Unit1", unit1, 2, "images/plant.ico", dbPlant_target, null);
                    item.Items.Add(itemUnit);

                    TreeViewItem itemProtectSystem = GetTreeViewItem("ProtectedSystem1", protectedsystem1, 3, "images/plant.ico", dbPlant_target, dbProtectedSystem_target);
                    itemUnit.Items.Add(itemProtectSystem);

                    TreeViewItem itemProtectSystemfile = GetTreeViewItem("ProtectedSystem1", visioProtectedSystem_target, 4, "images/project.ico", dbPlant_target, dbProtectedSystem_target);
                    itemProtectSystem.Items.Add(itemProtectSystemfile);

                    NavigationTreeView.Items.Add(item);
                    item.ExpandSubtree();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private TreeViewItem GetTreeViewItem(string text, string fullname, int type, string imagepath, string dbPlantFile, string dbProtectedSystemFile)
        {

            TreeViewItemData data = new TreeViewItemData();
            data.Text = text;
            data.Type = type;
            data.FullName = fullname;
            data.Pic = imagepath;
            data.dbPlantFile = dbPlantFile;
            data.dbProtectedSystemFile = dbProtectedSystemFile;
            TreeViewItem newTreeViewItem = new TreeViewItem();
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.Height = 20;
            // create Image
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(imagepath, UriKind.Relative));
            image.Width = 16;
            image.Height = 16;
            // Label
            TextBlock lbl = new TextBlock();
            lbl.Text = text;
            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);
            // assign stack to header
            newTreeViewItem.Header = stack;
            newTreeViewItem.Tag = data;
            return newTreeViewItem;
        }

        //HKEY_LOCAL_MACHINE\SOFTWARE\SIMSCI\PRO/II\9.1       
        private bool IsRegeditExit(string key, ref string proiiexe, ref string proiiini)
        {
            bool _exit = false;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
            RegistryKey simsci = software.OpenSubKey("SIMSCI", true);
            RegistryKey proii = simsci.OpenSubKey("PRO/II", true);
            RegistryKey info = proii.OpenSubKey(key, true);
            if (info != null)
            {
                proiiexe = info.GetValue("SecDir").ToString() + @"\Proii.exe";
                proiiini = info.GetValue("SecIni").ToString();
                _exit = true;
            }
            return _exit;
        }

        private void MainWindowApp_Loaded(object sender, RoutedEventArgs e)
        {
            //string proiiexe = string.Empty;
            //string proiiini = string.Empty;
            //string name = ("9.1").ToLower();
            //bool b = IsRegeditExit(name, ref proiiexe, ref proiiini);
            //if (b)
            //{
            //    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(proiiexe);
            //    psi.Arguments = @"/I='" + proiiini+"'";
            //    Process.Start(psi);
            //}
            version = ConfigurationManager.AppSettings["version"];
            defaultReliefProDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + version + @"\";
            if (!Directory.Exists(defaultReliefProDir))
                Directory.CreateDirectory(defaultReliefProDir);
            tempReliefProWorkDir = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + version + @"\";
            if (!Directory.Exists(tempReliefProWorkDir))
                Directory.CreateDirectory(tempReliefProWorkDir);
            initTower();
        }

        private void NavigationTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (NavigationTreeView.SelectedItem == null)
                    return;
                TreeViewItem tvi = (TreeViewItem)NavigationTreeView.SelectedItem;
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    TreeViewItemData data = tvi.Tag as TreeViewItemData;
                    if (data.Type == 4)
                    {
                        bool b = false;
                        foreach (LayoutDocument d in firstDocumentPane.Children)
                        {
                            if (d.Description == data.FullName)
                            {
                                b = true;
                                d.IsActive = true;
                                break;
                            }
                        }
                        if (!b)
                        {
                            LayoutDocument doc = new LayoutDocument();
                            doc.Title = data.Text;
                            doc.Description = data.FullName;
                            UCDrawingControl ucDrawingControl = new UCDrawingControl();
                            doc.Content = ucDrawingControl;
                            ucDrawingControl.Tag = data;


                            firstDocumentPane.Children.Add(doc);
                            visioControl = ucDrawingControl.visioControl;
                        }
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
                treeViewItem.Focus();
                e.Handled = true;
                TreeViewItemData data = new TreeViewItemData();
                if (treeViewItem.Tag != null)
                {
                    data = treeViewItem.Tag as TreeViewItemData;
                }
                ContextMenu rmenu = (ContextMenu)this.Resources["RightContextMenu"];
                if (data.Type == 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        MenuItem item = (MenuItem)rmenu.Items[i];
                        item.IsEnabled = true;
                    }
                    MenuItem item2 = (MenuItem)rmenu.Items[3];
                    item2.IsEnabled = false;
                }
                if (data.Type == 2)
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
                }
                if (data.Type == 4 || data.Type == 3)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        MenuItem item = (MenuItem)rmenu.Items[i];
                        item.IsEnabled = false;
                    }
                    MenuItem item1 = (MenuItem)rmenu.Items[0];
                    item1.IsEnabled = true;
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
            if (NavigationTreeView.SelectedItem == null)
                return;
            TreeViewItem tvi = (TreeViewItem)NavigationTreeView.SelectedItem;
            ImportDataView imptdata = new ImportDataView();
            TreeViewItemData data = tvi.Tag as TreeViewItemData;
            imptdata.dirInfo = data.FullName;
            imptdata.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            imptdata.Owner = this;
            imptdata.ShowDialog();

        }

        public void CreateUnit(object sender, RoutedEventArgs e)
        {
            if (NavigationTreeView.SelectedItem == null)
                return;
            TreeViewItem tvi = (TreeViewItem)NavigationTreeView.SelectedItem;
            TreeViewItemData data = tvi.Tag as TreeViewItemData;
            CreateUnitView v = new CreateUnitView();
            CreateUnitVM vm = new CreateUnitVM(data.FullName);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            v.Owner = this;
            if (v.ShowDialog() == true)
            {

                TreeViewItem itemUnit = GetTreeViewItem(vm.UnitName, vm.dirUnit, 2, "images/plant.ico", data.dbPlantFile, null);
                tvi.Items.Add(itemUnit);

                TreeViewItem itemProtectSystem = GetTreeViewItem("ProtectedSystem1", vm.dirProtectedSystem, 3, "images/plant.ico", data.dbPlantFile, vm.dbProtectedSystemFile);
                itemUnit.Items.Add(itemProtectSystem);

                TreeViewItem itemProtectSystemfile = GetTreeViewItem("ProtectedSystem1", vm.visioProtectedSystem, 4, "images/project.ico", data.dbPlantFile, vm.dbProtectedSystemFile);
                itemProtectSystem.Items.Add(itemProtectSystemfile);

                tvi.ExpandSubtree();
            }

        }

        public void CreateProtectedSystem(object sender, RoutedEventArgs e)
        {
            if (NavigationTreeView.SelectedItem == null)
                return;
            TreeViewItem tvi = (TreeViewItem)NavigationTreeView.SelectedItem;
            TreeViewItemData data = tvi.Tag as TreeViewItemData;
            CreateProtectedSystemView v = new CreateProtectedSystemView();
            CreateProtectedSystemVM vm = new CreateProtectedSystemVM(data.FullName);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            v.Owner = this;
            if (v.ShowDialog() == true)
            {
                TreeViewItem itemProtectSystem = GetTreeViewItem(vm.ProtectedSystemName, vm.dirProtectedSystem, 3, "images/plant.ico", data.dbPlantFile, vm.dbProtectedSystemFile);
                tvi.Items.Add(itemProtectSystem);

                TreeViewItem itemProtectSystemfile = GetTreeViewItem(vm.ProtectedSystemName, vm.visioProtectedSystem, 4, "images/project.ico", data.dbPlantFile, vm.dbProtectedSystemFile);
                itemProtectSystem.Items.Add(itemProtectSystemfile);

                tvi.ExpandSubtree();
            }

        }

        public void ReName(object sender, RoutedEventArgs e)
        {
            if (NavigationTreeView.SelectedItem == null)
                return;
            TreeViewItem tvi = (TreeViewItem)NavigationTreeView.SelectedItem;
            TreeViewItemData data = tvi.Tag as TreeViewItemData;
            ReNameView v = new ReNameView();
            ReNameVM vm = new ReNameVM(data.Text, data.FullName, data.Type);
            v.DataContext = vm;
            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            v.Owner = this;
            if (v.ShowDialog() == true)
            {
                data.Text = vm.NewName;
                data.FullName = vm.NewDir;


                StackPanel SP = (StackPanel)tvi.Header;
                foreach (UIElement uie in SP.Children)
                {
                    if (uie.GetType().ToString() == "TextBlock")
                    {
                        TextBlock lbl = (TextBlock)uie;
                        lbl.Text = data.Text;
                    }
                }
                tvi.Header = SP;

            }

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
                    case "Global Default":
                        OpenGloadDefalut();
                        break;
                    case "Report":
                        OpenReport();
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

        }


    }
    public class ListViewItemData
    {
        public string Name { get; set; }
        public string Pic { get; set; }
    }


}
