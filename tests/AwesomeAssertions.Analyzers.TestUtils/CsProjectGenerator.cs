using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AwesomeAssertions.Analyzers.TestUtils;

public static class CsProjectGenerator
{
    private const string DefaultFilePathPrefix = "Test";
    private const string CSharpDefaultFileExt = "cs";
    private const string VisualBasicDefaultExt = "vb";
    private const string TestProjectName = "TestProject";

    public static Document CreateDocument(CsProjectArguments arguments) => CreateProject(arguments).Documents.First();

    public static Project CreateProject(CsProjectArguments arguments)
    {
        string fileNamePrefix = DefaultFilePathPrefix;
        string fileExt = arguments.Language is LanguageNames.CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;

        var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

        Solution solution = new AdhocWorkspace()
            .CurrentSolution
            .AddProject(projectId, TestProjectName, TestProjectName, arguments.Language);
        foreach (PackageReference package in arguments.PackageReferences)
        {
            solution = solution.AddPackageReference(projectId, package);
        }

        solution = solution.AddTargetFrameworkReference(projectId, arguments.TargetFramework);

        for (int i = 0; i < arguments.Sources.Length; i++)
        {
            var newFileName = string.Create(CultureInfo.InvariantCulture, $"{fileNamePrefix}{i}.{fileExt}");
            var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
            solution = solution.AddDocument(documentId, newFileName, SourceText.From(arguments.Sources[i]));
        }

        return solution.GetProject(projectId);
    }
}
