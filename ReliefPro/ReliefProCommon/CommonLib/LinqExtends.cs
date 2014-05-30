/*
 * Linq扩展类
 * 
 * 业务中用到，用于扩展Linq中缺少的但很实用的方法
 * 
 * 
 * 青格勒
 * 2013-8-23
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProCommon.CommonLib
{
    public static class LinqExtends
    {
        public static IEnumerable<T> Distinct<T, TResult>(this IEnumerable<T> source, Func<T, TResult> func)
        {
            List<T> result = new List<T>();
            bool isSingle = true;
            foreach (var item in source)
            {
                isSingle = true;
                foreach (var it in result)
                {
                    if (func(it).Equals(func(item)))
                    {
                        isSingle = false;
                        break;
                    }
                }
                if (isSingle)
                    result.Add(item);
            }
            return result;
        }
    }
}
