# Modelboat Deckplanking - Initial Release Notes

Date: 2026-06-30

## Highlights

- Added the first complete Windows and Android version of Modelboat Deckplanking.
- Added deck planking calculations for model scale plank lengths and butt-shift patterns.
- Added a graphical deck preview with ruler, orientation, zoom, deck contour, king plank, and trenail display options.
- Added local project save, open, rename, and delete workflows.
- Added PNG, PDF, and print export.
- Added multilingual UI support for English, Dutch, German, French, Spanish, and Italian.
- Added light, dark, blue, saffron, and dark red theme preparation.
- Added feedback submission through a Cloudflare Worker that creates GitHub issues without exposing a GitHub token in the app.

## Windows

- Added a Windows desktop build named `Deckplanking.exe`.
- Added cleaned Windows artifact output under `artifacts\windows\Deckplanking-YY.MM.DD.xxx`.
- Added an Inno Setup installer build under `Installer`.
- Installer output is written to `artifacts\installer`.
- The installer has been tested locally: the app installs and starts successfully.

## Android

- Added an ARM64 APK build for direct Samsung device testing.
- Added an Android App Bundle build for possible future Google Play distribution.
- Android startup, project handling, preview, exports, and feedback were tested successfully.

## Build And Release

- Added local PowerShell build scripts for Windows, Android ARM64, Android Play Store AAB, and all-platform builds.
- Added automatic `YY.MM.DD.xxx` version injection through the build scripts.
- Added GitHub Actions CI for .NET tests and feedback worker tests.
- Added manual GitHub Actions workflows for Windows and Android artifacts.
- Added a release checklist in `docs\release-checklist.md`.

## Validation

- Core test suite passed.
- Feedback worker test suite passed.
- GitHub CI passed on the feature branch.
- Windows app was tested directly from the artifact and through the installer.
- Android ARM64 APK was tested on Samsung devices.
