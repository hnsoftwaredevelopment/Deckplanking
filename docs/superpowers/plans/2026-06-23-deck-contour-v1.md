# Deck Contour V1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a simple symmetrical deck contour to the preview, export, print, project files, and last-used settings.

**Architecture:** Put deck contour settings and polygon calculation in `DeckPlanking.Core`. Bind the settings through `ScaleInputViewModel`, render clipping in `DeckPatternPreviewDrawable`, and store the new fields in project JSON.

**Tech Stack:** .NET 10, .NET MAUI Graphics, CommunityToolkit.Mvvm, ResX, xUnit.

---

### Task 1: Core Contour Model

**Files:**
- Create: `src/DeckPlanking.Core/Configuration/DeckShapeKind.cs`
- Create: `src/DeckPlanking.Core/Configuration/DeckContourSettings.cs`
- Create: `src/DeckPlanking.Core/Preview/DeckContourPoint.cs`
- Create: `src/DeckPlanking.Core/Preview/DeckContourBuilder.cs`
- Test: `tests/DeckPlanking.Core.Tests/DeckContourBuilderTests.cs`

- [x] Add shape enum, settings record, polygon point record, and a tested polygon builder.
- [x] Validate percentages between 10 and 100.

### Task 2: Project And ViewModel State

**Files:**
- Modify: `src/DeckPlanking.Core/Projects/DeckPlankingProjectSettings.cs`
- Modify: `tests/DeckPlanking.Core.Tests/ProjectJsonSerializerTests.cs`
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`

- [x] Store deck shape, bow width percentage, and stern width percentage in project settings.
- [x] Add localized shape options and percentage inputs to the viewmodel.
- [x] Include the new values in capture/apply/last-used settings.

### Task 3: UI And Localization

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources*.resx`

- [x] Add deck shape picker and percentage inputs.
- [x] Hide stern percentage unless the shape narrows bow and stern.
- [x] Add localized labels/options in all supported languages.

### Task 4: Preview Clipping

**Files:**
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`

- [x] Pass contour settings into the drawable.
- [x] Clip plank rows, seams, selected segment, and trenails to the contour.
- [x] Draw the contour outline.
- [x] Use the same drawable path for screen, PNG, PDF, and print.

### Task 5: Verify And Mirror

**Files:**
- Update copied Markdown under `C:\Data\Obsidian\markdown\Development\HNSoftwareDevelopment\Deckplanking`

- [x] Run `dotnet test .\tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --no-restore`.
- [x] Run `dotnet build .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0 --no-restore`.
- [x] Run `dotnet publish .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -r android-arm64 -c Debug --no-restore`.
- [x] Mirror Markdown to Obsidian.

### Refinement Note

After user review, the contour was refined from a full-length trapezoid into a full-width middle section with local bow and stern taper zones. This better matches real deck plans and prepares the model for a later Bezier bow shape.
