using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[DebuggerDisplay(("Connects {ThisLevel.name} to {ConnectedLevel.name}"))]
[RequireComponent(typeof(WorldMapLevel))]
public class WorldMapConnection : MonoBehaviour
{
  [SerializeField] private WorldMapLevel connectedLevel = null;
  [SerializeField] private bool isEnabled = false;

  private WorldMapLevel thisLevel;

  public WorldMapLevel ThisLevel => this.GetComponentIfNull(ref this.thisLevel);
  public WorldMapLevel ConnectedLevel => this.connectedLevel;
  public bool IsEnabled => this.isEnabled;
  public float Distance => Vector3.Distance(ThisLevel.Position, ConnectedLevel.Position);

  private void Awake()
  {
    if (ConnectedLevel == null)
      Debug.LogWarning($"{name} has an uninitialized connection!");
  }

  private void OnDrawGizmosSelected()
  {
    if (ThisLevel == null || ConnectedLevel == null)
      return;

    Gizmos.color = IsEnabled ? Color.green : Color.red;
    //Gizmos.DrawLine(ThisLevel.Position, ConnectedLevel.Position);
    GizmosEx.DrawArrowTo(ThisLevel.Position, ConnectedLevel.Position);
  }

  public bool ConnectsToLevel(WorldMapLevel level)
    => ReferenceEquals(this.connectedLevel, level);
}