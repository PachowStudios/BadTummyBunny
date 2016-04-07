namespace PachowStudios.Assertions
{
  public class ObjectAssertion : ReferenceTypeAssertion<object, ObjectAssertion>
  {
    public ObjectAssertion(object subject)
      : base(subject) { }

    public AndConstraint<ObjectAssertion> Be(object @object, string reason = null)
      => Assert(Equals(Subject, @object), "be", @object, reason);

    public AndConstraint<ObjectAssertion> NotBe(object @object, string reason = null)
      => Assert(!Equals(Subject, @object), "not be", @object, reason);
  }
}