$apiKey = "oy2iqjnvl3zsdxp7wl3ag6pdlm2elqsuxp7epv537utk6u"

$packages = @(
    "rbkApiModules.Workflow.Relational";
)

$count = 0

$versions = @(
    "1";
    "2";
    "3";
    "4";
    "5";
    "6";
    "7";
    "8";
    "9";
    "10";
    "11";
    "12";
    "13";
    "14";
    "15";
    "16";
    "17";
    "18";
    "19";
    "20";
    "21";
    "22";
    "23";
    "24";
    "25";
    "26";
    "27";
    "28";
    "29";
    "30";
    "31";
    "32";
    "33";
    "34";
    "35";
    "36";
    "37";
    "38";
    "39";
    "40";
    "41";
    "42";
    "43";
    "44";
    "45";
    "46";
    "47";
    "48";
    "49";
    "50";
    "51";
    "52";
    "53";
    "54";
)

foreach ($packageId in $packages)
{
    foreach($versionNumber in $versions)
    {
        dotnet nuget delete $packageId 7.0.0-beta.$versionNumber --source https://api.nuget.org/v3/index.json --non-interactive --api-key $apiKey

        $count = $count + 1

        # if ($count -eq 150)
        # {
        #     Write-Host "*****************************************************************"
        #     Write-Host "   Maximum number of requests reached. Come back in one hour.    "
        #     Write-Host "*****************************************************************"
        # 
        #     return;
        # }
    }   
}











return
foreach ($packageId in $packages)
{
    $searchResponse = Invoke-WebRequest -Uri "https://azuresearch-usnc.nuget.org/query?q=$packageId" | ConvertFrom-Json

    $found = $false

    foreach($result in $searchResponse.data)
    {
	    $title = $result.title

        if ($title -eq $packageId)
        {
            $found = $true

            $versionCount = $result.versions.count

            Write-Host "***** $title ($($versionCount - 1) versions to unlist)"

	        $index = 0
	        foreach($version in $result.versions)
	        {
		        $versionNumber = $version.version
		        
                if ($index -eq ($versionCount - 1))
                {
                    Write-Host "    Ignoring $versionNumber"
                }
                else
                {
                    Write-Host "  Unlisting $versionNumber ($count)"
		            
                    dotnet nuget delete $packageId $versionNumber --source https://api.nuget.org/v3/index.json --non-interactive --api-key $apiKey

                    $count = $count + 1

                    if ($count -eq 150)
                    {
                        Write-Host "*****************************************************************"
                        Write-Host "   Maximum number of requests reached. Come back in one hour.    "
                        Write-Host "*****************************************************************"

                        return;
                    }
                }

                $index = $index + 1
	        }
        }
    }

    if ($found -eq $false)
    {
        Write-Host "***** $title was not found"
    }
}