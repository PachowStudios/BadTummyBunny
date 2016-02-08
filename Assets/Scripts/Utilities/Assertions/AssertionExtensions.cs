namespace PachowStudios.Assertions
{
  public static class AssertionExtensions
  {
    public static ObjectAssertion Should(this object @object)
      => new ObjectAssertion(@object);

    public static NumericAssertion<int> Should(this int value)
      => new NumericAssertion<int>(value);

    public static NumericAssertion<float> Should(this float value)
      => new NumericAssertion<float>(value);

    public static NumericAssertion<double> Should(this double value)
      => new NumericAssertion<double>(value);

    public static BooleanAssertion Should(this bool condition)
      => new BooleanAssertion(condition);
  }
}