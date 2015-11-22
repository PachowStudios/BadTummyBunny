using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Configuration/Level Star Config", fileName = "NewLevelStarConfig.asset")]
public class LevelStarConfig : ScriptableObject
{
  [UsedImplicitly] public string StarId;
  [UsedImplicitly] public string StarName;
}