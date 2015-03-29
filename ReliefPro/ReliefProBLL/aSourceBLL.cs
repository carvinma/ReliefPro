using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALinq;
using ReliefProModel;
using UOMLib;

namespace ReliefProBLL
{
    public class aSourceBLL
    {
        public tbSource GetModel(string streamName, int deviceId)
        {
            return UOMSingle.currentPlant.DataContext.tbSource.FirstOrDefault(p => p.Streamname == streamName && p.DeviceID == deviceId);
        }
        public void SaveSource(tbSource dbmodel)
        {
            UOMSingle.currentPlant.DataContext.tbSource.Update(p => dbmodel, p => p.Id == dbmodel.Id);
            UOMSingle.currentPlant.DataContext.SubmitChanges();
        }
    }
}
