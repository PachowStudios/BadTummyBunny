using System;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.Events;
using Touch = InControl.Touch;

[RequireComponent(typeof(Collider2D))]
public class TouchDetector : MonoBehaviour
{
  [SerializeField] private bool singleTouchOnly = true;
  [SerializeField] private TouchEvent touched = new TouchEvent();

  private Collider2D touchCollider;

  private Collider2D TouchCollider => this.GetComponentIfNull(ref this.touchCollider);

  private void Update()
  {
    if (!this.singleTouchOnly || TouchManager.TouchCount == 1)
      TouchManager.Touches
        .Where(t => TouchCollider.OverlapPoint(t.WorldPosition))
        .ForEach(this.touched.Invoke);
  }

  [Serializable]
  public class TouchEvent : UnityEvent<Touch> { }
}