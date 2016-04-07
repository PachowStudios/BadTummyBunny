using JetBrains.Annotations;

namespace PachowStudios.Assertions
{
  public class AndConstraint<T>
  {
    [NotNull] public T And { get; }

    public AndConstraint([NotNull] T parentConstraint)
    {
      And = parentConstraint;
    }
  }
}