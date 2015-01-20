using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.IO;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Models.HXs;
using ReliefProModel;
using ReliefProDAL;
using UOMLib;
using ReliefProCommon.CommonLib;
using ProII;

namespace ReliefProMain.ViewModel.HXs
{
    public class AirCooledHXFireSizeVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant;
        private string DirProtectedSystem;

        public AirCooledHXFireSizeModel model { get; set; }
        private HXBLL hxBLL;
        private CustomStreamDAL customStreamDAL = new CustomStreamDAL();
        public SourceFile SourceFileInfo { get; set; }
        public string FileFullPath { get; set; }
        string HeatMethod = string.Empty;

        public AirCooledHXFireSizeVM(int ID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;

            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);


            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.WettedBundleUnit = uomEnum.UserArea;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
        }

        public AirCooledHXFireSizeVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            FileFullPath = DirPlant + @"\" + sourceFileInfo.FileNameNoExt + @"\" + sourceFileInfo.FileName;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            hxBLL = new HXBLL(SessionPS, SessionPF);
            var airModel = hxBLL.GetAirCooledHXFireSizeModel(ScenarioID);
            airModel = hxBLL.ReadConvertAirCooledHXFireSizeModel(airModel);

            model = new AirCooledHXFireSizeModel(airModel);
            model.dbmodel.ScenarioID = ScenarioID;


            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.WettedBundleUnit = uomEnum.UserArea;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.WettedBundle = UnitConvert.Convert(model.WettedBundleUnit, UOMLib.UOMEnum.Area.ToString(), model.WettedBundle);
            model.dbmodel.PipingContingency = model.PipingContingency;

        }
        private void CalcResult(object obj)
        {
            if (!CheckData()) return;
            double Q = 0;

            CustomStream feed = new CustomStream();
            IList<CustomStream> feeds = customStreamDAL.GetAllList(SessionPS, false);
            if (feeds.Count > 0)
                feed = feeds[0];

            CustomStream product = new CustomStream();
            IList<CustomStream> products = customStreamDAL.GetAllList(SessionPS, true);
            if (products.Count > 0)
                product = products[0];



            if (feed.VaporFraction == 1 && product.VaporFraction == 1)
            {
                MessageBox.Show("No calc", "Message Box");
                return;
            }
            else
            {
                CustomStream maxTStream = feed;
                if (product.Temperature > feed.Temperature)
                    maxTStream = product;

                CustomStream stream = new CustomStream();
                if (maxTStream.VaporFraction == 1)
                    stream = getFlashCalcLiquidStreamVF1(maxTStream);
                else
                    stream = getFlashCalcLiquidStreamVF0(maxTStream);

                if (stream != null)
                {
                    PSVDAL psvDAL = new PSVDAL();
                    PSV psv = psvDAL.GetModel(SessionPS);
                    double pressure = psv.Pressure;

                    double reliefFirePressure = pressure * 1.21;
                    string tempdir = DirProtectedSystem + @"\temp\";
                    string dirLatent = tempdir + "Fire2";
                    if (Directory.Exists(dirLatent))
                    {
                        Directory.Delete(dirLatent, true);
                    }
                    Directory.CreateDirectory(dirLatent);

                    string gd = Guid.NewGuid().ToString();
                    string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                    string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                    string rate = "0.05";
                    int ImportResult = 0;
                    int RunResult = 0;
                    PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                    string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
                    IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    string tray1_f = fcalc.Calculate(content, 1, reliefFirePressure.ToString(), 6, rate,HeatMethod, stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
                    if (ImportResult == 1 || ImportResult == 2)
                    {
                        if (RunResult == 1 || RunResult == 2)
                        {
                            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                            reader.InitProIIReader(tray1_f);
                            ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                            ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                            reader.ReleaseProIIReader();
                            CustomStream vaporFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                            CustomStream liquidFire = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                            double latent = vaporFire.SpEnthalpy - liquidFire.SpEnthalpy;
                            model.ReliefLoad = Q / latent;
                            model.ReliefMW = vaporFire.BulkMwOfPhase;
                            model.ReliefPressure = reliefFirePressure;
                            model.ReliefTemperature = vaporFire.Temperature;
                            //model.ReliefCpCv = double.Parse(vaporFire.BulkCPCVRatio);
                            //model.ReliefZ = double.Parse(vaporFire.VaporZFmKVal);

                        }

                        else
                        {
                            MessageBox.Show("Prz file is error", "Message Box");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("inp file is error", "Message Box");
                        return;
                    }
                }
            }


        }
        private void Save(object obj)
        {
            if (!CheckData()) return;
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    //hxBLL.SaveAirCooledHXFireSize(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }

        private CustomStream getFlashCalcLiquidStreamVF0(CustomStream stream)
        {
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "Fire1";
            if (Directory.Exists(dirLatent))
            {
                Directory.Delete(dirLatent, true);
            }
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            string rate = "1";
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 5, "0",HeatMethod, stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    return liquidcs;

                }

                else
                {
                    MessageBox.Show("Prz file is error", "Message Box");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
                return null;
            }
        }

        private CustomStream getFlashCalcLiquidStreamVF1(CustomStream stream)
        {
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "Fire1";
            if (Directory.Exists(dirLatent))
            {
                Directory.Delete(dirLatent, true);
            }
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 4, "",HeatMethod, stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    reader.ReleaseProIIReader();
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    return liquidcs;

                }

                else
                {
                    MessageBox.Show("Prz file is error", "Message Box");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
                return null;
            }
        }
    }
}
