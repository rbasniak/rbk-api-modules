Remove-Item .\TestResults\ -Recurse -ErrorAction Ignore
dotnet test --collect:"Code Coverage" --settings:./CodeCoverage.runsettings
$coverageExePathFile = gci $env:USERPROFILE/.nuget/packages/microsoft.codecoverage -Recurse -fi CodeCoverage.exe | sort LastWriteTime | select -last 1 | % { Write-Output $_.FullName }
$coveragePathFile = gci ./rbkApiModules.Analytics.Core.Tests\TestResults -Recurse -fi *.coverage | select -f 1 | % { Write-Output $_.FullName }
& $coverageExePathFile analyze  /output:TestResults/coverage.cobertura.xml  $coveragePathFile
dotnet reportgenerator "-reports:D:\Repositories\pessoal\libraries\rbk-api-modules-next\TestResults\results.coveragexml" "-targetdir:TestResults/html" -reporttypes:HTML
.\TestResults\html\index.htm