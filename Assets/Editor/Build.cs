using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using static UnityEngine.Debug;
using UnityPlayerSettings = UnityEditor.PlayerSettings;

namespace PachowStudios.BadTummyBunny.Editor
{
  public class Build
  {
    private static string OutputFile { get; }
    private static string VersionNumber { get; }
    private static string VersionCode { get; }

    static Build()
    {
      Log("Reading build environment settings...");

      OutputFile = Environment.GetEnvironmentVariable("OUTPUT_FILE");
      VersionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER");
      VersionCode = Environment.GetEnvironmentVariable("VERSION_CODE");

      if (OutputFile == null)
        throw new ArgumentNullException(nameof(OutputFile));

      Log($"{nameof(OutputFile)}: {OutputFile}");

      if (VersionNumber == null)
        throw new ArgumentNullException(nameof(VersionNumber));

      Log($"{nameof(VersionNumber)}: {VersionNumber}");

      if (VersionCode == null)
        throw new ArgumentNullException(nameof(VersionCode));

      Log($"{nameof(VersionCode)}: {VersionCode}");

      UnityPlayerSettings.bundleVersion = VersionNumber;
    }

    [UsedImplicitly]
    public static void Android()
    {
      UnityPlayerSettings.Android.bundleVersionCode = int.Parse(VersionCode);

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