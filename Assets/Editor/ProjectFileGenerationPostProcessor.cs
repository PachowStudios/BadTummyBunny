using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
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
      ProjectFilesGenerator.ProjectFileGeneration += (fileName, fileContent) =>
      {
        var project = XDocument.Parse(fileContent);

        project
          .Descendants()
          .Where(e => e.Name.LocalName == "LangVersion"
                      || (e.Name.LocalName == "Reference"
                          && e.Attributes().Select(a => a.Value).Intersect(referencesToRemove).Any()))
          .Remove();

        using (var stringWriter = new Utf8StringWriter())
        {
          project.Save(stringWriter);

          return stringWriter.ToString();
        }
      };
    }

    private class Utf8StringWriter : StringWriter
    {
      public override Encoding Encoding => Encoding.UTF8;
    }
  }
}
