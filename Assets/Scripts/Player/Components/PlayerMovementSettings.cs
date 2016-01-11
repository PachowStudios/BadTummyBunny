using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Player/Player Movement Settings")]
  public class PlayerMovementSettings : BaseMovebaleSettings
  {
    [Header("Movement")]
    public float JumpHeight = 5f;

    [Header("Farting")]
    public FartType StartingFartType = FartType.Basic;
    public float MaxAvailableFart = 10f;
    public float FartRechargePerSecond = 1f;
    [Range(0f, 1f)] public float CarrotFartRechargePercent = 0.25f;
    public float FartDeadZone = 0.2f;
    public Vector2 FartUsageRange = default(Vector2);
  }
}