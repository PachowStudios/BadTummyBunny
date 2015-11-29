using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using Touch = InControl.Touch;

[AddComponentMenu("Bad Tummy Bunny/World Map/Level")]
public class WorldMapLevel : MonoBehaviour
{
  [SerializeField] private LevelConfig levelConfig = null;
  [SerializeField] private int collectedStars = 0;
  [SerializeField] private int possibleStars = 3;
  [SerializeField] private List<WorldMapConnection> connections = new List<WorldMapConnection>();

  public bool IsSelected => ReferenceEquals(this, WorldMap.SelectedLevel);

  public string LevelName => this.levelConfig.LevelName;
  public int CollectedStars => this.collectedStars;
  public int PossibleStars => this.possibleStars;
  public IEnumerable<WorldMapConnection> Connections => this.connections;
  public IEnumerable<WorldMapConnection> EnabledConnections => Connections.Where(c => c.IsEnabled);
  public bool HasEnabledConnections => Connections.Any(c => c.IsEnabled);
  public Vector3 Position => transform.position;

  [Inject]
  private WorldMap WorldMap { get; set; }

  private void Awake()
  {
    Assert.IsFalse(Connections.IsEmpty(), $"{name} doesn't have any connections!");
    Assert.IsFalse(Connections.Any(c => c.ConnectedLevel == null), $"{name} has an uninitialized connection!");
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
    => Application.LoadLevel(this.levelConfig.LevelScene);

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
