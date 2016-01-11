using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Status Effects/Burning Status Effect Settings")]
  public class BurningStatusEffectSettings : BaseStatusEffectSettings
  {
    public int Damage = 1;
    public float TimePerDamage = 0.5f;
    public float MinDuration = 2f;
    public float MaxDuration = 4f;
  }
}