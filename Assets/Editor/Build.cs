using System;
using System.Linq;
using UnityEditor;
using UnityPlayerSettings = UnityEditor.PlayerSettings;

namespace PachowStudios.BadTummyBunny.Editor
{
  public class Build
  {
    private static string BuildPath { get; }
    private static string VersionNumber { get; }
    private static string BuildNumber { get; }

    static Build()
    {
      BuildPath = Environment.GetEnvironmentVariable("BUILD_PATH");
      VersionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER");
      BuildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");

      if (BuildPath == null)
        throw new ArgumentNullException(nameof(BuildPath));

      if (VersionNumber == null)
        throw new ArgumentNullException(nameof(VersionNumber));

      if (BuildNumber == null)
        throw new ArgumentNullException(nameof(BuildNumber));

      UnityPlayerSettings.bundleVersion = VersionNumber;
    }

    public static void Android()
    {
      UnityPlayerSettings.Android.bundleVersionCode = int.Parse(BuildNumber);

      BuildPipeline.BuildPlayer(
        EditorBuildSettings.scenes
          .Where(s => s.enabled)
          .Select(s => s.path)
          .ToArray(),
        $@"{BuildPath}\BadTummyBunny-{VersionNumber}.apk",
        BuildTarget.Android,
        BuildOptions.None);
    }
  }
}