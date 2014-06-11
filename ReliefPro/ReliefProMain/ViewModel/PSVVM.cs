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
using ReliefProMain.Model;
using UOMLib;
using NHibernate;
using ProII;
using ReliefProCommon.CommonLib;
namespace ReliefProMain.ViewModel
{
    public class PSVVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        public string PrzFile { get; set; }
        public string PrzVersion { get; set; }
        public string EqName { get; set; }
        public string EqType { get; set; }

        private string psvPressureUnit;
        public string PSVPressureUnit
        {
            get { return psvPressureUnit; }
            set
            {
                psvPressureUnit = value;
                OnPropertyChanged("PSVPressureUnit");
            }
        }

        private string drumPSVPressureUnit;
        public string DrumPressureUnit
        {
            get { return drumPSVPressureUnit; }
            set
            {
                drumPSVPressureUnit = value;
                OnPropertyChanged("DrumPressureUnit");
            }
        }


        private string _CriticalPressureUnit;
        public string CriticalPressureUnit
        {
            get { return _CriticalPressureUnit; }
            set
            {
                _CriticalPressureUnit = value;
                OnPropertyChanged("CriticalPressureUnit");
            }
        }
        private double _CriticalPressure;
        public double CriticalPressure
        {
            get { return _CriticalPressure; }
            set
            {
                _CriticalPressure = value;
                OnPropertyChanged("CriticalPressure");
            }
        }

        public List<string> ValveTypes { get; set; }
        public PSVModel CurrentModel { get; set; }


