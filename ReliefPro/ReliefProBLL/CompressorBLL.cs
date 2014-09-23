using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel;

using System.Collections.ObjectModel;
using ReliefProDAL;
using NHibernate;
using ReliefProDAL.Compressors;
using ReliefProModel.Compressors;

namespace ReliefProBLL
{
    public class CompressorBLL
    {
        private ISession SessionPF;
        private ISession SessionPS;
        CompressorDAL compressordal = new CompressorDAL();
        CustomStreamDAL csdal = new CustomStreamDAL();
        SourceDAL sourcedal = new SourceDAL();
        SinkDAL sinkdal = new SinkDAL();
        ProtectedSystemDAL psdal = new ProtectedSystemDAL();

        public CompressorBLL(ISession SessionPF, ISession SessionPS)
        {
            this.SessionPF = SessionPF;
            this.SessionPS = SessionPS;
        }
        public Compressor GetModel()
        {
            Compressor model = null;
            model = compressordal.GetModel(SessionPS);
            return model;
        }

        public void Save(Compressor dbmodel,ObservableCollection<CustomStream> Feeds,ObservableCollection<CustomStream> Products,ProtectedSystem ps )
        {
            if (dbmodel.ID == 0)
            {
                compressordal.Save(SessionPS, dbmodel);

                psdal.Add(ps, SessionPS);
                foreach (CustomStream cs in Feeds)
                {
                    Source sr = new Source();
                    sr.MaxPossiblePressure = cs.Pressure;
                    sr.StreamName = cs.StreamName;
                    sr.SourceType = "Pump(Motor)";
                    sr.SourceName = cs.StreamName + "_Source";
                    sourcedal.Add(sr, SessionPS);
                    csdal.Add(cs, SessionPS);
                }

                foreach (CustomStream cs in Products)
                {
                    Sink sink = new Sink();
                    sink.MaxPossiblePressure = cs.Pressure;
                    sink.StreamName = cs.StreamName;
                    sink.SinkName = cs.StreamName + "_Sink";
                    sink.SinkType = "Pump(Motor)";

                    sinkdal.Add(sink, SessionPS);
                    csdal.Add(cs, SessionPS);
                }
            }
            else
            {
                compressordal.Save(SessionPS, dbmodel);
            }
        }
        
    }
}
