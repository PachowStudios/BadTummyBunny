using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IFartInfoProvider
  {
    bool IsFarting { get; }
    bool IsFartAiming { get; }
    float FartPower { get; }
    Vector2 FartDirection { get; }
  }
}
