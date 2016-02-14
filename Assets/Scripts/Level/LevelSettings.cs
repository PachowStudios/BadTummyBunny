using System;
using System.Collections.Generic;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Level/Level Settings")]
  public class LevelSettings : ScriptableObject
  {
    public Scene Scene;
    public int RequiredStars;
    public List<BaseStarSettings> Stars;

    public string Name => this.Scene.GetDescription();
  }
}