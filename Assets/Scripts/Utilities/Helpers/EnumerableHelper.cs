using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PachowStudios
{
  public static class EnumerableHelper
  {
    [NotNull, Pure]
    public static IEnumerable<T> Repeat<T>([NotNull] Func<T> factory, int count)
    {
      for (var i = 0; i < count; i++)
        yield return factory();
    }
  }
}