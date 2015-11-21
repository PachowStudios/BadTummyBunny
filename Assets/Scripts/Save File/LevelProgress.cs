using System.Collections.Generic;
using System.Xml.Serialization;

public class LevelProgress
{
  [XmlElement(nameof(LevelId))]
  public string LevelId { get; set; }

  [XmlElement(nameof(IsUnlocked))]
  public bool IsUnlocked { get; set; }

  [XmlElement(nameof(IsComplete))]
  public bool IsComplete { get; set; }

  [XmlArray(nameof(Stars))]
  [XmlArrayItem(nameof(LevelStarProgress))]
  public List<LevelStarProgress> Stars { get; set; }
}