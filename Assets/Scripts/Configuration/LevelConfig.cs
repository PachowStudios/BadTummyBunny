using System.Collections.Generic;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public class LevelConfig : ScriptableObject
  {
    public string LevelId;
    public string LevelScene;
    public string LevelName;

    public int RequiredStars;

    public List<LevelStarConfig> Stars;
  }
}