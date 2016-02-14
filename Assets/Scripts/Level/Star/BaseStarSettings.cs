using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class BaseStarSettings : ScriptableObject
  {
    public string Id;
    public string Name;

    public abstract StarRequirement Requirement { get; }
  }
}