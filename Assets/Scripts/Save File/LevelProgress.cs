using System.Collections.Generic;
using System.Linq.Extensions;
using System.Xml.Serialization;

namespace PachowStudios.BadTummyBunny.UserData
{
  public class LevelProgress
  {
    [XmlAttribute(nameof(Scene))]
    public Scene Scene { get; set; }

    [XmlAttribute(nameof(IsComplete))]
    public bool IsComplete { get; set; }

    [XmlArray(nameof(Stars))]
    [XmlArrayItem(nameof(StarProgress))]
    public List<StarProgress> Stars { get; set; } = new List<StarProgress>();

    private LevelProgress() { }

    public LevelProgress(Scene scene)
    {
      Scene = scene;
    }

    public StarProgress GetStar(string starId)
      => Stars.SingleOrAdd(s => s.StarId == starId, () => new StarProgress(starId));
  }
}