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
    public class aSinkBLL
    {
        public tbSink GetModel(string streamName, int deviceId)
        {
            return UOMSingle.currentPlant.DataContext.tbSink.FirstOrDefault(p => p.Streamname == streamName && p.DeviceID == deviceId);
        }
        public void SaveSink(tbSink dbmodel)
        {
            UOMSingle.currentPlant.DataContext.tbSink.Update(p => dbmodel, p => p.Id == dbmodel.Id);
            UOMSingle.currentPlant.DataContext.SubmitChanges();
        }
    }
}
