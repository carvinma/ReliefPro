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
            return UOMSingle.currentPlantContext.tbDevice.FirstOrDefault(p => p.DeviceType == deviceType && p.DeviceName == deviceName);
        }
        public void SaveCompressor(tbDevice dbmodel, ObservableCollection<tbStream> Feeds, ObservableCollection<tbStream> Products)
        {
            if (dbmodel.Id == 0)
            {
                UOMSingle.currentPlantContext.tbDevice.Insert(dbmodel);
                UOMSingle.currentPlantContext.tbStream.InsertAllOnSubmit(Feeds);
                UOMSingle.currentPlantContext.tbStream.InsertAllOnSubmit(Products);
                ConcurrentBag<tbSource> sourceBag = new ConcurrentBag<tbSource>();
                ConcurrentBag<tbSink> sinkBag = new ConcurrentBag<tbSink>();
                Feeds.AsParallel().ForAll(p => {
                    tbSource sr = new tbSource();
                    sr.Maxpossiblepressure = p.Pressure;
                    sr.Streamname = p.Streamname;
                    sr.Sourcetype = "Pump(Motor)";
                    sr.Sourcename = p.Streamname + "_Source";
                    sourceBag.Add(sr);
                });

                Products.AsParallel().ForAll(p => {
                    tbSink sink = new tbSink();
                    sink.Maxpossiblepressure = p.Pressure;
                    sink.Streamname = p.Streamname;
                    sink.Sinkname = p.Streamname + "_Sink";
                    sink.Sinktype = "Pump(Motor)";
                    sinkBag.Add(sink);
                });
                UOMSingle.currentPlantContext.tbSource.InsertAllOnSubmit(sourceBag);
                UOMSingle.currentPlantContext.tbSink.InsertAllOnSubmit(sinkBag);
                UOMSingle.currentPlantContext.SubmitChanges();
            }
            else
            {
                UOMSingle.currentPlantContext.tbDevice.Update(p => dbmodel, p => p.Id == dbmodel.Id);
            }
        }
    }
}
