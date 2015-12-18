using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System
{
  public static class AttributeExtensions
  {
    public static T GetAttributeOfType<T>(this PropertyInfo propertyInfo, bool inherit = false)
    => (T)propertyInfo.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();

    public static T GetAttributeOfType<T>(this Enum source)
      where T : Attribute
      => (T)source
        .GetType()
        .GetMember(source.ToString()).First()
        .GetCustomAttributes(typeof(T), false).FirstOrDefault();

    public static string GetDescription(this Enum source)
      => source.GetAttributeOfType<DescriptionAttribute>()?.Description
      ?? string.Empty;

    public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type, bool inherit = false)
      where T : Attribute
      => type
        .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
        .Where(p => Attribute.IsDefined(p, typeof(T), inherit))
        .ToList();
  }
}