using System;
using JetBrains.Annotations;

namespace PachowStudios
{
  public class AnimationCondition
  {
    private readonly Func<bool> condition;

    [NotNull] public string Name { get; }

    public bool IsConditionSatisfied => this.condition();

    public AnimationCondition([NotNull] string name, [NotNull] Func<bool> condition)
    {
      Name = name;
      this.condition = condition;
    }
  }
}