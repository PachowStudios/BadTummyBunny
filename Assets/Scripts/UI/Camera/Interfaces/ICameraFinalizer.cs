using UnityEngine;

public interface ICameraFinalizer
{
  bool IsEnabled { get; }
  int GetFinalizerPriority { get; }
  bool ShouldSkipSmoothingThisFrame { get; }

  Vector3 GetFinalCameraPosition(Bounds targetBounds, Vector3 currentCameraPosition, Vector3 desiredCameraPosition);

  #if UNITY_EDITOR
  // ReSharper disable once InconsistentNaming
  void onDrawGizmos(Vector3 basePosition);
  #endif
}
