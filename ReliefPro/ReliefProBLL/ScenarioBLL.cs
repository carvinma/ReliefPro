using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;
using ReliefProModel.Compressors;
using ReliefProModel.Drums;
using ReliefProModel.HXs;
using ReliefProModel.ReactorLoops;
using ReliefProModel.Towers;

using System.Collections.ObjectModel;
using ReliefProDAL;
using ReliefProDAL.Common;
using ReliefProDAL.Compressors;
using ReliefProDAL.Drums;
using ReliefProDAL.HXs;
using ReliefProDAL.ReactorLoops;
using ReliefProDAL.Towers;

using NHibernate;

namespace ReliefProBLL
{
    public class ScenarioBLL
    {
        ISession SessionProtectedSystem;

        public  ScenarioBLL(ISession SessionProtectedSystem)
        {
            this.SessionProtectedSystem = SessionProtectedSystem;
        }

        //重新导入的时候，需要所有删除信息.
       public void DeleteScenario()
        {
            var sql = " from ReliefProModel.Scenario ";
            SessionProtectedSystem.Delete(sql);
            SessionProtectedSystem.Flush();
        }

        //删除所有相关ICON的表，同时，清空tbScenario
        public void ClearScenario()
        {
            ScenarioDAL scenarioDAL = new ScenarioDAL();
            IList<Scenario> scList = scenarioDAL.GetAllList(SessionProtectedSystem);
            int count=scList.Count;
            for(int i=0;i<count;i++)
            {
                Scenario sc = scList[i];
                sc.dbPath = string.Empty;
                sc.Flooding = false;
                sc.IsSurgeCalculation = false;
                sc.Phase = string.Empty;
                sc.ReliefCpCv = 0;
                sc.ReliefLoad = 0;
                sc.ReliefMW = 0;
                sc.ReliefPressure = 0;
                sc.ReliefTemperature = 0;
                sc.ReliefVolumeRate = 0;
                sc.ReliefZ = 0;
                sc.SurgeTime = 0;
                scenarioDAL.Update(sc, SessionProtectedSystem);
            }
            
        }

        public void DeleteSCOther()
        {
            var sql = " from ReliefProModel.Compressors.CentrifugalBlockedOutlet ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Compressors.PistonBlockedOutlet ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.Drums.DrumBlockedOutlet ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Drums.DrumDepressuring ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Drums.DrumFireCalc ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Drums.DrumFireFluid ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Drums.DrumSize ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.HXs.AirCooledHXFireSize ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.HXs.HXBlockedIn ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.HXs.HXFireSize ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.HXs.TubeRupture ";
            SessionProtectedSystem.Delete(sql);


            sql = " from ReliefProModel.ReactorLoops.GeneralFailureCommon ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.ReactorLoops.GeneralFailureCommonDetail ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.ReactorLoops.ReactorLoopCommon ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.ReactorLoops.ReactorLoopDetail ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.Towers.BlockedVaporOutlet ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.AbnormalHeaterDetail ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.CondenserCalc ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.FeedBottomHX ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.FlashCalcResult ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.InletValveOpen ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.ReboilerPinch ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.ScenarioHeatSource ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.TowerFire ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerFireColumn ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerFireColumnDetail ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.TowerFireCooler ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerFireDrum ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerFireEq ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerFireHX ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerScenarioHX ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerScenarioStream ";
            SessionProtectedSystem.Delete(sql);

            SessionProtectedSystem.Flush();
        }

        
    }
}
