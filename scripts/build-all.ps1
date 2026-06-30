param(
    [string] $Configuration = 'Release',
    [string] $VersionOverride
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'build-common.ps1')

$buildVersion = Get-NextBuildVersion -VersionOverride $VersionOverride
$versionArgument = @{ VersionOverride = $buildVersion.DisplayVersion }

& (Join-Path $PSScriptRoot 'build-windows.ps1') -Configuration $Configuration @versionArgument
& (Join-Path $PSScriptRoot 'build-android-arm64.ps1') -Configuration $Configuration @versionArgument
