using System;

namespace PachowStudios.Assertions
{
  public abstract class BaseAssertion<TSubject, TSelf>
    where TSelf : BaseAssertion<TSubject, TSelf>
  {
    protected TSubject Subject { get; }

    protected BaseAssertion(TSubject subject)
    {
      Subject = subject;
    }

    protected AndConstraint<TSelf> Assert(bool condition, string requirement, TSubject value, string reason = null)
      => Assert(condition, requirement, value?.ToString(), reason);

    protected AndConstraint<TSelf> Assert(bool condition, string requirement, IFormattable value, string reason = null)
      => Assert(condition, requirement, value?.ToString(), reason);

    protected AndConstraint<TSelf> Assert(bool condition, string requirement, string value, string reason = null)
    {
      if (!condition)
        throw new AssertionFailedException(
          $"{Subject?.ToString() ?? "Object"} should {requirement} {value}", reason);

      return new AndConstraint<TSelf>((TSelf)this);
    }
  }
}