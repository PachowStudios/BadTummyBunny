using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using PachowStudios;

namespace System
{
  public static class AttributeExtensions
  {
    public static IEnumerable<MemberInfo> GetMembersWithAttribute<T>(this Type type, bool inherited = false)
      => type
        .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
        .Where(f => Attribute.IsDefined(f, typeof(T), inherited))
        .ToList();

    public static T GetAttributeOfType<T>(this MemberInfo memberInfo, bool inherit = false)
    => (T)memberInfo.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();

    public static T GetAttributeOfType<T>(this Enum @enum)
      where T : Attribute
      => (T)@enum
        .GetType()
        .GetMember(@enum.ToString()).First()
        .GetCustomAttributes(typeof(T), false).FirstOrDefault();

    public static string GetDescription(this Enum @enum)
      => @enum.GetAttributeOfType<DescriptionAttribute>()?.Description
      ?? string.Empty;

    public static Type GetTypeMapping(this Enum @enum)
      => @enum.GetAttributeOfType<TypeMappingAttribute>().Type;
  }
}