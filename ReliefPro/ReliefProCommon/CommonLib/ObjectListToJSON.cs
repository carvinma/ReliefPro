using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace Common.CommonLib
{
   public class ObjectListToJSON
    {
        private static char START_STRING = '[';
        private static char END_STRING = ']';

        private static char START_PRO_STRING = '{';
        private static char END_PRO_STRING = '}';

        private static char POINT_STRING = ':';
        private static char APART_STRING = ',';



        #region 反射一个对象所有属性和属性值和将一个对象的反射结果封装成jsons格式
        /**
        * 对象的全部属性和属性值。用于填写json的{}内数据
        * 生成后的格式类似
        * "属性1":"属性值"
        * 将这些属性名和属性值写入字符串列表返回
        * */
        private List<string> GetObjectProperty(object o)
        {
            List<string> propertyslist = new List<string>();
            PropertyInfo[] propertys = o.GetType().GetProperties();
            String str = "";
            foreach (PropertyInfo p in propertys)
            {


                if (p.GetValue(o, null) != null && (p.GetValue(o, null).ToString().ToLower() == "true" || p.GetValue(o, null).ToString().ToLower() == "false"))
                {
                    str = "\"" + p.Name.ToString() + "\":" + p.GetValue(o, null).ToString().ToLower();
                }
                else
                {
                    str = "\"" + p.Name.ToString() + "\":\"" + p.GetValue(o, null) + "\"";
                }

                propertyslist.Add(str);
            }
            return propertyslist;
        }

        /// <summary>
        /// 转换对象属性，属性名不带引号,值带引号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private List<string> GetObjectPropertyNoMaks(object o)
        {
            List<string> propertyslist = new List<string>();
            PropertyInfo[] propertys = o.GetType().GetProperties();
            String str = "";
            foreach (PropertyInfo p in propertys)
            {


                if (p.GetValue(o, null) != null && (p.GetValue(o, null).ToString().ToLower() == "true" || p.GetValue(o, null).ToString().ToLower() == "false"))
                {
                    str = "" + p.Name.ToString() + ":" + p.GetValue(o, null).ToString().ToLower();
                }
                else
                {
                    str = "" + p.Name.ToString() + ":\"" + p.GetValue(o, null) + "\"";
                }

                propertyslist.Add(str);
            }
            return propertyslist;
        }


        /**
          * 将一个对象的所有属性和属性值按json的格式要求输入为一个封装后的结果。
          *
          * 返回值类似{"属性1":"属性1值","属性2":"属性2值","属性3":"属性3值"}
          *
          * */
        private string OneObjectToJSON(object o)
        {
            string result = "{";
            List<string> ls_propertys = new List<string>();
            ls_propertys = GetObjectProperty(o);
            foreach (string str_property in ls_propertys)
            {
                if (result.Equals("{"))
                {
                    result = result + str_property;
                }
                else
                {
                    result = result + "," + str_property + "";
                }
            }
            return result + "}";
        }

        /// <summary>
        /// 生成的属性名称不带引号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private String OneObjectToJSONNoMaks(object o)
        {

            string result = "{";
            List<string> ls_propertys = new List<string>();
            ls_propertys = GetObjectPropertyNoMaks(o);
            foreach (string str_property in ls_propertys)
            {
                if (result.Equals("{"))
                {
                    result = result + str_property;
                }
                else
                {

                    result = result + "," + str_property + "";
                }
            }
            return result + "}";

        }


        #endregion
        /**
        * 把对象列表转换成json串
        * */
        public string toJSON(List<object> objlist)
        {
            return toJSON(objlist, string.Empty);
        }

        /// <summary>
        ///  add by st 2012/07/09
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tlist"></param>
        /// <returns></returns>
        public String toJsonNew<T>(IList<T> tlist) where T : new()
        {
            string result = "";
            bool firstline = true;//处理第一行前面不加","号
            foreach (T oo in tlist)
            {
                if (!firstline)
                {
                    result = result + "," + OneObjectToJSON(oo);
                }
                else
                {
                    result = result + OneObjectToJSON(oo) + "";
                    firstline = false;
                }
            }
            return result;
        }



        public string toJSON(List<object> objlist, string classname)
        {
            string result = "";
            result += "[";

            bool firstline = true;//处理第一行前面不加","号
            foreach (object oo in objlist)
            {
                if (!firstline)
                {
                    result = result + "," + OneObjectToJSON(oo);
                }
                else
                {
                    result = result + OneObjectToJSON(oo);
                    firstline = false;
                }
            }
            return result + "]";
        }

        /// <summary>
        /// 将一个对象转换成JSON格式数据
        /// </summary>
        /// <returns></returns>
        public String toJSONbyObject(object o)
        {
            return "[" + OneObjectToJSONNoMaks(o) + "]";
        }

        public String toJSONbyObjectMarks(object o)
        {
            return OneObjectToJSON(o);
        }

        /// <summary>
        /// 将目标集合转换成标准JSON数据
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public String toJSONbyDefault(List<Object[]> objList, int totalCount)
        {
            StringBuilder jsonresult = new StringBuilder("[");
            String str = "";
            StringBuilder result = new StringBuilder();

            if (objList == null)
            {
                // throw new Exception("目标集合为空，无法进行转换!");
                return "(" + START_PRO_STRING + "\"totalCount\"" + POINT_STRING + "\"" + totalCount + "\"" + APART_STRING + " \"records\"" + POINT_STRING + "" + "[]" + "" + END_PRO_STRING + ")";

            }


            Object[] obj = null;
            for (int j = 0; j < objList.Count; j++)
            {
                obj = objList[j];
                if (j != 0)
                {
                    jsonresult.Append(",");
                }
                result.Clear();
                result.Append("{");
                str = "";
                for (int i = 0; i < obj.Length; i++)
                {

                    if (!"".Equals(str))
                    {
                        str += ",";
                    }
                    if (obj[i] != null)
                    {
                        str = str + "\"col_" + i + "\":\"" + obj[i].ToString() + "\"";
                    }
                    else
                    {
                        str = str + "\"col_" + i + "\": \"\"";
                    }

                }
                result.Append(str + "}");
                jsonresult.Append(result.ToString());
            }
            jsonresult.Append("]");
            return "(" + START_PRO_STRING + "\"totalCount\"" + POINT_STRING + "\"" + totalCount + "\"" + APART_STRING + " \"records\"" + POINT_STRING + "" + jsonresult.ToString() + "" + END_PRO_STRING + ")";
        }
        /// <summary>
        /// 将目标集合转换成JSON数据
        /// 形如[{"属性1":"属性1值"},{"属性2":"属性2值"}]
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public String toJSONbyDefault(List<Object[]> objList)
        {
            if (objList == null) throw new Exception("目标集合为空，无法进行转换!");
            String str = "";
            StringBuilder result = new StringBuilder();
            StringBuilder jsonresult = new StringBuilder("[");
            Object[] obj = null;
            for (int j = 0; j < objList.Count; j++)
            {
                obj = objList[j];
                if (j != 0)
                {
                    jsonresult.Append(",");
                }
                result.Clear();
                result.Append("{");
                str = "";
                for (int i = 0; i < obj.Length; i++)
                {

                    if (!"".Equals(str))
                    {
                        str += ",";
                    }
                    if (obj[i] != null)
                    {
                        str = str + "\"col_" + i + "\":\"" + obj[i].ToString() + "\"";
                    }
                    else
                    {
                        str = str + "\"col_" + i + "\": \"\"";
                    }

                }
                result.Append(str + "}");
                jsonresult.Append(result.ToString());
            }
            jsonresult.Append("]");
            return jsonresult.ToString();
        }

        /// <summary>
        /// 转换成EXT 标准JSON 数据格式
        /// 并支持分页
        /// add by st 2011/8/18
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public String toGridbybyHttpProxy(List<object> objlist, int totalCount)
        {
            return "(" + START_PRO_STRING + "\"totalCount\"" + POINT_STRING + "\"" + totalCount + "\"" + APART_STRING + " \"records\"" + POINT_STRING + "" + toJSON(objlist) + "" + END_PRO_STRING + ")";

        }


        public String toGridbybyScriptTagProxy(List<object> objlist, int totalCount)
        {

            return START_PRO_STRING + "\"totalCount\"" + POINT_STRING + "\"" + totalCount + "\"" + APART_STRING + " \"records\"" + POINT_STRING + "" + toJSON(objlist) + "" + END_PRO_STRING;
        }

        public String formResult(bool bl, String msg)
        {
            return START_PRO_STRING + "success" + POINT_STRING + bl.ToString().ToLower() + APART_STRING + "msg" + POINT_STRING + "'" + msg + "'" + END_PRO_STRING;
            //	result ="{success:true , msg:'空指针错误'}";
        }
    }
}
