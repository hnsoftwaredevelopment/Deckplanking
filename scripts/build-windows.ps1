param(
    [string] $Configuration = 'Release',
    [string] $VersionOverride
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'build-common.ps1')

$root = Get-RepositoryRoot
$buildVersion = Get-NextBuildVersion -VersionOverride $VersionOverride
$projectPath = Join-Path $root 'src\DeckPlanking.App\DeckPlanking.App.csproj'
$publishDirectory = Join-Path $root "artifacts\windows\Deckplanking-$($buildVersion.DisplayVersion)"

if (Test-Path -LiteralPath $publishDirectory) {
    Remove-Item -LiteralPath $publishDirectory -Recurse -Force
}

New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null

$projectDirectory = Split-Path -Parent $projectPath
$windowsBuildDirectories = @(
    (Join-Path $projectDirectory "bin\$Configuration\net10.0-windows10.0.19041.0"),
    (Join-Path $projectDirectory "obj\$Configuration\net10.0-windows10.0.19041.0")
)

foreach ($directory in $windowsBuildDirectories) {
    if (-not (Test-Path -LiteralPath $directory)) {
        continue
    }

    $resolvedDirectory = (Resolve-Path -LiteralPath $directory).Path
    if (-not $resolvedDirectory.StartsWith($projectDirectory, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to clean outside the app project directory: $resolvedDirectory"
    }

    Remove-Item -LiteralPath $resolvedDirectory -Recurse -Force
}

$arguments = @(
    'publish',
    $projectPath,
    '-f', 'net10.0-windows10.0.19041.0',
    '-c', $Configuration,
    '-p:WindowsPackageType=None',
    "-p:PublishDir=$publishDirectory\"
) + (Get-VersionMSBuildArguments -BuildVersion $buildVersion -ApplicationDisplayVersion '1.0.0')

& dotnet @arguments
if ($LASTEXITCODE -ne 0) {
    throw "Windows publish failed with exit code $LASTEXITCODE."
}

Remove-UnusedSatelliteLanguageDirectories -PublishDirectory $publishDirectory

$exePath = Join-Path $publishDirectory 'Deckplanking.exe'
if (-not (Test-Path -LiteralPath $exePath)) {
    throw "Expected executable was not created: $exePath"
}

Write-BuildSummary -Platform 'Windows' -OutputPath $exePath -BuildVersion $buildVersion
