using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using UOMLib;
using NHibernate;
using System.Windows;
using ReliefProCommon.Enum;
using System.ComponentModel.DataAnnotations;
using ReliefProBLL;
using System.Windows.Media.Imaging;

namespace ReliefProMain.ViewModel
{
    public class AboutUsVM : ViewModelBase
    {
        private BitmapImage _Logo;
        public BitmapImage Logo
        {
            get
            {
                return this._Logo;
            }
            set
            {

                OnPropertyChanged("AboutUsVM");
            }
        }



        public AboutUsVM()
        {
            //try
            //{
            //    string imagepath = Environment.CurrentDirectory + @"\Images\app.ico";
            //    Logo = new BitmapImage(new Uri(imagepath, UriKind.Absolute));
            //}
            //catch (Exception ex)
            //{
            //}
            
        }

        

        
    }
}
