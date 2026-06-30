# PDF Preview Export Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add PDF export for the current deck preview using filename `deckplanking-YYYYMMDD-HHMM.pdf`.

**Architecture:** Reuse the existing preview image renderer, then place that image on a single landscape PDF page using Syncfusion PDF. Save the resulting PDF through the shared export saver that already supports Windows Save As and Android Downloads.

**Tech Stack:** .NET MAUI, Microsoft.Maui.Graphics.Skia, Syncfusion.Pdf.Net.Core, xUnit.

---

### Task 1: PDF Filename

**Files:**
- Modify: `src/DeckPlanking.Core/Export/ExportFileNameBuilder.cs`
- Modify: `tests/DeckPlanking.Core.Tests/ExportFileNameBuilderTests.cs`

- [ ] **Step 1: Add failing PDF filename test**

Add a test that expects `deckplanking-20260618-1453.pdf`.

- [ ] **Step 2: Run focused test and verify it fails**

`dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --filter BuildsTimestampedDeckplankingPdfFileName`

- [ ] **Step 3: Implement `BuildPdfFileName`**

Add a method parallel to `BuildPngFileName`.

- [ ] **Step 4: Verify focused test passes**

Run the focused test again.

### Task 2: Shared Preview Image Renderer

**Files:**
- Create: `src/DeckPlanking.App/Export/PreviewImageRenderer.cs`
- Modify: `src/DeckPlanking.App/Export/PreviewPngExporter.cs`

- [ ] **Step 1: Extract PNG rendering**

Move Skia rendering into `PreviewImageRenderer.RenderPngAsync`.

- [ ] **Step 2: Keep PNG export behavior**

Update `PreviewPngExporter` to call the renderer and write a temporary PNG.

### Task 3: PDF Exporter

**Files:**
- Modify: `src/DeckPlanking.App/DeckPlanking.App.csproj`
- Create: `src/DeckPlanking.App/Export/PreviewPdfExporter.cs`

- [ ] **Step 1: Add Syncfusion PDF package**

Add `Syncfusion.Pdf.Net.Core`.

- [ ] **Step 2: Create PDF exporter**

Render the preview PNG, create a landscape PDF page, fit the PNG inside the page margins, and write a temporary PDF to cache.

### Task 4: Storage Extension Support

**Files:**
- Modify: `src/DeckPlanking.App/Platforms/Windows/ExportFileSaver.Windows.cs`
- Modify: `src/DeckPlanking.App/Platforms/Android/ExportFileSaver.Android.cs`

- [ ] **Step 1: Windows supports PDF**

Set FileSavePicker file type choices based on the source extension and content type.

- [ ] **Step 2: Android supports PDF**

Write PDF to Downloads with `application/pdf`.

### Task 5: UI Wiring

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`

- [ ] **Step 1: Add PDF button next to PNG**

Add a `PDF` button in the preview controls.

- [ ] **Step 2: Handle PDF export**

Call `PreviewPdfExporter.ExportAsync`, then `ExportFileSaver.SaveAsync`, and show the same saved confirmation.

### Task 6: Verification

**Files:**
- No new files.

- [ ] **Step 1: Run core tests**

`dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj`

- [ ] **Step 2: Build Windows**

`dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0`

- [ ] **Step 3: Publish Android ARM64**

`dotnet publish src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -c Debug -p:RuntimeIdentifier=android-arm64 -p:AndroidPackageFormat=apk`
