using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using static UnityEditor.BuildPipeline;
using static UnityEngine.Debug;
using UnityPlayerSettings = UnityEditor.PlayerSettings;

namespace PachowStudios.BadTummyBunny.BuildPipeline
{
  public static class Build
  {
    private static string OutputFile { get; }
    private static string VersionNumber { get; }
    private static int VersionCode { get; }
    private static BuildOptions BuildOptions { get; }

    static Build()
    {
      Log("Reading build environment settings...");

      OutputFile = GetBuildVariable("OUTPUT_FILE");
      VersionNumber = GetBuildVariable("VERSION_NUMBER");
      VersionCode = int.Parse(GetBuildVariable("VERSION_CODE"));
      BuildOptions = GetBuildVariable("BUILD_OPTIONS").ToEnum<BuildOptions>();
    }

    [UsedImplicitly]
    public static void Android()
    {
      UnityPlayerSettings.bundleVersion = VersionNumber;
      UnityPlayerSettings.Android.bundleVersionCode = VersionCode;

      BuildPlayer(
        EditorBuildSettings.scenes
          .Where(s => s.enabled)
          .Select(s => s.path)
          .ToArray(),
        OutputFile,
        BuildTarget.Android,
        BuildOptions);
    }

    private static string GetBuildVariable(string variable)
    {
      var value = Environment.GetEnvironmentVariable(variable);

      if (value == null)
        throw new ArgumentNullException(variable);

      Log($"{variable}: {value}");

      return value;
    }
  }
}