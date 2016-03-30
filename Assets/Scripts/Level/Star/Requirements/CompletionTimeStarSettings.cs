using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Level/Star/Completion Time Settings")]
  public class CompletionTimeStarSettings : BaseStarSettings
  {
    public float TimeLimit;

    public override StarRequirement Requirement => StarRequirement.CompletionTime;
  }
}