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
    static ProjectFileGenerationPostProcessor()
    {
      ProjectFilesGenerator.ProjectFileGeneration += (fileName, fileContent) =>
      {
        var project = XDocument.Parse(fileContent);

        project.Descendants("LangVersion").FirstOrDefault()?.Remove();

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
