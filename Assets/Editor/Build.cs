using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityPlayerSettings = UnityEditor.PlayerSettings;

namespace PachowStudios.BadTummyBunny.Editor
{
  public class Build
  {
    private static string OutputFile { get; }
    private static string VersionNumber { get; }
    private static string BuildNumber { get; }

    static Build()
    {
      OutputFile = Environment.GetEnvironmentVariable("OUTPUT_FILE");
      VersionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER");
      BuildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");

      if (OutputFile == null)
        throw new ArgumentNullException(nameof(OutputFile));

      if (VersionNumber == null)
        throw new ArgumentNullException(nameof(VersionNumber));

      if (BuildNumber == null)
        throw new ArgumentNullException(nameof(BuildNumber));

      UnityPlayerSettings.bundleVersion = VersionNumber;
    }

    [UsedImplicitly]
    public static void Android()
    {
      UnityPlayerSettings.Android.bundleVersionCode = int.Parse(BuildNumber);

      BuildPipeline.BuildPlayer(
        EditorBuildSettings.scenes
          .Where(s => s.enabled)
          .Select(s => s.path)
          .ToArray(),
        OutputFile,
        BuildTarget.Android,
        BuildOptions.None);
    }
  }
}