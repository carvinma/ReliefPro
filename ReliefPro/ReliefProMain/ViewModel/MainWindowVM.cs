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
using ReliefProMain.Models;
using NHibernate;
using ReliefProMain.Commands;
using UOMLib;
using System.Threading.Tasks;
using System.Threading;
namespace ReliefProMain.ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        //版本信息
        string version;
        string defaultReliefProDir;
        string tempReliefProWorkDir;
        string currentPlantFile;
        string currentPlantName;
        string currentPlantWorkFolder;
        AxDrawingControl visioControl = new AxDrawingControl();
        private ObservableCollection<TVPlantViewModel> _Plants;
        public ObservableCollection<TVPlantViewModel> Plants
        {
            get { return _Plants; }
            set
            {
                _Plants = value;
                OnPropertyChanged("Plants");
            }
        }
        private bool _IsOpenPlant;
        public bool IsOpenPlant
        {
            get { return _IsOpenPlant; }
            set
            {
                _IsOpenPlant = value;
                OnPropertyChanged("IsOpenPlant");
            }
        }
        public ICommand NewPlantCommand { get; set; }
        public ICommand OpenPlantCommand { get; set; }

        public ICommand ClosePlantCommand { get; set; }

        private bool busy;
        public bool isBusy
        {
            get
            {
                return busy;
            }
            set
            {
                busy = value;
                this.NotifyPropertyChanged("isBusy");
            }
        }


        public MainWindowVM()
        {
            IsOpenPlant = false;
            NewPlantCommand = new DelegateCommand<object>(CreatePlant);

            OpenPlantCommand = new DelegateCommand<object>(OpenPlant);
            ClosePlantCommand = new DelegateCommand<object>(ClosePlant);

            _Plants = new ObservableCollection<TVPlantViewModel>();

            version = ConfigurationManager.AppSettings["version"];
            defaultReliefProDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + version;
            if (!Directory.Exists(defaultReliefProDir))
            {
                Directory.CreateDirectory(defaultReliefProDir);  //这是我的文档里的工作文件夹。[不需要删除]
            }
                
            tempReliefProWorkDir = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + version;
            if (!Directory.Exists(tempReliefProWorkDir))
            {
                Directory.CreateDirectory(tempReliefProWorkDir);//这是appdata里工作目录文件夹
            }
                

        }

        private void CreatePlant(object obj)
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
                    if (saveFileDialog1.FileName.Trim().Contains(" "))
                    {
                        MessageBox.Show("Plant Name could not contain space", "Message Box");
                        return;
                    }
                    
                    Task.Factory.StartNew(() =>
                       {
                           Application.Current.Dispatcher.Invoke(new Action(() =>
                           {
                               isBusy = true;
                           }), null);
                           Thread.Sleep(1000);
                       }).ContinueWith((t) =>
                       {
                           Application.Current.Dispatcher.Invoke(new Action(() =>
                          {
                              currentPlantFile = saveFileDialog1.FileName;
                              currentPlantName = System.IO.Path.GetFileNameWithoutExtension(currentPlantFile);
                              currentPlantWorkFolder = tempReliefProWorkDir + @"\" + currentPlantName;
                              if (Directory.Exists(currentPlantWorkFolder))
                              {
                                  try
                                  {
                                      Directory.Delete(currentPlantWorkFolder, true);
                                  }
                                  catch (Exception ex)
                                  {
                                      MessageBox.Show("The Plant Folder or one file of this folder is open,please close it.","Message Box");
                                      return;
                                  }
                              }
                              //IsOpenPlant = true;
                              Directory.CreateDirectory(currentPlantWorkFolder);
                              string dbPlant = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\plant.mdb";
                              string dbPlant_target = currentPlantWorkFolder + @"\plant.mdb";
                              System.IO.File.Copy(dbPlant, dbPlant_target, true);
                              NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbPlant_target);
                              ISession SessionPlant = helperProtectedSystem.GetCurrentSession();
                              CreateUnitView unitView = new CreateUnitView();
                              CreateUnitVM unitVM = new CreateUnitVM(SessionPlant, currentPlantWorkFolder);
                              unitView.DataContext = unitVM;
                              unitView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                              unitView.ShowDialog();
                              //string unit1 = currentPlantWorkFolder + @"\Unit1";
                              //Directory.CreateDirectory(unit1);
                              //string protectedsystem1 = unit1 + @"\ProtectedSystem1";
                              //Directory.CreateDirectory(protectedsystem1);
                              //string dbProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.mdb";
                              //string dbProtectedSystem_target = protectedsystem1 + @"\protectedsystem.mdb";
                              //System.IO.File.Copy(dbProtectedSystem, dbProtectedSystem_target, true);
                              //string visioProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.vsd";
                              //string visioProtectedSystem_target = protectedsystem1 + @"\design.vsd";
                              //System.IO.File.Copy(visioProtectedSystem, visioProtectedSystem_target, true);


                              //ReliefProCommon.CommonLib.CSharpZip.CompressZipFile(currentPlantWorkFolder, currentPlantFile);
                              
                              //TreeUnitDAL treeUnitDAL = new TreeUnitDAL();
                              //TreeUnit treeUnit = new TreeUnit();
                              //treeUnit.UnitName = "Unit1";
                              //treeUnitDAL.Add(treeUnit, SessionPlant);

                              //TreePSDAL treePSDAL = new TreePSDAL();
                              //TreePS treePS = new TreePS();
                              //treePS.PSName = "ProtectedSystem1";
                              //treePS.UnitID = treeUnit.ID;
                              //treePSDAL.Add(treePS, SessionPlant);

                              TVPlant p = new TVPlant();
                              p.FullPath = currentPlantWorkFolder;
                              p.FullRefPath = currentPlantFile;
                              p.Name = currentPlantName;
                              TVPlantViewModel m1 = new TVPlantViewModel(p);
                              Plants.Add(m1);
                              SavePlant();
                              IsOpenPlant = true;
                              if (!UOMSingle.UomEnums.Exists(uom => uom.SessionDBPath == dbPlant_target))
                              {
                                  UOMEnum uomEnum = new UOMEnum(SessionPlant);
                                  UOMSingle.UomEnums.Add(uomEnum);
                              }
                          }), null);
                       }).ContinueWith((t) =>
                       {
                           Application.Current.Dispatcher.Invoke(new Action(() =>
                           {
                               isBusy = false;
                           }), null);
                       });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


        private void OpenPlant(object obj)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlgOpenDiagram = new Microsoft.Win32.OpenFileDialog();
                dlgOpenDiagram.Filter = "Relief(*.ref) |*.ref";
                dlgOpenDiagram.InitialDirectory = defaultReliefProDir;
                if (dlgOpenDiagram.ShowDialog() == true)
                {

                    Task.Factory.StartNew(() =>
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                isBusy = true;
                            }), null);
                            Thread.Sleep(1000);
                        }).ContinueWith((t) =>
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                currentPlantFile = dlgOpenDiagram.FileName;
                                currentPlantName = System.IO.Path.GetFileNameWithoutExtension(currentPlantFile);
                                currentPlantWorkFolder = tempReliefProWorkDir + @"\" + currentPlantName;
                                if (IsSamePlantOpen(currentPlantName))
                                {
                                    MessageBox.Show("Same Plant is Opened!", "Message Box");
                                    return;
                                }
                                try
                                {
                                    if (Directory.Exists(currentPlantWorkFolder))
                                    {
                                        Directory.Delete(currentPlantWorkFolder, true);
                                    }
                                    Directory.CreateDirectory(currentPlantWorkFolder);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Current plant folder is opened,close it and try again.or plant Name is not legal.", "Message Box");
                                    return;
                                }

                                //ReliefProCommon.CommonLib.CSharpZip.ExtractZipFile(currentPlantFile, "1", currentPlantWorkFolder);
                                ZipFile.ExtractToDirectory(currentPlantFile, currentPlantWorkFolder);
                                string dbPlant_target = currentPlantWorkFolder + @"\plant.mdb";

                                TVPlant p = new TVPlant();
                                p.FullPath = currentPlantWorkFolder;
                                p.FullRefPath = currentPlantFile;
                                p.Name = currentPlantName;
                                TVPlantViewModel m1 = new TVPlantViewModel(p);
                                Plants.Add(m1);
                                IsOpenPlant = true;
                                if (!UOMSingle.UomEnums.Exists(uom => uom.SessionDBPath == dbPlant_target))
                                {
                                    NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbPlant_target);
                                    ISession SessionPlant = helperProtectedSystem.GetCurrentSession();
                                    UOMEnum uomEnum = new UOMEnum(SessionPlant);
                                    UOMSingle.UomEnums.Add(uomEnum);
                                }
                            }), null);

                        }).ContinueWith((t) =>
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                isBusy = false;
                            }), null);
                        });

                }
            }
            catch (Exception ex)
            {
            }
        }

        private bool IsSamePlantOpen(string PlantName)
        {
            bool b = false;
            foreach (TVPlantViewModel p in Plants)
            {
                if (p.Name.ToLower() == PlantName.ToLower().Trim())
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        private void SavePlant()
        {
            //ReliefProCommon.CommonLib.CSharpZip.CompressZipFile(currentPlantWorkFolder, currentPlantFile);
            File.Delete(currentPlantFile);
            ZipFile.CreateFromDirectory(currentPlantWorkFolder, currentPlantFile);
        }

        private void ClosePlant(object obj)
        {
            if (Plants.Count == 0)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are your sure clear all plants?", "Message Box", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Plants.Clear();
            }
        }


    }





}
