using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
  [SerializeField] private WorldMapLevel selectedLevel = null;

  private HashSet<WorldMapLevel> levels = new HashSet<WorldMapLevel>();

  public WorldMapLevel SelectedLevel => this.selectedLevel;
  public IEnumerable<WorldMapLevel> Levels => this.levels;

  private void Awake()
  {
    this.levels = new HashSet<WorldMapLevel>(GetComponentsInChildren<WorldMapLevel>());

    if (this.selectedLevel == null)
      Debug.LogWarning("There is no default selected level!");

    this.selectedLevel?.Select();
  }

  public bool CanNavigateToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel)
    => !NavigateToLevel(startLevel, endLevel).IsEmpty();

  public Stack<WorldMapLevel> NavigateToLevel(WorldMapLevel startLevel, WorldMapLevel endLevel)
  {
    var path = new Stack<WorldMapLevel>();
    var nodes = new List<WorldMapLevel>(Levels);
    var distances = new Dictionary<WorldMapLevel, float>();
    var previousNodes = new Dictionary<WorldMapLevel, WorldMapLevel>();

    foreach (var node in nodes)
      distances[node] = float.MaxValue;

    distances[startLevel] = 0f;

    while (!nodes.IsEmpty())
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

        return path;
      }

      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (distances[smallestNode] == float.MaxValue)
        break;

      foreach (var connection in smallestNode.EnabledConnections)
      {
        var distance = distances[smallestNode] + connection.Distance;
        var connectedLevel = connection.ConnectedLevel;

        if (distance >= distances[connectedLevel])
          continue;

        distances[connectedLevel] = distance;
        previousNodes[connectedLevel] = smallestNode;
      }
    }

    return path;
  } 
}