using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel;
using System.ComponentModel;
using ALinq;

namespace UOMLib
{
    public class UnitInfo
    {
        private readonly string dbConnectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\plant.mdb";

        private ORDesignerPlantDataContext plantContext = new ORDesignerPlantDataContext();
        public IList<systbBasicUnit> GetBasicUnit()
        {
            var query = from q in plantContext.systbBasicUnit select q;
            return query.ToList();
        }
        public systbBasicUnit GetBasicUnitUOM()
        {
            return plantContext.systbBasicUnit.First(p => p.IsDefault == 1);
        }
        public IList<systbBasicUnitDefault> GetBasicUnitDefault()
        {
            var query = from q in plantContext.systbBasicUnitDefault select q;
            return query.ToList();
        }

        public IList<systbBasicUnitCurrent> GetBasicUnitCurrent()
        {
            var query = from q in plantContext.systbBasicUnitCurrent select q;
            return query.ToList();
        }

        public IList<systbSystemUnit> GetSystemUnit()
        {
            var query = from q in plantContext.systbSystemUnit select q;
            return query.ToList();
        }
        public IList<systbUnitType> GetUnitType()
        {
            var query = from q in plantContext.systbUnitType select q;
            return query.ToList();
        }
        public int BasicUnitAdd(systbBasicUnit model)
        {
            //对于Insert方法，如果成功插入数据，如果主键是自增长的整数，则返回主键的值，否则返回1，如果失败则返回0。
            int id = plantContext.systbBasicUnit.Insert(model);
            return id;
        }
        public void BasicUnitDel(systbBasicUnit model)
        {
            plantContext.systbBasicUnitDefault.Delete(p => p.BasicUnitID == model.Id);
            plantContext.systbBasicUnit.Delete(p => p.Id == model.Id);
        }
        public int BasicUnitSetDefault(int id)
        {
            string sql = "update tbBasicUnit a set IsDefault=0 where a.ID<>{0}";
            string sql2 = "update tbBasicUnit a set IsDefault=1 where a.ID=:ID";
            try
            {
                int i = plantContext.ExecuteCommand(sql, new object[] { id });
                i = plantContext.ExecuteCommand(sql2, new object[] { id });
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public int BasicUnitSetDefault(int id)
        //{
        //    string sql = "update tbBasicUnit a set IsDefault=0 where a.ID<>:ID";
        //    string sql2 = "update tbBasicUnit a set IsDefault=1 where a.ID=:ID";

        //    try
        //    {
        //        int i = plantContext.ExecuteCommand(sql, new object[] { id });
        //        i = plantContext.ExecuteCommand(sql2, new object[] { id });
        //        return i;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public void Save(IList<systbBasicUnitDefault> lst)
        {
            try
            {
                plantContext.systbBasicUnitDefault.InsertAllOnSubmit(lst);
                plantContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void SaveCurrent(IList<systbBasicUnitCurrent> lst)
        {
            try
            {
                plantContext.systbBasicUnitCurrent.Delete(p => p.Id > 0);
                plantContext.systbBasicUnitCurrent.InsertAllOnSubmit(lst);
                plantContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
