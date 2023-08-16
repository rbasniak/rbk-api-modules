param(
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]
	$MigrationName,
	
	[ValidateNotNullOrEmpty()]
	[string]
	$ContextName = "DatabaseContext" 
)

$nativeDatabaseProvider = "Postgres"
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Host $scriptPath

$apiProjectName = Split-Path (Split-Path $scriptPath -Parent) -Leaf
$projectName = $apiProjectName.split(".")[0]

$startupProjectPath = $scriptPath
$databaseProjectPath = "${scriptPath}\..\${$projectName}.Database"

if (Test-Path -Path $databaseProjectPath) {
	# solution uses multiple projects
	$migrationsFolder = "Migrations"
} else {
	# single project in the solution
    $databaseProjectPath = $startupProjectPath
	$migrationsFolder = "Database\Migrations"
}

Write-Host "Creating migration '$MigrationName' for context '$ContextName' and native database provider"
dotnet ef migrations add $MigrationName --context $ContextName --startup-project $startupProjectPath --project $databaseProjectPath --output-dir "${migrationsFolder}\${nativeDatabaseProvider}" -- --provider native

Write-Host "Creating migration '$MigrationName' for context 'Testing${ContextName}' and SQLite provider"
dotnet ef migrations add $MigrationName --context Testing${ContextName} --startup-project $startupProjectPath --project $databaseProjectPath --output-dir "${migrationsFolder}\SQLite" -- --provider sqlite

Write-Host "Done!"