using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("UI/Camera/Object Activator")]
[RequireComponent(typeof(Collider2D))]
public sealed class ObjectActivator : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other)
    => other.GetInterface<IActivatable>()?.Activate();

  private void OnTriggerExit2D(Collider2D other)
    => other.GetInterface<IActivatable>()?.Deactivate();

  [Conditional("UNITY_EDITOR")]
  private void Update() { }
}
