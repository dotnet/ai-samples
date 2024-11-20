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
Set-DotnetUserSecrets -path "./azure-openai/01-HikeBenefitsSummary/" -lines $lines


Set-DotnetUserSecrets -path "./semantic-kernel/01-HikeBenefitsSummary/" -lines $lines
Set-DotnetUserSecrets -path "./semantic-kernel/02-HikerAI/"     -lines $lines
Set-DotnetUserSecrets -path "./semantic-kernel/03-ChattingAboutMyHikes/" -lines $lines
Set-DotnetUserSecrets -path "./semantic-kernel/04-HikerAIPro/" -lines $lines
Set-DotnetUserSecrets -path "./semantic-kernel/05-HikeImages/" -lines $lines
