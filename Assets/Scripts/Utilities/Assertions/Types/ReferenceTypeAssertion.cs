using System;

namespace PachowStudios.Assertions
{
  public abstract class ReferenceTypeAssertion<TSubject, TSelf> : BaseAssertion<TSubject, TSelf>
    where TSubject : class
    where TSelf : ReferenceTypeAssertion<TSubject, TSelf>
  {
    protected ReferenceTypeAssertion(TSubject subject)
      : base(subject) { }

    public AndConstraint<TSelf> BeNull(string reason = null)
      => Assert(Subject == null, "be", "null", reason);

    public AndConstraint<TSelf> NotBeNull(string reason = null)
      => Assert(Subject != null, "not be", "null", reason);

    public AndConstraint<TSelf> ReferTo(TSubject @object, string reason = null)
      => Assert(Subject.RefersTo(@object), "refer to", @object, reason);

    public AndConstraint<TSelf> NotReferTo(TSubject @object, string reason = null)
      => Assert(!Subject.RefersTo(@object), "not refer to", @object, reason);
  }
}