        public PSV psv;
        PSVDAL dbpsv = new PSVDAL();
        UOMLib.UOMEnum uomEnum;
        public List<string> GetValveTypes()
        {
            List<string> list = new List<string>();
            list.Add("Modulation");
            list.Add("Pop Action");
            list.Add("Rupture Disk");
            list.Add("Temperature Actuated");
            return list;
        }
        public PSVVM(string eqName, string eqType, string przFile, string version, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            EqName = eqName;
            EqType = eqType;
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            ValveTypes = GetValveTypes();
            PrzFile = przFile;
            PrzVersion = version;

            uomEnum = new UOMLib.UOMEnum(sessionPlant);
            this.psvPressureUnit = uomEnum.UserPressure;
            this.drumPSVPressureUnit = uomEnum.UserPressure;

            psv = dbpsv.GetModel(sessionProtectedSystem);
            if (psv != null)
            {
                CurrentModel = ConvertModel(psv);
            }
            else
            {
                psv = new PSV();
                CurrentModel = new PSVModel();
            }
        }
        private PSVModel ConvertModel(PSV m)
        {
            PSVModel model = new PSVModel();
            model.ID = psv.ID;
            model.PSVName = m.PSVName;
            model.Pressure = UnitConvert.Convert(UOMEnum.Pressure, uomEnum.UserPressure, double.Parse(m.Pressure)).ToString();
            model.ReliefPressureFactor = m.ReliefPressureFactor;
            model.ValveNumber = m.ValveNumber;
            model.ValveType = m.ValveType;
            model.DrumPSVName = m.DrumPSVName;
            if (!string.IsNullOrEmpty(m.DrumPressure))
            {
                model.DrumPressure = UnitConvert.Convert(UOMEnum.Pressure, uomEnum.UserPressure, double.Parse(m.DrumPressure)).ToString();
            }
            model.Description = m.Description;
            model.LocationDescription = m.LocationDescription;
            return model;
        }
        private void ConvertModel(PSVModel m, ref PSV model)
        {
            model.PSVName = m.PSVName;
            model.Pressure = UnitConvert.Convert(psvPressureUnit, UOMEnum.Pressure, double.Parse(m.Pressure)).ToString();
            model.ReliefPressureFactor = m.ReliefPressureFactor;
            model.ValveNumber = m.ValveNumber;
            model.ValveType = m.ValveType;
            model.DrumPSVName = m.DrumPSVName;
            if (!string.IsNullOrEmpty(m.DrumPressure))
            {
                model.DrumPressure = UnitConvert.Convert(DrumPressureUnit, UOMEnum.Pressure, double.Parse(m.DrumPressure)).ToString();
            }
            model.Description = m.Description;
            model.LocationDescription = m.LocationDescription;
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
            if (string.IsNullOrEmpty(CurrentModel.Pressure))
            {
                MessageBox.Show("PSV Pressure can't be empty.", "Message Box");
                return;
            }
            if (string.IsNullOrEmpty(CurrentModel.ReliefPressureFactor))
            {
                MessageBox.Show("Relief Pressure Factor can't be empty.", "Message Box");
                return;
            }
            try
            {
                if (CurrentModel.ID == 0)
                {
                    if (EqType == "Tower")
                    {
                        CreateTowerPSV();
                    }
                    ConvertModel(CurrentModel, ref psv);
                    dbpsv.Add(psv, SessionProtectedSystem);
                }
                else if (psv.ReliefPressureFactor == CurrentModel.ReliefPressureFactor && psv.Pressure == CurrentModel.Pressure)
                {
                    ConvertModel(CurrentModel, ref psv);
                    dbpsv.Update(psv, SessionProtectedSystem);
                    SessionProtectedSystem.Flush();
                }
                else
                {
                    //需要删除
                    CreateTowerPSV();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            System.Windows.Window wd = window as System.Windows.Window;
            if (wd != null)
            {
                wd.Close();
            }
        }
        public void CreateTowerPSV()
        {
            //判断压力是否更改，relief pressure 是否更改。  （drum的是否修改，会影响到火灾的计算）

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

            string fileName=System.IO.Path.GetFileName(PrzFile);
            string copyFile=dirCopyStream+@"\"+fileName;
            File.Copy(PrzFile, copyFile);
            CustomStream stream = CopyTop1Liquid(copyFile);
            if (stream.Pressure == "0")
            {
                MessageBox.Show("Please Rerun this ProII file and save it.","Message Box");
                return;
            }
            PROIIFileOperator.DecompressProIIFile(PrzFile, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);

            double ReliefPressure = double.Parse(CurrentModel.ReliefPressureFactor) * double.Parse(CurrentModel.Pressure);
            
            double criticalPressure = 0;
            bool b=CalcCriticalPressure(content, ReliefPressure, stream, dirPhase,  ref criticalPressure);
            if(b==false)
                return ;
            double latentEnthalpy = 0;
            double ReliefTemperature = 0;
            LatentProduct latentVapor = new LatentProduct();
            LatentProduct latentLiquid = new LatentProduct();
            if (criticalPressure > ReliefPressure)
            {
                b = CalcLatent(content, ReliefPressure, stream, dirLatent, ref latentVapor, ref latentLiquid);
                if (b == false)
                    return;
                latentEnthalpy = double.Parse(latentVapor.SpEnthalpy) - double.Parse(latentLiquid.SpEnthalpy);
                ReliefTemperature = double.Parse(latentVapor.Temperature);
            }
            else
            {
                latentEnthalpy = 116.3152;
                ReliefTemperature = double.Parse(stream.Temperature);
            }
            IList<CustomStream> products = null;
            CustomStreamDAL dbstream = new CustomStreamDAL();
            products = dbstream.GetAllList(SessionProtectedSystem, true);
            double overHeadPressure = GetTowerOverHeadPressure(products);

            int ImportResult=0;
            int RunResult = 0;
            List<FlashResult> listFlashResult = new List<FlashResult>();
            int count = products.Count;
            for (int i = 1; i <= count; i++)
            {
                CustomStream p = products[i - 1];
                if (p.TotalMolarRate != "0")
                {
                    IFlashCalculate fc = ProIIFactory.CreateFlashCalculate(PrzVersion);
                    string gd = Guid.NewGuid().ToString();
                    string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                    string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                    string prodtype = p.ProdType;
                    string tray = p.Tray;
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
                    string usablecontent = PROIIFileOperator.getUsableContent(p.StreamName, tempdir);

                    if (prodtype == "4" || (prodtype == "2" && tray == "1")) // 2个条件是等同含义，后者是有气有液
                    {
                        f = fc.Calculate(usablecontent, 1, ReliefPressure.ToString(), 4, "", p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
                    }

                    else if (prodtype == "6" || prodtype == "3") //3 气相  6 沉积水 
                    {
                        f = fc.Calculate(usablecontent, 2, ReliefTemperature.ToString(), 4, "", p, vapor, liquid, dirflash, ref ImportResult, ref RunResult);
                    }
                    else
                    {
                        double press = ReliefPressure + (double.Parse(p.Pressure) - overHeadPressure);
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
                            MessageBoxResult r = MessageBox.Show("Prz file is error", "Message Box", MessageBoxButton.OKCancel);
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe",f);
                            
                            if (r == MessageBoxResult.Yes)
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
                            return;
                        }
                    }
                    else
                    {
                        MessageBoxResult r = MessageBox.Show("inp file is error", "Message Box", MessageBoxButton.OKCancel);
                        if (r == MessageBoxResult.Yes)
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
                        return;
                    }
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
                    IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
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
                    product.Temperature = cs.Temperature;

                    listFlashProduct.Add(product);
                }
            }



            LatentDAL dblatent = new LatentDAL();
            //dbLatentProduct dblatentproduct = new dbLatentProduct();
            TowerFlashProductDAL dbFlashProduct = new TowerFlashProductDAL();


            Critical c = new Critical();
            c.CriticalPressure = criticalPressure.ToString() ;
            CriticalDAL dbcritical = new CriticalDAL();
            dbcritical.Add(c, SessionProtectedSystem);



            Latent latent = new Latent();
            latent.LatentEnthalpy = latentEnthalpy.ToString();
            latent.ReliefTemperature = ReliefTemperature.ToString();
            latent.ReliefOHWeightFlow = latentVapor.BulkMwOfPhase;
            latent.ReliefPressure = (double.Parse(CurrentModel.Pressure) * double.Parse(CurrentModel.ReliefPressureFactor)).ToString();
            dblatent.Add(latent, SessionProtectedSystem);

            foreach (TowerFlashProduct p in listFlashProduct)
            {
                dbFlashProduct.Add(p, SessionProtectedSystem);
            }


        }

        private CustomStream CopyTop1Liquid(string copyPrzFile)
        {
            CustomStream cs = new CustomStream();
            IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
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
                    overHeadPressure = double.Parse(cs.Pressure);
                    break;
                }
            }
            return overHeadPressure;

        }

