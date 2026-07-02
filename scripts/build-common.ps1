Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RepositoryRoot {
    return (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
}

function Get-NextBuildVersion {
    param(
        [string] $VersionOverride
    )

    if (-not [string]::IsNullOrWhiteSpace($VersionOverride)) {
        if ($VersionOverride -notmatch '^\d{2}\.\d{2}\.\d{2}\.\d{1,5}$') {
            throw "VersionOverride must use YY.MM.DD.xxx, for example 26.06.30.001."
        }

        $parts = $VersionOverride.Split('.')
        return [pscustomobject]@{
            DisplayVersion = $VersionOverride
            AssemblyVersion = '{0}.{1}.{2}.{3}' -f [int]$parts[0], [int]$parts[1], [int]$parts[2], [int]$parts[3]
            BuildNumber = [int]$parts[3]
        }
    }

    $root = Get-RepositoryRoot
    $artifactsDirectory = Join-Path $root 'artifacts'
    $statePath = Join-Path $artifactsDirectory 'build-version-state.json'
    $today = Get-Date -Format 'yy.MM.dd'
    $buildNumber = 1

    if (Test-Path -LiteralPath $statePath) {
        $state = Get-Content -LiteralPath $statePath -Raw | ConvertFrom-Json
        if ($state.date -eq $today) {
            $buildNumber = [int]$state.build + 1
        }
    }

    New-Item -ItemType Directory -Path $artifactsDirectory -Force | Out-Null
    [pscustomobject]@{
        date = $today
        build = $buildNumber
    } | ConvertTo-Json | Set-Content -LiteralPath $statePath -Encoding UTF8

    $displayVersion = '{0}.{1:000}' -f $today, $buildNumber
    $parts = $displayVersion.Split('.')

    return [pscustomobject]@{
        DisplayVersion = $displayVersion
        AssemblyVersion = '{0}.{1}.{2}.{3}' -f [int]$parts[0], [int]$parts[1], [int]$parts[2], [int]$parts[3]
        BuildNumber = $buildNumber
    }
}

function Get-VersionMSBuildArguments {
    param(
        [Parameter(Mandatory = $true)] $BuildVersion,
        [string] $ApplicationDisplayVersion = $BuildVersion.DisplayVersion
    )

    return @(
        "-p:AssemblyVersion=$($BuildVersion.AssemblyVersion)",
        "-p:FileVersion=$($BuildVersion.AssemblyVersion)",
        "-p:Version=$($BuildVersion.DisplayVersion)",
        "-p:InformationalVersion=$($BuildVersion.DisplayVersion)",
        '-p:IncludeSourceRevisionInInformationalVersion=false',
        "-p:ApplicationDisplayVersion=$ApplicationDisplayVersion",
        "-p:ApplicationVersion=$($BuildVersion.BuildNumber)"
    )
}

function Remove-UnusedSatelliteLanguageDirectories {
    param(
        [Parameter(Mandatory = $true)] [string] $PublishDirectory,
        [string[]] $KeepLanguages = @('en', 'en-us', 'en-gb', 'nl', 'nl-nl', 'de', 'de-de', 'fr', 'fr-fr', 'es', 'es-es', 'it', 'it-it')
    )

    if (-not (Test-Path -LiteralPath $PublishDirectory)) {
        throw "Publish directory does not exist: $PublishDirectory"
    }

    $keep = @{}
    foreach ($language in $KeepLanguages) {
        $keep[$language.ToLowerInvariant()] = $true
    }

    foreach ($directory in Get-ChildItem -LiteralPath $PublishDirectory -Directory) {
        $directoryName = $directory.Name.ToLowerInvariant()
        if ($keep.ContainsKey($directoryName)) {
            continue
        }

        $files = @(Get-ChildItem -LiteralPath $directory.FullName -File -Recurse -ErrorAction SilentlyContinue)
        if ($files.Count -eq 0) {
            continue
        }

        $containsOnlySatelliteResources = $true
        foreach ($file in $files) {
            if ($file.Extension -notin @('.mui', '.resources.dll')) {
                $containsOnlySatelliteResources = $false
                break
            }
        }

        if ($containsOnlySatelliteResources) {
            Remove-Item -LiteralPath $directory.FullName -Recurse -Force
        }
    }
}

function Write-BuildSummary {
    param(
        [Parameter(Mandatory = $true)] [string] $Platform,
        [Parameter(Mandatory = $true)] [string] $OutputPath,
        [Parameter(Mandatory = $true)] $BuildVersion
    )

    Write-Host "$Platform build complete"
    Write-Host "Version: $($BuildVersion.DisplayVersion)"
    Write-Host "Output: $OutputPath"
}
