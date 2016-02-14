using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Level/Star/Collect Coins Settings")]
  public class CollectCoinsStarSettings : BaseStarSettings
  {
    public int RequiredCoins;

    public override StarRequirement Requirement => StarRequirement.CollectCoins;
  }
}