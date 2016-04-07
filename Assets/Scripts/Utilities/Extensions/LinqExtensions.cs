using System.Collections.Generic;
using JetBrains.Annotations;

// Needs to be in a sub-namespace so we don't conflict with plugins that have the same extensions
namespace System.Linq.Extensions
{
  public static class LinqExtensions
  {
    [Pure]
    public static bool IsEmpty<T>([NotNull, InstantHandle] this IEnumerable<T> source)
      => !source.Any();

    [Pure, ContractAnnotation("null => true")]
    public static bool IsNullOrEmpty<T>([CanBeNull, InstantHandle] this IEnumerable<T> source)
      => source == null || source.IsEmpty();

    [Pure]
    public static bool None<T>(
      [NotNull, InstantHandle] this IEnumerable<T> source,
      [NotNull, InstantHandle] Func<T, bool> condition)
      => !source.Any(condition);

    [Pure]
    public static bool HasAtLeast<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.Take(amount).Count() == amount;

    [Pure]
    public static bool HasMoreThan<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.HasAtLeast(amount + 1);

    [Pure]
    public static bool HasAtMost<T>([NotNull, InstantHandle] this IEnumerable<T> souce, int amount)
      => souce.Take(amount + 1).Count() <= amount;

    [Pure]
    public static bool HasLessThan<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.HasAtMost(amount - 1);

    [Pure]
    public static bool HasExactly<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.Take(amount + 1).Count() == amount;

    [Pure]
    public static bool HasSingle<T>([NotNull, InstantHandle] this IEnumerable<T> source)
      => source.HasExactly(1);

    [Pure]
    public static bool HasMultiple<T>([NotNull, InstantHandle] this IEnumerable<T> source)
      => source.HasAtLeast(2);

    public static void ForEach<T>(
      [NotNull, InstantHandle] this IEnumerable<T> source,
      [CanBeNull, InstantHandle] Action<T> action)
    {
      foreach (var item in source)
        action?.Invoke(item);
    }

    [NotNull, LinqTunnel]
    public static IEnumerable<T> Do<T>([NotNull] this IEnumerable<T> source, [CanBeNull] Action<T> action)
    {
      foreach (var item in source)
      {
        action?.Invoke(item);
        yield return item;
      }
    }

    [CanBeNull, Pure]
    public static TSource Lowest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector)
      => source.Lowest(selector, Comparer<TKey>.Default);

    [CanBeNull, Pure]
    public static TSource Lowest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector,
      [NotNull, InstantHandle] IComparer<TKey> comparer)
      => source.Aggregate((lowest, current)
        => comparer.Compare(selector(lowest), selector(current)) < 0
          ? lowest : current);

    [CanBeNull, Pure]
    public static TSource Highest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector)
      => source.Highest(selector, Comparer<TKey>.Default);

    [CanBeNull, Pure]
    public static TSource Highest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector,
      [NotNull, InstantHandle] IComparer<TKey> comparer)
      => source.Aggregate((highest, current)
        => comparer.Compare(selector(highest), selector(current)) > 0
          ? highest : current);

    [NotNull, Pure, LinqTunnel]
    public static IEnumerable<T> Shuffle<T>([NotNull] this IEnumerable<T> source)
      => source.OrderBy(x => Guid.NewGuid());

    public static void Add<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] IEnumerable<T> items)
      => items.ForEach(source.Add);

    [NotNull]
    public static T SingleOrAdd<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] Func<T, bool> predicate)
      where T : class, new()
      => source.SingleOrAdd(predicate, () => new T());

    [NotNull]
    public static T SingleOrAdd<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] Func<T, bool> predicate,
      [NotNull, InstantHandle] Func<T> factory)
      where T : class
    {
      var result = source.SingleOrDefault(predicate);

      if (result == null)
        source.Add(result = factory());

      return result;
    }

    [CanBeNull]
    public static T RemoveSingle<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] Func<T, bool> predicate)
    {
      var item = source.SingleOrDefault(predicate);

      if (Equals(item, default(T)))
        source.Remove(item);

      return item;
    }

    public static void ReplaceAll<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] IEnumerable<T> items)
    {
      source.Clear();
      source.Add(items);
    }

    [CanBeNull, Pure]
    public static T GetRandom<T>([NotNull] this IList<T> source)
      => source[UnityEngine.Random.Range(0, source.Count)];

    [CanBeNull]
    public static T Pop<T>([NotNull] this IList<T> source)
    {
      var lastIndex = source.Count - 1;
      var lastItem = source[lastIndex];

      source.RemoveAt(lastIndex);

      return lastItem;
    }

    [NotNull]
    public static TValue GetOrAdd<TKey, TValue>(
      [NotNull] this IDictionary<TKey, TValue> source,
      [NotNull] TKey key,
      [NotNull, InstantHandle] Func<TValue> factory)
      where TValue : class
      => source.GetOrAdd(key, k => factory());

    [NotNull]
    public static TValue GetOrAdd<TKey, TValue>(
      [NotNull] this IDictionary<TKey, TValue> source,
      [NotNull] TKey key,
      [NotNull, InstantHandle] Func<TKey, TValue> factory)
      where TValue : class
    {
      if (!source.ContainsKey(key) || source[key] == null)
        source[key] = factory(key);

      return source[key];
    }
  }
}