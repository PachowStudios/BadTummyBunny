using System;
using System.Linq;
using System.Linq.Extensions;
using InControl;
using UnityEngine;
using UnityEngine.Events;
using Touch = InControl.Touch;

namespace PachowStudios.BadTummyBunny
{
  [RequireComponent(typeof(Collider2D))]
  public class TouchDetector : MonoBehaviour
  {
    [SerializeField] private bool singleTouchOnly = true;
    [SerializeField] private TouchEvent touched = new TouchEvent();

    private Collider2D touchColliderComponent;

    private Collider2D TouchCollider => this.GetComponentIfNull(ref this.touchColliderComponent);

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
}