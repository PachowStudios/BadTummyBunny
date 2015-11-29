using System.IO;
using System.Xml;
using UnityEngine;

public class SaveService
{
  private static string SaveFilePath { get; } = Path.Combine(Application.persistentDataPath, "BadTummyBunny.save.xml");

  public SaveFile CurrentSave { get; private set; } = new SaveFile();

  public void Load()
  {
    if (!File.Exists(SaveFilePath))
      return;

    var xmlDoc = new XmlDocument();

    xmlDoc.Load(SaveFilePath);
    CurrentSave = xmlDoc.DeserializeToObject<SaveFile>();
  }

  public void Save()
  {
    if (CurrentSave == null)
      return;

    var xmlDoc = CurrentSave.SerializeToXml();

    xmlDoc.Save(SaveFilePath);
  }
}