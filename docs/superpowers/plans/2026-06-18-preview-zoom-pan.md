# Preview Zoom Pan Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add basic zoom and pan controls to the graphical deck preview.

**Architecture:** Keep zoom and pan state in a small core `PreviewViewport` model with clamp behavior covered by tests. The MAUI page owns interaction events and passes viewport values to the drawable.

**Tech Stack:** .NET MAUI, MAUI Graphics, xUnit.

---

### Task 1: Viewport Model

**Files:**
- Create: `src/DeckPlanking.Core/Preview/PreviewViewport.cs`
- Create: `tests/DeckPlanking.Core.Tests/PreviewViewportTests.cs`

- [x] Add failing tests for zoom clamping, panning, and reset.
- [x] Implement the viewport record and operations.
- [x] Re-run focused tests and confirm they pass.

### Task 2: MAUI Controls And Gestures

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`

- [x] Add compact zoom buttons above the preview.
- [x] Add pan and pinch gesture handlers to the graphics view.
- [x] Apply viewport translate and scale in the drawable.

### Task 3: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Launch the Windows exe and verify a visible responding window.
- [x] Commit the implementation.
