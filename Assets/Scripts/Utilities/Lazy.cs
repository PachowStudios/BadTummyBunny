using System;

namespace PachowStudios
{
  public class Lazy<T>
    where T : class
  {
    private readonly Func<T> valueFactory;

    private T value;

    // We use the conditional operator instead of the null coalescing operator
    // because MonoBehavior's custom null check doesn't work with it.
    public T Value => this.value != null ? this.value : (this.value = CreateValue());

    public Lazy(Func<T> valueFactory = null)
    {
      this.valueFactory = valueFactory;
    }

    private T CreateValue()
      => this.valueFactory?.Invoke()
      ?? Activator.CreateInstance<T>();

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