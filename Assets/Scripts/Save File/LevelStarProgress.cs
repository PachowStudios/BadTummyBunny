using System.Xml.Serialization;

public class LevelStarProgress
{
  [XmlElement(nameof(StarId))]
  public string StarId { get; set; }

  [XmlElement(nameof(IsComplete))]
  public bool IsComplete { get; set; }
}