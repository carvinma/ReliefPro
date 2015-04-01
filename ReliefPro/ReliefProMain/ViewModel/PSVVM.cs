/*该文件是psv界面对应的VM
 * 该文件极其重要，在这里要计算临界压力
 * 对于蒸馏塔，其他2种类型塔的处理是不同的。
 * 对于hx设备也是不同的。
 * 对于反应循环 无需任何计算操作。只是保存psv信息
 * 
 * 进度条问题，等计算完之后，再抛出异常问题。
 * 
 * 
 * 1 对于塔是多条路径的，需要记录下来最后提示下用户
 * 
 * 
 * 
*/
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
using ReliefProDAL.HXs;
using ReliefProModel.HXs;
using ReliefProMain.Util;
using ReliefProDAL.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProMain.View.Towers;

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
        double criticalPressure = 0;
        double criticalTemperature = 0;
        double cricondenbarPressure = 0;
        double cricondenbarTemperature = 0;
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

        private ICommand _LatentEnthalpyCommand;
        public ICommand LatentEnthalpyCommand
        {
            get
            {
                if (_LatentEnthalpyCommand == null)
                {
                    _LatentEnthalpyCommand = new RelayCommand(CalcLatentEnthalpy);

                }
                return _LatentEnthalpyCommand;
            }
        }


        public PSV psv;
        UOMLib.UOMEnum uomEnum;
        PSVDAL dbpsv = new PSVDAL();
        Latent latent = new Latent();
        string HeatMethod = string.Empty;

        int ErrorType = 0;//0 无错误 1 临界
        double CriticalLatent = 0;

        public ObservableCollection<string> GetValveTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Conventional");
            list.Add("Balanced");
            list.Add("Pilot Operated");
            list.Add("Rupture Disk");
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

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="eqName">设备名称</param>
        /// <param name="eqType">类型</param>
        /// <param name="sourceFileInfo">源文件信息</param>
        /// <param name="sessionPlant"></param>
        /// <param name="sessionProtectedSystem"></param>
        /// <param name="dirPlant"></param>
        /// <param name="dirProtectedSystem"></param>
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
            GlobalDefaultBLL globalbll = new GlobalDefaultBLL(SessionPlant);
            ConditionsSettings settings = globalbll.GetConditionsSettings();
            CriticalLatent = settings.LatentHeatSettings;

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
                psv.PSVName = "";
                psv.ValveNumber = 1;
                psv.PSVName_Color = ColorBorder.red.ToString();
                psv.ValveNumber_Color = ColorBorder.green.ToString();
                psv.Pressure_Color = ColorBorder.red.ToString();
                psv.ReliefPressureFactor_Color = ColorBorder.green.ToString();
                psv.DrumPressure_Color = ColorBorder.green.ToString();
                psv.DrumPressure_Color = ColorBorder.green.ToString();
                psv.DischargeTo_Color = ColorBorder.green.ToString();
                psv.ValveType_Color = ColorBorder.green.ToString();
                psv.Location_Color = ColorBorder.green.ToString();
                psv.LocationDescription_Color = ColorBorder.red.ToString();
            }
            CurrentModel = new PSVModel(psv, sessionPlant, sourceFileInfo.FileName);
            CurrentModel.LocationDescriptions = GetLocationDescriptions(EqType);
            if (psv.ID == 0)
            {
                CurrentModel.ValveType = ValveTypes[0];
                CurrentModel.DischargeTo = DischargeTos[0];
                if (CurrentModel.LocationDescriptions != null)
                {
                    CurrentModel.LocationDescription = CurrentModel.LocationDescriptions[0];
                }
                CurrentModel.Location = eqName;
                if (eqType == "ReactorLoop")
                {
                    if (string.IsNullOrEmpty(eqName))
                    {
                        ReactorLoopDAL reactorLoopDAL = new ReactorLoopDAL();
                        ReactorLoop reactor = reactorLoopDAL.GetModel(SessionProtectedSystem);
                        CurrentModel.Location = reactor.ColdHighPressureSeparator;
                        EqName = reactor.ColdHighPressureSeparator;
                    }

                }
            }
            CurrentModel.CriticalPressureUnit = uomEnum.UserPressure;
            CurrentModel.CriticalTemperatureUnit = uomEnum.UserTemperature;
            CurrentModel.PSVPressureUnit = uomEnum.UserPressure;
            CurrentModel.DrumPressureUnit = uomEnum.UserPressure;
            ReadConvert();
        }

        public ObservableCollection<string> GetLocationDescriptions(string eqType)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            if (eqType == "HX")
            {
                HeatExchangerDAL heatExchangerDAL = new HeatExchangerDAL();
                HeatExchanger heatExchanger = heatExchangerDAL.GetModel(SessionProtectedSystem);

                if (heatExchanger.HXType == "Shell-Tube")
                {
                    list.Add("Shell");
                }
                list.Add("Tube");
            }
            else if (eqType == "AirCooledHX")
            {
                list.Add("Tube");
            }
            else
            {
                list.Add("Top");
                list.Add("Bottom");
            }
            return list;
        }

        /// <summary>
        /// 读取时的单位转换
        /// </summary>
        private void ReadConvert()
        {
            CurrentModel.Pressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.PSVPressureUnit, CurrentModel.Pressure);
            CurrentModel.CriticalPressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.CriticalPressureUnit, CurrentModel.CriticalPressure);
            CurrentModel.DrumPressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.DrumPressureUnit, CurrentModel.DrumPressure);
            CurrentModel.CriticalTemperature = UnitConvert.Convert(UOMEnum.Temperature, CurrentModel.CriticalTemperatureUnit, CurrentModel.CriticalTemperature);

            CurrentModel.CricondenbarPress = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.CricondenbarPressUnit, CurrentModel.CricondenbarPress);
            CurrentModel.CricondenbarTemp = UnitConvert.Convert(UOMEnum.Temperature, CurrentModel.CricondenbarTempUnit, CurrentModel.CricondenbarTemp);
        }

        /// <summary>
        /// 写入数据库时的单位转换
        /// </summary>
        private void WriteConvert()
        {
            CurrentModel.dbmodel.Pressure = UnitConvert.Convert(CurrentModel.PSVPressureUnit, UOMEnum.Pressure, CurrentModel.Pressure);
            CurrentModel.dbmodel.CriticalPressure = UnitConvert.Convert(CurrentModel.CriticalPressureUnit, UOMEnum.Pressure, CurrentModel.CriticalPressure);
            CurrentModel.dbmodel.DrumPressure = UnitConvert.Convert(CurrentModel.DrumPressureUnit, UOMEnum.Pressure, CurrentModel.DrumPressure);
            CurrentModel.dbmodel.CriticalTemperature = UnitConvert.Convert(CurrentModel.CriticalTemperatureUnit, UOMEnum.Temperature, CurrentModel.CriticalTemperature);
            CurrentModel.dbmodel.CricondenbarPress = UnitConvert.Convert(CurrentModel.CricondenbarPressUnit, UOMEnum.Pressure, CurrentModel.CricondenbarPress);
            CurrentModel.dbmodel.CricondenbarTemp = UnitConvert.Convert(CurrentModel.CricondenbarTempUnit, UOMEnum.Temperature, CurrentModel.CricondenbarTemp);
            CurrentModel.dbmodel.ValveNumber = CurrentModel.ValveNumber;
            CurrentModel.dbmodel.Description = CurrentModel.Description;
            CurrentModel.dbmodel.LocationDescription = CurrentModel.LocationDescription;
            CurrentModel.dbmodel.Location = CurrentModel.Location;
            CurrentModel.dbmodel.DischargeTo = CurrentModel.DischargeTo;
            CurrentModel.dbmodel.DischargeTo_Color = CurrentModel.DischargeTo_Color;
            CurrentModel.dbmodel.ValveType = CurrentModel.ValveType;
            CurrentModel.dbmodel.ValveType_Color = CurrentModel.ValveType_Color;
            CurrentModel.dbmodel.PSVName = CurrentModel.PSVName;
            CurrentModel.dbmodel.PSVName_Color = CurrentModel.PSVName_Color;
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
                MessageBox.Show("PSV Name can't be empty.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                CurrentModel.PSVName_Color = ColorBorder.red.ToString();
                return;
            }
            double pressure = UnitConvert.Convert(CurrentModel.PSVPressureUnit, "Kpa", CurrentModel.Pressure);
            if (pressure <= 0)
            {
                MessageBox.Show("PSV Pressure can't be empty.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                CurrentModel.Pressure_Color = ColorBorder.red.ToString();
                return;
            }
            if (CurrentModel.ReliefPressureFactor <= 0)
            {
                MessageBox.Show("Relief Pressure Factor can't be empty.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(CurrentModel.Location))
            {
                MessageBox.Show("Location must be selected.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(CurrentModel.DischargeTo))
            {
                MessageBox.Show("Discharge To can't be empty.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(CurrentModel.ValveType))
            {
                MessageBox.Show("Valve Type can't be empty.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CurrentModel.ValveNumber <= 0)
            {
                MessageBox.Show("Valve Number can't be zero.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    SplashScreenManager.Show(10);
                    SplashScreenManager.SentMsgToScreen("Creating PSV... 10%");

                    if (EqType == "Tower")
                    {
                        TowerDAL towerdal = new TowerDAL();
                        Tower tower = towerdal.GetModel(SessionProtectedSystem);
                        if (tower.TowerType == "Distillation" || tower.TowerType == "Absorbent Regenerator")
                        {
                            CreateTowerPSV();
                        }
                    }
                    else if (EqType == "ReactorLoop")
                    {
                    }
                    else
                    {
                        CreateCommonPSV();
                    }
                    CurrentModel.dbmodel.PSVName = CurrentModel.PSVName;
                    CurrentModel.dbmodel.Pressure = UnitConvert.Convert(CurrentModel.PSVPressureUnit, UOMEnum.Pressure, CurrentModel.Pressure);
                    CurrentModel.dbmodel.ReliefPressureFactor = CurrentModel.ReliefPressureFactor;
                    CurrentModel.dbmodel.ValveNumber = CurrentModel.ValveNumber;
                    CurrentModel.dbmodel.ValveType = CurrentModel.ValveType;
                    CurrentModel.dbmodel.DrumPSVName = CurrentModel.DrumPSVName;
                    CurrentModel.dbmodel.Location = CurrentModel.Location;
                    //CurrentModel.dbmodel.DrumPressure = UnitConvert.Convert(CurrentModel.DrumPressureUnit,UOMEnum.Pressure,CurrentModel.DrumPressure);
                    //CurrentModel.dbmodel.CriticalPressure = UnitConvert.Convert(CurrentModel.CriticalPressureUnit,UOMEnum.Pressure,CurrentModel.CriticalPressure);
                    //CurrentModel.dbmodel.CriticalTemperature = UnitConvert.Convert(CurrentModel.CriticalTemperatureUnit,UOMEnum.Temperature,CurrentModel.CriticalTemperature);
                    //CurrentModel.dbmodel.CricondenbarTemp = UnitConvert.Convert(CurrentModel.CricondenbarTempUnit, UOMEnum.Temperature, CurrentModel.CricondenbarTemp);
                    //CurrentModel.dbmodel.CricondenbarPress = UnitConvert.Convert(CurrentModel.CricondenbarPressUnit,UOMEnum.Pressure,CurrentModel.CricondenbarPress);
                    //CurrentModel.dbmodel.Description = CurrentModel.Description;
                    //CurrentModel.dbmodel.LocationDescription = CurrentModel.LocationDescription;
                    CurrentModel.dbmodel.DischargeTo = CurrentModel.DischargeTo;

                    CurrentModel.dbmodel.PSVName_Color = CurrentModel.PSVName_Color;
                    CurrentModel.dbmodel.Pressure_Color = CurrentModel.Pressure_Color;
                    CurrentModel.dbmodel.DrumPressure_Color = CurrentModel.DrumPressure_Color;
                    CurrentModel.dbmodel.DischargeTo_Color = CurrentModel.DischargeTo_Color;
                    CurrentModel.dbmodel.ReliefPressureFactor_Color = CurrentModel.ReliefPressureFactor_Color;
                    CurrentModel.dbmodel.ValveNumber_Color = CurrentModel.ValveNumber_Color;
                    CurrentModel.dbmodel.ValveType_Color = CurrentModel.ValveType_Color;
                    CurrentModel.dbmodel.Location_Color = CurrentModel.Location_Color;
                    CurrentModel.dbmodel.DrumPSVName_Color = CurrentModel.DrumPSVName_Color;

                    SplashScreenManager.SentMsgToScreen("Converting Unit... 70%");
                    WriteConvert();
                    SplashScreenManager.SentMsgToScreen("Saving Data... 80%");
                    dbpsv.Add(CurrentModel.dbmodel, SessionProtectedSystem);
                    SplashScreenManager.SentMsgToScreen("Calculation finished...100%");
                }
                else if (psv.ReliefPressureFactor == CurrentModel.ReliefPressureFactor && psv.Pressure == UnitConvert.Convert(CurrentModel.PSVPressureUnit, UOMEnum.Pressure, CurrentModel.Pressure) && psv.LocationDescription == CurrentModel.LocationDescription)
                {
                    SplashScreenManager.Show(2);
                    WriteConvert();
                    SplashScreenManager.SentMsgToScreen("Saving Data... 50%");
                    dbpsv.Update(CurrentModel.dbmodel, SessionProtectedSystem);
                    SplashScreenManager.SentMsgToScreen("Calculation finished...100%");
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
                    MessageBoxResult r = MessageBox.Show("Are you sure to edit data? ", "Message Box", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (r == MessageBoxResult.Yes)
                    {
                        SplashScreenManager.Show(10);
                        SplashScreenManager.SentMsgToScreen("Editing data ... 10%");
                        ScenarioBLL scBLL = new ScenarioBLL(SessionProtectedSystem);
                        scBLL.DeleteSCOther();
                        scBLL.ClearScenario();

                        PSVBLL psvbll = new PSVBLL(SessionProtectedSystem);
                        psvbll.DeletePSVData();

                        if (EqType == "Tower")
                        {
                            TowerDAL towerdal = new TowerDAL();
                            Tower tower = towerdal.GetModel(SessionProtectedSystem);
                            if (tower.TowerType == "Distillation")
                            {
                                CreateTowerPSV();
                            }
                        }
                        else if (EqType == "ReactorLoop")
                        {
                        }
                        else
                        {
                            CreateCommonPSV();
                        }
                        SplashScreenManager.SentMsgToScreen("Converting Unit... 90%");
                        CurrentModel.dbmodel.PSVName = CurrentModel.PSVName;
                        CurrentModel.dbmodel.Pressure = UnitConvert.Convert(CurrentModel.PSVPressureUnit, UOMEnum.Pressure, CurrentModel.Pressure);
                        CurrentModel.dbmodel.ReliefPressureFactor = CurrentModel.ReliefPressureFactor;
                        CurrentModel.dbmodel.ValveNumber = CurrentModel.ValveNumber;
                        CurrentModel.dbmodel.ValveType = CurrentModel.ValveType;
                        CurrentModel.dbmodel.DrumPSVName = CurrentModel.DrumPSVName;
                        CurrentModel.dbmodel.Location = CurrentModel.Location;
                        //CurrentModel.dbmodel.DrumPressure = CurrentModel.DrumPressure;
                        //CurrentModel.dbmodel.CriticalPressure = CurrentModel.CriticalPressure;
                        //CurrentModel.dbmodel.CriticalTemperature = CurrentModel.CriticalTemperature;
                        //CurrentModel.dbmodel.CricondenbarTemp = CurrentModel.CricondenbarTemp;
                        //CurrentModel.dbmodel.CricondenbarPress = CurrentModel.CricondenbarPress;
                        //CurrentModel.dbmodel.Description = CurrentModel.Description;
                        //CurrentModel.dbmodel.LocationDescription = CurrentModel.LocationDescription;
                        //CurrentModel.dbmodel.DischargeTo = CurrentModel.DischargeTo;

                        CurrentModel.dbmodel.PSVName_Color = CurrentModel.PSVName_Color;
                        CurrentModel.dbmodel.Pressure_Color = CurrentModel.Pressure_Color;
                        CurrentModel.dbmodel.DrumPressure_Color = CurrentModel.DrumPressure_Color;
                        CurrentModel.dbmodel.DischargeTo_Color = CurrentModel.DischargeTo_Color;
                        CurrentModel.dbmodel.ReliefPressureFactor_Color = CurrentModel.ReliefPressureFactor_Color;
                        CurrentModel.dbmodel.ValveNumber_Color = CurrentModel.ValveNumber_Color;
                        CurrentModel.dbmodel.ValveType_Color = CurrentModel.ValveType_Color;
                        CurrentModel.dbmodel.Location_Color = CurrentModel.Location_Color;
                        CurrentModel.dbmodel.DrumPSVName_Color = CurrentModel.DrumPSVName_Color;


                        WriteConvert();
                        SplashScreenManager.SentMsgToScreen("Calculation finished... 100%");
                        dbpsv.Add(CurrentModel.dbmodel, SessionProtectedSystem);

                    }
                }

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
                switch (ErrorType)
                {
                    case -1:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningTray1LiquidZero"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 1:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower1"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 2:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower2"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 3:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower3"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 4:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower4"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;


                }


            }

        }

        public void CreateTowerPSV()
        {
            //判断压力是否更改，relief pressure 是否更改。  （drum的是否修改，会影响到火灾的计算）
            string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
            string tempdir = DirProtectedSystem + @"\temp\";
            if (Directory.Exists(tempdir))
            {
                Directory.Delete(tempdir, true);
            }
            Directory.CreateDirectory(tempdir);

            string dirPhase = tempdir + "Phase";
            if (Directory.Exists(dirPhase))
            {
                Directory.Delete(dirPhase, true);
            }
            Directory.CreateDirectory(dirPhase);
            string dirLatent = tempdir + "Latent";
            if (Directory.Exists(dirLatent))
            {
                Directory.Delete(dirLatent, true);
            }
            Directory.CreateDirectory(dirLatent);
            string dirCopyStream = tempdir + "CopyStream";
            if (Directory.Exists(dirCopyStream))
            {
                Directory.Delete(dirCopyStream, true);
            }
            Directory.CreateDirectory(dirCopyStream);

            SplashScreenManager.SentMsgToScreen("Creating PSV... 20%");

            string copyFile = dirCopyStream + @"\" + SourceFileInfo.FileName;
            File.Copy(FileFullPath, copyFile, true);
            CustomStream stream = CopyTop1Liquid(copyFile);

            SplashScreenManager.SentMsgToScreen("Creating PSV... 30%");
            double internPressure = UnitConvert.Convert("MPAG", "KPA", stream.Pressure);
            if (internPressure == 0)
            {
                ErrorType = -1;
                return;
            }

            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string onlyfilenamenoext = new FileInfo(FileFullPath).Name.Replace(".prz", "");
            string[] sourceFiles = Directory.GetFiles(tempdir, onlyfilenamenoext + "*.inp");
            string sourceFile = sourceFiles[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            HeatMethod = ProIIMethod.GetHeatMethod(lines, EqName);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, lines);
            string phasecontent = PROIIFileOperator.getUsablePhaseContent(stream.StreamName, tempdir);
            double ReliefPressure = CurrentModel.ReliefPressureFactor * UnitConvert.Convert(CurrentModel.PSVPressureUnit, UOMEnum.Pressure, CurrentModel.Pressure);

            SplashScreenManager.SentMsgToScreen("Calculating critical pressure... 40%");
            double ReliefTemperature = 0;
            LatentProduct latentVapor = new LatentProduct();
            LatentProduct latentLiquid = new LatentProduct();

            bool b = CalcCriticalPressure(phasecontent, ReliefPressure, stream, dirPhase);
            SplashScreenManager.SentMsgToScreen("Calculating latent... 50%");
            if (b) //可以计算出临界压力
            {
                CurrentModel.CriticalPressure = criticalPressure;
                CurrentModel.CriticalTemperature = criticalTemperature;
                CurrentModel.CricondenbarPress = cricondenbarPressure;
                CurrentModel.CricondenbarTemp = cricondenbarTemperature;

                if (ReliefPressure < criticalPressure)
                {
                    //进入3，4，5，6路径  fluidtype这是是2 
                    CalcLatent(content, ReliefPressure, stream, dirLatent, 2, ref latentVapor, ref latentLiquid);
                }
                else
                {
                    //suppercritical--- flash @prelief,trelief  //进入流程图1或2的路径
                    string dirSupper = tempdir + "Supper";
                    if (Directory.Exists(dirSupper))
                    {
                        Directory.Delete(dirSupper, true);
                    }
                    Directory.CreateDirectory(dirSupper);
                    int supperResult = CalSupperCritical(content, ReliefPressure, criticalTemperature, stream, dirSupper);
                    if (supperResult == 1 || supperResult == 2)
                    {

                    }
                }

            }
            else
            {
                //计算不出临界压力 进入4，5，7
                //warning1 
                ErrorType = 1;
                CalcLatent(content, ReliefPressure, stream, dirLatent, 3, ref latentVapor, ref latentLiquid);

            }
            SplashScreenManager.SentMsgToScreen("Calculating product data... 60%");

            ReliefTemperature = latent.ReliefTemperature;
            IList<CustomStream> products = null;
            CustomStreamDAL dbstream = new CustomStreamDAL();
            products = dbstream.GetAllList(SessionProtectedSystem, true);
            double overHeadPressure = GetTowerOverHeadPressure(products);


            List<FlashResult> listFlashResult = new List<FlashResult>();
            //List<FlashResult> listFlashResult1 = new List<FlashResult>();
            //List<FlashResult> listFlashResult2 = new List<FlashResult>();
            //List<FlashResult> listFlashResult3 = new List<FlashResult>();
            //List<FlashResult> listFlashResult4 = new List<FlashResult>();
            int count = products.Count;
            //int avgct = count / 4;
            Task<List<FlashResult>> task1 = new Task<List<FlashResult>>(() => RunProductFlashCalc(products, 1, count, tempdir, ReliefPressure, ReliefTemperature, overHeadPressure));
            //Task<List<FlashResult>> task2 = new Task<List<FlashResult>>(() => RunProductFlashCalc(products, avgct + 1, 2 * avgct, tempdir, ReliefPressure, ReliefTemperature, overHeadPressure));
            //Task<List<FlashResult>> task3 = new Task<List<FlashResult>>(() => RunProductFlashCalc(products, 2 * avgct + 1, 3 * avgct, tempdir, ReliefPressure, ReliefTemperature, overHeadPressure));
            //Task<List<FlashResult>> task4 = new Task<List<FlashResult>>(() => RunProductFlashCalc(products, 3 * avgct + 1, count, tempdir, ReliefPressure, ReliefTemperature, overHeadPressure));

            task1.Start();
            //task2.Start();
            //task3.Start();
            //task4.Start();
            //Task.WaitAll(task1, task2, task3, task4);
            Task.WaitAll(task1);
            listFlashResult = listFlashResult.Union(task1.Result).ToList();
            //listFlashResult = listFlashResult.Union(task2.Result).ToList();
            //listFlashResult = listFlashResult.Union(task3.Result).ToList();
            //listFlashResult = listFlashResult.Union(task4.Result).ToList();
            SplashScreenManager.SentMsgToScreen("Converting product data... 70%");
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
                    //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    //reader.InitProIIReader(fr.PrzFile);
                    ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, fr.PrzFile);
                    if (prodtype == "4" || (prodtype == "2" && tray == 1) || prodtype == "3" || prodtype == "6")
                    {
                        ProIIStreamData data = reader.GetStreamInfo(fr.VaporName);
                        cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
                    }
                    else
                    {
                        ProIIStreamData data = reader.GetStreamInfo(fr.LiquidName);
                        cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);

                    }
                    //reader.ReleaseProIIReader();
                }

                product.SpEnthalpy = cs.SpEnthalpy;
                product.StreamName = fr.StreamName;
                product.WeightFlow = cs.WeightFlow;
                product.ProdType = fr.ProdType;
                product.Tray = tray;
                product.Temperature = cs.Temperature;

                listFlashProduct.Add(product);

            }
            SplashScreenManager.SentMsgToScreen("Saving product data... 80%");
            TowerFlashProductDAL dbFlashProduct = new TowerFlashProductDAL();
            foreach (TowerFlashProduct p in listFlashProduct)
            {
                dbFlashProduct.Add(p, SessionProtectedSystem);
            }


        }

        private List<FlashResult> RunProductFlashCalc(IList<CustomStream> products, int start, int end, string tempdir, double ReliefPressure, double ReliefTemperature, double overHeadPressure)
        {
            List<FlashResult> list = new List<FlashResult>();
            int ImportResult = 0;
            int RunResult = 0;
            for (int i = start; i <= end; i++)
            {
                CustomStream p = products[i - 1];
                if (p.TotalMolarRate > 0)
                {
                    //IFlashCalculate fc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    ProIICalculate proiicalc = new ProIICalculate(SourceFileInfo.FileVersion);
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
                    if (Directory.Exists(dirflash))
                    {
                        Directory.Delete(dirflash, true);
                    }
                    Directory.CreateDirectory(dirflash);
                    double prodpressure = 0;

                    prodpressure = p.Pressure;

                    string usablecontent = PROIIFileOperator.getUsableContent(p.StreamName, tempdir);

                    if (prodtype == "4" || (prodtype == "2" && tray == 1)) // 2个条件是等同含义，后者是有气有液
                    {
                        f = proiicalc.FlashCalculate(usablecontent, 1, ReliefPressure.ToString(), 4, "", HeatMethod, p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
                    }

                    else if (prodtype == "6" || prodtype == "3") //3 气相  6 沉积水 
                    {
                        f = proiicalc.FlashCalculate(usablecontent, 2, ReliefTemperature.ToString(), 4, "", HeatMethod, p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
                    }
                    else
                    {
                        double press = ReliefPressure + (p.Pressure - overHeadPressure);
                        f = proiicalc.FlashCalculate(usablecontent, 1, press.ToString(), 3, "", HeatMethod, p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
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
                            list.Add(fr);
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
                            list.Add(fr);
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
                        list.Add(fr);
                        //}
                        //return;
                    }
                }
            }
            return list;
        }

        public void CreateCommonPSV()
        {
            //判断压力是否更改，relief pressure 是否更改。  （drum的是否修改，会影响到火灾的计算）
            string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
            string tempdir = DirProtectedSystem + @"\temp\";
            if (Directory.Exists(tempdir))
            {
                Directory.Delete(tempdir, true);
            }
            Directory.CreateDirectory(tempdir);
            SplashScreenManager.SentMsgToScreen("Creating PSV... 20%");
            string dirPhase = tempdir + "Phase";
            if (Directory.Exists(dirPhase))
            {
                Directory.Delete(dirPhase, true);
            }
            Directory.CreateDirectory(dirPhase);
            SplashScreenManager.SentMsgToScreen("Creating PSV... 30%");
            List<CustomStream> arrFeeds = new List<CustomStream>();
            List<string> lstLiquidFeed = new List<string>();
            CustomStreamDAL csdal = new CustomStreamDAL();
            List<CustomStream> feedlist = csdal.GetAllList(SessionProtectedSystem, false).ToList();
            IList<CustomStream> productlist = csdal.GetAllList(SessionProtectedSystem, true);
            CustomStream stream = feedlist[0];
            SplashScreenManager.SentMsgToScreen("Creating PSV... 40%");
            if (EqType == "HX")
            {
                HeatExchangerDAL hxdal = new HeatExchangerDAL();
                HeatExchanger hx = hxdal.GetModel(SessionProtectedSystem);
                if (CurrentModel.LocationDescription == "Shell")
                {
                    string[] feeds = hx.ShellFeedStreams.Split(',');
                    foreach (string cold in feeds)
                    {
                        CustomStream cs = csdal.GetModel(SessionProtectedSystem, cold);
                        arrFeeds.Add(cs);
                    }
                }
                else
                {
                    string[] feeds = hx.TubeFeedStreams.Split(',');
                    foreach (string cold in feeds)
                    {
                        CustomStream cs = csdal.GetModel(SessionProtectedSystem, cold);
                        arrFeeds.Add(cs);
                    }
                }

            }
            else
            {
                arrFeeds = feedlist;
            }
            SplashScreenManager.SentMsgToScreen("Creating PSV... 50%");
            if (arrFeeds.Count > 0)
            {
                string[] streamComps = stream.TotalComposition.Split(',');
                int len = streamComps.Length;
                if (len == 1)
                {
                    //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    //reader.InitProIIReader(FileFullPath);
                    //double[] compInData = reader.GetCompInInfo(streamComps[0]);
                    //reader.ReleaseProIIReader();

                    ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, FileFullPath);
                    double[] phdata = reader.GetCompInInfo(streamComps[0]);
                    SplashScreenManager.SentMsgToScreen("Creating PSV... 60%");
                    criticalPressure = UnitConvert.Convert("KPA", UOMEnum.Pressure, phdata[0]);
                    criticalTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, phdata[1]);
                    SplashScreenManager.SentMsgToScreen("Creating PSV... 70%");
                }
                else
                {
                    double[] streamCompValues = new double[len];
                    double sumTotalMolarRate = 0;
                    foreach (CustomStream cs in arrFeeds)
                    {
                        sumTotalMolarRate = sumTotalMolarRate + cs.TotalMolarRate;
                    }
                    foreach (CustomStream cs in arrFeeds)
                    {
                        string[] comps = cs.TotalComposition.Split(',');
                        for (int i = 0; i < len; i++)
                        {
                            streamCompValues[i] = streamCompValues[i] + double.Parse(comps[i]) * cs.TotalMolarRate / sumTotalMolarRate;
                        }
                    }
                    SplashScreenManager.SentMsgToScreen("Creating PSV... 60%");
                    StringBuilder sumComposition = new StringBuilder();
                    foreach (double comp in streamCompValues)
                    {
                        sumComposition.Append(",").Append(comp.ToString());
                    }
                    stream.TotalComposition = sumComposition.ToString().Substring(1);
                    double internPressure = UnitConvert.Convert("MPAG", "KPA", stream.Pressure);
                    if (internPressure == 0)
                    {
                        MessageBox.Show("Please Rerun this ProII file and save it.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);

                    string phasecontent = PROIIFileOperator.getUsablePhaseContent(stream.StreamName, tempdir);
                    double ReliefPressure = CurrentModel.ReliefPressureFactor * UnitConvert.Convert(uomEnum.UserPressure, UOMEnum.Pressure, CurrentModel.Pressure);

                    bool b = CalcCriticalPressure(phasecontent, ReliefPressure, stream, dirPhase);
                    SplashScreenManager.SentMsgToScreen("Creating PSV... 70%");
                    if (b == false)
                    {
                        SplashScreenManager.SentMsgToScreen("Creating PSV... 80%");
                        return;
                    }
                }
                CurrentModel.CriticalPressure = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.CriticalPressureUnit, criticalPressure);
                CurrentModel.CriticalTemperature = UnitConvert.Convert(UOMEnum.Temperature, CurrentModel.CriticalTemperatureUnit, criticalTemperature);
                CurrentModel.CricondenbarPress = UnitConvert.Convert(UOMEnum.Pressure, CurrentModel.CriticalPressureUnit, cricondenbarPressure);
                CurrentModel.CricondenbarTemp = UnitConvert.Convert(UOMEnum.Temperature, CurrentModel.CricondenbarTempUnit, cricondenbarTemperature);
                SplashScreenManager.SentMsgToScreen("Creating PSV... 80%");
            }
            else
            {
                SplashScreenManager.SentMsgToScreen("Creating PSV... 60%");
                SplashScreenManager.SentMsgToScreen("Creating PSV... 70%");
                SplashScreenManager.SentMsgToScreen("Creating PSV... 80%");
            }
        }

        /// <summary>
        /// 复制第一层塔盘的液相
        /// </summary>
        /// <param name="copyPrzFile"></param>
        /// <returns></returns>
        private CustomStream CopyTop1Liquid(string copyPrzFile)
        {
            CustomStream cs = new CustomStream();
            //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
            //reader.InitProIIReader(copyPrzFile);
            //ProIIStreamData proIITray1StreamData = reader.CopyStream(EqName, 1, 2, 1);
            //reader.ReleaseProIIReader();
            ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, copyPrzFile);
            ProIIStreamData proIITray1StreamData = reader.CopyStreamInfo(EqName, 1, 2, 1);
            cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIITray1StreamData);
            return cs;
        }

        /// <summary>
        /// 获取塔顶的压力
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 计算临界压力，true表示可计算，false=no
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ReliefPressure"></param>
        /// <param name="stream"></param>
        /// <param name="dirPhase"></param>
        /// <returns></returns>
        private bool CalcCriticalPressure(string content, double ReliefPressure, CustomStream stream, string dirPhase)
        {
            int ImportResult = 0;
            int RunResult = 0;
            criticalPressure = 0;
            criticalTemperature = 0;
            //IPHASECalculate PhaseCalc = ProIIFactory.CreatePHASECalculate(SourceFileInfo.FileVersion);
            string PH = "PH" + Guid.NewGuid().ToString().Substring(0, 4);
            string criticalPress = string.Empty;
            string criticalTemp = string.Empty;
            string cricondenbarPress = string.Empty;
            string cricondenbarTemp = string.Empty;
            //string phasef = PhaseCalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH, dirPhase, ref ImportResult, ref RunResult);
            ProIICalculate proiicalc = new ProIICalculate(SourceFileInfo.FileVersion);
            string phasef = proiicalc.PHASECalculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH, dirPhase, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    //reader.InitProIIReader(phasef);
                    //criticalPress = reader.GetCriticalPressure(PH);
                    //criticalTemp = reader.GetCriticalTemperature(PH);
                    //cricondenbarPress = reader.GetCricondenbarPress(PH);
                    //cricondenbarTemp = reader.GetCricondenbarTemp(PH);
                    //reader.ReleaseProIIReader();
                    ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, phasef);
                    double[] phdata = reader.GetPHInfo(PH);
                    criticalPressure = UnitConvert.Convert("KPA", UOMEnum.Pressure, phdata[0]);
                    criticalTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, phdata[1]);
                    cricondenbarPressure = UnitConvert.Convert("KPA", UOMEnum.Pressure, phdata[2]);
                    cricondenbarTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, phdata[3]);
                    if (phdata[0] <= 0)
                        return false;
                    return true;
                }
                else
                {
                    //MessageBox.Show("Prz file is error", "Message Box");
                    return false;
                }
            }
            else
            {
                //MessageBox.Show("inp file is error", "Message Box");
                return false;
            }

        }

        /// <summary>
        /// 计算塔的latent 蒸发焓
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ReliefPressure"></param>
        /// <param name="stream"></param>
        /// <param name="dirLatent"></param>
        /// <param name="FluidType"></param>
        /// <param name="latentVapor"></param>
        /// <param name="latentLiquid"></param>
        private void CalcLatent(string content, double ReliefPressure, CustomStream stream, string dirLatent, int FluidType, ref LatentProduct latentVapor, ref LatentProduct latentLiquid)
        {
            int result = 0;
            LatentProductDAL dblp = new LatentProductDAL();
            int ImportResult = 0;
            int RunResult = 0;
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            //IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            //string tray1_f = fcalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "",HeatMethod, stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);

            ProIICalculate proiicalc = new ProIICalculate(SourceFileInfo.FileVersion);
            string tray1_f = proiicalc.FlashCalculate(content, 1, ReliefPressure.ToString(), 4, "", HeatMethod, stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);

            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    //reader.InitProIIReader(tray1_f);
                    //ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    //ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    //ProIIStreamData proIIStream= reader.GetSteamInfo(stream.StreamName);
                    //ProIIEqData proiiEq=reader.GetEqInfo("Flash","F_1");
                    //reader.ReleaseProIIReader();



                    ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, tray1_f);
                    List<ProIIStreamData> lst = reader.GetStreamInfo(new string[] { vapor, liquid, stream.StreamName });

                    ProIIStreamData proIIVapor = lst[0];
                    ProIIStreamData proIILiquid = lst[1];
                    ProIIStreamData proIIStream = lst[2];
                    ProIIEqData proiiEq = reader.GetEqInfo("Flash", "F_1");

                    latentVapor = ProIIToDefault.ConvertProIIStreamToLatentProduct(proIIVapor);
                    latentVapor.ProdType = "1";
                    latentLiquid = ProIIToDefault.ConvertProIIStreamToLatentProduct(proIILiquid);
                    latentLiquid.ProdType = "2";

                    CustomStream csStream = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIStream);
                    LatentProduct latentStream = ProIIToDefault.ConvertCustomStreamToLatentProduct(csStream);
                    latentStream.ProdType = "-1";
                    dblp.Add(latentStream, SessionProtectedSystem);
                    dblp.Add(latentVapor, SessionProtectedSystem);
                    dblp.Add(latentLiquid, SessionProtectedSystem);

                    LatentDAL dblatent = new LatentDAL();
                    latent = new Latent();
                    latent.LatentEnthalpy = latentVapor.SpEnthalpy - latentLiquid.SpEnthalpy; ;
                    latent.ReliefTemperature = latentVapor.Temperature;
                    latent.ReliefOHWeightFlow = latentVapor.BulkMwOfPhase;
                    latent.ReliefPressure = UnitConvert.Convert(CurrentModel.PSVPressureUnit, UOMEnum.Pressure, CurrentModel.Pressure) * CurrentModel.ReliefPressureFactor;
                    latent.ReliefCpCv = latentVapor.BulkCPCVRatio;
                    latent.ReliefZ = latentVapor.VaporZFmKVal;
                    if (FluidType == 2)
                    {
                        //if (latent.ReliefCpCv > 0.9)
                        if (latent.ReliefPressure / criticalPressure > 0.9)
                        {
                            //warning4   流程图3
                            ErrorType = 4;
                            latent.LatentEnthalpy = CriticalLatent;
                            latent.ReliefTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, double.Parse(proiiEq.TempCalc));
                        }
                        else if (latent.LatentEnthalpy < CriticalLatent)
                        {
                            //warning2   流程图5
                            ErrorType = 2;
                            latent.LatentEnthalpy = CriticalLatent;
                            latent.LatentEnthalpy = CriticalLatent;
                            latent.ReliefTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, double.Parse(proiiEq.TempCalc));
                        }
                    }
                    else if (FluidType == 3)
                    {
                        if (latent.LatentEnthalpy < CriticalLatent)
                        {
                            ErrorType = 3;
                            latent.LatentEnthalpy = CriticalLatent;
                            latent.ReliefTemperature = UnitConvert.Convert("K", UOMEnum.Temperature, double.Parse(proiiEq.TempCalc));
                        }
                    }

                    dblatent.Add(latent, SessionProtectedSystem);
                }

                else
                {
                    //MessageBox.Show("Prz file is error", "Message Box");
                    result = 1;
                }
            }
            else
            {
                //MessageBox.Show("inp file is error", "Message Box");
                result = 2;
            }
            if (result != 0)
            {
                //warning3  流程图6  
                ErrorType = 3;
                LatentDAL dblatent = new LatentDAL();
                latent = new Latent();
                latent.LatentEnthalpy = 46;
                latent.ReliefTemperature = stream.Temperature;
                latent.ReliefOHWeightFlow = stream.BulkMwOfPhase;
                latent.ReliefPressure = ReliefPressure;
                latent.ReliefCpCv = stream.BulkCPCVRatio;
                latent.ReliefZ = stream.VaporZFmKVal;
                dblatent.Add(latent, SessionProtectedSystem);
            }
        }

        /// <summary>
        /// 临界压力下计算蒸发焓
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ReliefPressure"></param>
        /// <param name="ReliefTemperature"></param>
        /// <param name="stream"></param>
        /// <param name="dirSupper"></param>
        /// <returns></returns>
        private int CalSupperCritical(string content, double ReliefPressure, double ReliefTemperature, CustomStream stream, string dirSupper)
        {
            int result = 0;
            int ImportResult = 0;
            int RunResult = 0;
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = string.Empty;
            //IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            //string tray1_f = fcalc.Calculate(content, 1, ReliefPressure.ToString(), 2, ReliefTemperature.ToString(), HeatMethod, stream, vapor, liquid, dirSupper, ref ImportResult, ref RunResult);

            ProIICalculate proiicalc = new ProIICalculate(SourceFileInfo.FileVersion);
            string tray1_f = proiicalc.FlashCalculate(content, 1, ReliefPressure.ToString(), 2, ReliefTemperature.ToString(), HeatMethod, stream, vapor, liquid, dirSupper, ref ImportResult, ref RunResult);

            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    //IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    //reader.InitProIIReader(tray1_f);
                    //ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    //reader.ReleaseProIIReader();

                    ProIIReader reader = new ProIIReader(SourceFileInfo.FileVersion, tray1_f);
                    ProIIStreamData proIIVapor = reader.GetStreamInfo(vapor);

                    CustomStream csVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    LatentDAL dblatent = new LatentDAL();
                    //dbLatentProduct dblatentproduct = new dbLatentProduct();
                    TowerFlashProductDAL dbFlashProduct = new TowerFlashProductDAL();
                    //流程图1
                    latent = new Latent();
                    latent.LatentEnthalpy = CriticalLatent;
                    latent.ReliefTemperature = ReliefTemperature;
                    latent.ReliefOHWeightFlow = csVapor.BulkMwOfPhase;
                    latent.ReliefPressure = ReliefPressure;
                    latent.ReliefCpCv = csVapor.BulkCPCVRatio;
                    latent.ReliefZ = csVapor.VaporZFmKVal;

                    if (csVapor.BulkCPCVRatio == 0)//流程图2
                    {
                        latent.ReliefCpCv = 1.4;
                    }
                    dblatent.Add(latent, SessionProtectedSystem);
                    return result;
                }

                else
                {
                    //MessageBox.Show("Prz file is error", "Message Box");
                    return 1;
                }
            }
            else
            {
                //MessageBox.Show("inp file is error", "Message Box");
                return 2;
            }
        }


        private void CalcLatentEnthalpy(object window)
        {
            try
            {
                //LatentEnthalpyView v = new LatentEnthalpyView();
                //LatentEnthalpyVM vm = new LatentEnthalpyVM();
                //v.DataContext = vm;
                //bool? reslut = v.ShowDialog();
                //if (reslut.HasValue && reslut.Value)
                //{
                //}

                //System.Windows.Window wd = window as System.Windows.Window;
                //if (wd != null)
                //{
                //    wd.DialogResult = true;
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                SplashScreenManager.Close();
                switch (ErrorType)
                {
                    case -1:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningTray1LiquidZero"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 1:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower1"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 2:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower2"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 3:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower3"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 4:
                        MessageBox.Show(LanguageHelper.GetValueByKey("WarningPSVTower4"), "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;


                }


            }

        }

    }
}
