$apiKey = "oy2pyvbxsqymi rgbcsuafx4irl4aqe hu7ek2h35og3362u"

$packages = @(
#   "rbkApiModules.Analytics.Core";
#   "rbkApiModules.Analytics.LiteDB";    
#   "rbkApiModules.Analytics.Postgres";
#   "rbkApiModules.Analytics.SQLite";
#   "rbkApiModules.Analytics.SQLServer";
#   "rbkApiModules.Auditing.Core";
#   "rbkApiModules.Auditing.Relational";
#   "rbkApiModules.Comments.Core";
#   "rbkApiModules.Comments.Relational";
#   "rbkApiModules.Commons.Core";
#   "rbkApiModules.Commons.LiteDB";
#   "rbkApiModules.Commons.Relational";
#   "rbkApiModules.Diagnostics.Core";
#   "rbkApiModules.Diagnostics.LiteDB";
#   "rbkApiModules.Diagnostics.Postgres";
#   "rbkApiModules.Diagnostics.SQLite";
#   "rbkApiModules.Diagnostics.SQLServer";
#   "rbkApiModules.Faqs.Core";
#   "rbkApiModules.Faqs.Relational";
#   "rbkApiModules.Identity.Core";
#   "rbkApiModules.Identity.Relational";
#   "rbkApiModules.Notifications.Core";
#   "rbkApiModules.Notifications.Relational";
#   "rbkApiModules.SharedUI.Core";
#   "rbkApiModules.Testing.Core";
#   "rbkApiModules.Workflow.Core";
#   "rbkApiModules.Workflow.Relational";

    "rbkApiModules.Analytics.Core";
    "rbkApiModules.Analytics.Relational.Core";
    "rbkApiModules.Analytics.Relational.SqlServer";
    "rbkApiModules.Analytics.Relational.SQLite";
    "rbkApiModules.Auditing.SqlServer";
    "rbkApiModules.Authentication";
    "rbkApiModules.CodeGeneration";
    "rbkApiModules.CodeGeneration.Commons";
    "rbkApiModules.Comments";
    "rbkApiModules.Diagnostics.Commons";
    "rbkApiModules.Diagnostics.Core";
    "rbkApiModules.Diagnostics.Relational.Core";
    "rbkApiModules.Diagnostics.Relational.SqlServer";
    "rbkApiModules.Diagnostics.Relational.SQLite";
    "rbkApiModules.Faqs";
    "rbkApiModules.Infrastructure.Api";
    "rbkApiModules.Infrastructure.MediatR.Core";
    "rbkApiModules.Infrastructure.MediatR.MongoDB";
    "rbkApiModules.Infrastructure.MediatR.SqlServer";
    "rbkApiModules.Infrastructure.Models";
    "rbkApiModules.Logs.Core";
    "rbkApiModules.Logs.Relational.Core";
    "rbkApiModules.Logs.Relational.SQLite";
    "rbkApiModules.Logs.Relational.SqlServer";
    "rbkApiModules.Notifications";
    "rbkApiModules.Payment.SqlServer";
    "rbkApiModules.Paypal.SqlServer";
    "rbkApiModules.SharedUI";
    "rbkApiModules.UIAnnotations";
    "rbkApiModules.UIAnnotations.Commons";
    "rbkApiModules.Utilities";
    "rbkApiModules.Utilities.EFCore";
    "rbkApiModules.Utilities.Excel";
    "rbkApiModules.Utilities.MongoDB";
    "rbkApiModules.Utilities.Testing";
)

$count = 0

foreach ($packageId in $packages)
{
    $searchResponse = Invoke-WebRequest -Uri "https://azuresearch-usnc.nuget.org/query?q=$packageId&prerelease=false" | ConvertFrom-Json

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