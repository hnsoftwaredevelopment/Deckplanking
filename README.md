# Modelboat Deckplanking

Modelboat Deckplanking is a .NET MAUI application for designing model ship deck planking patterns on Windows desktop and Android. It helps model builders calculate plank lengths, choose butt-shift patterns, preview the deck layout, and export the result for workshop use.

## Features

- Scale plank length calculation for metric and imperial display units.
- Standard butt-shift patterns: every 2, every 3, every 4, and every 5.
- Configurable row calculation based on deck width and plank width.
- Optional king plank with separate width.
- Bow and stern contour settings with taper length and roundness.
- Graphical deck preview with ruler, orientation switch, zoom, and trenail display options.
- PNG, PDF, and print export.
- Local project save, open, rename, and delete.
- Multilingual UI: English, Dutch, German, French, Spanish, and Italian.
- App feedback flow that creates GitHub issues through a Cloudflare Worker.

## Solution Structure

- `src/DeckPlanking.Core` contains platform-independent calculation, preview, export naming, project serialization, and feedback payload logic.
- `tests/DeckPlanking.Core.Tests` contains unit tests for the core.
- `src/DeckPlanking.App` contains the MAUI application.
- `feedback-worker` contains the Cloudflare Worker that receives app feedback and creates GitHub issues.
- `scripts` contains local build scripts for Windows and Android artifacts.
- `installer` is reserved for the Windows installer scripts.

## Platform Targets

- Windows: `net10.0-windows10.0.19041.0`
- Android: `net10.0-android`
- Minimum Android API: 21.

## Local Build Scripts

Run the build scripts from the repository root.

```powershell
.\scripts\build-windows.ps1
.\scripts\build-android-arm64.ps1
.\scripts\build-android-playstore.ps1
```

Or build all artifacts with one shared version:

```powershell
.\scripts\build-all.ps1
```

The scripts write output to `artifacts`:

- `artifacts/windows/Deckplanking-YY.MM.DD.xxx`
- `artifacts/android-arm64/Deckplanking-YY.MM.DD.xxx`
- `artifacts/android-playstore/Deckplanking-YY.MM.DD.xxx`

Use `-VersionOverride` for a deterministic build:

```powershell
.\scripts\build-windows.ps1 -VersionOverride 26.06.30.001
```

## Syncfusion License

The MAUI project can generate a local source file from one of these ignored license files:

- `docs/SyncfusionLicense.txt`
- `docs/SynfusionLicense.txt`
- `docs/synfusion.txt`
- `syncfusion-license.txt`

Do not commit Syncfusion license files. In GitHub Actions, use the `SYNCFUSION_LICENSE` repository secret.

## Android Signing

`scripts/build-android-arm64.ps1` creates an ARM64 APK for direct device testing.

`scripts/build-android-playstore.ps1` creates an AAB for Google Play. For a real Play Store upload, provide signing values through parameters or environment variables:

```powershell
$env:ANDROID_KEYSTORE_PATH="C:\path\to\upload.keystore"
$env:ANDROID_KEYSTORE_PASSWORD="..."
$env:ANDROID_KEY_ALIAS="..."
$env:ANDROID_KEY_PASSWORD="..."
.\scripts\build-android-playstore.ps1
```

## GitHub Actions

The repository contains these workflows:

- `CI`: runs .NET core tests and feedback worker tests.
- `Build Windows`: manually publishes a Windows artifact.
- `Build Android`: manually publishes the ARM64 APK and Play Store AAB artifacts.

Build workflows use `YY.MM.DD.xxx` versions. On GitHub, `xxx` is based on the workflow run number unless a version override is supplied manually.

## Build Output Notes

- Windows artifacts intentionally keep only the app languages plus required runtime directories such as `Microsoft.UI.Xaml` and `NpuDetect`.
- MAUI generates scaled PNG assets from SVG resources for Windows and Android. These generated image files are part of the platform asset pipeline and should not be removed manually from build output.
