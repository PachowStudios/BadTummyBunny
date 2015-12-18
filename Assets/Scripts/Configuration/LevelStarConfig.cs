using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [CreateAssetMenu(menuName = "Configuration/Level Star Config", fileName = "NewLevelStarConfig.asset")]
  public class LevelStarConfig : ScriptableObject
  {
    public string StarId;
    public string StarName;
  }
}