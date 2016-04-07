using JetBrains.Annotations;

namespace System
{
  public static class TypeExtensions
  {
    [Pure]
    public static bool IsAssignableFrom<T>([NotNull] this Type parent)
      => parent.IsAssignableFrom(typeof(T));
  }
}