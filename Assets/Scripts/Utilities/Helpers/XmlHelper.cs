using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PachowStudios
{
  public static class XmlHelper
  {
    public static XmlDocument SerializeToXml(object toSerialize)
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

    public static T DeserializeToObject<T>(XmlDocument xmlDoc)
      where T : class
    {
      var serialzier = new XmlSerializer(typeof(T));

      using (var xmlReader = new XmlNodeReader(xmlDoc))
        return (T)serialzier.Deserialize(xmlReader);
    }

    public static byte[] XmlToBytes(XmlDocument xmlDoc)
    {
      using (var memoryStream = new MemoryStream())
      {
        xmlDoc.Save(memoryStream);

        return memoryStream.ToArray();
      }
    }

    public static XmlDocument BytesToXml(byte[] bytes)
    {
      var xmlDoc = new XmlDocument();

      using (var memoryStream = new MemoryStream(bytes))
        xmlDoc.Load(memoryStream);

      return xmlDoc;
    }
  }

  public static class XmlHelperExtensions
  {
    public static XmlDocument SerializeToXml(this object toSerialize)
      => XmlHelper.SerializeToXml(toSerialize);

    public static T DeserializeToObject<T>(this XmlDocument xmlDoc)
      where T : class 
      => XmlHelper.DeserializeToObject<T>(xmlDoc);

    public static byte[] ToBytes(this XmlDocument xmlDoc)
      => XmlHelper.XmlToBytes(xmlDoc);

    public static XmlDocument ToXml(this byte[] bytes)
      => XmlHelper.BytesToXml(bytes);
  }
}