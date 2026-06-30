# Project Persistence Phase 1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add local JSON project save/open for the current deck-planking configuration.

**Architecture:** Keep project document serialization in `DeckPlanking.Core` so the JSON format is testable without MAUI. Add small MAUI platform services for Save As/Open behavior, and expose capture/apply methods on `ScaleInputViewModel` so file IO stays out of calculation logic.

**Tech Stack:** .NET 10, .NET MAUI, System.Text.Json, xUnit, Windows FileSavePicker/FileOpenPicker, MAUI FilePicker, Android MediaStore Downloads.

---

## File Structure

- Create `src/DeckPlanking.Core/Projects/DeckPlankingProjectSettings.cs`: serializable DTO for current app settings.
- Create `src/DeckPlanking.Core/Projects/DeckPlankingProjectDocument.cs`: serializable root JSON document with schema version and timestamp.
- Create `src/DeckPlanking.Core/Projects/ProjectJsonSerializer.cs`: stable JSON round-trip and schema validation.
- Create `tests/DeckPlanking.Core.Tests/ProjectJsonSerializerTests.cs`: TDD coverage for round-trip, enum strings, and unsupported schema rejection.
- Modify `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`: capture current settings and apply loaded settings.
- Create `src/DeckPlanking.App/Projects/ProjectFileService.cs`: shared partial API used by `MainPage`.
- Create `src/DeckPlanking.App/Projects/ProjectFileResult.cs`: result DTO for save/open operations.
- Create `src/DeckPlanking.App/Platforms/Windows/ProjectFileService.Windows.cs`: Windows Save As/Open implementation.
- Create `src/DeckPlanking.App/Platforms/Android/ProjectFileService.Android.cs`: Android Downloads save and file picker open implementation.
- Modify `src/DeckPlanking.App/MainPage.xaml`: add Save project and Open project controls.
- Modify `src/DeckPlanking.App/MainPage.xaml.cs`: wire save/open handlers and reset viewport after opening.

## Task 1: Project JSON Format

**Files:**
- Create: `src/DeckPlanking.Core/Projects/DeckPlankingProjectSettings.cs`
- Create: `src/DeckPlanking.Core/Projects/DeckPlankingProjectDocument.cs`
- Create: `src/DeckPlanking.Core/Projects/ProjectJsonSerializer.cs`
- Create: `tests/DeckPlanking.Core.Tests/ProjectJsonSerializerTests.cs`

- [ ] **Step 1: Write failing round-trip test**

Add a test named `RoundTripsRepresentativeProject` that creates a project with meters, decimal scale, every-5 pattern, king plank enabled, stern-left orientation, and alternating trenails. Serialize and deserialize it, then assert all values match.

Run:

```powershell
dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --filter RoundTripsRepresentativeProject
```

Expected: compile failure because `DeckPlanking.Core.Projects` does not exist.

- [ ] **Step 2: Implement minimal project DTOs and serializer**

Add immutable records:

```csharp
public sealed record DeckPlankingProjectSettings(
    double RealPlankLength,
    LengthUnit LengthUnit,
    ScaleMode ScaleMode,
    double DecimalScale,
    double ImperialInchesPerFoot,
    double DeckLengthMillimeters,
    int RowCount,
    int StartPoint,
    ShiftPatternKind ShiftPattern,
    bool UseKingPlank,
    DeckOrientation DeckOrientation,
    TrenailPatternKind TrenailPattern);

public sealed record DeckPlankingProjectDocument(
    int SchemaVersion,
    DateTimeOffset SavedAt,
    DeckPlankingProjectSettings Settings);
```

Add `ProjectJsonSerializer.Serialize` and `ProjectJsonSerializer.Deserialize` using `JsonStringEnumConverter`, indented JSON, and camel-case property names.

- [ ] **Step 3: Verify round-trip test passes**

Run the focused test again.

Expected: pass.

- [ ] **Step 4: Write failing enum string test**

Add `SerializesEnumsAsStrings`, asserting the JSON contains `"lengthUnit": "Feet"` and `"shiftPattern": "Every3"`.

Run the focused test.

Expected: fail if enums are numeric or missing.

- [ ] **Step 5: Make enum string serialization pass**

Ensure the serializer options include `new JsonStringEnumConverter()`.

- [ ] **Step 6: Write failing unsupported schema test**

Add `RejectsUnsupportedFutureSchemaVersion`, passing JSON with `"schemaVersion": 2`, valid `savedAt`, and valid `settings`.

Expected: `InvalidDataException`.

- [ ] **Step 7: Implement schema validation**

