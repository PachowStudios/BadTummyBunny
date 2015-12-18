using System.Collections.Generic;
using System.Xml.Serialization;

namespace PachowStudios.BadTummyBunny
{
  [XmlRoot(nameof(SaveFile), Namespace = "http://schema.pachowstudios.com/badtummybunny/savefile")]
  public class SaveFile
  {
    [XmlElement(nameof(Version))]
    public VersionCode Version { get; set; } = new VersionCode(0, 1);

    [XmlArray(nameof(Levels))]
    [XmlArrayItem(nameof(LevelProgress))]
    public List<LevelProgress> Levels { get; set; } 
  }
}