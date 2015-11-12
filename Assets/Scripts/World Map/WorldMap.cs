using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("World Map/Map")]
public class WorldMap : MonoBehaviour
{
  public event Action<WorldMapLevel> LevelSelected;
  public event Action<WorldMapLevel> LevelDeselected;

  [SerializeField] private WorldMapLevel startingLevel = null;
  [SerializeField] private WorldMapPlayer player = null;

  private HashSet<WorldMapLevel> levels;

  public static WorldMap Instance { get; private set; }

  public WorldMapLevel SelectedLevel { get; private set; }

  public IEnumerable<WorldMapLevel> Levels => this.levels;

  private void Awake()
  {
    Assert.IsNotNull(this.startingLevel, nameof(this.startingLevel));
    Assert.IsNotNull(this.player, nameof(this.player));

    Instance = this;

    this.levels = new HashSet<WorldMapLevel>(GetComponentsInChildren<WorldMapLevel>());

    SelectLevel(this.startingLevel);
  }

  public void SelectLevel(WorldMapLevel level)
  {
    Assert.IsNotNull(level);

    SelectedLevel?.OnDeselected();
    SelectedLevel = level;
    SelectedLevel.OnSelected();

    LevelSelected?.Invoke(SelectedLevel);
  }

  public void DeselectLevel()
  {
    SelectedLevel?.OnDeselected();

    LevelDeselected?.Invoke(SelectedLevel);

    SelectedLevel = null;
  }

  public void NavigateToLevel(WorldMapLevel targetLevel)
  {
    IList<WorldMapLevel> path;

    if (TryGetPathToLevel(SelectedLevel, targetLevel, out path))
    {
      DeselectLevel();
      this.player.NavigatePath(path, onCompleted: SelectLevel);
    }
  }

  public bool TryGetPathToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel, out IList<WorldMapLevel> path)
    => (path = GetPathToLevel(startLevel, endLevel)).Any();

  public IList<WorldMapLevel> GetPathToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel)
  {
    var path = new Stack<WorldMapLevel>();
    var nodes = new List<WorldMapLevel>(Levels);
    var distances = new Dictionary<WorldMapLevel, float>();
    var previousNodes = new Dictionary<WorldMapLevel, WorldMapLevel>();

    if (startLevel == null || endLevel == null)
      return path.ToList();

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

        return path.ToList();
      }

      if (distances[smallestNode] >= float.MaxValue)
        break;

      foreach (var connection in smallestNode.EnabledConnections)
      {
        var distance =
          distances[smallestNode]
          + smallestNode.Position.DistanceTo(connection.ConnectedLevel.Position);
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