# Modelboat Deckplanking

Modelboat Deckplanking is a .NET MAUI application for designing model ship deck planking patterns on Windows desktop and Android.

The first implementation slice focuses on a tested calculation core:

- scale plank length calculation from real-world plank lengths;
- standard butt-shift patterns: every 2, every 3, every 4, and every 5;
- seam-position generation per plank row;
- separate measurement rounding and pattern division concepts.

## Solution Structure

- `src/DeckPlanking.Core` contains platform-independent calculation logic.
- `tests/DeckPlanking.Core.Tests` contains unit tests for the core.
- `src/DeckPlanking.App` contains the MAUI application shell.

## Platform Targets

- Windows: `net10.0-windows10.0.19041.0`
- Android: `net10.0-android`
- Minimum Android API: 21, matching the current MAUI template baseline.

## Syncfusion License

Syncfusion licensing is loaded at startup from either:

- environment variable `DECKPLANKING_SYNCFUSION_LICENSE`; or
- local file `syncfusion-license.txt` next to the app binaries.

The local license file is ignored by Git and must not be committed.

## Build

```powershell
dotnet test
dotnet build
```

## Technical TODO

- Review the Windows build output before release and remove avoidable generated clutter from the distributable package, including unused image scales and unnecessary localization folders where MAUI allows this safely.
