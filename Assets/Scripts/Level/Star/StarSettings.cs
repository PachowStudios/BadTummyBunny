using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class StarSettings : ScriptableObject
  {
    public string Id;
    public string Name;

    public StarRequirement Requirement;
  }
}