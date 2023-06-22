$apiKey = "oy2pyvbxsqymi rgbcsuafx4irl4aqe hu7ek2h35og3362u"

$packages = @(
    "rbkApiModules.Analytics.Core";
    "rbkApiModules.Analytics.LiteDB";
    "rbkApiModules.Analytics.Postgres";
    "rbkApiModules.Analytics.SQLite";
    "rbkApiModules.Analytics.SQLServer";
    "rbkApiModules.Auditing.Core";
    "rbkApiModules.Auditing.Relational";
    "rbkApiModules.Comments.Core";
    "rbkApiModules.Comments.Relational";
    "rbkApiModules.Commons.Core";
    "rbkApiModules.Commons.LiteDB";
    "rbkApiModules.Commons.Relational";
    "rbkApiModules.Diagnostics.Core";
    "rbkApiModules.Diagnostics.LiteDB";
    "rbkApiModules.Diagnostics.Postgres";
    "rbkApiModules.Diagnostics.SQLite";
    "rbkApiModules.Diagnostics.SQLServer";
    "rbkApiModules.Faqs.Core	";
    "rbkApiModules.Faqs.Relational";
    "rbkApiModules.Identity.Core";
    "rbkApiModules.Identity.Relational";
    "rbkApiModules.Notifications.Core";
    "rbkApiModules.Notifications.Relational";
    "rbkApiModules.SharedUI.Core";
    "rbkApiModules.Testing.Core";
    "rbkApiModules.Workflow.Core";
    "rbkApiModules.Workflow.Relational";
)

$count = 0

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