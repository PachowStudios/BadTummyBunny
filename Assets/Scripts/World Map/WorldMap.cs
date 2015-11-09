using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("World Map/Map")]
public class WorldMap : MonoBehaviour
{
  [SerializeField] private WorldMapLevel selectedLevel;
  [SerializeField] private WorldMapPlayer player = null;

  private HashSet<WorldMapLevel> levels;

  public IEnumerable<WorldMapLevel> Levels => this.levels;

  public WorldMapLevel SelectedLevel => this.selectedLevel;

  private void Awake()
  {
    Assert.IsNotNull(this.selectedLevel, nameof(this.selectedLevel));
    Assert.IsNotNull(this.player, nameof(this.player));

    this.levels = new HashSet<WorldMapLevel>(GetComponentsInChildren<WorldMapLevel>());
    this.selectedLevel.Select();
  }

  public void SelectLevel(WorldMapLevel level)
  {
    if (level.IsSelected)
      return;

    this.selectedLevel?.OnDeselected();
    this.selectedLevel = level;
    this.selectedLevel.OnSelected();
  }

  public void NavigateToLevel(WorldMapLevel targetLevel)
  {
    IList<WorldMapLevel> path;

    if (TryGetPathToLevel(SelectedLevel, targetLevel, out path))
      this.player.NavigatePath(path);
  }

  public bool TryGetPathToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel, out IList<WorldMapLevel> path)
    => (path = GetPathToLevel(startLevel, endLevel)).Any();

  public IList<WorldMapLevel> GetPathToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel)
  {
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