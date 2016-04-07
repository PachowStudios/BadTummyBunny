using JetBrains.Annotations;

namespace System
{
  public static class ObjectExtensions
  {
    [Pure]
    public static int ToInt(this bool value)
      => value ? 1 : 0;

    [Pure]
    public static bool RefersTo<T1, T2>([CanBeNull] this T1 objectA, [CanBeNull] T2 objectB)
      where T1 : class
      where T2 : class
      => ReferenceEquals(objectA, objectB);
  }
}