using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Level/Star/Kill Enemies Settings")]
  public class KillEnemiesStarSettings : BaseStarSettings
  {
    public int RequiredEnemies;

    public override StarRequirement Requirement => StarRequirement.KillEnemies;
  }
}