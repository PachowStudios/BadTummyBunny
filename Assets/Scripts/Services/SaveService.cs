using System.IO;
using System.Xml;
using PachowStudios.BadTummyBunny.UserData;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public class SaveService : ISaveContainer
  {
    private const string SaveFileName = "BadTummyBunny.save.xml";

    private static string SaveFilePath { get; } = Path.Combine(Application.persistentDataPath, SaveFileName);

    private SaveFile saveFile;

    public SaveFile SaveFile
    {
      get
      {
        if (this.saveFile == null)
          Load();

        return this.saveFile;
      }
      private set { this.saveFile = value; }
    }

    public void Load()
    {
      if (!File.Exists(SaveFilePath))
      {
        SaveFile = new SaveFile();
        return;
      }

      var xmlDoc = new XmlDocument();

      xmlDoc.Load(SaveFilePath);
      SaveFile = xmlDoc.DeserializeToObject<SaveFile>();
    }

    public void Save()
      => SaveFile.SerializeToXml().Save(SaveFilePath);
  }
}