# King Plank Toggle Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a king plank toggle that visually inserts a distinct center plank between the mirrored deck halves.

**Architecture:** Extend the existing mirrored preview model with an optional `KingPlank` row so center layout semantics stay in `DeckPlanking.Core`. The MAUI viewmodel exposes a boolean toggle, and the drawable renders the king plank with red dashed boundary lines above and below.

**Tech Stack:** .NET MAUI, MAUI Graphics, CommunityToolkit.Mvvm, xUnit.

---

### Task 1: Core King Plank Row

**Files:**
- Modify: `src/DeckPlanking.Core/Preview/PatternPreviewSide.cs`
- Modify: `src/DeckPlanking.Core/Preview/MirroredPatternPreviewRow.cs`
- Modify: `src/DeckPlanking.Core/Preview/MirroredPatternPreviewBuilder.cs`
- Modify: `tests/DeckPlanking.Core.Tests/MirroredPatternPreviewBuilderTests.cs`

- [x] Add a failing test proving `Build(sourceRows, includeKingPlank: true)` inserts exactly one `KingPlank` row between upper and lower rows.
- [x] Run the filtered test and confirm it fails because the optional king plank behavior does not exist.
- [x] Add `PatternPreviewSide.KingPlank` and an `IsKingPlank` marker.
- [x] Insert the king plank row between upper and lower rows when requested.
- [x] Re-run the filtered test and confirm it passes.

### Task 2: UI Toggle And Drawing

**Files:**
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`

- [x] Add an `UseKingPlank` boolean to the viewmodel.
- [x] Add a compact checkbox in the inputs.
- [x] Pass the toggle into the drawable.
- [x] Draw the king plank as a distinct center plank with red dashed lines above and below it.

### Task 3: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Launch `DeckPlanking.App.exe`, verify a non-zero window handle and `Responding=True`, then close it.
- [x] Commit the implementation in English.
