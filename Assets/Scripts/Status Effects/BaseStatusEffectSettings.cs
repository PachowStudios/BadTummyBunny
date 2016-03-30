using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class BaseStatusEffectSettings : ScriptableObject
  {
    public StatusEffectType Type;
    public string Name = "New Status Effect";
    public GameObject Prefab;
  }
}