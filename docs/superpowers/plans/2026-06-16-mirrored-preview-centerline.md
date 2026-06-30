# Mirrored Preview Centerline Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Show both sides of the deck in the graphical preview by mirroring the calculated plank rows around a red dashed centerline.

**Architecture:** Keep mirror ordering in `DeckPlanking.Core` so the UI does not invent deck semantics. The MAUI drawable consumes the mirrored row model and draws upper rows, centerline, and lower rows without introducing king plank behavior yet.

**Tech Stack:** .NET MAUI, MAUI Graphics, xUnit.

---

### Task 1: Mirrored Row Data

**Files:**
- Create: `src/DeckPlanking.Core/Preview/PatternPreviewSide.cs`
- Create: `src/DeckPlanking.Core/Preview/MirroredPatternPreviewRow.cs`
- Create: `src/DeckPlanking.Core/Preview/MirroredPatternPreviewBuilder.cs`
- Create: `tests/DeckPlanking.Core.Tests/MirroredPatternPreviewBuilderTests.cs`

- [x] Write a failing test proving source rows `1,2,3` become upper rows `3,2,1` and lower rows `1,2,3`.
- [x] Run the filtered test and confirm it fails because the builder does not exist.
- [x] Implement the enum, record, and builder.
- [x] Re-run the filtered test and confirm it passes.

### Task 2: Draw Mirrored Preview

**Files:**
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml`

- [x] Draw the upper half using reversed source rows.
- [x] Draw a red dashed centerline between the halves.
- [x] Draw the lower half using the original row order.
- [x] Increase the preview height enough for the doubled row count.

### Task 3: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Launch `DeckPlanking.App.exe`, verify a non-zero window handle and `Responding=True`, then close it.
- [x] Commit the implementation in English.
