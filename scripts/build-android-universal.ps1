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
$publishDirectory = Join-Path $root "artifacts\android-universal\Deckplanking-$($buildVersion.DisplayVersion)"

if (Test-Path -LiteralPath $publishDirectory) {
    Remove-Item -LiteralPath $publishDirectory -Recurse -Force
}

New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null

$arguments = @(
    'publish',
    $projectPath,
    '-f', 'net10.0-android',
    '-c', $Configuration,
    '-p:AndroidPackageFormat=apk',
    '-p:AndroidCreatePackagePerAbi=false',
    '-m:1',
    '-nr:false',
    "-p:PublishDir=$publishDirectory\"
) + (Get-VersionMSBuildArguments -BuildVersion $buildVersion)

& dotnet @arguments
if ($LASTEXITCODE -ne 0) {
    throw "Android universal publish failed with exit code $LASTEXITCODE."
}

$apkCandidates = Get-ChildItem -LiteralPath $publishDirectory -Filter '*-Signed.apk' -File -ErrorAction SilentlyContinue
$selectedApk = $apkCandidates | Select-Object -First 1
if ($null -eq $selectedApk) {
    throw "Expected signed universal APK was not created in: $publishDirectory"
}

$apkPath = Join-Path $publishDirectory "Deckplanking-$($buildVersion.DisplayVersion)-universal.apk"
if ($selectedApk.FullName -ne $apkPath) {
    Move-Item -LiteralPath $selectedApk.FullName -Destination $apkPath -Force
}

if (-not (Test-Path -LiteralPath $apkPath)) {
    throw "Expected signed universal APK was not created: $apkPath"
}

Write-BuildSummary -Platform 'Android Universal' -OutputPath $apkPath -BuildVersion $buildVersion
