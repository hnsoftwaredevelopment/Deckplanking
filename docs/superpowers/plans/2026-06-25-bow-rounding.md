# Bow Rounding Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a persisted numeric bow-rounding setting that rounds the narrowed bow contour in preview, export, PDF, and print.

**Architecture:** Extend `DeckContourSettings` and `DeckContourBuilder` in core so the app renderer keeps consuming one polygon contour. Bind the new value through `ScaleInputViewModel`, add one localized input row, and pass it to `DeckPatternPreviewDrawable`.

**Tech Stack:** .NET 10, .NET MAUI, ResX, xUnit.

---

### Task 1: Core Bow Rounding Geometry

**Files:**
- Modify: `src/DeckPlanking.Core/Configuration/DeckContourSettings.cs`
- Modify: `src/DeckPlanking.Core/Preview/DeckContourBuilder.cs`
- Test: `tests/DeckPlanking.Core.Tests/DeckContourBuilderTests.cs`

- [x] Add `BowRoundnessPercentage` to `DeckContourSettings`, defaulting to `0m`.
- [x] Validate bow roundness between `0m` and `100m`.
- [x] Keep the current straight contour when bow roundness is zero.
- [x] Generate sampled quadratic bow curve points when bow roundness is greater than zero.
- [x] Add tests for straight compatibility, rounded curve points, and invalid values.

### Task 2: Project Persistence And ViewModel

**Files:**
- Modify: `src/DeckPlanking.Core/Projects/DeckPlankingProjectSettings.cs`
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Test: `tests/DeckPlanking.Core.Tests/ProjectJsonSerializerTests.cs`

- [x] Add `BowRoundnessPercentage` to project settings.
- [x] Include the value in project capture/apply and last-used settings.
- [x] Default missing or zero values from older projects to `0`.
- [x] Add serialization assertions.

### Task 3: UI, Localization, And Drawable Wiring

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources*.resx`

- [x] Add the bow rounding input row below bow taper length.
- [x] Show it only when bow narrowing is enabled.
- [x] Add translations in Dutch, English, German, French, Spanish, and Italian.
- [x] Pass the bow rounding value into the drawable, export snapshot, contour path, and hit testing.

### Task 4: Verification And Docs

**Files:**
- Update copied Markdown under `C:\Data\Obsidian\markdown\Development\HNSoftwareDevelopment\Deckplanking`

- [x] Run `dotnet test .\tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --no-restore`.
- [x] Run `dotnet build .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0 --no-restore`.
- [x] Run `dotnet publish .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -r android-arm64 -c Debug --no-restore`.
- [x] Mirror Markdown to Obsidian.
