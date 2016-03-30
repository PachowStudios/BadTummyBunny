using System.Collections.Generic;

namespace PachowStudios.Assertions
{
  public static class AssertionExtensions
  {
    public static ObjectAssertion Should(this object @object)
      => new ObjectAssertion(@object);

    public static NumericAssertion<int> Should(this int number)
      => new NumericAssertion<int>(number);

    public static NumericAssertion<float> Should(this float number)
      => new NumericAssertion<float>(number);

    public static NumericAssertion<double> Should(this double number)
      => new NumericAssertion<double>(number);

    public static BooleanAssertion Should(this bool condition)
      => new BooleanAssertion(condition);

    public static EnumerableAssertion<T> Should<T>(this IEnumerable<T> enumerable)
      => new EnumerableAssertion<T>(enumerable);

    public static CollectionAssertion<T> Should<T>(this ICollection<T> collection)
      => new CollectionAssertion<T>(collection);
  }
}