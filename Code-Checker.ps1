$dir = "C:\git\Repositories\rbk-api-modules\Demo1"

$files = Get-ChildItem -Path $dir -Recurse -Include "*.cs" | Where-Object { $_.FullName -notmatch "\\bin\\|\\obj\\|\\Migrations\\" }

$found = 0

foreach ($file in $files) {
    $messages = @()

    $content = Get-Content $file.FullName -Raw

    # Check for namespace declarations using brackets
    if ($content -match "(?s)namespace\s+[\w\.]+\s*\{") {
        $messages += "Namespace declaration using brackets"
        $found++
    }

    # Check for more than 3 using clauses in the beginning of the file
    $usingClauses = ($content -split '\r?\n' | Select-String "^using\s" | Select-Object -Unique).Count
    if ($usingClauses -gt 3) {
        $messages += "More than 3 using clauses"
        $found++
    }

    # Check for async methods without the Async sufix
    $taskNamingPattern = "async\s+Task(<\w+>)?\s+(\w+)(?<!Async)\("
    $namedTaskMethods = [regex]::Matches($content, $taskNamingPattern)
    foreach ($match in $namedTaskMethods) {
        $methodName = $match.Groups[2].Value
        if ($methodName -notmatch "Async" -and $methodName -ne "Handle") {
            Write-Output "Defines async method '$methodName' without 'Async' in the name"
            $found++
        }
    }

    # Check for methods calls of methods postfixed with Async word and not passing a variable called cancellation or cancellationToken
    $asyncMethodCalls = $content | Select-String "\b\w+Async\s*\((?:(?!(cancellation|cancellationToken)\b)[^)]*)\);" -AllMatches
    if ($asyncMethodCalls.Matches.Count -gt 0) {
        foreach ($match in $asyncMethodCalls.Matches) {
            $messages += "Async method call without passing a CancellationToekn: .$match"
            $found++
        }
    }

    # Check for method definitions that return Task or Task<T> and doesn't receive a CancellationToken as parameter
    $asyncMethods = $content | Select-String "(public|private|protected|internal)+\s*(async\s+)?\b(Task|ValueTask)<?.+>?\s\b\w+\b\s*\(.*\)" -AllMatches
    if ($asyncMethods.Matches.Count -gt 0) {
        foreach ($method in $asyncMethods.Matches) {
            $inputString = $method
            $cancellationTokenPattern = "\bCancellationToken\b"
            if ($inputString -notmatch $cancellationTokenPattern) {
                $methodNamePattern = "\b\w+\b(?=\()"
                $methodName = [regex]::Match($inputString, $methodNamePattern).Value
                $messages += "Async method definition without a CancellationToken parameter: $methodName"
                $found++
            }
        }
    }

    # Check for method receiving a parameter of type Request and the parameter called command
    $requestMethods = $content | Select-String "(public|private|protected|internal)+\s+.*\(Request\s+command" -AllMatches
    if ($requestMethods.Matches.Count -gt 0) {
        $messages += "Badly named parameter of type 'Request'"
        $found++
    }

    # Check for Newtonsoft calls
    $requestMethods = $content | Select-String "\bNewtonsoft\b" -AllMatches
    if ($requestMethods.Matches.Count -gt 0) {
        $messages += "Newtonsoft.JSON is not allowed"
        $found++
    }

    # Check for Update calls on the DbContext
    $requestMethods = $content | Select-String "\b_context.\w*.Update\(" -AllMatches
    if ($requestMethods.Matches.Count -gt 0) {
        $messages += "'Update' method from DbContext is not allowed"
        $found++
    }

    # Too many namespaces
    $requestMethods = $content | Select-String "\bnamespace\s*\w*.\w*.\w*" -AllMatches
    if ($requestMethods.Matches.Count -gt 0) {
        $messages += "Namespace with too many regions, only two are allowed"
        $found++
    }

    # Output messages for this file
    if ($messages.Count -gt 0) {
        Write-Host "File: $($file.FullName)"
        foreach ($message in $messages) {
            Write-Host " - $message"
        }
    }
}
