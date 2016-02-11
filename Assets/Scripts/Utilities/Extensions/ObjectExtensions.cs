namespace System
{
  public static class ObjectExtensions
  {
    public static int ToInt(this bool value)
      => value ? 1 : 0;

    public static bool RefersTo<T1, T2>(this T1 objectA, T2 objectB)
      where T1 : class
      where T2 : class
      => ReferenceEquals(objectA, objectB);
  }
}