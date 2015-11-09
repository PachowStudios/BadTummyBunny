using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("Connects to {ConnectedLevel.name}")]
[Serializable]
public class WorldMapConnection
{
  [SerializeField] private WorldMapLevel connectedLevel = null;
  [SerializeField] private bool isEnabled = false;

  public WorldMapLevel ConnectedLevel => this.connectedLevel;
  public bool IsEnabled => this.isEnabled;

  public bool ConnectsToLevel(WorldMapLevel level)
    => ReferenceEquals(this.connectedLevel, level);
}