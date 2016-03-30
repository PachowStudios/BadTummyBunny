using System;

namespace PachowStudios.Assertions
{
  public class NumericAssertion<T> : BaseAssertion<IComparable<T>>
    where T : struct, IComparable<T>
  {
    public NumericAssertion(IComparable<T> subject)
      : base(subject) { }

    public void Be(T value, string reason = null)
      => Assert(Subject.CompareTo(value) == 0, "be", value, reason);

    public void NotBe(T value, string reason = null)
      => Assert(Subject.CompareTo(value) != 0, "not be", value, reason);

    public void BeAtLeast(T value, string reason = null)
      => Assert(Subject.CompareTo(value) >= 0, "be at least", value, reason);

    public void BeGreaterThan(T value, string reason = null)
      => Assert(Subject.CompareTo(value) > 0, "be greater than", value, reason);

    public void BeNoMoreThan(T value, string reason = null)
      => Assert(Subject.CompareTo(value) <= 0, "be no more than", value, reason);

    public void BeLessthan(T value, string reason = null)
      => Assert(Subject.CompareTo(value) < 0, "be less than", value, reason);
  }
}