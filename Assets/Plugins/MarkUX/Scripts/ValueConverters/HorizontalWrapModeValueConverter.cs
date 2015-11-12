using System;
using UnityEngine;

namespace MarkUX.ValueConverters
{
  public class HorizontalWrapModeValueConverter : ValueConverter
  {
    public HorizontalWrapModeValueConverter()
    {
      this._type = typeof(HorizontalWrapMode);
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
        var convertedValue = Enum.Parse(typeof(HorizontalWrapMode), stringValue, true);
        return new ConversionResult(convertedValue);
      }
      catch (Exception e)
      {
        return ConversionFailed(value, e);
      }
    }
  }
}
