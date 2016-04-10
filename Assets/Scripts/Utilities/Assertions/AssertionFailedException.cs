using System;
using JetBrains.Annotations;

namespace PachowStudios.Assertions
{
  public class AssertionFailedException : Exception
  {
    public override string Message { get; }

    public AssertionFailedException([NotNull] string assertion, [CanBeNull] string reason)
    {
      Message = reason.IsNullOrEmpty()
        ? $"{assertion}"
        : $"{assertion} {reason.StartWith("because ").EndWith(".")}";
    }
  }
}