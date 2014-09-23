using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProDAL.GlobalDefault;
using ReliefProDAL.Reports;
using ReliefProModel;
using ReliefProModel.GlobalDefault;
using ReliefProModel.Reports;
using UOMLib;

namespace ReliefProLL
{
    public class ReportBLL
    {
        private GlobalDefaultDAL globalDefaultDAL = new GlobalDefaultDAL();
        private PSVDAL psvDAL = new PSVDAL();
        private ISession SessionPlant;
        private volatile List<ISession> lstSession;
        private ScenarioDAL scenarioDAL = new ScenarioDAL();
        public ConcurrentBag<PSV> PSVBag;
        public ConcurrentBag<Scenario> ScenarioBag;
        public List<string> ProcessUnitReportPath;
        private PUsummaryGridDS SumDs = new PUsummaryGridDS();
        public List<string> listPlantCalc = new List<string> { "Direct Summation", "100-30-30", "Sum of first 2 Max", "100-50-50" };
        public string[] ScenarioName = new string[] {"Controlling Single Scenario", "General Electric Power Failure",
            "General Cooling Water Failure","General Instument Air Failure","Steam Failure","Fire" };

        List<string> listScenario = new List<string> { "PowerDS", "WaterDS", "AirDS", "SteamDS", "FireDS" };
        List<string> listProperty = new List<string> { "ReliefLoad", "ReliefMW", "ReliefTemperature", "ReliefZ" };
        List<PUsummaryGridDS> listGrid;
        private int UnitID;
        private string ProcessUnitName;
        List<double?> tmpResult = new List<double?>();
        public ReportBLL()
        {
            lstSession = new List<ISession>();
        }
        public ReportBLL(int UnitID, List<string> ProcessUnitReportPath)
        {
            lstSession = new List<ISession>();
            this.UnitID = UnitID;
            PSVBag = new ConcurrentBag<PSV>();
            ScenarioBag = new ConcurrentBag<Scenario>();
            this.ProcessUnitReportPath = ProcessUnitReportPath;
            GenerateDbSession();
        }
        public List<FlareSystem> GetDisChargeTo()
        {
            return globalDefaultDAL.GetFlareSystem(SessionPlant).ToList();
        }

        #region PlantSummary
        public List<PUsummaryReportSource> CalcPlantSummary(List<PlantSummaryGridDS> listPlant)
        {
            List<PUsummaryReportSource> listPlantDS = new List<PUsummaryReportSource>();

            int CalcType = 0;
            listPlantCalc.ForEach(p =>
            {
                listPlantDS.Add(new PUsummaryReportSource
                {
                    Device = p,
                    PowerReliefRate = GetPlantSumResult(listPlant, CalcType, "PowerDS", "ReliefLoad"),
                    PowerVolumeRate = GetPlantSumResult(listPlant, CalcType, "PowerDS", "ReliefVolumeRate"),
                    PowerMWorSpGr = GetPlantSumResult(listPlant, CalcType, "PowerDS", "ReliefMW"),
                    PowerT = GetPlantSumResult(listPlant, CalcType, "PowerDS", "ReliefTemperature"),
                    PowerCpCv = GetPlantSumResult(listPlant, CalcType, "PowerDS", "ReliefCpCv"),

                    WaterReliefRate = GetPlantSumResult(listPlant, CalcType, "WaterDS", "ReliefLoad"),
                    WaterVolumeRate = GetPlantSumResult(listPlant, CalcType, "WaterDS", "ReliefVolumeRate"),
                    WaterMWorSpGr = GetPlantSumResult(listPlant, CalcType, "WaterDS", "ReliefMW"),
                    WaterT = GetPlantSumResult(listPlant, CalcType, "WaterDS", "ReliefTemperature"),
                    WaterCpCv = GetPlantSumResult(listPlant, CalcType, "WaterDS", "ReliefCpCv"),

                    AirReliefRate = GetPlantSumResult(listPlant, CalcType, "AirDS", "ReliefLoad"),
                    AirVolumeRate = GetPlantSumResult(listPlant, CalcType, "AirDS", "ReliefVolumeRate"),
                    AirMWorSpGr = GetPlantSumResult(listPlant, CalcType, "AirDS", "ReliefMW"),
                    AirT = GetPlantSumResult(listPlant, CalcType, "AirDS", "ReliefTemperature"),
                    AirCpCv = GetPlantSumResult(listPlant, CalcType, "AirDS", "ReliefCpCv"),
                });
                CalcType++;
            });

            return listPlantDS;
        }

