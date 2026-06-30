# Continuous Centerline Pattern Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Correct the deck preview so pattern rows continue over the centerline/king plank instead of mirroring the same row order.

**Architecture:** Add a centerline preview builder in `DeckPlanking.Core` that creates upper rows, optional king plank, and lower rows from one continuous pattern sequence. Keep the drawable as a renderer and replace the MAUI checkbox with a `Switch`.

**Tech Stack:** .NET MAUI, MAUI Graphics, xUnit.

---

### Task 1: Continuous Centerline Preview Data

**Files:**
- Modify: `src/DeckPlanking.Core/Preview/PatternPreviewRow.cs`
- Modify: `src/DeckPlanking.Core/Preview/PatternPreviewBuilder.cs`
- Create: `src/DeckPlanking.Core/Preview/CenterlinePatternPreviewBuilder.cs`
- Create: `src/DeckPlanking.Core/Preview/CenterlinePatternPreviewRow.cs`
- Create: `tests/DeckPlanking.Core.Tests/CenterlinePatternPreviewBuilderTests.cs`

- [x] Add failing tests for Every 5 without king plank: upper phases `1,3,5,2,4,1,3,5`, lower phases `2,4,1,3,5,2,4,1`.
- [x] Add failing tests for Every 5 with king plank: king plank phase `2`, lower phases start at `4`.
- [x] Expose `Phase` on `PatternPreviewRow`.
- [x] Implement the centerline builder from one continuous generated sequence.
- [x] Re-run focused tests and confirm they pass.

### Task 2: Render Continuous Rows

**Files:**
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`

- [x] Pass pattern kind, row count, start point, and king plank state into the drawable.
- [x] Let the drawable render centerline rows from `CenterlinePatternPreviewBuilder`.
- [x] Keep visual seams and ruler behavior unchanged.

### Task 3: Modern Toggle UI

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`

- [x] Replace the `CheckBox` with a `Switch` bound to `UseKingPlank`.

### Task 4: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Launch `DeckPlanking.App.exe`, verify a non-zero window handle and `Responding=True`, then close it.
- [x] Commit the implementation in English.
