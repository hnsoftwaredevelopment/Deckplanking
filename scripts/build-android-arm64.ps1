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
$publishDirectory = Join-Path $root "artifacts\android-arm64\Deckplanking-$($buildVersion.DisplayVersion)"

if (Test-Path -LiteralPath $publishDirectory) {
    Remove-Item -LiteralPath $publishDirectory -Recurse -Force
}

New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null

$arguments = @(
    'publish',
    $projectPath,
    '-f', 'net10.0-android',
    '-c', $Configuration,
    '-r', 'android-arm64',
    '-p:AndroidPackageFormat=apk',
    '-m:1',
    '-nr:false',
    "-p:PublishDir=$publishDirectory\"
) + (Get-VersionMSBuildArguments -BuildVersion $buildVersion)

& dotnet @arguments
if ($LASTEXITCODE -ne 0) {
    throw "Android ARM64 publish failed with exit code $LASTEXITCODE."
}

$apkPath = Join-Path $publishDirectory 'nl.hnsoftwaredevelopment.deckplanking-Signed.apk'
if (-not (Test-Path -LiteralPath $apkPath)) {
    throw "Expected signed APK was not created: $apkPath"
}

Write-BuildSummary -Platform 'Android ARM64' -OutputPath $apkPath -BuildVersion $buildVersion
