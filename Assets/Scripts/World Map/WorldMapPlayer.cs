using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("World Map/Player")]
  public class WorldMapPlayer : MonoBehaviour
  {
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Ease moveEase = Ease.Linear;

    public bool IsNavigating { get; private set; }

    public void NavigatePath(IList<WorldMapLevel> path, Action<WorldMapLevel> onCompleted = null)
    {
      if (IsNavigating)
        return;

      IsNavigating = true;

      transform
        .DOPath(
          path.Select(p => p.Position).ToArray(),
          this.moveSpeed,
          PathType.Linear,
          PathMode.TopDown2D)
        .SetEase(this.moveEase)
        .SetSpeedBased()
        .OnComplete(() =>
        {
          onCompleted?.Invoke(path.Last());
          IsNavigating = false;
        });
    }
  }
}