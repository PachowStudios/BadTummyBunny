using System;
using JetBrains.Annotations;

namespace PachowStudios
{
  [AttributeUsage(AttributeTargets.Method)]
  [MeansImplicitUse(ImplicitUseKindFlags.Access)]
  public class AnimationEventAttribute : Attribute { }
}