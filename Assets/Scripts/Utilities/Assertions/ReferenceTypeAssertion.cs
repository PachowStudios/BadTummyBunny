using System;

namespace PachowStudios.Assertions
{
  public abstract class ReferenceTypeAssertion<T> : BaseAssertion<T>
    where T : class
  {
    protected ReferenceTypeAssertion(T subject)
      : base(subject) { }

    public void BeNull(string reason = null)
      => Assert(Subject == null, "be", null, reason);

    public void NotBeNull(string reason = null)
      => Assert(Subject != null, "not be", null, reason);

    public void ReferTo(T @object, string reason = null)
      => Assert(Subject.RefersTo(@object), "refer to", @object, reason);

    public void NotReferTo(T @object, string reason = null)
      => Assert(!Subject.RefersTo(@object), "not refer to", @object, reason);
  }
}