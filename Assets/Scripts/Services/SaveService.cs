using System.IO;
using System.Xml;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class SaveService
  {
    private const string SaveFileName = "BadTummyBunny.save.xml";

    private static string SaveFilePath { get; } = Path.Combine(Application.persistentDataPath, SaveFileName);

    public SaveFile CurrentSave { get; private set; }

    public void Load()
    {
      if (!File.Exists(SaveFilePath))
        return;

      var xmlDoc = new XmlDocument();

      xmlDoc.Load(SaveFilePath);
      CurrentSave = xmlDoc.DeserializeToObject<SaveFile>();
    }

    public void Save()
      => (CurrentSave ?? new SaveFile()).SerializeToXml().Save(SaveFilePath);
  }
}