$packageId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
$apiKey = "oy2peqe43cv4sscbgy6qnt x7yqtv3kdexh3gmosht2u5vq"

$json = Invoke-WebRequest -Uri "https://api.nuget.org/v3-flatcontainer/$PackageId/index.json" | ConvertFrom-Json

foreach($version in $json.versions)
{
  Write-Host "Unlisting $packageId, Ver $version"
  dotnet nuget delete $packageId $version --source https://api.nuget.org/v3/index.json --non-interactive --api-key $apiKey
}