using System.Xml.Serialization;

namespace PachowStudios.BadTummyBunny
{
  public class LevelStarProgress
  {
    [XmlAttribute(nameof(StarId))]
    public string StarId { get; set; }

    [XmlAttribute(nameof(IsComplete))]
    public bool IsComplete { get; set; }
  }
}