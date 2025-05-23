
param (
    [switch]$ValidateNoChanges
)

if ($ValidateNoChanges) {
    $output = git status --porcelain=v1 -- docs
    if ($output) {
        $diff = git diff -- docs # HACK to ignore crlf changes 
        if ($diff) {
            git diff -- docs 
            throw "The docs generator has made changes to the docs folder. Please commit these changes and push them to the repository."
        }
    }
}

function GenerateDocs {
    param (
        [string]$project
    )

    Push-Location src
    Push-Location $project
    dotnet run generate
    Pop-Location
    Pop-Location
}

GenerateDocs -project AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs
GenerateDocs -project AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs.Nunit4
GenerateDocs -project AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs.Nunit3
GenerateDocs -project AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs.Xunit