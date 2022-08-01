using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPR.MVVM.Converters;

namespace AppLauncher.Infrastructure.Converters
{
    [ValueConversion(typeof(bool), typeof(WindowState))]
    public class BoolToMinimizedWindowStateConverter: ConverterBase
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            return v is true ? WindowState.Minimized : WindowState.Normal;
        }
    }
}
