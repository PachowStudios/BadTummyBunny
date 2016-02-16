using System.Collections.Generic;
using System.Xml.Serialization;

namespace PachowStudios.BadTummyBunny
{
  public class LevelProgress
  {
    [XmlAttribute(nameof(Scene))]
    public Scene Scene { get; set; }

    [XmlAttribute(nameof(IsUnlocked))]
    public bool IsUnlocked { get; set; }

    [XmlAttribute(nameof(IsComplete))]
    public bool IsComplete { get; set; }

    [XmlArray(nameof(Stars))]
    [XmlArrayItem(nameof(LevelStarProgress))]
    public List<LevelStarProgress> Stars { get; set; }
  }
}