In `Deserialize`, throw `InvalidDataException("Project schema version 2 is not supported.")` when schema version is not `1`.

- [ ] **Step 8: Verify all core tests**

Run:

```powershell
dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj
```

Expected: all tests pass.

## Task 2: ViewModel Capture And Apply

**Files:**
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Test indirectly through app build; core serialization already covers the format.

- [ ] **Step 1: Add capture method**

Add `CaptureProjectSettings()` returning `DeckPlankingProjectSettings` from the current view model fields.

- [ ] **Step 2: Add apply method**

Add `ApplyProjectSettings(DeckPlankingProjectSettings settings)` that sets all editable settings by selecting matching `OptionItem<T>` values and assigning numeric/bool properties. It should reuse existing property setters so recalculation and notifications remain consistent.

- [ ] **Step 3: Build app**

Run:

```powershell
$out = Join-Path $env:TEMP 'deckplanking-verify-windows'
dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0 -o $out
```

Expected: build succeeds.

## Task 3: Project File Service

**Files:**
- Create: `src/DeckPlanking.App/Projects/ProjectFileResult.cs`
- Create: `src/DeckPlanking.App/Projects/ProjectFileService.cs`
- Create: `src/DeckPlanking.App/Platforms/Windows/ProjectFileService.Windows.cs`
- Create: `src/DeckPlanking.App/Platforms/Android/ProjectFileService.Android.cs`

- [ ] **Step 1: Add shared partial service API**

Create a partial `ProjectFileService` with:

```csharp
public static partial Task<ProjectFileResult> SaveAsync(DeckPlankingProjectDocument document, CancellationToken cancellationToken = default);
public static partial Task<DeckPlankingProjectDocument?> OpenAsync(CancellationToken cancellationToken = default);
```

- [ ] **Step 2: Implement Windows Save/Open**

Use `FileSavePicker` with `.deckplanking.json`, suggested name `deckplanking-project`, and `FileOpenPicker` filtered to `.deckplanking.json` and `.json`. Serialize with `ProjectJsonSerializer`.

- [ ] **Step 3: Implement Android Save/Open**

Save to Downloads using `MediaStore.Downloads` for Android 29+ and fallback to public downloads for lower API levels. Open with `FilePicker.PickAsync` using JSON/project file types and deserialize the selected file stream.

- [ ] **Step 4: Build Windows**

Run the Windows temp build.

Expected: build succeeds.

## Task 4: UI Wiring

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`

- [ ] **Step 1: Add buttons**

Add `Save project` and `Open project` buttons in the top input/action area, before export actions or in a compact project action row.

- [ ] **Step 2: Save handler**

Capture settings, create `DeckPlankingProjectDocument` with schema version `1` and `DateTimeOffset.UtcNow`, call `ProjectFileService.SaveAsync`, and show a confirmation if saved.

- [ ] **Step 3: Open handler**

Call `ProjectFileService.OpenAsync`, apply the loaded settings when not null, reset `previewViewport` to `PreviewViewport.Default`, update the preview, and show an "Project opened" confirmation.

- [ ] **Step 4: Error handling**

Wrap save/open handlers in try/catch and show friendly messages:

- Save: `Project could not be saved`
- Open: `Project could not be opened`

## Task 5: Verification And Commit

**Files:**
- No new production files beyond the previous tasks.

- [ ] **Step 1: Run core tests**

```powershell
dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj
```

Expected: all tests pass.

- [ ] **Step 2: Build Windows**

```powershell
$out = Join-Path $env:TEMP 'deckplanking-verify-windows'
dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0 -o $out
```

Expected: build succeeds.

- [ ] **Step 3: Publish Android ARM64 APK**

```powershell
dotnet publish src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -c Debug -p:RuntimeIdentifier=android-arm64 -p:AndroidPackageFormat=apk
```

Expected: publish succeeds and updates `src\DeckPlanking.App\bin\Debug\net10.0-android\android-arm64\publish\nl.hnsoftwaredevelopment.deckplanking-Signed.apk`.

- [ ] **Step 4: Commit**

```powershell
git add docs\superpowers\plans\2026-06-19-project-persistence-phase-1.md src tests
git commit -m "Add JSON project persistence" -m "Save and open deck-planking project settings through local JSON files with platform-specific file handling."
```

## Self-Review

- Spec coverage: the plan covers JSON file format, save/open controls, platform behavior, schema validation, and excluding zoom/pan.
- Placeholder scan: no TODO/TBD placeholders remain.
- Type consistency: project settings, document, serializer, service, and view model method names are consistent across tasks.

