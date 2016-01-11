using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Player/Fart Aim Lean Settings")]
  public class FartAimLeanSettings : ScriptableObject
  {
    public float EffectorWeight = 5f;
    public float LeanDistance = 3f;
    public float MinimumPower = 0.2f;
    public AnimationCurve EffectorFalloff = AnimationCurve.Linear(0f, 0f, 1f, 1f);
  }
}