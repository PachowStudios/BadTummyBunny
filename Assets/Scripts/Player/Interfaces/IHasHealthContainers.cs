using System;

public interface IHasHealthContainers
{
  int HealthContainers { get; }
  int HealthPerContainer { get; }
}
