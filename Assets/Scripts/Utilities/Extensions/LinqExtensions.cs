using System.Collections.Generic;
using JetBrains.Annotations;

// Needs to be in a sub-namespace so we don't conflict with plugins that have the same extensions
namespace System.Linq.Extensions
{
  public static class LinqExtensions
  {
    [Pure]
    public static bool IsEmpty<T>([NotNull] this IEnumerable<T> source)
      => !source.Any();

    [Pure]
    public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> source)
      => source == null || source.IsEmpty();

    [Pure]
    public static bool None<T>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, bool> condition)
      => !source.Any(condition);

    [Pure]
    public static bool HasAtLeast<T>([NotNull] this IEnumerable<T> source, int amount)
      => source.Take(amount).Count() == amount;

    [Pure]
    public static bool HasMoreThan<T>([NotNull] this IEnumerable<T> source, int amount)
      => source.HasAtLeast(amount + 1);

    [Pure]
    public static bool HasAtMost<T>([NotNull] this IEnumerable<T> souce, int amount)
      => souce.Take(amount + 1).Count() <= amount;

    [Pure]
    public static bool HasLessThan<T>([NotNull] this IEnumerable<T> source, int amount)
      => source.HasAtMost(amount - 1);

    [Pure]
    public static bool HasSingle<T>([NotNull] this IEnumerable<T> source)
      => source.HasAtMost(1);

    [Pure]
    public static bool HasMultiple<T>([NotNull] this IEnumerable<T> source)
      => source.HasAtLeast(2);

    public static void ForEach<T>([NotNull] this IEnumerable<T> source, [CanBeNull] Action<T> action)
    {
      foreach (var item in source)
        action?.Invoke(item);
    }

    [NotNull]
    public static IEnumerable<T> Do<T>([NotNull] this IEnumerable<T> source, [CanBeNull] Action<T> action)
    {
      foreach (var item in source)
      {
        action?.Invoke(item);
        yield return item;
      }
    }

    [Pure, CanBeNull]
    public static TSource Lowest<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TKey> selector)
      => source.Lowest(selector, Comparer<TKey>.Default);

    [Pure, CanBeNull]
    public static TSource Lowest<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TKey> selector, [NotNull] IComparer<TKey> comparer)
      => source.Aggregate((lowest, current)
        => comparer.Compare(selector(current), selector(lowest)) < 0
          ? current : lowest);

    [Pure, CanBeNull]
    public static TSource Highest<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TKey> selector)
      => source.Highest(selector, Comparer<TKey>.Default);

    [Pure, CanBeNull]
    public static TSource Highest<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TKey> selector, [NotNull] IComparer<TKey> comparer)
      => source.Aggregate((highest, current)
        => comparer.Compare(selector(current), selector(highest)) > 0
          ? current : highest);

    [NotNull]
    public static IEnumerable<T> Shuffle<T>([NotNull] this IEnumerable<T> source)
      => source.OrderBy(x => Guid.NewGuid());

    [Pure, NotNull]
    public static T GetRandom<T>([NotNull] this IList<T> source)
      => source[UnityEngine.Random.Range(0, source.Count)];

    [NotNull]
    public static T Remove<T>([NotNull] this IList<T> source, [NotNull] Func<T, bool> predicate)
    {
      var item = source.Single(predicate);

      source.Remove(item);

      return item;
    }

    [NotNull]
    public static T Pop<T>([NotNull] this IList<T> source)
    {
      var lastIndex = source.Count - 1;
      var lastItem = source[lastIndex];

      source.RemoveAt(lastIndex);

      return lastItem;
    }

    [NotNull]
    public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> source, TKey key, Func<TValue> factory)
      => source.GetOrAdd(key, k => factory());

    [NotNull]
    public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> factory)
    {
      if (!source.ContainsKey(key) || source[key] == null)
        source[key] = factory.Invoke(key);

      return source[key];
    }
  }
}