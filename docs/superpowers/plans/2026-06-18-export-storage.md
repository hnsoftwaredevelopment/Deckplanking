# Export Storage Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Save PNG exports to a user-visible location: Save As on Windows and Downloads on Android.

**Architecture:** Keep preview rendering unchanged: it still creates a temporary PNG file. Add a generic export storage service with a shared contract and platform-specific partial implementations. The UI calls the saver after rendering and shows a confirmation message when a file is saved.

**Tech Stack:** .NET MAUI, Windows FileSavePicker, Android MediaStore Downloads, xUnit for existing core coverage.

---

### Task 1: Shared Export Result Contract

**Files:**
- Create: `src/DeckPlanking.App/Export/ExportSaveResult.cs`
- Modify: `src/DeckPlanking.App/Export/PreviewPngExporter.cs`

- [ ] **Step 1: Create save result record**

Create a record with `Saved`, `FileName`, and `DisplayLocation`.

- [ ] **Step 2: Keep renderer return focused**

Keep `PreviewPngExporter.ExportAsync` returning the temporary file. The saver consumes that file and decides the final platform location.

### Task 2: Platform Export Saver

**Files:**
- Create: `src/DeckPlanking.App/Export/ExportFileSaver.cs`
- Create: `src/DeckPlanking.App/Platforms/Windows/ExportFileSaver.Windows.cs`
- Create: `src/DeckPlanking.App/Platforms/Android/ExportFileSaver.Android.cs`

- [ ] **Step 1: Add shared partial saver**

Expose `ExportFileSaver.SaveAsync(FileResult sourceFile, CancellationToken cancellationToken)`.

- [ ] **Step 2: Add Windows implementation**

Use `FileSavePicker`, initialize it with the current MAUI window handle, prefill the PNG file name, and copy the temporary file to the chosen target. Return `Saved = false` when the picker is cancelled.

- [ ] **Step 3: Add Android implementation**

Use `ContentResolver` and `MediaStore.Downloads` to write the temporary PNG to public Downloads with the same file name. Return `DisplayLocation = "Downloads"`.

### Task 3: UI Integration

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`

- [ ] **Step 1: Replace share request**

After rendering the PNG, call `ExportFileSaver.SaveAsync`.

- [ ] **Step 2: Show confirmation**

If saved, show `Saved deckplanking-YYYYMMDD-HHMM.png to Downloads` on Android and a path/location confirmation on Windows. If cancelled, do not show an error.

### Task 4: Verification

**Files:**
- No new files.

- [ ] **Step 1: Run core tests**

`dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj`

- [ ] **Step 2: Build Windows**

`dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0`

- [ ] **Step 3: Publish Android ARM64**

`dotnet publish src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -c Debug -p:RuntimeIdentifier=android-arm64 -p:AndroidPackageFormat=apk`
