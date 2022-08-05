using System;
using System.Globalization;
using System.Windows.Data;
using AppLauncher.ViewModels;
using WPR.MVVM.Converters;

namespace AppLauncher.Infrastructure.Converters
{
    [ValueConversion(typeof(WindowsStartPosition), typeof(string))]
    public class WindowStartPositionToStringConverter : ConverterBase
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            var value = (WindowsStartPosition)v;
            return value switch
            {
                WindowsStartPosition.Center => "По центру экрана",
                WindowsStartPosition.BottomCenter => "Снизу по центру",
                WindowsStartPosition.BottomLeft => "Снизу слева",
                _ => null,
            };
        }
    }
}
