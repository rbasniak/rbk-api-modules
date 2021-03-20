dotnet test --collect:"XPlat Code Coverage" --settings CodeCoverage.runsettings
reportgenerator "-reports:./**/coverage.cobertura.xml" "-targetdir:TestResults/html" -reporttypes:HTML;
.\@TestResults\html\index.html