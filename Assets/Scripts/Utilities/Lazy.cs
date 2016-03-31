using System;
using System.Collections.Generic;

namespace PachowStudios
{
  public class Lazy<T>
    where T : class
  {
    private readonly Func<T> valueFactory;

    private T value;

    // This null check is done with EqualityComparer<T>
    // because MonoBehavior's custom null check doesn't work
    // with unconstrained generics...
    public T Value => HasValue ? this.value : (this.value = CreateValue());

    private bool HasValue => !EqualityComparer<T>.Default.Equals(this.value, default(T));

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