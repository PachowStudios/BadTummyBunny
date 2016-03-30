namespace PachowStudios.Assertions
{
  public class ObjectAssertion : ReferenceTypeAssertion<object>
  {
    public ObjectAssertion(object subject)
      : base(subject) { }

    public void Be(object @object, string reason = null)
      => Assert(Equals(Subject, @object), "be", @object, reason);

    public void NotBe(object @object, string reason = null)
      => Assert(!Equals(Subject, @object), "not be", @object, reason);
  }
}