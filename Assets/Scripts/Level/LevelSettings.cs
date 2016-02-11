using System.Collections.Generic;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings]
  public class LevelSettings : ScriptableObject
  {
    public string Name;
    public Scene Scene;

    public int RequiredStars;

    public List<StarSettings> Stars;
  }
}