using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Player/Player Settings")]
  public class PlayerSettings : ScriptableObject
  {
    public PlayerMovementSettings Movement;
    public PlayerHealthSettings Health;
    public FartAimLeanSettings FartAimLean;
  }
}