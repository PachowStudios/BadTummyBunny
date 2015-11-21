using System.IO;
using System.Xml;
using System.Xml.Serialization;

public static class Serialization
{
  public static byte[] SerializeToXmlBytes<T>(T target)
  {
    using (var memoryStream = new MemoryStream())
      using (var xmlWriter = XmlWriter.Create(memoryStream))
      {
        new XmlSerializer(typeof(T)).Serialize(xmlWriter, target);

        return memoryStream.ToArray();
      }
  }

  public static T DeserializeXmlBytes<T>(byte[] bytes)
  {
    using (var memoryStream = new MemoryStream(bytes))
      using (var xmlReader = XmlReader.Create(memoryStream))
        return (T)new XmlSerializer(typeof(T)).Deserialize(xmlReader);
  }
}