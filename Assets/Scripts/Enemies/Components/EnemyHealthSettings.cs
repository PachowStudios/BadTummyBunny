using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Enemies/Enemy Health Settings")]
  public class EnemyHealthSettings : ScriptableObject
  {
    public int MaxHealth = 4;
    public Color FlashColor = new Color(1f, 0.47f, 0.47f, 1f);
    public float FlashLength = 0.25f;
  }
}