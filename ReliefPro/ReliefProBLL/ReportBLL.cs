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
        private ScenarioDAL scenarioDAL = new ScenarioDAL();
        public ConcurrentBag<PSV> PSVBag = new ConcurrentBag<PSV>();
        public ConcurrentBag<Scenario> ScenarioBag = new ConcurrentBag<Scenario>();
        static CountdownEvent countdown = new CountdownEvent(0);
        public List<string> ReportPath;
        private PUsummaryGridDS SumDs = new PUsummaryGridDS();
        string ss = "Controlling Single Scenario";
        public string[] ScenarioName = new string[] {"Controlling Single Scenario", "General Electric Power Failure",
            "General Cooling Water Failure","General Instument Air Failure","Steam Failure","Fire" };

        List<string> listScenario = new List<string> { "PowerDS", "WaterDS", "AirDS", "SteamDS", "FireDS" };
        List<string> listProperty = new List<string> { "ReliefLoad", "ReliefMW", "ReliefTemperature", "ReliefZ" };
        List<PUsummaryGridDS> listGrid;

        public ReportBLL()
        { }
        public ReportBLL(List<string> ReportPath)
        {
            this.ReportPath = ReportPath;
            GenerateDbSession();
        }
        public List<FlareSystem> GetDisChargeTo()
        {
            return globalDefaultDAL.GetFlareSystem(TempleSession.Session).ToList();
        }
        private void GetPSVInfo(ISession SessionPS)
        {
            var PSVInfo = psvDAL.GetAllList(SessionPS).ToList();
            PSVInfo.ForEach(psv => { psv.dbPath = SessionPS.Connection.ConnectionString; PSVBag.Add(psv); });
        }
        private void GetScenarioInfo(ISession SessionPS)
        {
            var ScenarioInfo = scenarioDAL.GetAllList(SessionPS).ToList();
            ScenarioInfo.ForEach(p => { p.dbPath = SessionPS.Connection.ConnectionString; ScenarioBag.Add(p); });
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
            ReportPath.AsParallel().ForAll(p =>
            {
                NHibernateHelper helperProtectedSystem = new NHibernateHelper(p);
                var tmpSession = helperProtectedSystem.GetCurrentSession();
                GetPSVInfo(tmpSession);
                GetScenarioInfo(tmpSession);
            });
            //Parallel.ForEach(ReportPath, (p, loopState) =>
            //{
            //    countdown.AddCount();
            //    NHibernateHelper helperProtectedSystem = new NHibernateHelper(p);
            //    var tmpSession = helperProtectedSystem.GetCurrentSession();
            //    GetPSVInfo(tmpSession);
            //    GetScenarioInfo(tmpSession);
            //    countdown.Signal();
            //});
            //countdown.Signal();
            //countdown.Wait();
            InitInfo();
        }

        #region Subtotal
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
                if (p.SingleDS == null) return null;
                if (!string.IsNullOrEmpty(p.SingleDS.ReliefLoad))
                    return double.Parse(p.SingleDS.ReliefLoad);
                else return null;
            }).ToString();
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
            SumDs.GetType().GetProperty(ScenarioType).GetValue(SumDs, null).GetType();

            PropertyInfo pInfo = SumDs.GetType().GetProperty(ScenarioType);
            Scenario scenario = (Scenario)SumDs.GetType().GetProperty(ScenarioType).GetValue(SumDs, null);

            listProperty.ForEach(p =>
            {
                PropertyInfo pInfoScenario = scenario.GetType().GetProperty(p);
                string result = GetSumResult(ScenarioType, p);
                pInfoScenario.SetValue(scenario, result, null);
            });

            pInfo.SetValue(SumDs, scenario, null);
        }
        private string GetSumResult(string ScenarioType, string ScenarioProperty)
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
            a = a ?? 0;
            if (ScenarioProperty.Equals("ReliefLoad"))
            {
                return a == 0 ? "" : a.ToString();
            }

            b = listGrid.Sum(p =>
            {
                Scenario scenario = (Scenario)p.GetType().GetProperty(ScenarioType).GetValue(p, null);
                if (scenario == null) return null;
                obj = scenario.GetType().GetProperty(ScenarioProperty).GetValue(scenario, null);
                if (obj == null) return null;
                if (!string.IsNullOrEmpty(scenario.ReliefMW))
                {
                    double mw = double.Parse(scenario.ReliefMW);
                    if (mw != 0)
                        return double.Parse(obj.ToString()) / mw;
                }
                return null;
            });
            b = b == 0 ? null : b;

            double? reslut = a / b;
            reslut = reslut ?? 0;
            return reslut == 0 ? "" : reslut.ToString();
        }
        #endregion
    }
}
