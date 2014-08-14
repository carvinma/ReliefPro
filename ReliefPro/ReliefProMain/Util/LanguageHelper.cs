using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ReliefProMain.Util
{
    public class LanguageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="languagefileName"></param>
        public static void LoadLanguageFile(string languagefileName)
        {
            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
            };
        }

        public static void LoadEnUS()
        {
            LanguageHelper.LoadLanguageFile("pack://siteOfOrigin:,,,/Resources/Langs/en-US.xaml");
        }
        public static void LoadZhCN()
        {
            LanguageHelper.LoadLanguageFile("pack://siteOfOrigin:,,,/Resources/Langs/zh-CN.xaml");
        }
        /// <summary>
        /// 根据key获取value,如果在资源文件中不存在"Key:,那么将返回空的字符串
        /// </summary>
        /// <param name="Key">Key</param>
        /// <returns>Value</returns>
        public static string GetValueByKey(string Key)
        {
            if (Application.Current.Resources.MergedDictionaries[0].Contains(Key))
            {
                return Application.Current.Resources.MergedDictionaries[0][Key].ToString();
            }
            return "";
        }
    }
}
