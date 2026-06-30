param(
    [string] $Configuration = 'Release',
    [string] $VersionOverride,
    [string] $KeystorePath = $env:ANDROID_KEYSTORE_PATH,
    [string] $KeystorePassword = $env:ANDROID_KEYSTORE_PASSWORD,
    [string] $KeyAlias = $env:ANDROID_KEY_ALIAS,
    [string] $KeyPassword = $env:ANDROID_KEY_PASSWORD
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'build-common.ps1')

$root = Get-RepositoryRoot
$buildVersion = Get-NextBuildVersion -VersionOverride $VersionOverride
$projectPath = Join-Path $root 'src\DeckPlanking.App\DeckPlanking.App.csproj'
$publishDirectory = Join-Path $root "artifacts\android-playstore\Deckplanking-$($buildVersion.DisplayVersion)"

if (Test-Path -LiteralPath $publishDirectory) {
    Remove-Item -LiteralPath $publishDirectory -Recurse -Force
}

New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null

$arguments = @(
    'publish',
    $projectPath,
    '-f', 'net10.0-android',
    '-c', $Configuration,
    '-p:AndroidPackageFormat=aab',
    '-m:1',
    '-nr:false',
    "-p:PublishDir=$publishDirectory\"
) + (Get-VersionMSBuildArguments -BuildVersion $buildVersion)

$hasSigningConfiguration =
    -not [string]::IsNullOrWhiteSpace($KeystorePath) -or
    -not [string]::IsNullOrWhiteSpace($KeystorePassword) -or
    -not [string]::IsNullOrWhiteSpace($KeyAlias) -or
    -not [string]::IsNullOrWhiteSpace($KeyPassword)

if ($hasSigningConfiguration) {
    if ([string]::IsNullOrWhiteSpace($KeystorePath) -or
        [string]::IsNullOrWhiteSpace($KeystorePassword) -or
        [string]::IsNullOrWhiteSpace($KeyAlias) -or
        [string]::IsNullOrWhiteSpace($KeyPassword)) {
        throw "Provide all Android signing values: KeystorePath, KeystorePassword, KeyAlias, and KeyPassword."
    }

    $resolvedKeystorePath = (Resolve-Path -LiteralPath $KeystorePath).Path
    $arguments += @(
        '-p:AndroidKeyStore=true',
        "-p:AndroidSigningKeyStore=$resolvedKeystorePath",
        "-p:AndroidSigningStorePass=$KeystorePassword",
        "-p:AndroidSigningKeyAlias=$KeyAlias",
        "-p:AndroidSigningKeyPass=$KeyPassword"
    )
} else {
    Write-Warning "No Android signing configuration was supplied. The AAB is useful for build validation, but Google Play requires a signed bundle with your upload key."
}

& dotnet @arguments
if ($LASTEXITCODE -ne 0) {
    throw "Android Play Store publish failed with exit code $LASTEXITCODE."
}

$defaultOutputDirectory = Join-Path $root 'src\DeckPlanking.App\bin\Release\net10.0-android'
$aabCandidates = @(
    Get-ChildItem -LiteralPath $publishDirectory -Filter '*-Signed.aab' -File -ErrorAction SilentlyContinue
    Get-ChildItem -LiteralPath $defaultOutputDirectory -Filter '*-Signed.aab' -File -ErrorAction SilentlyContinue
    Get-ChildItem -LiteralPath $publishDirectory -Filter '*.aab' -File -ErrorAction SilentlyContinue
    Get-ChildItem -LiteralPath $defaultOutputDirectory -Filter '*.aab' -File -ErrorAction SilentlyContinue
) | Where-Object { $_ -ne $null } | Select-Object -Unique

$selectedAab = $aabCandidates | Select-Object -First 1
if ($null -eq $selectedAab) {
    throw "Expected AAB was not created in: $publishDirectory"
}

$temporaryAabPath = Join-Path ([System.IO.Path]::GetTempPath()) ("deckplanking-" + [System.Guid]::NewGuid().ToString("N") + ".aab")
Copy-Item -LiteralPath $selectedAab.FullName -Destination $temporaryAabPath -Force

Get-ChildItem -LiteralPath $publishDirectory -Force |
    Remove-Item -Recurse -Force

$aabPath = Join-Path $publishDirectory $selectedAab.Name
Copy-Item -LiteralPath $temporaryAabPath -Destination $aabPath -Force
Remove-Item -LiteralPath $temporaryAabPath -Force

if (-not (Test-Path -LiteralPath $aabPath)) {
    throw "Expected AAB was not copied to: $aabPath"
}

Write-BuildSummary -Platform 'Android Play Store' -OutputPath $aabPath -BuildVersion $buildVersion
