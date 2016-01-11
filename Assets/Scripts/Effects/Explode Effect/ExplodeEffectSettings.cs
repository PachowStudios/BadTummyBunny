using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Effects/Explode Effect Settings")]
  public class ExplodeEffectSettings : ScriptableObject
  {
    public ExplodeEffectView ExplosionPrefab;
    public float Duration = 5f;
    public float ParticleLifetime = 1f;
    public string SortingLayer = "Effects";
    public int SortingOrder = 1;
  }
}