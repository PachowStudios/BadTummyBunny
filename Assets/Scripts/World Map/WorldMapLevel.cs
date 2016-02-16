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
    [SerializeField] private List<WorldMapConnection> connections = new List<WorldMapConnection>();

    public string LevelName => LevelConfig.Name;
    public IEnumerable<WorldMapConnection> Connections => this.connections;
    public IEnumerable<WorldMapConnection> EnabledConnections => Connections.Where(c => c.IsEnabled);
    public bool IsSelected => ReferenceEquals(this, WorldMap.SelectedLevel);
    public Vector3 Position => transform.position;

    public IEnumerable<BaseStarSettings> Stars => LevelConfig.Stars;
    public IEnumerable<BaseStarSettings> CompletedStars { get; private set; }

    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }
    [Inject] private WorldMap WorldMap { get; set; }
    [Inject] private ISceneLoader SceneLoader { get; set; }
    [Inject] private ISaveContainer SaveContainer { get; set; }

    private LevelSettings LevelConfig { get; set; }

    [PostInject]
    private void PostInject()
      => LevelConfig = LevelSettings[this.scene];

    private void Awake()
    {
      Connections.IsEmpty().Should().BeFalse($"because {name} must connect to other levels.");
      Connections.None(c => c.ConnectedLevel == null).Should().BeTrue($"because no connections to {name} can be null");
    }

    // Save related initialization must occurr during the Start phase
    // because the save file won't be loaded yet during the PostInject phase.
    private void Start()
    {
      var levelProgress = SaveContainer.SaveFile.GetLevel(this.scene);

      CompletedStars = Stars.Where(s => levelProgress.GetStar(s.Id).IsCompleted).ToList();
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