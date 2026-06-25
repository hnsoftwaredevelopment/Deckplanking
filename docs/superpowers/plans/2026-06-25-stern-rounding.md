# Stern Rounding Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a persisted numeric stern-rounding setting for bow-and-stern narrowed deck contours.

**Architecture:** Extend the existing contour settings and polygon builder with a stern roundness percentage. Reuse the sampled quadratic curve model so the app renderer keeps one shared contour for preview, hit testing, export, PDF, and print.

**Tech Stack:** .NET 10, .NET MAUI, ResX, xUnit.

---

### Task 1: Core Stern Rounding Geometry

**Files:**
- Modify: `src/DeckPlanking.Core/Configuration/DeckContourSettings.cs`
- Modify: `src/DeckPlanking.Core/Preview/DeckContourBuilder.cs`
- Test: `tests/DeckPlanking.Core.Tests/DeckContourBuilderTests.cs`

- [x] Add `SternRoundnessPercentage` to `DeckContourSettings`, defaulting to `0m`.
- [x] Validate stern roundness between `0m` and `100m`.
- [x] Keep current straight stern contour when stern roundness is zero.
- [x] Generate sampled mirrored stern curve points when stern roundness is greater than zero and shape is `NarrowedBowAndStern`.
- [x] Add tests for straight compatibility, mirrored stern curve points, and invalid values.

### Task 2: Project Persistence And ViewModel

**Files:**
- Modify: `src/DeckPlanking.Core/Projects/DeckPlankingProjectSettings.cs`
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Test: `tests/DeckPlanking.Core.Tests/ProjectJsonSerializerTests.cs`

- [x] Add `SternRoundnessPercentage` to project settings.
- [x] Include it in project capture/apply and last-used settings.
- [x] Default older projects to `0`.
- [x] Add serialization assertions.

### Task 3: UI, Localization, And Drawable Wiring

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources*.resx`

- [x] Add the stern rounding input row below stern taper length.
- [x] Show it only when stern narrowing is enabled.
- [x] Add translations in Dutch, English, German, French, Spanish, and Italian.
- [x] Pass the stern rounding value into the drawable, export snapshot, contour path, and hit testing.

### Task 4: Verification And Docs

**Files:**
- Update copied Markdown under `C:\Data\Obsidian\markdown\Development\HNSoftwareDevelopment\Deckplanking`

- [x] Run `dotnet test .\tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --no-restore`.
- [x] Run `dotnet build .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0 --no-restore`.
- [x] Run `dotnet publish .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -r android-arm64 -c Debug --no-restore`.
- [x] Mirror Markdown to Obsidian.