        public PlantSummaryGridDS GetPlantReprotDS(List<PUsummaryGridDS> ProcessUnitReprotDS, int CalcType)
        {
            PlantSummaryGridDS plant = new PlantSummaryGridDS();
            if (ProcessUnitReprotDS.Count > 0)
            {
                plant.PowerDS = new Scenario();
                plant.WaterDS = new Scenario();
                plant.AirDS = new Scenario();
                plant.ProcessUnit = ProcessUnitReprotDS.FirstOrDefault().ProcessUnit;
                plant.ControllingDS = ProcessUnitReprotDS.OrderByDescending(p => p.SingleDS.ReliefLoad).FirstOrDefault().SingleDS;

                plant.PowerDS.ReliefLoad = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "PowerDS", "ReliefLoad");
                plant.PowerDS.ReliefVolumeRate = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "PowerDS", "ReliefVolumeRate");
                plant.PowerDS.ReliefMW = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "PowerDS", "ReliefMW");
                plant.PowerDS.ReliefTemperature = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "PowerDS", "ReliefTemperature");
                plant.PowerDS.ReliefCpCv = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "PowerDS", "ReliefCpCv");

                plant.WaterDS.ReliefLoad = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "WaterDS", "ReliefLoad");
                plant.WaterDS.ReliefVolumeRate = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "WaterDS", "ReliefVolumeRate");
                plant.WaterDS.ReliefMW = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "WaterDS", "ReliefMW");
                plant.WaterDS.ReliefTemperature = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "WaterDS", "ReliefTemperature");
                plant.WaterDS.ReliefCpCv = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "WaterDS", "ReliefCpCv");

                plant.AirDS.ReliefLoad = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "AirDS", "ReliefLoad");
                plant.AirDS.ReliefVolumeRate = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "AirDS", "ReliefVolumeRate");
                plant.AirDS.ReliefMW = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "AirDS", "ReliefMW");
                plant.AirDS.ReliefTemperature = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "AirDS", "ReliefTemperature");
                plant.AirDS.ReliefCpCv = GetPlantSumResult(ProcessUnitReprotDS, CalcType, "AirDS", "ReliefCpCv");
                return plant;
            }
            return null;
        }
        private double GetPlantSumResult<T>(List<T> ProcessUnitReprotDS, int CalcType, string ScenarioType, string ScenarioProperty)
        {
            if (CalcType == 2)
            {
                SumOfFirst2Max(ProcessUnitReprotDS, ScenarioType, ScenarioProperty);
            }
            double? maxValue = ProcessUnitReprotDS.Max(p =>
            {
                Scenario scenario = (Scenario)p.GetType().GetProperty(ScenarioType).GetValue(p, null);
                if (scenario == null) return null;
                var obj = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
                if (obj == null) return null;
                return double.Parse(obj.ToString());
            });
            if (maxValue != null)
            {
                ProcessUnitReprotDS.ForEach(p =>
                 {
                     Scenario scenario = (Scenario)p.GetType().GetProperty(ScenarioType).GetValue(p, null);
                     if (scenario == null) return;
                     var obj = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
                     if (obj == null) return;
                     double value = double.Parse(obj.ToString());
                     switch (CalcType)
                     {
                         case 0: DirectSummation(value); break;
                         case 1: Calc100_30_30(value, maxValue.Value); break;
                         case 3: Calc100_50_50(value, maxValue.Value); break;
                         default: break;
                     }
                 });
            }
            return CalcPlantSumResult();
        }
        private double GetDouble(string value)
        {
            double reslut = 0;
            if (double.TryParse(value, out reslut))
                return reslut;
            return 0;
        }
        private double CalcPlantSumResult()
        {
            if (tmpResult.Count == 0) return 0;
            double? Result = tmpResult.Sum();
            tmpResult.Clear();
            return Result.Value;
        }

        private void DirectSummation(double value)
        {
            tmpResult.Add(value);
        }
        private void SumOfFirst2Max<T>(List<T> ProcessUnitReprotDS, string ScenarioType, string ScenarioProperty)
        {
            //ProcessUnitReprotDS.OrderBy(p => { return p.PowerDS.ReliefLoad; }).Take(2);
            var query = ProcessUnitReprotDS.OrderByDescending(p =>
             {
                 //return p.GetType().GetProperty(ScenarioType).GetValue(p, null).GetType().GetProperty(ScenarioProperty);
                 Scenario scenario = (Scenario)p.GetType().GetProperty(ScenarioType).GetValue(p, null);
                 return scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null); ;
             }).Take(2);
            foreach (var result in query)
            {
                Scenario scenario = (Scenario)result.GetType().GetProperty(ScenarioType).GetValue(result, null);
                if (scenario == null) continue;
                object obj = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
                if (obj != null)
                {
                    tmpResult.Add(double.Parse(obj.ToString()));
                }
            }
        }
        private void Calc100_30_30(double value, double maxValue)
        {
            CalcFun(value, maxValue, 0.3);
        }
        private void Calc100_50_50(double value, double maxValue)
        {
            CalcFun(value, maxValue, 0.5);
        }
        private void CalcFun(double value, double maxValue, double Scale)
        {
            if (value == maxValue)
            {
                if (tmpResult.Contains(value))
                {
                    tmpResult.Add(value * Scale);
                }
                else
                {
                    tmpResult.Add(value);
                }
            }
            else
            {
                tmpResult.Add(value * Scale);
            }
        }
        #endregion

        #region PUsummary


        public List<PUsummaryGridDS> GetPuReprotDS(List<PSV> listPSV)
        {
            var listGrid = new List<PUsummaryGridDS>();
            foreach (var PSV in listPSV)
            {
                PUsummaryGridDS gridDs = new PUsummaryGridDS();
                gridDs.ProcessUnit = ProcessUnitName;
                gridDs.psv = PSV;
                gridDs.PowerDS = this.ScenarioBag.FirstOrDefault(p => p.ScenarioName == this.ScenarioName[1] && p.dbPath == PSV.dbPath);
                gridDs.WaterDS = this.ScenarioBag.FirstOrDefault(p => p.ScenarioName == this.ScenarioName[2] && p.dbPath == PSV.dbPath);
                gridDs.AirDS = this.ScenarioBag.FirstOrDefault(p => p.ScenarioName == this.ScenarioName[3] && p.dbPath == PSV.dbPath);
                gridDs.SteamDS = this.ScenarioBag.FirstOrDefault(p => p.ScenarioName == this.ScenarioName[4] && p.dbPath == PSV.dbPath);
                gridDs.FireDS = this.ScenarioBag.FirstOrDefault(p => p.ScenarioName == this.ScenarioName[5] && p.dbPath == PSV.dbPath);
                InitControllingSingleScenarioDS(ref gridDs);
                listGrid.Add(gridDs);
            }
            return listGrid;
        }

        #region  ControllingSingleScenarioDS
        private void InitControllingSingleScenarioDS(ref PUsummaryGridDS gridDs)
        {
            double? maxPowerDS = gridDs.PowerDS.ReliefLoad;
            double? maxWaterDS = gridDs.WaterDS.ReliefLoad;
            double? maxAirDS = gridDs.AirDS.ReliefLoad;
            double? maxSteamDS = gridDs.SteamDS.ReliefLoad;
            double? maxFireDS = gridDs.FireDS.ReliefLoad;
            List<double?> MaxList = new List<double?> { maxPowerDS, maxWaterDS, maxAirDS, maxSteamDS, maxFireDS };
            var v = MaxList.Select((m, index) => new { index, m }).OrderByDescending(n => n.m).Take(1);
            int Index = 0;
            foreach (var t in v)
            {
                Index = t.index; break;
            }
            switch (Index)
            {
                case 0: gridDs.SingleDS = gridDs.PowerDS; break;
                case 1: gridDs.SingleDS = gridDs.WaterDS; break;
                case 2: gridDs.SingleDS = gridDs.AirDS; break;
                case 3: gridDs.SingleDS = gridDs.SteamDS; break;
                case 4: gridDs.SingleDS = gridDs.FireDS; break;
                default: break;
            }
        }
        #endregion

        #region Process Unit ALL Info
        private void GetProcessUnitName(ISession SessionPT)
        {
            TreeUnitDAL dal = new TreeUnitDAL();
            var TreeUnit = dal.GetModel(UnitID, SessionPT);
            if (TreeUnit != null)
            {
                ProcessUnitName = TreeUnit.PUName;
            }
        }
        private void GetPSVInfo(ISession SessionPS)
        {
            var PSVInfo = psvDAL.GetAllList(SessionPS).ToList();
            PSVInfo.ForEach(psv => { psv.dbPath = SessionPS.Connection.ConnectionString; PSVBag.Add(psv); });
        }
        private void GetScenarioInfo(ISession SessionPS)
        {
            double pback = 0;
            var PBack = this.GetDisChargeTo().FirstOrDefault(p => p.FlareName.ToUpper().Contains("HP"));
            if (PBack != null)
            {
                pback = PBack.DesignBackPressure;
                pback = UnitConvert.Convert(UOMEnum.Pressure, "Kpa", pback);
            }
            var ScenarioInfo = scenarioDAL.GetAllList(SessionPS).ToList();
            ScenarioInfo.ForEach(p =>
            {
                p.dbPath = SessionPS.Connection.ConnectionString;
                if (pback != 0)
                {
                    double W, T, MW;
                    W = p.ReliefLoad;
                    T = p.ReliefTemperature;
                    MW = p.ReliefMW;
                    if (MW != 0)
                        p.ReliefVolumeRate = (W * 8.314 * (T + 273.15)) / (pback * MW);
                }
                ScenarioBag.Add(p);
            });
        }
        private void InitInfo()
        {
            PSVBag.AsParallel().ForAll(p =>
            {
                foreach (string scenarioName in ScenarioName)
                {
                    var scenarioInfo = ScenarioBag.FirstOrDefault(s => s.dbPath == p.dbPath && s.ScenarioName == scenarioName);
                    if (scenarioInfo == null || scenarioInfo.ID <= 0)
                    {
                        Scenario scenario = new Scenario();
                        scenario.dbPath = p.dbPath;
                        scenario.ScenarioName = scenarioName;
                        ScenarioBag.Add(scenario);
                    }
                }
            });

            PSVBag.OrderBy(p => p.dbPath);
            ScenarioBag.OrderBy(p => p.dbPath).ThenBy(p => p.ScenarioName);
        }
        private void GenerateDbSession()
        {
            ProcessUnitReportPath.ForEach(p =>
            {
                var findSession = lstSession.Find(s => s.Connection.ConnectionString == p);
                if (findSession == null)
                {
                    NHibernateHelper helperProtectedSystem = new NHibernateHelper(p);
                    findSession = helperProtectedSystem.GetCurrentSession();
                    lstSession.Add(findSession);

                    if (p.Contains("plant.mdb"))
                    {
                        
                            SessionPlant = findSession;
                            GetProcessUnitName(findSession);
                        
                    }
                    else
                    {
                        GetPSVInfo(findSession);
                        GetScenarioInfo(findSession);
                    }
                }


            });
            InitInfo();
        }

        public void ClearSession()
        {
            foreach (ISession session in lstSession)
            {
                session.Close();
                session.Dispose();
            }
        }
        #endregion

        #region PUsummary Subtotal
        public List<PUsummaryGridDS> CalcMaxSum(List<PUsummaryGridDS> lstGrid)
        {
            this.listGrid = lstGrid;
            SumDs.psv = new PSV();
            SumDs.psv.ValveType = "Summation";
            SumDs.SingleDS = new Scenario();
            Calc();
            listGrid.Add(SumDs);

            PUsummaryGridDS MaxDs = new PUsummaryGridDS();
            MaxDs.psv = new PSV();
            MaxDs.psv.ValveType = "Max";
            MaxDs.SingleDS = new Scenario();
            MaxDs.SingleDS.Phase = string.Empty;
            MaxDs.SingleDS.ReliefLoad = listGrid.Max(p =>
            {
                if (p.SingleDS == null) return 0;
                if (p.SingleDS.ReliefLoad != null)
                    return p.SingleDS.ReliefLoad;
                else return 0;
            });
            listGrid.Insert(listGrid.Count - 1, MaxDs);
            return listGrid;
        }
        private void Calc()
        {
            SumDs.PowerDS = new Scenario();
            SumDs.WaterDS = new Scenario();
            SumDs.AirDS = new Scenario();
            SumDs.SteamDS = new Scenario();
            SumDs.FireDS = new Scenario();
            listScenario.ForEach(p =>
            {
                CalcSum(p);
            });
            SumDs.SingleDS.Phase = string.Empty;
            SumDs.PowerDS.Phase = string.Empty;
            SumDs.WaterDS.Phase = string.Empty;
            SumDs.AirDS.Phase = string.Empty;
            SumDs.SteamDS.Phase = string.Empty;
            SumDs.FireDS.Phase = string.Empty;
        }
        private void CalcSum(string ScenarioType)
        {
            PropertyInfo pInfo = SumDs.GetType().GetProperty(ScenarioType);
            Scenario scenario = (Scenario)SumDs.GetType().GetProperty(ScenarioType).GetValue(SumDs, null);

            listProperty.ForEach(p =>
            {
                PropertyInfo pInfoScenario = scenario.GetType().GetProperty(p);
                var result = GetSumResult(ScenarioType, p);
                pInfoScenario.SetValue(scenario, result, null);
            });

            pInfo.SetValue(SumDs, scenario, null);
        }
        private double? GetSumResult(string ScenarioType, string ScenarioProperty)
        {
            object obj;
            double? a, b;
            a = listGrid.Sum(p =>
            {
                Scenario scenario = (Scenario)p.GetType().GetProperty(ScenarioType).GetValue(p, null);
                if (scenario == null) return null;
                obj = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
                if (obj == null) return null;
                else return double.Parse(obj.ToString());
            });
            // a = a ?? 0;
            if (ScenarioProperty.Equals("ReliefLoad"))
            {
                return a;
            }
            else
            {
                var wmw = listGrid.Sum(p =>
                {
                    Scenario scenario = (Scenario)p.GetType().GetProperty(ScenarioType).GetValue(p, null);
                    if (scenario == null) return null;
                    var objw = scenario.GetType().GetProperty("ReliefLoad").GetValue(scenario, null);
                    var objmw = scenario.GetType().GetProperty("ReliefMW").GetValue(scenario, null);
                    if (objw == null || objmw == null || objmw.ToString() == "0") return null;
                    else return double.Parse(objw.ToString()) / double.Parse(objmw.ToString());
                });
                if (ScenarioProperty.Equals("ReliefMW"))
                {
                    var w = GetSumResult(ScenarioType, "ReliefLoad");
                    if (w == null || wmw == 0) return null;
                    else return w / wmw;
                }
                else
                {
                    if (wmw == null || wmw == 0) return null;
                    var twmw = listGrid.Sum(p =>
                    {
                        Scenario scenario = (Scenario)p.GetType().GetProperty(ScenarioType).GetValue(p, null);
                        if (scenario == null) return null;
                        var objw = scenario.GetType().GetProperty("ReliefLoad").GetValue(scenario, null);
                        var objmw = scenario.GetType().GetProperty("ReliefMW").GetValue(scenario, null);
                        var thisvalue = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
                        if (objw == null || objmw == null || objmw.ToString() == "0" || thisvalue == null) return null;
                        else return (double.Parse(objw.ToString()) / double.Parse(objmw.ToString()) / wmw) * double.Parse(thisvalue.ToString());
                    });
                    return twmw;
                }
            }

        }
        #endregion

        #endregion

        #region OperatePUsummary
        public PUsummary GetPUsummaryModel(int UnitID)
        {
            PUsummaryDAL puSummaryDAL = new PUsummaryDAL();
            return puSummaryDAL.GetModel(UnitID, SessionPlant);
        }
        public void SavePUsummary(PUsummary model)
        {
            PUsummaryDAL puSummaryDAL = new PUsummaryDAL();
            puSummaryDAL.Save(SessionPlant, model);
        }
        #endregion
    }
}
