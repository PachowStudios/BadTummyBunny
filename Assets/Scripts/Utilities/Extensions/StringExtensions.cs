using System.Text;
using JetBrains.Annotations;

namespace System
{
  public static class StringExtensions
  {
    [Pure, ContractAnnotation("null => true")]
    public static bool IsNullOrEmpty([CanBeNull] this string @string)
      => string.IsNullOrEmpty(@string);

    [NotNull, Pure]
    public static string Repeat([NotNull] this string @string, int count)
      => new StringBuilder(@string.Length * count).Insert(0, @string, count).ToString();

    [NotNull, Pure]
    public static string StartWith([NotNull] this string @string, [NotNull] string startingString)
      => @string.StartsWith(startingString, StringComparison.OrdinalIgnoreCase)
        ? @string : startingString + @string;

    [NotNull, Pure]
    public static string EndWith([NotNull] this string @string, [NotNull] string endingString)
      => @string.EndsWith(endingString, StringComparison.OrdinalIgnoreCase)
        ? @string : @string + endingString;

    [Pure]
    public static T ToEnum<T>([NotNull] this string @string, bool ignoreCase = true)
    {
      try
      {
        return (T)Enum.Parse(typeof(T), @string, ignoreCase);
      }
      catch
      {
        return default(T);
      }
    }
  }
}