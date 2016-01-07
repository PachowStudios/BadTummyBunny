using System.Collections.Generic;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public class LevelConfig : ScriptableObject
  {
    public string Name;
    public Scene Scene;

    public int RequiredStars;

    public List<LevelStarConfig> Stars;
  }
}