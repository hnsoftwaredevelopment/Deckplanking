# Preview Ruler And Collapsible Seams Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a segment ruler above the graphical preview and make the seam details table collapsible.

**Architecture:** Calculate ruler tick positions in `DeckPlanking.Core` so the drawing layer receives numeric positions and labels without parsing display text. Keep the MAUI page as a thin consumer that passes segment length and visibility state into the existing `GraphicsView` and table.

**Tech Stack:** .NET MAUI, MAUI Graphics, CommunityToolkit.Mvvm, xUnit.

---

### Task 1: Ruler Tick Data

**Files:**
- Create: `src/DeckPlanking.Core/Preview/RulerTick.cs`
- Create: `src/DeckPlanking.Core/Preview/RulerTickBuilder.cs`
- Create: `tests/DeckPlanking.Core.Tests/RulerTickBuilderTests.cs`

- [x] Write a failing test that `RulerTickBuilder.Build(28m, 100m)` returns ticks at 28, 56, and 84 with labels `28`, `56`, `84`.
- [x] Run the filtered test and confirm it fails because the type does not exist.
- [x] Implement `RulerTick` and `RulerTickBuilder`.
- [x] Re-run the filtered test and confirm it passes.

### Task 2: Draw Ruler

**Files:**
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`

- [x] Expose numeric `SegmentLengthMillimeters` from the viewmodel.
- [x] Pass segment length into the drawable.
- [x] Draw top ruler tick marks and compact labels without `mm` suffix.
- [x] Leave the actual plank seam lines drawn inside each row.

### Task 3: Collapsible Seam Table

**Files:**
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml`

- [x] Add `IsSeamTableVisible` state and a toggle command.
- [x] Add a compact button in the preview panel to show or hide seam details.
- [x] Bind the table header and collection view visibility to the state.

### Task 4: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Launch `DeckPlanking.App.exe`, verify a non-zero window handle and `Responding=True`, then close it.
- [x] Commit the implementation in English.
