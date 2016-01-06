using System;

namespace PachowStudios
{
  public class Lazy<T>
    where T : class
  {
    private readonly Func<T> initializer;

    private T value;

    public T Value => this.value ?? (this.value = this.initializer());

    public Lazy(Func<T> initializer)
    {
      this.initializer = initializer;
    }

    public static implicit operator T(Lazy<T> @this)
      => @this.Value;
  }

  public static class Lazy
  {
    public static Lazy<T> From<T>(Func<T> func)
      where T : class
      => new Lazy<T>(func);
  }
}