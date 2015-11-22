using System.IO;
using System.Xml;
using UnityEngine;

public static class SaveService
{
  private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "BadTummyBunny.save.xml");

  public static SaveFile CurrentSave { get; private set; } = new SaveFile();

  public static void Load()
  {
    if (!File.Exists(SaveFilePath))
      return;

    var xmlDoc = new XmlDocument();

    xmlDoc.Load(SaveFilePath);
    CurrentSave = xmlDoc.DeserializeToObject<SaveFile>();
  }

  public static void Save()
  {
    if (CurrentSave == null)
      return;

    var xmlDoc = CurrentSave.SerializeToXml();

    xmlDoc.Save(SaveFilePath);
  }
}