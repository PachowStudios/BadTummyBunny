using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace PachowStudios
{
  public static class XmlHelper
  {
    [NotNull, Pure]
    public static XmlDocument SerializeToXml([NotNull] object toSerialize)
    {
      var serializer = new XmlSerializer(toSerialize.GetType());

      using (var memoryStream = new MemoryStream())
      {
        using (var xmlWriter = XmlWriter.Create(memoryStream))
          serializer.Serialize(xmlWriter, toSerialize);

        memoryStream.Seek(0, SeekOrigin.Begin);

        var xmlDoc = new XmlDocument();

        xmlDoc.Load(memoryStream);

        return xmlDoc;
      }
    }

    [NotNull, Pure]
    public static T DeserializeToObject<T>([NotNull] XmlDocument xmlDoc)
      where T : class
    {
      var serialzier = new XmlSerializer(typeof(T));

      using (var xmlReader = new XmlNodeReader(xmlDoc))
        return (T)serialzier.Deserialize(xmlReader);
    }

    [NotNull, Pure]
    public static byte[] XmlToBytes([NotNull] XmlDocument xmlDoc)
    {
      using (var memoryStream = new MemoryStream())
      {
        xmlDoc.Save(memoryStream);

        return memoryStream.ToArray();
      }
    }

    [NotNull, Pure]
    public static XmlDocument BytesToXml([NotNull] byte[] bytes)
    {
      var xmlDoc = new XmlDocument();

      using (var memoryStream = new MemoryStream(bytes))
        xmlDoc.Load(memoryStream);

      return xmlDoc;
    }
  }

  public static class XmlHelperExtensions
  {
    [NotNull, Pure]
    public static XmlDocument SerializeToXml([NotNull] this object toSerialize)
      => XmlHelper.SerializeToXml(toSerialize);

    [NotNull, Pure]
    public static T DeserializeToObject<T>([NotNull] this XmlDocument xmlDoc)
      where T : class 
      => XmlHelper.DeserializeToObject<T>(xmlDoc);

    [NotNull, Pure]
    public static byte[] ToBytes([NotNull] this XmlDocument xmlDoc)
      => XmlHelper.XmlToBytes(xmlDoc);

    [NotNull, Pure]
    public static XmlDocument ToXml([NotNull] this byte[] bytes)
      => XmlHelper.BytesToXml(bytes);
  }
}