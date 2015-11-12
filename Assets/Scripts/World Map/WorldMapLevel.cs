using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Touch = InControl.Touch;

[AddComponentMenu("World Map/Level")]
public class WorldMapLevel : MonoBehaviour
{
  [SerializeField] private Scenes levelScene = Scenes.Level1;
  [SerializeField] private List<WorldMapConnection> connections = new List<WorldMapConnection>();

  private WorldMap worldMap;

  public bool IsSelected => ReferenceEquals(this, WorldMap.SelectedLevel);

  public string LevelName => this.levelScene.GetDescription();
  public IEnumerable<WorldMapConnection> Connections => this.connections;
  public IEnumerable<WorldMapConnection> EnabledConnections => Connections.Where(c => c.IsEnabled);
  public bool HasEnabledConnections => Connections.Any(c => c.IsEnabled);
  public Vector3 Position => transform.position;

  private WorldMap WorldMap => this.GetComponentInParentIfNull(ref this.worldMap);

  private void Awake()
  {
    if (Connections.IsEmpty())
      Debug.LogWarning($"{name} doesn't have any connections!");
    else if (Connections.Any(c => c.ConnectedLevel == null))
      Debug.LogWarning($"{name} has an uninitialized connection!");
  }

  private void OnDrawGizmosSelected()
  {
    foreach (var connection in Connections)
    {
      Gizmos.color = connection.IsEnabled ? Color.green : Color.red;
      GizmosEx.DrawArrowTo(Position, connection.ConnectedLevel.Position);
    }
  }

  public void LoadScene()
    => Application.LoadLevel(LevelName);

  public bool HasNeighbor(WorldMapLevel level)
    => this.connections.Any(c => c.ConnectsToLevel(level));

  public void OnSelected()
  {
    
  }

  public void OnDeselected()
  {
    
  }

  public void OnTouched(Touch touch)
  {
    if (touch.phase == TouchPhase.Began)
      WorldMap.NavigateToLevel(this);
  }
}
