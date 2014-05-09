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

namespace ReliefProDAL
{
    public class dbProIIStreamData : IBaseDAL<ProIIStreamData>
    {
        

        public ProIIStreamData GetModel(ISession session,string streamName)
        {
            ProIIStreamData model = null;
            IList<ProIIStreamData> list = null;
            try
            {
                list = session.CreateCriteria<ProIIStreamData>().Add(Expression.Eq("StreamName", streamName)).List<ProIIStreamData>();
                if (list.Count > 0)
                {
                    model = list[0];
                }
                else
                    model = null;
            }
            catch (Exception ex)
            {
                model = null;
                throw ex;

            }

            return model;
        }
    }
}
