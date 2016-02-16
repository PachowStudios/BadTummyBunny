using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public abstract class BaseMovableSettings : ScriptableObject
  {
    public float Gravity = -35f;
    public float MoveSpeed = 5f;
    public float GroundDamping = 10f;
    public float AirDamping = 5f;
  }
}