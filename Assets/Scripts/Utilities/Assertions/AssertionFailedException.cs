using System;

namespace PachowStudios.Assertions
{
  public class AssertionFailedException : Exception
  {
    public override string Message { get; }

    public AssertionFailedException(string assertion, string reason)
    {
      Message = reason.IsNullOrEmpty()
        ? $"{assertion}"
        : $"{assertion} {reason.StartWith("because ")}";
    }
  }
}