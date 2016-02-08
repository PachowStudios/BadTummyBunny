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
    {
      if (!condition)
        throw new AssertionFailedException(
          $"{Subject} should {requirement} {value}", reason);
    }
  }
}