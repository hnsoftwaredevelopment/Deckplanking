# Preview Orientation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make the bow/stern orientation in the graphical preview selectable without changing deck calculations.

**Architecture:** Keep the orientation label decision in `DeckPlanking.Core` and pass the selected orientation from the MAUI viewmodel to the renderer. The drawable remains responsible only for drawing the chosen left and right labels.

**Tech Stack:** .NET MAUI, MAUI Graphics, xUnit.

---

### Task 1: Orientation Label Model

**Files:**
- Create: `src/DeckPlanking.Core/Preview/DeckOrientation.cs`
- Create: `src/DeckPlanking.Core/Preview/DirectionGuide.cs`
- Create: `src/DeckPlanking.Core/Preview/DirectionGuideBuilder.cs`
- Create: `tests/DeckPlanking.Core.Tests/DirectionGuideBuilderTests.cs`

- [x] Add failing tests for `BowLeft` and `SternLeft`.
- [x] Implement the enum, record, and builder.
- [x] Re-run focused tests and confirm they pass.

### Task 2: UI And Renderer Binding

**Files:**
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`

- [x] Add an orientation picker to the input panel.
- [x] Pass the selected orientation into the drawable.
- [x] Draw the direction guide labels from the core builder.

### Task 3: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Launch the Windows exe and verify a visible responding window.
- [x] Commit the implementation.
