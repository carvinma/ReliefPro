﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;
using ReliefProModel.Compressors;

namespace ReliefProDAL.Compressors
{
    public class CompressorDAL : IBaseDAL<Compressor>
    {

        public IList<Compressor> GetAllList(ISession session)
        {
            IList<Compressor> list = null;
            try
            {
                list = session.CreateCriteria<Compressor>().List<Compressor>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public Compressor GetModel(ISession session)
        {
            Compressor m = null;
            IList<Compressor> list = null;
            try
            {
                list = session.CreateCriteria<Compressor>().List<Compressor>();
                if (list.Count > 0)
                {
                    m = list[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return m;
        }
    }
}