using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Player/Player Movement Settings")]
  public class PlayerMovementSettings : BaseMovableSettings
  {
    [Header("Movement")]
    public float JumpHeight = 5f;

    [Header("Farting")]
    public FartType StartingFartType = FartType.Basic;
    public float FartDeadZone = 0.2f;
  }
}