using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReliefProCommon
{
    /// <summary>
    /// MIME type helper，有已知的mime类型的值， 参考 http://msdn.microsoft.com/en-us/library/ms775147.aspx
    /// 貌似能把这些值放到数据库里好点？。。 算了 随意吧
    /// </summary>
    public static class MimeTypeHelper
    {
        #region 文本类型
        public static string TextPlain
        {
            get { return "text/plain"; }
        }

        public static string TextHtml
        {
            get { return "text/html"; }
        }

        public static string TextXml
        {
            get { return "text/xml"; }
        }

        public static string TextRichtext
        {
            get { return "text/richtext"; }
        }

        public static string TextScriptlet
        {
            get { return "text/scriptlet"; }
        }        
        #endregion

        #region 音频类型
        public static string AudioXaiff
        {
            get { return "audio/x-aiff"; }
        }

        public static string AudioBasic
        {
            get { return "audio/basic"; }
        }

        public static string AudioMid
        {
            get { return "audio/mid"; }
        }

        public static string AudioWav
        {
            get { return "audio/wav"; }
        }
        #endregion

        #region 图片类型
        public static string ImageGif
        {
            get { return "image/gif"; }
        }

        public static string ImageJpeg
        {
            get { return "image/jpeg"; }
        }

        public static string ImagePng
        {
            get { return "image/png"; }
        }

        public static string ImageBmp
        {
            get { return "image/bmp"; }
        }
        #endregion

        #region binary类型
        /// <summary>
        /// pdf格式
        /// </summary>
        public static string ApplicationPdf
        {
            get { return "application/pdf"; }
        }

        /// <summary>
        /// word格式
        /// </summary>
        public static string ApplicationWord
        {
            get { return "application/msword"; }
        }

        /// <summary>
        /// excel格式
        /// </summary>
        public static string ApplicationExcel
        {
            get { return "application/vnd.ms-excel"; }
        }

        /// <summary>
        /// 二进制文件 默认类型
        /// </summary>
        public static string ApplicationOctetStream
        {
            get { return "application/octet-stream"; }
        }
        #endregion

        /// <summary>
        /// 默认类型
        /// </summary>
        public static string DefaultType
        {
            get { return ApplicationOctetStream; }
        }

        /// <summary>
        /// 根据文件扩展名取mime类型
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static string GetMimeType(string fileExtension)
        {
            fileExtension = fileExtension.ToLower();

            string mimeType = string.Empty;

            #region 选择mime类型
            switch (fileExtension)
            {
                    //word
                case "doc":
                    mimeType = ApplicationWord;
                    break;
                case "docx":
                    mimeType = ApplicationWord;
                    break;

                    //excel
                case "xls":
                    mimeType = ApplicationExcel;
                    break;
                case "xlsx":
                    mimeType = ApplicationExcel;
                    break;

                    //pdf
                case "pdf":
                    mimeType = ApplicationPdf;
                    break;

                    //images
                case "jpg":
                    mimeType = ImageJpeg;
                    break;

                case "jpeg":
                    mimeType = ImageJpeg;
                    break;

                case "gif":
                    mimeType = ImageGif;
                    break;

                case "bmp":
                    mimeType = ImageBmp;
                    break;

                case "png":
                    mimeType = ImagePng;
                    break;

                    //txt
                case "txt":
                    mimeType = TextPlain;
                    break;

                case "html":
                    mimeType = TextHtml;
                    break;

                case "htm":
                    mimeType = TextHtml;
                    break;

                case "xml":
                    mimeType = TextXml;
                    break;

                case "rtf":
                    mimeType = TextRichtext;
                    break;

                default:
                    mimeType = DefaultType;
                    break;
            }
            #endregion

            return mimeType;
        }
    }
}
