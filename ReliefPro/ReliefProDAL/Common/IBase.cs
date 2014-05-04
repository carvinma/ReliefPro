using System;
using NHibernate;


namespace ReliefProDAL.Common
{
    public interface IBase<A>
    {

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        bool Exists(Object mainId, ISession session);
        /// <summary>
        /// 增加一条数据
        /// </summary>
        Object Add(A model, ISession session);

        /// <summary>
        /// 增加或修改一条数据
        /// </summary>
        void AddOrUpdate(A model, ISession session);
        /// <summary>
        /// 更新一条数据
        /// </summary>
        void Update(A model, ISession session);
        /// <summary>
        /// 删除一条数据
        /// </summary>
        void Delete(A model, ISession session);
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        A GetModel(Object mainId, ISession session);


    }
}
