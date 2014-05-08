using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProDAL.Drum;
using ReliefProModel;
using ReliefProModel.Drum;

namespace ReliefProLL
{
    public class DrumBll
    {
        private dbDrumBlockedOutlet dbBlockedOutlet = new dbDrumBlockedOutlet();
        public void SaveDrumBlockedOutlet(DrumBlockedOutlet model)
        {
            using (var helper = new NHibernateHelper(""))
            {
                var Session = helper.GetCurrentSession();
                dbBlockedOutlet.SaveDrumBlockedOutlet(Session, model);
            }
        }
        public DrumBlockedOutlet GetBlockedOutletModel(int drumID)
        {
            DrumBlockedOutlet Model = new DrumBlockedOutlet();
            using (var helper = new NHibernateHelper(""))
            {
                var Session = helper.GetCurrentSession();
                var tmpModel = dbBlockedOutlet.GetModelByDrumID(Session, drumID);
                if (tmpModel != null)
                {
                    Model = tmpModel;
                    return Model;
                }
                dbSource dbSource = new dbSource();
                List<Source> listSource = dbSource.GetAllList(Session).ToList();
                if (listSource.Count() > 0)
                {
                    double MaxPressure = 0;
                    if (double.TryParse(listSource.First().MaxPossiblePressure, out MaxPressure))
                    {
                        Model.MaxPressure = MaxPressure;
                    }
                }
                dbTowerScenario towerScenario = new dbTowerScenario();
                List<TowerScenario> listTowerScenario = towerScenario.GetAllList(Session).ToList();
                if (listTowerScenario.Count() > 0)
                {
                    double MaxStreamRate = 0;
                    if (double.TryParse(listTowerScenario.First().ReliefLoad, out MaxStreamRate))
                        Model.MaxStreamRate = MaxStreamRate;
                }
                //Model.DrumType
                //Model.NormalFlashDuty
                dbStream stream = new dbStream();
                var customStream = stream.GetAllList(Session).Where(p => p.IsProduct == true && p.ProdType == "2").FirstOrDefault();
                if (customStream != null)
                {
                }
            }
            return Model;
        }
    }
}
