using System.Text;
using JetBrains.Annotations;

namespace System
{
  public static class StringExtensions
  {
    [ContractAnnotation("null => true")]
    public static bool IsNullOrEmpty([CanBeNull] this string @string)
      => string.IsNullOrEmpty(@string);

    [NotNull]
    public static string Repeat(this char @char, int count)
      => new string(@char, count);

    [NotNull]
    public static string Repeat([NotNull] this string @string, int count)
      => new StringBuilder(@string.Length * count).Insert(0, @string, count).ToString();

    [NotNull]
    public static string StartWith([NotNull] this string @string, string startingString)
      => @string.StartsWith(startingString, StringComparison.OrdinalIgnoreCase)
        ? @string : startingString + @string;
  }
}