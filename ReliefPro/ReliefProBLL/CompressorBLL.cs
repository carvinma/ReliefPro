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
            compressordal.Save(SessionPS, dbmodel);
            foreach (CustomStream cs in Feeds)
            {
                Source sr = new Source();
                sr.MaxPossiblePressure = cs.Pressure;
                sr.StreamName = cs.StreamName;
                sr.SourceType = "Compressor(Motor)";
                sourcedal.Add(sr, SessionPS);
                csdal.Add(cs, SessionPS);
            }

            foreach (CustomStream cs in Products)
            {
                csdal.Add(cs, SessionPS);
            }
        }
        
    }
}
