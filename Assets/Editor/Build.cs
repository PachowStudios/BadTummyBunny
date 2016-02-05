using System;
using System.Linq;
using UnityEditor;
using UnityPlayerSettings = UnityEditor.PlayerSettings;

namespace PachowStudios.BadTummyBunny.Editor
{
  public static class Build
  {
    public static string VersionNumber { get; }
    public static string BuildNumber { get; }

    static Build()
    {
      VersionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER") ?? "1.0.0.0";
      BuildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "1";

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
        "BadTummyBunny.apk",
        BuildTarget.Android,
        BuildOptions.None);
    }
  }
}