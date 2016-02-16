using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class BaseEnemyAISettings : BaseEnemyMovementSettings
  {
    public LayerMask BlockVisibilityLayers = default(LayerMask);
  }
}