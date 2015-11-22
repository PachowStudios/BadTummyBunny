using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Configuration/Level Config", fileName = "NewLevelConfig.asset")]
public class LevelConfig : ScriptableObject
{
  [UsedImplicitly] public string LevelId;
  [UsedImplicitly] public string LevelScene;
  [UsedImplicitly] public string LevelName;

  [UsedImplicitly] public int RequiredStars;

  [UsedImplicitly] public List<LevelStarConfig> Stars;
}
