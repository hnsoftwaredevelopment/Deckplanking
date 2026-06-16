# Graphical Preview And Windows Icon Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a first graphical deck pattern preview and use the supplied `.ico` for Windows shell/taskbar icon behavior.

**Architecture:** Keep plank/seam calculations in `DeckPlanking.Core` and expose numeric seam positions alongside the existing display text. Render the preview in MAUI with a small `GraphicsView` drawable so the same view works on Windows and Android.

**Tech Stack:** .NET MAUI, MAUI Graphics, xUnit, Windows `.ico` application resource.

---

### Task 1: Preview Data

**Files:**
- Modify: `src/DeckPlanking.Core/Preview/PatternPreviewRow.cs`
- Modify: `src/DeckPlanking.Core/Preview/PatternPreviewBuilder.cs`
- Test: `tests/DeckPlanking.Core.Tests/PatternPreviewBuilderTests.cs`

- [x] Add a failing test asserting `PatternPreviewRow.SeamPositionsMillimeters` contains the numeric seam positions.
- [x] Run `dotnet test tests/DeckPlanking.Core.Tests/DeckPlanking.Core.Tests.csproj --filter PatternPreviewBuilderTests` and confirm the test fails because the property does not exist.
- [x] Add `IReadOnlyList<decimal> SeamPositionsMillimeters` to `PatternPreviewRow`.
- [x] Populate that property from `PlankRow.SeamPositionsMillimeters` while keeping `SeamPositionsText` unchanged.
- [x] Re-run the filtered test and confirm it passes.

### Task 2: MAUI Graphics Preview

**Files:**
- Create: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`

- [x] Create an `IDrawable` that draws horizontal plank rows and vertical seams from the numeric preview rows.
- [x] Add a `GraphicsView` above the text table in the pattern preview panel.
- [x] Wire the page code-behind so changes in `PatternRows` invalidate the `GraphicsView`.
- [x] Build the app to confirm XAML and platform code compile.

### Task 3: Windows Icon Resource

**Files:**
- Add: `src/DeckPlanking.App/Resources/AppIcon/appicon.ico`
- Modify: `src/DeckPlanking.App/DeckPlanking.App.csproj`

- [x] Copy `C:\Users\hnijk\OneDrive\Downloads\appicon.ico` into the app icon resource folder.
- [x] Set the Windows application icon in the project file with `ApplicationIcon`.
- [x] Build and launch the Windows exe to confirm the app still starts with a visible window.

### Task 4: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Launch `DeckPlanking.App.exe`, verify a non-zero window handle and `Responding=True`, then close it.
- [x] Commit the implementation in English.
