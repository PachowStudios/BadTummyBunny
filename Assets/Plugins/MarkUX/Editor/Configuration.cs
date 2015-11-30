using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MarkUX.Editor
{
  /// <summary>
  /// Serializable system configuration used by the asset processor.
  /// </summary>
  public class Configuration : ScriptableObject
  {
    public string UILayer;
    public List<string> ViewPaths;
    private static Configuration instance;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public Configuration()
    {
      ViewPaths = new List<string>
      {
        "Assets/UI/Views/",
        "Assets/UI/Themes/"
      };

      UILayer = "UI";
    }

    /// <summary>
    /// Loads global configuration asset.
    /// </summary>
    public static Configuration Load()
    {
      return null;
    }

    /// <summary>
    /// Gets global configuration instance.
    /// </summary>
    public static Configuration Instance
    {
      get
      {
        if (instance != null)
          return instance;

        // attempt to load configuration asset
        var configuration = AssetDatabase.LoadAssetAtPath<Configuration>("Assets/Resources/MarkUX.asset");

        if (configuration == null)
        {
          // create new asset                        
          Directory.CreateDirectory("Assets/Resources/");
          configuration = CreateInstance<Configuration>();
          AssetDatabase.CreateAsset(configuration, "Assets/Resources/MarkUX.asset");
          AssetDatabase.Refresh();
        }

        // validate some values
        if (!configuration.ViewPaths.Any())
        {
          Debug.LogError("[MarkUX.356] No view paths found. Using default configuration.");
          configuration = CreateInstance<Configuration>();
        }
        else if (string.IsNullOrEmpty(configuration.UILayer))
        {
          Debug.LogError("[MarkUX.357] UILayer not set. Using default configuration.");
          configuration = CreateInstance<Configuration>();
        }
        else
        {
          foreach (var viewPath in configuration.ViewPaths)
          {
            if (string.IsNullOrEmpty(viewPath)
                || !viewPath.StartsWith("Assets/")
                || !viewPath.EndsWith("/"))
            {
              Debug.LogError("[MarkUX.358] Invalid view path in configuration. The path must start with 'Assets/' and end with '/'. Using default configuration.");
              Debug.LogError("This sometimes happens if Unity hasn't converted the configuration asset to correct serialization mode. To fix go to [Edit -> Project settings -> Editor] and change Asset Serialization Mode to another mode and back to the desired mode. If you inspect the Configuration asset at MarkUX/Configuration/Configuration.asset the values should be in plain text and the view paths should look like file path strings (not a bunch of numbers).");

              configuration = CreateInstance<Configuration>();
              break;
            }
          }
        }

        instance = configuration;

        return instance;
      }
    }
  }
}
