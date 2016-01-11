using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Player/Player Health Settings")]
  public class PlayerHealthSettings : ScriptableObject
  {
    public int HealthContainers = 3;
    public int CarrotHealthRecharge = 1;
    public int FalloutDamage = 1;
    public float InvincibilityPeriod = 2f;
  }
}