param(
    [string]$Configuration = "Release",
    [string]$VersionOverride = "",
    [string]$WindowsArtifactPath = "",
    [string]$InstallerOutputDir = "",
    [string]$InnoCompilerPath = "",
    [switch]$SkipPublish
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$windowsArtifactsRoot = Join-Path $repoRoot "artifacts\windows"
$installerDir = if ([string]::IsNullOrWhiteSpace($InstallerOutputDir)) {
    Join-Path $repoRoot "artifacts\installer"
} else {
    $InstallerOutputDir
}
$scriptPath = Join-Path $PSScriptRoot "Installer.iss"

New-Item -ItemType Directory -Force $installerDir | Out-Null

if (-not $SkipPublish) {
    $buildScript = Join-Path $repoRoot "scripts\build-windows.ps1"
    $buildArguments = @{
        Configuration = $Configuration
    }

    if (-not [string]::IsNullOrWhiteSpace($VersionOverride)) {
        $buildArguments.VersionOverride = $VersionOverride
    }

    & $buildScript @buildArguments

    if ($LASTEXITCODE -ne 0) {
        throw "Windows artifact build failed with exit code $LASTEXITCODE."
    }
}

if ([string]::IsNullOrWhiteSpace($WindowsArtifactPath)) {
    if (-not [string]::IsNullOrWhiteSpace($VersionOverride)) {
        $WindowsArtifactPath = Join-Path $windowsArtifactsRoot "Deckplanking-$VersionOverride"
    } else {
        $WindowsArtifactPath = Get-ChildItem -LiteralPath $windowsArtifactsRoot -Directory -Filter "Deckplanking-*" -ErrorAction SilentlyContinue |
            Where-Object { Test-Path (Join-Path $_.FullName "Deckplanking.exe") } |
            Sort-Object LastWriteTime -Descending |
            Select-Object -First 1 -ExpandProperty FullName
    }
}

if ([string]::IsNullOrWhiteSpace($WindowsArtifactPath) -or -not (Test-Path (Join-Path $WindowsArtifactPath "Deckplanking.exe"))) {
    throw "Windows artifact output was not found. Build it first or pass -WindowsArtifactPath."
}

$WindowsArtifactPath = (Resolve-Path -LiteralPath $WindowsArtifactPath).Path
$installerDir = (Resolve-Path -LiteralPath $installerDir).Path
$setupIconPath = Join-Path $repoRoot "src\DeckPlanking.App\Resources\AppIcon\appicon.ico"
$artifactVersion = Split-Path -Leaf $WindowsArtifactPath
if ($artifactVersion -match '^Deckplanking-(?<Version>\d{2}\.\d{2}\.\d+)$') {
    $artifactVersion = $Matches.Version
} else {
    $artifactVersion = (Get-Item -LiteralPath (Join-Path $WindowsArtifactPath "Deckplanking.exe")).VersionInfo.ProductVersion
}

if ([string]::IsNullOrWhiteSpace($InnoCompilerPath)) {
    $candidates = @(
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
        "C:\Program Files\Inno Setup 6\ISCC.exe",
        "C:\Users\hnijk\OneDrive\DevOps\hnsoftwaredevelopment\InnoSetup\Inno-All-in-One-Setup-master\IsPack_5_5_2\isfiles-unicode\ISCC.exe",
        "C:\Users\hnijk\OneDrive\DevOps\hnsoftwaredevelopment\InnoSetup\Inno-All-in-One-Setup-master\IsPack_5_5_2\isfiles\ISCC.exe"
    )

    $InnoCompilerPath = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
}

if (-not (Test-Path $InnoCompilerPath)) {
    throw "ISCC.exe was not found. Install Inno Setup 6 or pass -InnoCompilerPath."
}

& $InnoCompilerPath `
    "/DPublishDir=$WindowsArtifactPath" `
    "/DInstallerOutputDir=$installerDir" `
    "/DSetupIconFile=$setupIconPath" `
    "/DAppVersion=$artifactVersion" `
    $scriptPath

if ($LASTEXITCODE -ne 0) {
    throw "Inno Setup compiler failed with exit code $LASTEXITCODE."
}

Get-ChildItem -LiteralPath $installerDir -Filter "DeckplankingSetup-*.exe" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1
