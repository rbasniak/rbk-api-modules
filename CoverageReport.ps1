Get-ChildItem * -Include *coverage.cobertura.xml -Recurse | Remove-Item
dotnet tool install -g dotnet-reportgenerator-globaltool
dotnet test --collect:"XPlat Code Coverage" --settings:CodeCoverage.runsettings
reportgenerator "-reports:./**/coverage.cobertura.xml" "-targetdir:@TestResults/html" -reporttypes:HTML;
Get-ChildItem * -Include *coverage.cobertura.xml -Recurse | Remove-Item
.\@TestResults\html\index.html