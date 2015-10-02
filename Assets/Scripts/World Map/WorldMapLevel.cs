using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldMapLevel : MonoBehaviour
{
  [SerializeField] private Scenes levelScene = Scenes.Level1;

  private HashSet<WorldMapConnection> connections = new HashSet<WorldMapConnection>(); 

  public bool IsSelected { get; set; }
  public IEnumerable<WorldMapConnection> Connections => this.connections;
  public IEnumerable<WorldMapConnection> EnabledConnections => Connections.Where(c => c.IsEnabled);
  public bool HasEnabledConnections => Connections.Any(c => c.IsEnabled);
  public Vector3 Position => transform.position;

  private void Awake()
  {
    this.connections = new HashSet<WorldMapConnection>(GetComponents<WorldMapConnection>());

    if (Connections.IsEmpty())
      Debug.LogWarning($"{name} doesn't have any connections!");
  }

  public void LoadScene()
    => Application.LoadLevel(this.levelScene.GetDescription());

  public bool HasNeighbor(WorldMapLevel level)
    => this.connections.Any(c => c.ConnectsToLevel(level));

  public bool Select()
  {
    if (IsSelected)
      return false;

    return IsSelected = true;
  }

  public bool Deselect()
  {
    if (!IsSelected)
      return false;

    IsSelected = false;

    return true;
  }
}
