namespace System
{
  public static class TypeExtensions
  {
    public static bool IsAssignableFrom<T>(this Type parent)
      => parent.IsAssignableFrom(typeof(T));
  }
}