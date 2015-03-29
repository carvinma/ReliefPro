using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ReliefProDAL;
using ReliefProDAL.Common;
using ReliefProDAL.Compressors;
using ReliefProDAL.Drums;
using ReliefProDAL.HXs;
using ReliefProDAL.ReactorLoops;
using ReliefProDAL.Towers;
using ReliefProModel;
using NHibernate;
using UOMLib;
using ALinq;

namespace ReliefProBLL
{
    public class aScenarioBLL
    {
        public List<tbScenario> GetList(int deviceId)
        {
            return UOMSingle.currentPlant.DataContext.tbScenario.ToList();
        }

        public tbScenario GetModel(int deviceId)
        {
            return UOMSingle.currentPlant.DataContext.tbScenario.FirstOrDefault(p => p.DeviceID == deviceId);
        }
        public void SaveScenario(tbScenario dbmodel)
        {
            UOMSingle.currentPlant.DataContext.tbScenario.Update(p => dbmodel, p => p.Id == dbmodel.Id);
            UOMSingle.currentPlant.DataContext.SubmitChanges();
        }
       
        //重新导入的时候，需要所有删除信息.
       public void DeleteScenario()
        {
            string sql = "delete from tbScenario";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);
        }

        //删除所有相关ICON的表，同时，清空tbScenario
        public void ClearScenario()
        {
            string sql = "update tbScenario set ReliefLoad=0,ReliefPressure=0,ReliefTemperature=0,ReliefMW=0,ReliefCpCv = 0,ReliefZ=0";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);
            ////ScenarioDAL scenarioDAL = new ScenarioDAL();
            ////IList<tbScenario> scList = scenarioDAL.GetAllList(SessionProtectedSystem);
            ////int count=scList.Count;
            ////for(int i=0;i<count;i++)
            ////{
            ////    Scenario sc = scList[i];
            ////    sc.dbPath = string.Empty;
            ////    sc.Flooding = false;
            ////    sc.IsSurgeCalculation = false;
            ////    sc.Phase = string.Empty;
            ////    sc.ReliefCpCv = 0;
            ////    sc.ReliefLoad = 0;
            ////    sc.ReliefMW = 0;
            ////    sc.ReliefPressure = 0;
            ////    sc.ReliefTemperature = 0;
            ////    sc.ReliefVolumeRate = 0;
            ////    sc.ReliefZ = 0;
            ////    sc.SurgeTime = 0;
            ////    scenarioDAL.Update(sc, SessionProtectedSystem);
            ////}
            
        }

        public void DeleteSCOther()
        {
            string sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);

            sql = "delete from ";
            UOMSingle.currentPlant.DataContext.ExecuteCommand(sql);


            var sql = " from ReliefProModel.Compressors.CentrifugalBlockedOutlet ";
            SessionProtectedSystem.Delete(sql);
            //sql = " from ReliefProModel.Compressors.PistonBlockedOutlet "; // 和CentrifugalBlockedOutlet存储的表一样，所以只需要删除一次即可
            //result = SessionProtectedSystem.Delete(sql);

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
