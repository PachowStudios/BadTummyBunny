using System;

public interface IHasHealthContainers
{
  event Action<int> HealthContainersChanged;

  int HealthContainers { get; }
  int HealthPerContainer { get; }
}
