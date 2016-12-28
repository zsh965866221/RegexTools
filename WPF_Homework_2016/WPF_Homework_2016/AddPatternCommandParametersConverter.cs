using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPF_Homework_2016
{
    class AddPatternCommandParametersConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            AddPatternCommandParameters parameters=new AddPatternCommandParameters();
            if (values.Length == 2)
            {
                parameters.RegexText = values[0] as string;
                parameters.RegexType = values[1] as string;
            }

            return parameters;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
