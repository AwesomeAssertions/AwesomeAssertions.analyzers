param (
    [switch]$FormatAndExecuteTestsAgain
)

function RunTestsAndValidate {
    param (
        [string]$project
    )

    Push-Location src/docs
    Push-Location $project
    dotnet test

    if ($FormatAndExecuteTestsAgain) {
        $i = 1;
        do {
            Write-Host "formatting code... - Iteration $i"
            $out = dotnet format analyzers --diagnostics FAA0001 FAA0002 FAA0003 FAA0004 --severity info --verbosity normal 2>&1 | Out-String | Join-String

            Write-Host "-------------$i-------------"
            Write-Host $out
            Write-Host "-------------$i-------------"
            Write-Host "output length: $($out.Length)"

            $i++
        } while ($out.Contains("Unable to fix FAA000"))

        dotnet test
    }
    Pop-Location
    Pop-Location
}

RunTestsAndValidate -project AwesomeAssertions.Analyzers.Docs
RunTestsAndValidate -project AwesomeAssertions.Analyzers.Docs.MSTest3
RunTestsAndValidate -project AwesomeAssertions.Analyzers.Docs.Nunit4
RunTestsAndValidate -project AwesomeAssertions.Analyzers.Docs.Nunit3
RunTestsAndValidate -project AwesomeAssertions.Analyzers.Docs.Xunit