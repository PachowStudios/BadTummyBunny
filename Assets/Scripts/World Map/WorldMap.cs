using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PachowStudios.Assertions;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/World Map/Map")]
  public class WorldMap : MonoBehaviour
  {
    [SerializeField] private WorldMapLevel startingLevel = null;

    private HashSet<WorldMapLevel> levels;

    [Inject] private WorldMapPlayer Player { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; }

    public WorldMapLevel SelectedLevel { get; private set; }

    public IEnumerable<WorldMapLevel> Levels => this.levels;

    private void Awake()
    {
      this.startingLevel.Should().NotBeNull("because a starting level must be set.");

      this.levels = new HashSet<WorldMapLevel>(GetComponentsInChildren<WorldMapLevel>());
    }

    private void Start()
      => SelectLevel(this.startingLevel);

    public void SelectLevel([NotNull] WorldMapLevel level)
    {
      SelectedLevel?.OnDeselected();
      SelectedLevel = level;
      SelectedLevel.OnSelected();

      EventAggregator.Publish(new LevelSelectedMessage(SelectedLevel));
    }

    public void DeselectLevel()
    {
      SelectedLevel?.OnDeselected();

      EventAggregator.Publish(new LevelDeselectedMessage(SelectedLevel));

      SelectedLevel = null;
    }

    public void NavigateToLevel([NotNull] WorldMapLevel targetLevel)
    {
      IList<WorldMapLevel> path;

      if (!TryGetPathToLevel(SelectedLevel, targetLevel, out path))
        return;

      DeselectLevel();
      Player.NavigatePath(path, onCompleted: SelectLevel);
    }

    public bool TryGetPathToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel, out IList<WorldMapLevel> path)
      => (path = GetPathToLevel(startLevel, endLevel)).Any();

    public IList<WorldMapLevel> GetPathToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel)
    {
      if (startLevel == null || endLevel == null)
        return new List<WorldMapLevel>();

      var path = new Stack<WorldMapLevel>();
      var nodes = new List<WorldMapLevel>(Levels);
      var distances = new Dictionary<WorldMapLevel, float>();
      var previousNodes = new Dictionary<WorldMapLevel, WorldMapLevel>();

      foreach (var node in nodes)
        distances[node] = float.MaxValue;

      distances[startLevel] = 0f;

      while (nodes.Any())
      {
        nodes.Sort((x, y) => distances[x].CompareTo(distances[y]));

        var smallestNode = nodes.First();

        nodes.Remove(smallestNode);

        if (ReferenceEquals(smallestNode, endLevel))
        {
          while (previousNodes.ContainsKey(smallestNode))
          {
            path.Push(smallestNode);
            smallestNode = previousNodes[smallestNode];
          }

          break;
        }

        if (distances[smallestNode] >= float.MaxValue)
          break;

        foreach (var connection in smallestNode.EnabledConnections)
        {
          var distance = distances[smallestNode] + smallestNode.Position.DistanceTo(connection.ConnectedLevel.Position);
          var connectedLevel = connection.ConnectedLevel;

          if (distance >= distances[connectedLevel])
            continue;

          distances[connectedLevel] = distance;
          previousNodes[connectedLevel] = smallestNode;
        }
      }

      return path.ToList();
    }
  }
}