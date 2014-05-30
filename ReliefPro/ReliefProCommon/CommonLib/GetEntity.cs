using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;

namespace ReliefProCommon
{
    public class GetEntity
    {
        private static ArrayList al = null;
        /// <summary>
        /// 返回指定实体对象
        /// </summary>
        /// <param name="cl">需要返回的实体</param>
        /// <param name="ctrl">this</param>
        /// <returns></returns>
        public static object refEntity(object cl, Control ctrl)
        {
            return refEntity(cl, ctrl, "");
        }
        /// <summary>
        /// 返回指定实体对象
        /// </summary>
        /// <param name="cl">需要返回的实体</param>
        /// <param name="ctrl">this</param>
        /// <param name="rep">替换ID指定字符串</param>
        /// <returns></returns>
        public static object refEntity(object cl, Control ctrl, string rep)
        {
            al = new ArrayList();

            getCtrl(ctrl);

            for (int i = 0; i < al.Count; i++)
            {
                switch (al[i].GetType().Name)
                {
                    case "TextBox":
                        TextBox text = (TextBox)al[i];
                        setVlue(text.ID.Replace(rep, ""), text.Text, cl);
                        break;
                    case "HtmlInputText":
                        HtmlInputText Itext = (HtmlInputText)al[i];
                        setVlue(Itext.ID.Replace(rep, ""), Itext.Value, cl);
                        break;
                    case "HtmlTextArea":
                        HtmlTextArea area = (HtmlTextArea)al[i];
                        setVlue(area.ID.Replace(rep, ""), area.Value, cl);
                        break;
                    case "HtmlInputCheckBox":
                        HtmlInputCheckBox checkbox = (HtmlInputCheckBox)al[i];
                        setVlue(checkbox.ID.Replace(rep, ""), checkbox.Checked.ToString(), cl);
                        break;
                    default:
                        break;
                }
            }
            return cl;
        }
        private static void getCtrl(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
            {
                if (c is TextBox)
                {
                    al.Add(c);
                }
                else if (c is HtmlInputText)
                {
                    al.Add(c);
                }
                else if (c is HtmlTextArea)
                {
                    al.Add(c);
                }
                else if (c is HtmlInputCheckBox)
                {
                    al.Add(c);
                }
                else if (c.HasControls())
                {
                    getCtrl(c);
                }
            }
        }
        private static object setVlue(string id, string value, object cl)
        {
            PropertyInfo p = cl.GetType().GetProperty(id);
            string name = "";
            if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                name = p.PropertyType.GetGenericArguments()[0].Name;
            }
            else
            {
                name = p.PropertyType.Name;
            }
            switch (name)
            {
                case "String":
                    cl.GetType().GetProperty(id).SetValue(cl, value, null);
                    break;
                case "Int32":
                    cl.GetType().GetProperty(id).SetValue(cl, int.Parse(value), null);
                    break;
                case "Boolean":
                    cl.GetType().GetProperty(id).SetValue(cl, bool.Parse(value), null);
                    break;
                case "Double":
                    cl.GetType().GetProperty(id).SetValue(cl, double.Parse(value), null);
                    break;
                case "DateTime":
                    cl.GetType().GetProperty(id).SetValue(cl, DateTime.Parse(value), null);
                    break;
                case "Long":
                    cl.GetType().GetProperty(id).SetValue(cl, long.Parse(value), null);
                    break;
                case "Int64":
                    cl.GetType().GetProperty(id).SetValue(cl, long.Parse(value), null);
                    break;
                default:
                    break;
            }
            return cl;
        }

        public static Dictionary<string, object> GetPerporty(object o)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type t = o.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pi.DeclaringType.Name.ToLower() == "PageControllerBase".ToLower())  //base class property
                    continue;
                object value1 = pi.GetValue(o, null);
                string name = pi.Name;
                //if(value1.GetType() == typeof(int))
                dic.Add(pi.Name, value1);

            }
            return dic;
        }
    }
}
