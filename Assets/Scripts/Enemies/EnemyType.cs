using JetBrains.Annotations;
using PachowStudios.BadTummyBunny.AI.Patrol;

namespace PachowStudios.BadTummyBunny
{
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public enum EnemyType
  {
    [TypeMapping(typeof(PatrolAI))]
    Fox
  }
}