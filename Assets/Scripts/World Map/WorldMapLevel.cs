using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using JetBrains.Annotations;
using PachowStudios.Assertions;
using PachowStudios.Collections;
using UnityEngine;
using Zenject;
using Touch = InControl.Touch;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/World Map/Level")]
  public class WorldMapLevel : MonoBehaviour
  {
    [SerializeField] private Scene scene = Scene.Level1;
    [SerializeField] private int collectedStars = 0;
    [SerializeField] private List<WorldMapConnection> connections = new List<WorldMapConnection>();

    public bool IsSelected => ReferenceEquals(this, WorldMap.SelectedLevel);

    public string LevelName => LevelConfig.Name;
    public int CollectedStars => this.collectedStars;
    public int PossibleStars => LevelConfig.Stars.Count;
    public IEnumerable<WorldMapConnection> Connections => this.connections;
    public IEnumerable<WorldMapConnection> EnabledConnections => Connections.Where(c => c.IsEnabled);
    public Vector3 Position => transform.position;

    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }
    [Inject] private WorldMap WorldMap { get; set; }
    [Inject] private ISceneLoader SceneLoader { get; set; }

    private LevelSettings LevelConfig => LevelSettings[this.scene];

    private void Awake()
    {
      Connections.IsEmpty().Should().BeFalse($"because {name} must connect to other levels.");
      Connections.None(c => c.ConnectedLevel == null).Should().BeTrue($"because no connections to {name} can be null");
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
      => SceneLoader.LoadScene(this.scene);

    public bool HasNeighbor(WorldMapLevel level)
      => this.connections.Any(c => c.ConnectsToLevel(level));

    public virtual void OnSelected() { }

    public virtual void OnDeselected() { }

    [UsedImplicitly]
    public virtual void OnTouched(Touch touch)
    {
      if (touch.phase == TouchPhase.Began)
        WorldMap.NavigateToLevel(this);
    }
  }
}