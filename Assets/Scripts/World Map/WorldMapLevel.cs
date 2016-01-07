using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using Touch = InControl.Touch;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/World Map/Level")]
  public class WorldMapLevel : MonoBehaviour
  {
    [SerializeField] private LevelConfig levelConfig = null;
    [SerializeField] private int collectedStars = 0;
    [SerializeField] private int possibleStars = 3;
    [SerializeField] private List<WorldMapConnection> connections = new List<WorldMapConnection>();

    public bool IsSelected => ReferenceEquals(this, WorldMap.SelectedLevel);

    public string LevelName => this.levelConfig.Name;
    public int CollectedStars => this.collectedStars;
    public int PossibleStars => this.possibleStars;
    public IEnumerable<WorldMapConnection> Connections => this.connections;
    public IEnumerable<WorldMapConnection> EnabledConnections => Connections.Where(c => c.IsEnabled);
    public bool HasEnabledConnections => Connections.Any(c => c.IsEnabled);
    public Vector3 Position => transform.position;

    [Inject] private WorldMap WorldMap { get; set; }
    [Inject] private ISceneLoader SceneLoader { get; }

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
        GizmosHelper.DrawArrowTo(Position, connection.ConnectedLevel.Position);
      }
    }

    public void LoadScene()
      => SceneLoader.LoadScene(this.levelConfig.Scene);

    public bool HasNeighbor(WorldMapLevel level)
      => this.connections.Any(c => c.ConnectsToLevel(level));

    public void OnSelected() { }

    public void OnDeselected() { }

    public void OnTouched(Touch touch)
    {
      if (touch.phase == TouchPhase.Began)
        WorldMap.NavigateToLevel(this);
    }
  }
}