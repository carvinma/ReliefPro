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
    public class ReImportBLL
    {
        ISession SessionProtectedSystem;

        public ReImportBLL(ISession SessionProtectedSystem)
        {
            this.SessionProtectedSystem = SessionProtectedSystem;
        }

        //重新导入的时候，需要所有删除信息.
        public void DeleteAllData()
        {
            DeleteEqInfo();
            ScenarioBLL scBLL = new ScenarioBLL(SessionProtectedSystem);
            scBLL.DeleteSCOther();
            scBLL.DeleteScenario();

            PSVBLL psvbll = new PSVBLL(SessionProtectedSystem);
            psvbll.DeletePSVData();
        }

        public void DeleteEqInfo()
        {
            var sql = " from ReliefProModel.Drums.Drum ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.HXs.HeatExchanger ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.ReactorLoops.ReactorLoop ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Accumulator ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Compressor ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.CustomStream ";
            SessionProtectedSystem.Delete(sql);
            //sql = " from ReliefProModel.FlashCalcResult ";
            //SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.HeatSource ";
            SessionProtectedSystem.Delete(sql);
            //sql = " from ReliefProModel.Latent ";
            //SessionProtectedSystem.Delete(sql);
            //sql = " from ReliefProModel.LatentProduct ";
            //SessionProtectedSystem.Delete(sql);
            //sql = " from ReliefProModel.PSV ";
            //SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.SideColumn ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Sink ";
            SessionProtectedSystem.Delete(sql);

            sql = " from ReliefProModel.Source ";
            SessionProtectedSystem.Delete(sql);
            //sql = " from ReliefProModel.SourceFile ";
            //SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.StorageTank ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.Tower ";
            SessionProtectedSystem.Delete(sql);
            sql = " from ReliefProModel.TowerHX ";
            SessionProtectedSystem.Delete(sql);
            
            SessionProtectedSystem.Flush();
        }
    }
}