        private bool CalcCriticalPressure(string content, double ReliefPressure, CustomStream stream, string dirPhase, ref double criticalPressure)
        {
            int ImportResult = 0;
            int RunResult = 0;
            criticalPressure = 0;
            IPHASECalculate PhaseCalc = ProIIFactory.CreatePHASECalculate(PrzVersion);
            string PH = "PH" + Guid.NewGuid().ToString().Substring(0, 4);
            string criticalPress = string.Empty;
            string phasef = PhaseCalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH, dirPhase, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
                    reader.InitProIIReader(phasef);
                    criticalPress = reader.GetCriticalPressure(PH);
                    reader.ReleaseProIIReader();
                    criticalPress = UnitConvert.Convert("KPA", "MPAG", double.Parse(criticalPress)).ToString();
                    criticalPressure= double.Parse(criticalPress);
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

        private bool CalcLatent(string content, double ReliefPressure, CustomStream stream, string dirLatent, ref LatentProduct latentVapor, ref LatentProduct latentLiquid )
        {
            LatentProductDAL dblp = new LatentProductDAL();
            int ImportResult = 0;
            int RunResult = 0;
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(PrzVersion);
            string tray1_f = fcalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                   IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    latentVapor = ProIIToDefault.ConvertProIIStreamToLatentProduct(proIIVapor);
                    latentVapor.ProdType = "1";
                    latentLiquid = ProIIToDefault.ConvertProIIStreamToLatentProduct(proIILiquid);
                    latentVapor.ProdType = "2";
                    
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
        