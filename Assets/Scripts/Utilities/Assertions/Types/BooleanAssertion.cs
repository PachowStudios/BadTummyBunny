namespace PachowStudios.Assertions
{
  public class BooleanAssertion : BaseAssertion<bool, BooleanAssertion>
  {
    public BooleanAssertion(bool subject)
      : base(subject) { }

    public AndConstraint<BooleanAssertion> Be(bool condition, string reason = null)
    {
      Assert(Subject == condition, "be", condition, reason);

      return new AndConstraint<BooleanAssertion>(this);
    }

    public AndConstraint<BooleanAssertion> BeTrue(string reason = null)
    {
      Assert(Subject, "be", true, reason);

      return new AndConstraint<BooleanAssertion>(this);
    }

    public AndConstraint<BooleanAssertion> BeFalse(string reason = null)
    {
      Assert(!Subject, "be", false, reason);

      return new AndConstraint<BooleanAssertion>(this);
    }
  }
}