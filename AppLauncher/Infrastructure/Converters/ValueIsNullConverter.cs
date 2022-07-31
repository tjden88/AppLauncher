using System;
using System.Globalization;
using System.Windows.Data;
using WPR.MVVM.Converters;

namespace AppLauncher.Infrastructure.Converters;

/// <summary>
/// Истина, если значение - пустая ссылка
/// </summary>

[ValueConversion(typeof(object), typeof(bool))]
public class ValueIsNullConverter : ConverterBase
{
    protected override object Convert(object v, Type t, object p, CultureInfo c) => v == null;
}