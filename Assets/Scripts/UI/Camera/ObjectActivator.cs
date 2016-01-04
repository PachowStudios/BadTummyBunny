using System.Diagnostics;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Camera/Object Activator")]
  [RequireComponent(typeof(Collider2D))]
  public sealed class ObjectActivator : MonoBehaviour
  {
    private void OnTriggerEnter2D(Collider2D other)
    {
      var activatable = other.GetComponent<IActivatable>();

      if (activatable != null && !activatable.IsActivated)
        activatable.IsActivated = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      var activatable = other.GetComponent<IActivatable>();

      if (activatable != null && activatable.IsActivated)
        activatable.IsActivated = false;
    }

    [Conditional("UNITY_EDITOR")]
    private void Update() { }
  }
}
