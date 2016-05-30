using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using ModestTree;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEditor;

namespace PachowStudios.BadTummyBunny.BuildPipeline
{
  [InitializeOnLoad, UsedImplicitly]
  public static class ProjectFileGenerationPostProcessor
  {
    private static readonly string[] referencesToRemove =
    {
      "Boo.Lang",
      "UnityScript.Lang"
    };

    static ProjectFileGenerationPostProcessor()
    {
      ProjectFilesGenerator.ProjectFileGeneration += OnProjectFileGeneration;
    }

    private static string OnProjectFileGeneration(string fileName, string fileContent)
    {
      var project = XDocument.Parse(fileContent);
      var xmlns = project.Root?.GetDefaultNamespace();

      ProcessProject(project, xmlns);

      using (var stringWriter = new Utf8StringWriter())
      {
        project.Save(stringWriter);

        return stringWriter.ToString();
      }
    }

    private static void ProcessProject(XDocument project, XNamespace xmlns)
    {
      // Remove C# v4 tag
      project.FindDescendants("LangVersion").Remove();

      // Remove unused references
      project
        .FindDescendants("Reference")
        .Where(e => e
          .Attributes("Include")
          .Select(a => a.Value)
          .Intersect(referencesToRemove)
          .Any())
        .Remove();

      // Nest CS files in their corresponding XML files
      foreach (var pair in
        from cs in project.FindDescendants("Compile").Attributes("Include")
        join xml in project.FindDescendants("None").Attributes("Include")
          on GetPathWithoutExtension(cs.Value)
          equals GetPathWithoutExtension(xml.Value)
        where cs.Parent?.Descendants().IsEmpty() ?? false
        select new { cs, xml })
        pair.cs.Parent?.Add(new XElement(
          xmlns + "DependentUpon",
          Path.GetFileName(pair.xml.Value)));

      // Force the project file to regenerate by defining a magic constant
      project
        .FindDescendants("DefineConstants")
        .First()
        .Value += $";MAGIC_{Guid.NewGuid():n}";
    }

    private static IEnumerable<XElement> FindDescendants(this XContainer container, string name)
      => container.Descendants().Where(e => e.Name.LocalName == name);

    private static string GetPathWithoutExtension(string fullPath)
      => $"{Path.GetDirectoryName(fullPath)}\\{Path.GetFileNameWithoutExtension(fullPath)}";

    private class Utf8StringWriter : StringWriter
    {
      public override Encoding Encoding => Encoding.UTF8;
    }
  }
}
