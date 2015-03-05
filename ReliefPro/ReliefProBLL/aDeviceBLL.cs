using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using ALinq;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace ReliefProBLL
{
    public class aDeviceBLL
    {
        private ORDesignerPlantDataContext plantContext = new ORDesignerPlantDataContext();

        public tbDevice GetModel(int deviceType,string deviceName)
        {
            return plantContext.tbDevice.FirstOrDefault(p => p.DeviceType == deviceType && p.DeviceName==deviceName);
        }
        public void SaveCompressor(tbDevice dbmodel, ObservableCollection<tbStream> Feeds, ObservableCollection<tbStream> Products)
        {
            if (dbmodel.Id == 0)
            {
                plantContext.tbDevice.Insert(dbmodel);
                plantContext.tbStream.InsertAllOnSubmit(Feeds);
                plantContext.tbStream.InsertAllOnSubmit(Products);
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
                plantContext.tbSource.InsertAllOnSubmit(sourceBag);
                plantContext.tbSink.InsertAllOnSubmit(sinkBag);
                plantContext.SubmitChanges();
            }
            else
            {
                plantContext.tbDevice.Update(p=>dbmodel,p=>p.Id==dbmodel.Id);
            }
        }
    }
}
