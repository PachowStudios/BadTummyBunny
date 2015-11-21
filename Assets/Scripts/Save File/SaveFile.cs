using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot(nameof(SaveFile))]
public class SaveFile
{
  [XmlArray(nameof(Levels))]
  [XmlArrayItem(nameof(LevelProgress))]
  public List<LevelProgress> Levels { get; set; } 
}