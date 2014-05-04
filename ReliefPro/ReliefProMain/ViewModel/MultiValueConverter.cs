using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReliefProMain.ViewModel
{
    public class MultiValueConverter : IMultiValueConverter     
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //string one = values[0] as string;
            //string two = values[1] as string;
            
            //if (!string.IsNullOrEmpty(one) && !string.IsNullOrEmpty(two) && !string.IsNullOrEmpty(three))
            //{
           //     return one + two + three + three4.Title;
           // }
            //return null;
            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
