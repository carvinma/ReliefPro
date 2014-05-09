using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProDAL.Common;
using ReliefProModel;
using System.Data;
using NHibernate.Criterion;
using ReliefProModel.Drum;

namespace ReliefProDAL
{
    public class dbDrum: IBaseDAL<ReliefProModel.Drum.Drum>
    {
       
        public ReliefProModel.Drum.Drum GetModel(ISession session)
        {
            ReliefProModel.Drum.Drum model = null;
            IList<ReliefProModel.Drum.Drum> list = null;
            try
            {
                list = session.CreateCriteria<ReliefProModel.Drum.Drum>().List<ReliefProModel.Drum.Drum>();
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
