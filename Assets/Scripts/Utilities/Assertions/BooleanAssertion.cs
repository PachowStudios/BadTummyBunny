namespace PachowStudios.Assertions
{
  public class BooleanAssertion : BaseAssertion<bool>
  {
    public BooleanAssertion(bool subject)
      : base(subject) { }

    public void Be(bool condition, string reason = null)
      => Assert(Subject == condition, "be", condition, reason);

    public void BeTrue(string reason = null)
      => Assert(Subject, "be", true, reason);

    public void BeFalse(string reason = null)
      => Assert(!Subject, "be", false, reason);
  }
}