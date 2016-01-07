namespace System
{
  public static class ObjectExtensions
  {
    public static int ToInt(this bool value)
      => value ? 1 : 0;
  }
}