using System.Collections.Generic;
using JetBrains.Annotations;

namespace MarkLight
{
  public static class MarkLightExtensions
  {
    [Pure]
    public static ObservableList<T> ToObservableList<T>([NotNull, InstantHandle] this IEnumerable<T> source)
      => new ObservableList<T>(source);
  }
}