using System.Collections.Generic;
using JetBrains.Annotations;

namespace PachowStudios.Assertions
{
  public static class AssertionExtensions
  {
    [Pure]
    public static ObjectAssertion Should([CanBeNull] this object @object)
      => new ObjectAssertion(@object);

    [Pure]
    public static NumericAssertion<int> Should(this int number)
      => new NumericAssertion<int>(number);

    [Pure]
    public static NumericAssertion<float> Should(this float number)
      => new NumericAssertion<float>(number);

    [Pure]
    public static NumericAssertion<double> Should(this double number)
      => new NumericAssertion<double>(number);

    [Pure]
    public static BooleanAssertion Should(this bool condition)
      => new BooleanAssertion(condition);

    [Pure]
    public static IEnumerableAssertion<T> Should<T>([CanBeNull] this IEnumerable<T> enumerable)
      => new IEnumerableAssertion<T>(enumerable);

    [Pure]
    public static ICollectionAssertion<T> Should<T>([CanBeNull] this ICollection<T> collection)
      => new ICollectionAssertion<T>(collection);

    [Pure]
    public static IDictionaryAssertion<TKey, TValue> Should<TKey, TValue>([CanBeNull] this IDictionary<TKey, TValue> dictionary)
      => new IDictionaryAssertion<TKey, TValue>(dictionary);
  }
}