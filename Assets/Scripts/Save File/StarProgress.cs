using System.Xml.Serialization;

namespace PachowStudios.BadTummyBunny.UserData
{
  public class StarProgress
  {
    [XmlAttribute(nameof(StarId))]
    public string StarId { get; set; }

    [XmlAttribute(nameof(IsCompleted))]
    public bool IsCompleted { get; set; }

    private StarProgress() { }

    public StarProgress(string starId)
    {
      StarId = starId;
    }
  }
}