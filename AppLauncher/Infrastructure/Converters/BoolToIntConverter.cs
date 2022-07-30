using System;
using System.Globalization;
using System.Windows.Data;
using WPR.MVVM.Converters;

namespace AppLauncher.Infrastructure.Converters
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class BoolToIntConverter : ConverterBase
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            var obj = (bool)v;
            return obj ? 1 : 0;
        }
    }
}
