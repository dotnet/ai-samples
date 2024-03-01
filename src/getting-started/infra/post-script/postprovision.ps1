function Set-DotnetUserSecrets {
    param ($path, $lines)
    Push-Location
    cd $path
    dotnet user-secrets init
    dotnet user-secrets clear
    foreach ($line in $lines) {
        $name, $value = $line -split '='
        $value = $value -replace '"', ''
        $name = $name -replace '__', ':'
        if ($value -ne '') {
            dotnet user-secrets set $name $value | Out-Null
        }
    }
    Pop-Location
}

$lines = (azd env get-values) -split "`n"
Set-DotnetUserSecrets -path "./01-HikeBenefitsSummary/" -lines $lines
Set-DotnetUserSecrets -path "./02-HikerAI/"     -lines $lines
Set-DotnetUserSecrets -path "./03-ChattingAboutMyHikes/" -lines $lines
# Set-DotnetUserSecrets -path "./04-HikeTracker/" -lines $lines
Set-DotnetUserSecrets -path "./05-HikeImages/" -lines $lines
