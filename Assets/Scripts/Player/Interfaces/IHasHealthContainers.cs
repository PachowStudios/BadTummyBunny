using System;

namespace PachowStudios.BadTummyBunny
{
  public interface IHasHealthContainers
  {
    int HealthContainers { get; }
    int HealthPerContainer { get; }
  }
}
