using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Windows;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.Models;
using UOMLib;
using NHibernate;
using ProII;
using ReliefProCommon.CommonLib;
using ReliefProDAL.GlobalDefault;
using ReliefProModel.GlobalDefault;
using System.Threading;
using System.Collections.ObjectModel;
using ReliefProCommon.Enum;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class PSVVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { set; get; }
        public string EqName { get; set; }
        public string EqType { get; set; }
        public string FileFullPath { get; set; }
       
        
        private string _ReflexDrumVisible;
        public string ReflexDrumVisible
        {
            get { return _ReflexDrumVisible; }
            set
            {
                _ReflexDrumVisible = value;
                OnPropertyChanged("ReflexDrumVisible");
            }
        }
        public ObservableCollection<string> ValveTypes { get; set; }
        public PSVModel CurrentModel { get; set; }

        public ObservableCollection<string> DischargeTos { get; set; }
        public ObservableCollection<string> Locations { get; set; }
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        public PSV psv;
        PSVDAL dbpsv = new PSVDAL();
        UOMLib.UOMEnum uomEnum;
        public ObservableCollection<string> GetValveTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Modulation");
            list.Add("Pop Action");
            list.Add("Rupture Disk");
            list.Add("Temperature Actuated");
            return list;
        }
        public ObservableCollection<string> GetDischargeTos()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            GlobalDefaultDAL gdDAL = new GlobalDefaultDAL();
            IList<FlareSystem> fs = gdDAL.GetFlareSystem(SessionPlant);
            foreach (FlareSystem m in fs)
            {
                list.Add(m.FlareName);
            }

            return list;
        }
        public ObservableCollection<string> GetLocations()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            if (EqType == "StorageTank")
            {
                CustomStreamDAL csdal = new CustomStreamDAL();
                IList<CustomStream> cslist = csdal.GetAllList(SessionProtectedSystem);
                foreach (CustomStream m in cslist)
                {
                    list.Add(m.StreamName);
                }
            }
            else
            {
                ProIIEqDataDAL gdDAL = new ProIIEqDataDAL();
                IList<ProIIEqData> fs = gdDAL.GetAllList(SessionPlant);
                foreach (ProIIEqData m in fs)
                {
                    list.Add(m.EqName);
                }
            }
            return list;
        }



        public PSVVM(string eqName, string eqType, SourceFile sourceFileInfo, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            EqName = eqName;
            EqType = eqType;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            ValveTypes = GetValveTypes();
            DischargeTos = GetDischargeTos();
            Locations = GetLocations();
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            
            if (eqType == "Tower")
            {
                ReflexDrumVisible = "Visible";
            }
            else
            {
                ReflexDrumVisible = "Hidden";
            }

            psv = dbpsv.GetModel(sessionProtectedSystem);
            if (psv == null)
            {
                psv = new PSV();
                psv.PSVName = "PSV1";
                psv.ValveNumber = 2;
                psv.Pressure_Color = ColorBorder.green.ToString();
                psv.ReliefPressureFactor_Color = ColorBorder.green.ToString();
                psv.DrumPressure_Color = ColorBorder.green.ToString();
            }
            CurrentModel = new PSVModel(psv);
            CurrentModel.ValveType = ValveTypes[0];
            if (DischargeTos.Count > 0)
            {
                CurrentModel.DischargeTo = DischargeTos[0];
            }

            if (eqType == "StorageTank")
            {
                CurrentModel.Location = Locations[0]; ;
            }
            else
            {
                CurrentModel.Location = eqName;
            }
            CurrentModel.CriticalPressureUnit = uomEnum.UserPressure;
            CurrentModel.CriticalTemperatureUnit = uomEnum.UserTemperature;
            CurrentModel.PSVPressureUnit = uomEnum.UserPressure;
            CurrentModel.DrumPressureUnit = uomEnum.UserPressure;
            ReadConvert();
        }
        
       

        private void ReadConvert()
        {
            CurrentModel.Pressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.PressureUnit, CurrentModel.Pressure);
            CurrentModel.CriticalPressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.CriticalPressureUnit, CurrentModel.CriticalPressure);
            CurrentModel.DrumPressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.DrumPressureUnit, CurrentModel.DrumPressure);
            CurrentModel.CriticalTemperature = UnitConvert.Convert(UOMEnum.Temperature, CurrentModel.CriticalTemperatureUnit, CurrentModel.CriticalTemperature);
        }
        private void WriteConvert()
        {
            CurrentModel.dbmodel.Pressure = UnitConvert.Convert(CurrentModel.PressureUnit,UOMEnum.Pressure,  CurrentModel.Pressure);
            CurrentModel.dbmodel.CriticalPressure = UnitConvert.Convert(CurrentModel.CriticalPressureUnit,UOMEnum.Pressure,  CurrentModel.CriticalPressure);
            CurrentModel.dbmodel.DrumPressure = UnitConvert.Convert(CurrentModel.DrumPressureUnit,UOMEnum.Pressure,  CurrentModel.DrumPressure);
            CurrentModel.dbmodel.CriticalTemperature = UnitConvert.Convert(CurrentModel.CriticalTemperatureUnit,UOMEnum.Temperature,  CurrentModel.CriticalTemperature);
        }

        private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(Save);

                }
                return _SaveCommand;
            }
        }

        private void Save(object window)
        {
            if (string.IsNullOrEmpty(CurrentModel.PSVName))
            {
                MessageBox.Show("PSV Name can't be empty.", "Message Box");
                return;
            }
            if (CurrentModel.Pressure <= 0)
            {
                MessageBox.Show("PSV Pressure can't be empty.", "Message Box");
                return;
            }
            if (CurrentModel.ReliefPressureFactor <= 0)
            {
                MessageBox.Show("Relief Pressure Factor can't be empty.", "Message Box");
                return;
            }
            if (string.IsNullOrEmpty(CurrentModel.Location))
            {
                MessageBox.Show("Location must be selected.", "Message Box");
                return;
            }
            if (string.IsNullOrEmpty(CurrentModel.DischargeTo))
            {
                MessageBox.Show("Discharge To can't be empty.", "Message Box");
                return;
            }
            if (string.IsNullOrEmpty(CurrentModel.ValveType))
            {
                MessageBox.Show("Valve Type can't be empty.", "Message Box");
                return;
            }
            if (CurrentModel.ValveNumber <= 0)
            {
                MessageBox.Show("Valve Number can't be zero.", "Message Box");
                return;
            }
            try
            {

                // this.IsBusy = true;
                //  var t1 = Task.Factory.StartNew(() =>
                //  {
                //Thread.Sleep(3000);

                if (CurrentModel.ID == 0)
                {
                    SplashScreenManager.Show();
                    SplashScreenManager.SentMsgToScreen("Creating PSV");

                    if (EqType == "Tower")
                    {
                        TowerDAL towerdal = new TowerDAL();
                        Tower tower = towerdal.GetModel(SessionProtectedSystem);
                        if (tower.TowerType == "Distillation")
                        {
                            CreateTowerPSV();
                        }
                    }
                    else
                    {
                        CreateCommonPSV();
                    }
                    CurrentModel.dbmodel.PSVName = CurrentModel.PSVName;
                    CurrentModel.dbmodel.Pressure = CurrentModel.Pressure;
                    CurrentModel.dbmodel.ReliefPressureFactor = CurrentModel.ReliefPressureFactor;
                    CurrentModel.dbmodel.ValveNumber = CurrentModel.ValveNumber;
                    CurrentModel.dbmodel.ValveType = CurrentModel.ValveType;
                    CurrentModel.dbmodel.DrumPSVName = CurrentModel.DrumPSVName;
                    CurrentModel.dbmodel.Location = CurrentModel.Location;
                    CurrentModel.dbmodel.DrumPressure = CurrentModel.DrumPressure;
                    CurrentModel.dbmodel.CriticalPressure = CurrentModel.CriticalPressure;
                    CurrentModel.dbmodel.Description = CurrentModel.Description;
                    CurrentModel.dbmodel.LocationDescription = CurrentModel.LocationDescription;
                    CurrentModel.dbmodel.DischargeTo = CurrentModel.DischargeTo;

                    CurrentModel.dbmodel.PSVName_Color = CurrentModel.PSVName_Color;
                    CurrentModel.dbmodel.Pressure_Color = CurrentModel.Pressure_Color;
                    CurrentModel.dbmodel.DrumPressure_Color = CurrentModel.DrumPressure_Color;
                    SplashScreenManager.SentMsgToScreen("Converting Unit");
                    WriteConvert();
                    SplashScreenManager.SentMsgToScreen("Saving Data");
                    dbpsv.Add(CurrentModel.dbmodel, SessionProtectedSystem);
                    SplashScreenManager.SentMsgToScreen("Calculation finished");
                }
                else if (psv.ReliefPressureFactor == CurrentModel.ReliefPressureFactor && psv.Pressure == CurrentModel.Pressure)
                {
                    SplashScreenManager.Show();
                    SplashScreenManager.SentMsgToScreen("Converting Unit");
                    WriteConvert();
                    SplashScreenManager.SentMsgToScreen("Saving Data");
                    dbpsv.Update(CurrentModel.dbmodel, SessionProtectedSystem);
                    SplashScreenManager.SentMsgToScreen("Calculation finished");
                    //SessionProtectedSystem.Flush();

                }
                else
                {
                    //需要重新复制一份ProtectedSystem.mdb，相当于重新分析。----error
                    //string dbProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.mdb";
                    //string dbProtectedSystem_target = DirProtectedSystem + @"\protectedsystem.mdb";
                    //System.IO.File.Copy(dbProtectedSystem, dbProtectedSystem_target, true);
                    //NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbProtectedSystem_target);
                    //SessionProtectedSystem = helperProtectedSystem.GetCurrentSession();

                    //应该是删除所有的和定压相关的表的数据
                    MessageBoxResult r = MessageBox.Show("Are you sure to edit data? it need to rerun all Scenario", "Message Box", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        SplashScreenManager.Show();
                        SplashScreenManager.SentMsgToScreen("Editing data");
                        ScenarioBLL scBLL = new ScenarioBLL(SessionProtectedSystem);
                        scBLL.DeleteSCOther();
                        scBLL.ClearScenario();
                        PSVBLL psvbll = new PSVBLL(SessionProtectedSystem);
                        psvbll.DeletePSVData();
                        SplashScreenManager.SentMsgToScreen("Creating PSV");
                        if (EqType == "Tower")
                        {
                            TowerDAL towerdal = new TowerDAL();
                            Tower tower = towerdal.GetModel(SessionProtectedSystem);
                            if (tower.TowerType == "Distillation")
                            {
                                CreateTowerPSV();
                            }
                        }
                        else
                        {
                            CreateCommonPSV();
                        }

                        SplashScreenManager.SentMsgToScreen("Converting Unit");
                        WriteConvert();
                        SplashScreenManager.SentMsgToScreen("Saving Data");
                        dbpsv.Add(CurrentModel.dbmodel, SessionProtectedSystem);
                        SplashScreenManager.SentMsgToScreen("Calculation finished");
                    }
                }
                //Thread.Sleep(3000);
                //}).ContinueWith((t) => { });

                //Task.WaitAll(t1);
                //this.IsBusy = false;

                System.Windows.Window wd = window as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                SplashScreenManager.Close();
            }

        }
        public void CreateTowerPSV()
        {
            //判断压力是否更改，relief pressure 是否更改。  （drum的是否修改，会影响到火灾的计算）
            string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
            string tempdir = DirProtectedSystem + @"\temp\";
            if (!Directory.Exists(tempdir))
                Directory.CreateDirectory(tempdir);

            string dirPhase = tempdir + "Phase";
            if (!Directory.Exists(dirPhase))
                Directory.CreateDirectory(dirPhase);
            string dirLatent = tempdir + "Latent";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string dirCopyStream = tempdir + "CopyStream";
            if (!Directory.Exists(dirCopyStream))
                Directory.CreateDirectory(dirCopyStream);

            string copyFile = dirCopyStream + @"\" + SourceFileInfo.FileName;
            File.Copy(FileFullPath, copyFile, true);
            CustomStream stream = CopyTop1Liquid(copyFile);
            double internPressure = UnitConvert.Convert("MPAG", "KPA", stream.Pressure);
            if (internPressure == 0)
            {
                MessageBox.Show("Please Rerun this ProII file and save it.", "Message Box");
                return;
            }
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);

            string phasecontent = PROIIFileOperator.getUsablePhaseContent(stream.StreamName, tempdir);
            double ReliefPressure = CurrentModel.ReliefPressureFactor * CurrentModel.Pressure;

            double criticalPressure = 0;
            double criticalTempressure = 0;
            bool b = CalcCriticalPressure(phasecontent, ReliefPressure, stream, dirPhase, ref criticalPressure, ref criticalTempressure);
            if (b == false)
                return;
            CurrentModel.CriticalPressure = criticalPressure;
            CurrentModel.CriticalTemperature = criticalTempressure;
            double latentEnthalpy = 0;
            double ReliefTemperature = 0;
            LatentProduct latentVapor = new LatentProduct();
            LatentProduct latentLiquid = new LatentProduct();
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            if (criticalPressure > ReliefPressure)
            {
                b = CalcLatent(content, ReliefPressure, stream, dirLatent, ref latentVapor, ref latentLiquid);
                if (b == false)
                    return;
                latentEnthalpy = latentVapor.SpEnthalpy - latentLiquid.SpEnthalpy;
                ReliefTemperature = latentVapor.Temperature;
            }
            else
            {
                latentEnthalpy = 116.3152;
                ReliefTemperature = stream.Temperature;
            }
            IList<CustomStream> products = null;
            CustomStreamDAL dbstream = new CustomStreamDAL();
            products = dbstream.GetAllList(SessionProtectedSystem, true);
            double overHeadPressure = GetTowerOverHeadPressure(products);

            int ImportResult = 0;
            int RunResult = 0;
            List<FlashResult> listFlashResult = new List<FlashResult>();
            int count = products.Count;
            for (int i = 1; i <= count; i++)
            {
                CustomStream p = products[i - 1];
                if (p.TotalMolarRate > 0)
                {
                    IFlashCalculate fc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    string gd = Guid.NewGuid().ToString();
                    string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                    string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                    string prodtype = p.ProdType;
                    int tray = p.Tray;
                    string streamname = p.StreamName;
                    double strPressure = p.Pressure;
                    double strTemperature = p.Temperature;
                    string f = string.Empty;


                    string dirflash = tempdir + p.StreamName;
                    if (!Directory.Exists(dirflash))
                        Directory.CreateDirectory(dirflash);
                    double prodpressure = 0;

                    prodpressure = p.Pressure;

                    string usablecontent = PROIIFileOperator.getUsableContent(p.StreamName, tempdir);

                    if (prodtype == "4" || (prodtype == "2" && tray == 1)) // 2个条件是等同含义，后者是有气有液
                    {
                        f = fc.Calculate(usablecontent, 1, ReliefPressure.ToString(), 4, "", p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
                    }

                    else if (prodtype == "6" || prodtype == "3") //3 气相  6 沉积水 
                    {
                        f = fc.Calculate(usablecontent, 2, ReliefTemperature.ToString(), 4, "", p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
                    }
                    else
                    {
                        double press = ReliefPressure + (p.Pressure - overHeadPressure);
                        f = fc.Calculate(usablecontent, 1, press.ToString(), 3, "", p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
                    }
                    if (ImportResult == 1 || ImportResult == 2)
                    {
                        if (RunResult == 1 || RunResult == 2)
                        {
                            FlashResult fr = new FlashResult();
                            fr.LiquidName = liquid;
                            fr.VaporName = vapor;
                            fr.StreamName = streamname;
                            fr.PrzFile = f;
                            fr.Tray = tray;
                            fr.ProdType = prodtype;
                            listFlashResult.Add(fr);
                        }
                        else
                        {
                            //MessageBoxResult r = MessageBox.Show("Prz file is error", "Message Box", MessageBoxButton.OKCancel);
                            //System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe", f);

                            //if (r == MessageBoxResult.Yes)
                            //{
                                FlashResult fr = new FlashResult();
                                fr.LiquidName = liquid;
                                fr.VaporName = vapor;
                                fr.StreamName = streamname;
                                fr.PrzFile = f;
                                fr.Tray = tray;
                                fr.ProdType = prodtype;
                                listFlashResult.Add(fr);
                            //}
                           // return;
                        }
                    }
                    else
                    {
                        //MessageBoxResult r = MessageBox.Show("inp file is error", "Message Box", MessageBoxButton.OKCancel);
                        //if (r == MessageBoxResult.Yes)
                        //{
                            FlashResult fr = new FlashResult();
                            fr.LiquidName = liquid;
                            fr.VaporName = vapor;
                            fr.StreamName = streamname;
                            fr.PrzFile = f;
                            fr.Tray = tray;
                            fr.ProdType = prodtype;
                            listFlashResult.Add(fr);
                        //}
                        //return;
                    }
                }
            }

            List<TowerFlashProduct> listFlashProduct = new List<TowerFlashProduct>();
            count = listFlashResult.Count;
            for (int i = 1; i <= count; i++)
            {
                TowerFlashProduct product = new TowerFlashProduct();
                FlashResult fr = listFlashResult[i - 1];
                CustomStream cs = null;
                string prodtype = fr.ProdType;
                int tray = fr.Tray;
                if (fr.PrzFile == "")
                {
                    cs = dbstream.GetModel(SessionProtectedSystem, fr.StreamName);
                }
                else
                {                   
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(fr.PrzFile);
                    
                    if (prodtype == "4" || (prodtype == "2" && tray == 1) || prodtype == "3" || prodtype == "6")
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
                }
                
                product.SpEnthalpy = cs.SpEnthalpy;
                product.StreamName = fr.StreamName;
                product.WeightFlow = cs.WeightFlow;
                product.ProdType = fr.ProdType;
                product.Tray = tray;
                product.Temperature = cs.Temperature;

                listFlashProduct.Add(product);

            }



            LatentDAL dblatent = new LatentDAL();
            //dbLatentProduct dblatentproduct = new dbLatentProduct();
            TowerFlashProductDAL dbFlashProduct = new TowerFlashProductDAL();

            Latent latent = new Latent();
            latent.LatentEnthalpy = latentEnthalpy;
            latent.ReliefTemperature = ReliefTemperature;
            latent.ReliefOHWeightFlow = latentVapor.BulkMwOfPhase;
            latent.ReliefPressure = CurrentModel.Pressure * CurrentModel.ReliefPressureFactor;
            latent.ReliefCpCv = latentVapor.BulkCPCVRatio;
            latent.ReliefZ = latentVapor.VaporZFmKVal;
            dblatent.Add(latent, SessionProtectedSystem);

            foreach (TowerFlashProduct p in listFlashProduct)
            {
                dbFlashProduct.Add(p, SessionProtectedSystem);
            }


        }

        public void CreateCommonPSV()
        {
            //判断压力是否更改，relief pressure 是否更改。  （drum的是否修改，会影响到火灾的计算）
            string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
            string tempdir = DirProtectedSystem + @"\temp\";
            if (!Directory.Exists(tempdir))
                Directory.CreateDirectory(tempdir);

            string dirPhase = tempdir + "Phase";
            if (!Directory.Exists(dirPhase))
                Directory.CreateDirectory(dirPhase);

            CustomStreamDAL csdal = new CustomStreamDAL();
            IList<CustomStream> feedlist = csdal.GetAllList(SessionProtectedSystem, false);
            CustomStream stream = feedlist[0];
            stream.TotalMolarRate = 0;
            foreach (CustomStream cs in feedlist)
            {
                stream.TotalMolarRate = stream.TotalMolarRate + cs.TotalMolarRate;
            }
            double internPressure = UnitConvert.Convert("MPAG", "KPA", stream.Pressure);
            if (internPressure == 0)
            {
                MessageBox.Show("Please Rerun this ProII file and save it.", "Message Box");
                return;
            }
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);

            string phasecontent = PROIIFileOperator.getUsablePhaseContent(stream.StreamName, tempdir);
            double ReliefPressure = CurrentModel.ReliefPressureFactor * CurrentModel.Pressure;

            double criticalPressure = 0;
            double criticalTempressure = 0;
            bool b = CalcCriticalPressure(phasecontent, ReliefPressure, stream, dirPhase, ref criticalPressure, ref criticalTempressure);
            if (b == false)
                return;
            CurrentModel.CriticalPressure = criticalPressure;
            CurrentModel.CriticalTemperature = criticalTempressure;
        }

        private CustomStream CopyTop1Liquid(string copyPrzFile)
        {
            CustomStream cs = new CustomStream();
            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
            reader.InitProIIReader(copyPrzFile);
            ProIIStreamData proIITray1StreamData = reader.CopyStream(EqName, 1, 2, 1);
            reader.ReleaseProIIReader();
            cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIITray1StreamData);
            return cs;
        }


        private double GetTowerOverHeadPressure(IList<CustomStream> products)
        {
            double overHeadPressure = 0;
            foreach (CustomStream cs in products)
            {
                if (cs.ProdType == "3" || cs.ProdType == "4")
                {
                    overHeadPressure = cs.Pressure;
                    break;
                }
            }
            return overHeadPressure;

        }

        private bool CalcCriticalPressure(string content, double ReliefPressure, CustomStream stream, string dirPhase, ref double criticalPressure,ref double criticalTemperature)
        {
            int ImportResult = 0;
            int RunResult = 0;
            criticalPressure = 0;
            criticalTemperature = 0;
            IPHASECalculate PhaseCalc = ProIIFactory.CreatePHASECalculate(SourceFileInfo.FileVersion);
            string PH = "PH" + Guid.NewGuid().ToString().Substring(0, 4);
            string criticalPress = string.Empty;
            string criticalTemp = string.Empty;
            string phasef = PhaseCalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH, dirPhase, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(phasef);
                    criticalPress = reader.GetCriticalPressure(PH);
                    criticalTemp = reader.GetCriticalTemperature(PH);
                    reader.ReleaseProIIReader();
                    criticalPressure = UnitConvert.Convert("KPA", CurrentModel.CriticalPressureUnit, double.Parse(criticalPress));

                    criticalTemperature = UnitConvert.Convert("C", CurrentModel.CriticalTemperatureUnit, double.Parse(criticalTemp));
                    return true;
                }
                else
                {
                    MessageBox.Show("Prz file is error", "Message Box");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
                return false;
            }

        }

        private bool CalcLatent(string content, double ReliefPressure, CustomStream stream, string dirLatent, ref LatentProduct latentVapor, ref LatentProduct latentLiquid)
        {
            LatentProductDAL dblp = new LatentProductDAL();
            int ImportResult = 0;
            int RunResult = 0;
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    latentVapor = ProIIToDefault.ConvertProIIStreamToLatentProduct(proIIVapor);
                    latentVapor.ProdType = "1";
                    latentLiquid = ProIIToDefault.ConvertProIIStreamToLatentProduct(proIILiquid);
                    latentVapor.ProdType = "2";

                    LatentProduct latentStream = ProIIToDefault.ConvertCustomStreamToLatentProduct(stream);
                    latentStream.ProdType = "-1";
                    dblp.Add(latentStream, SessionProtectedSystem);
                    dblp.Add(latentVapor, SessionProtectedSystem);
                    dblp.Add(latentLiquid, SessionProtectedSystem);
                    return true;
                }

                else
                {
                    MessageBox.Show("Prz file is error", "Message Box");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
                return false;
            }
        }


        

    }
}
