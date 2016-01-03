using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface ICameraPositionAssertion
  {
    Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity);
  }
}
