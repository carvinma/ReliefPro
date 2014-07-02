using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProDAL.GlobalDefault;
using ReliefProModel;

namespace ReliefProLL
{
    public class ReportBLL
    {
        private GlobalDefaultDAL globalDefaultDAL = new GlobalDefaultDAL();
        private PSVDAL psvDAL = new PSVDAL();
        private ScenarioDAL scenarioDAL = new ScenarioDAL();
        private ConcurrentBag<ISession> BagList = new ConcurrentBag<ISession>();
        private ConcurrentBag<PSV> PSVBag = new ConcurrentBag<PSV>();
        private ConcurrentBag<Scenario> ScenarioBag = new ConcurrentBag<Scenario>();
        static CountdownEvent countdown = new CountdownEvent(Environment.ProcessorCount);
        public List<string> ReportPath;
        string ss = "Controlling Single Scenario";
        string[] ScenarioName = new string[] {"Controlling Single Scenario", "General Electric Power Failure",
            "General Cooling Water Failure","General Instument Air Failure","Steam Failure","Fire" };
        public ReportBLL(List<string> ReportPath)
        {
            this.ReportPath = ReportPath;
            GenerateDbSession();
        }
        private void GetDisChargeTo(ISession SessionPS)
        {
            globalDefaultDAL.GetFlareSystem(SessionPS).ToList();
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
                    if (scenarioInfo.ID <= 0)
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
            Parallel.ForEach(ReportPath, (p, loopState) =>
            {
                countdown.AddCount();
                NHibernateHelper helperProtectedSystem = new NHibernateHelper(p);
                var tmpSession = helperProtectedSystem.GetCurrentSession();
                GetPSVInfo(tmpSession);
                GetScenarioInfo(tmpSession);
                countdown.Signal();
            });
            countdown.Signal();
            countdown.Wait();
            InitInfo();
        }
    }
}
