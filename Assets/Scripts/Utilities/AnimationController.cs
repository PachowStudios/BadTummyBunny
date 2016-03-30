using System;
using UnityEngine;
using Zenject;

namespace PachowStudios
{
  public class AnimationController : ITickable
  {
    private Animator Animator { get; }
    private AnimationCondition[] Conditions { get; }

    public AnimationController(Animator animator, params AnimationCondition[] conditions)
    {
      Animator = animator;
      Conditions = conditions;
    }

    public void Tick()
    {
      foreach (var condition in Conditions)
        Animator.SetBool(condition.Name, condition.IsConditionSatisfied);
    }
  }

  public class AnimationCondition
  {
    public string Name { get; }

    public bool IsConditionSatisfied => Condition();

    private Func<bool> Condition { get; }

    public AnimationCondition(string name, Func<bool> condition)
    {
      Name = name;
      Condition = condition;
    }
  }
}