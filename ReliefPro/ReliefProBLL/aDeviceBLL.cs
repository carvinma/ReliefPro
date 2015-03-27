using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using ALinq;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using UOMLib;

namespace ReliefProBLL
{
    public class aDeviceBLL
    {

        public tbDevice GetModel(int deviceType,string deviceName)
        {
            return UOMSingle.currentPlant.DataContext.tbDevice.FirstOrDefault(p => p.DeviceType == deviceType && p.DeviceName == deviceName);
        }
        public void SaveCompressor(tbDevice dbmodel, ObservableCollection<tbStream> Feeds, ObservableCollection<tbStream> Products)
        {
            if (dbmodel.Id == 0)
            {
                UOMSingle.currentPlantContext.tbDevice.Insert(dbmodel);

                ConcurrentBag<tbStream> feedBag = new ConcurrentBag<tbStream>();
                ConcurrentBag<tbStream> productBag = new ConcurrentBag<tbStream>();

                ConcurrentBag<tbSource> sourceBag = new ConcurrentBag<tbSource>();
                ConcurrentBag<tbSink> sinkBag = new ConcurrentBag<tbSink>();
                Feeds.AsParallel().ForAll(p => {
                    tbStream s = p;
                    s.DeviceID = dbmodel.Id;
                    feedBag.Add(s);

                    tbSource sr = new tbSource();
                    sr.Maxpossiblepressure = p.Pressure;
                    sr.Streamname = p.Streamname;
                    sr.Sourcetype = "Pump(Motor)";
                    sr.Sourcename = p.Streamname + "_Source";
                    sr.DeviceID = dbmodel.Id;                    
                    sourceBag.Add(sr);
                });

                Products.AsParallel().ForAll(p => {
                    tbStream s = p;
                    s.DeviceID = dbmodel.Id;
                    productBag.Add(s);

                    tbSink sink = new tbSink();
                    sink.Maxpossiblepressure = p.Pressure;
                    sink.Streamname = p.Streamname;
                    sink.Sinkname = p.Streamname + "_Sink";
                    sink.Sinktype = "Pump(Motor)";
                    sink.DeviceID = dbmodel.Id;
                    sinkBag.Add(sink);
                });
                UOMSingle.currentPlantContext.tbStream.InsertAllOnSubmit(feedBag);
                UOMSingle.currentPlantContext.tbStream.InsertAllOnSubmit(productBag);
                UOMSingle.currentPlantContext.tbSource.InsertAllOnSubmit(sourceBag);
                UOMSingle.currentPlantContext.tbSink.InsertAllOnSubmit(sinkBag);
                UOMSingle.currentPlantContext.SubmitChanges();

            }
            else
            {
                UOMSingle.currentPlant.DataContext.tbDevice.Update(p => dbmodel, p => p.Id == dbmodel.Id);
            }
        }
    
    
    }
}
