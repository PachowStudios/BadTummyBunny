using JetBrains.Annotations;
using PachowStudios.BadTummyBunny.Enemies.AI;

namespace PachowStudios.BadTummyBunny
{
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public enum EnemyType
  {
    [TypeMapping(typeof(PatrolAI))]
    Fox
  }
}