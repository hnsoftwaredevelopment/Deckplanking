# Project Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the initial Deckplanking solution with a testable calculation core and MAUI app foundation.

**Architecture:** Keep calculation logic in `DeckPlanking.Core` and keep the MAUI app as a thin consumer. Core tests define the calculation behavior before production code is added.

**Tech Stack:** .NET 10 SDK, .NET MAUI, xUnit, RESX resources, JSON-ready persistence model, Syncfusion MAUI startup hook.

---

### Task 1: Solution Skeleton

**Files:**
- Create: `DeckPlanking.slnx`
- Create: `src/DeckPlanking.Core/DeckPlanking.Core.csproj`
- Create: `tests/DeckPlanking.Core.Tests/DeckPlanking.Core.Tests.csproj`
- Create: `src/DeckPlanking.App/DeckPlanking.App.csproj`
- Modify: `README.md`
- Modify: `.gitignore`

- [x] Create the solution and projects with `dotnet new`.
- [x] Add project references from tests and app to the core project.
- [x] Add ignored local Syncfusion license file patterns.
- [x] Update README with app purpose and setup notes.
- [x] Build the solution to establish the baseline.

### Task 2: Core Pattern Tests

**Files:**
- Create: `tests/DeckPlanking.Core.Tests/ShiftPatternTests.cs`
- Create: `src/DeckPlanking.Core/Patterns/ShiftPatternKind.cs`
- Create: `src/DeckPlanking.Core/Patterns/ShiftPattern.cs`
- Create: `src/DeckPlanking.Core/Patterns/ShiftPatternCatalog.cs`

- [x] Write failing tests for Every 2/3/4/5 sequences and division counts.
- [x] Write failing test proving Custom is not exposed in v1.
- [x] Run the tests and verify they fail because the pattern API does not exist.
- [x] Implement the minimal pattern catalog.
- [x] Run the tests and verify they pass.

### Task 3: Scale And Rounding Tests

**Files:**
- Create: `tests/DeckPlanking.Core.Tests/ScaleLengthCalculatorTests.cs`
- Create: `src/DeckPlanking.Core/Measurement/LengthUnit.cs`
- Create: `src/DeckPlanking.Core/Measurement/ScaleMode.cs`
- Create: `src/DeckPlanking.Core/Measurement/ScaleSettings.cs`
- Create: `src/DeckPlanking.Core/Measurement/ScaleLengthCalculator.cs`
- Create: `src/DeckPlanking.Core/Measurement/DisplayRounding.cs`

- [x] Write failing tests for decimal scale conversion from 9 meters at 1:64.
- [x] Write failing tests for imperial scale conversion.
- [x] Write failing tests showing metric decimal rounding does not change pattern division.
- [x] Implement minimal scale and display rounding logic.
- [x] Run the tests and verify they pass.

### Task 4: Seam Generation Tests

**Files:**
- Create: `tests/DeckPlanking.Core.Tests/DeckPatternGeneratorTests.cs`
- Create: `src/DeckPlanking.Core/Generation/DeckPatternRequest.cs`
- Create: `src/DeckPlanking.Core/Generation/DeckPatternGenerator.cs`
- Create: `src/DeckPlanking.Core/Generation/PlankRow.cs`

- [x] Write failing tests for the Excel Every 3 example with 141 mm plank length and start point 0.
- [x] Write failing tests for start point offset wrapping and the zero-to-division rule.
- [x] Implement minimal seam generation.
- [x] Run the tests and verify they pass.

### Task 5: MAUI Foundation

**Files:**
- Modify: `src/DeckPlanking.App/DeckPlanking.App.csproj`
- Modify: `src/DeckPlanking.App/MauiProgram.cs`
- Create: `src/DeckPlanking.App/Infrastructure/SyncfusionLicenseRegistration.cs`
- Create: `src/DeckPlanking.App/Resources/Strings/*.resx`
- Create: `src/DeckPlanking.App/Resources/Themes/*.xaml`

- [x] Configure app metadata names.
- [x] Add Syncfusion package reference and local license loading.
- [x] Add RESX files and theme dictionaries.
- [x] Build the solution.

### Task 6: Verification And Commit

- [x] Run `dotnet test DeckPlanking.slnx`.
- [x] Run `dotnet build DeckPlanking.slnx`.
- [x] Review `git status`.
- [x] Commit the first slice with an English commit message explaining what changed and why JSON/local-license setup was chosen over SQLite/committed secrets.
