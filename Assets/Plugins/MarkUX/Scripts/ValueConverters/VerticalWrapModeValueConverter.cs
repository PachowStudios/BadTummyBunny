using System;
using UnityEngine;

namespace MarkUX.ValueConverters
{
  public class VerticalWrapModeValueConverter : ValueConverter
  {
    public VerticalWrapModeValueConverter()
    {
      this._type = typeof(VerticalWrapMode);
    }

    public override ConversionResult Convert(object value, ValueConverterContext context)
    {
      if (value == null)
        return base.Convert(value, context);

      if (value.GetType() != typeof(string))
        return ConversionFailed(value);

      var stringValue = (string)value;

      try
      {
        var convertedValue = Enum.Parse(typeof(VerticalWrapMode), stringValue, true);
        return new ConversionResult(convertedValue);
      }
      catch (Exception e)
      {
        return ConversionFailed(value, e);
      }
    }
  }
}
