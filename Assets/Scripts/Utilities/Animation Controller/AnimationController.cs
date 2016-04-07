using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace PachowStudios
{
  public class AnimationController : ITickable
  {
    private readonly Animator animator;
    private readonly AnimationCondition[] conditions;

    public AnimationController([NotNull] Animator animator, [NotNull] params AnimationCondition[] conditions)
    {
      this.animator = animator;
      this.conditions = conditions;
    }

    public void Tick()
    {
      foreach (var condition in this.conditions)
        this.animator.SetBool(condition.Name, condition.IsConditionSatisfied);
    }
  }
}