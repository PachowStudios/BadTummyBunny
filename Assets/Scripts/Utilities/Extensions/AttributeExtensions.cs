using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using PachowStudios;

namespace System
{
  public static class AttributeExtensions
  {
    [Pure]
    public static IEnumerable<MemberInfo> GetMembersWithAttribute<T>([NotNull] this Type type, bool inherited = false)
      => type
        .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
        .Where(f => Attribute.IsDefined(f, typeof(T), inherited))
        .ToList();

    [Pure]
    public static T GetAttributeOfType<T>([NotNull] this MemberInfo memberInfo, bool inherit = false)
    => (T)memberInfo.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();

    [Pure]
    public static T GetAttributeOfType<T>(this Enum @enum)
      where T : Attribute
      => (T)@enum
        .GetType()
        .GetMember(@enum.ToString()).First()
        .GetCustomAttributes(typeof(T), false).FirstOrDefault();

    [Pure]
    public static string GetDescription(this Enum @enum)
      => @enum.GetAttributeOfType<DescriptionAttribute>()?.Description
      ?? string.Empty;

    [Pure]
    public static Type GetTypeMapping(this Enum @enum)
      => @enum.GetAttributeOfType<TypeMappingAttribute>().Type;
  }
}