namespace PachowStudios.Assertions
{
  public abstract class BaseAssertion<T>
  {
    protected T Subject { get; }

    protected BaseAssertion(T subject)
    {
      Subject = subject;
    }

    protected void Assert(bool condition, string requirement, T value, string reason = null)
      => Assert(condition, requirement, value?.ToString(), reason);

    protected void Assert(bool condition, string requirement, string value, string reason = null)
    {
      if (!condition)
        throw new AssertionFailedException(
          $"{Subject?.ToString() ?? "Object"} should {requirement} {value}", reason);
    }
  }
}