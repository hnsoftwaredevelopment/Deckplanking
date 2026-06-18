# PNG Preview Export Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add PNG export for the current deck preview using filename `deckplanking-YYYYMMDD-HHMM.png`.

**Architecture:** Keep filename generation testable in core. Add an app export service that renders `DeckPatternPreviewDrawable` into an off-screen PNG and writes it to a platform-accessible cache file. Wire a preview toolbar button to export and share/open the generated file.

**Tech Stack:** .NET MAUI, Microsoft.Maui.Graphics, CommunityToolkit.Mvvm, xUnit.

---

### Task 1: Filename Generator

**Files:**
- Create: `src/DeckPlanking.Core/Export/ExportFileNameBuilder.cs`
- Test: `tests/DeckPlanking.Core.Tests/ExportFileNameBuilderTests.cs`

- [ ] **Step 1: Write the failing filename test**

Create `tests/DeckPlanking.Core.Tests/ExportFileNameBuilderTests.cs`:

```csharp
using DeckPlanking.Core.Export;

namespace DeckPlanking.Core.Tests;

public sealed class ExportFileNameBuilderTests
{
    [Fact]
    public void BuildsTimestampedDeckplankingPngFileName()
    {
        var timestamp = new DateTimeOffset(2026, 6, 18, 14, 53, 0, TimeSpan.Zero);

        var fileName = ExportFileNameBuilder.BuildPngFileName(timestamp);

        Assert.Equal("deckplanking-20260618-1453.png", fileName);
    }
}
```

- [ ] **Step 2: Run the focused test and verify it fails**

Run:

`dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --filter ExportFileNameBuilderTests`

Expected: compile failure because `ExportFileNameBuilder` does not exist.

- [ ] **Step 3: Implement the filename builder**

Create `src/DeckPlanking.Core/Export/ExportFileNameBuilder.cs`:

```csharp
namespace DeckPlanking.Core.Export;

public static class ExportFileNameBuilder
{
    public static string BuildPngFileName(DateTimeOffset timestamp)
    {
        return $"deckplanking-{timestamp:yyyyMMdd-HHmm}.png";
    }
}
```

- [ ] **Step 4: Run the focused test and verify it passes**

Run:

`dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --filter ExportFileNameBuilderTests`

Expected: 1 passed.

### Task 2: PNG Export Service

**Files:**
- Create: `src/DeckPlanking.App/Export/PreviewPngExporter.cs`
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`

- [ ] **Step 1: Add a drawable snapshot method**

Add a `CreateSnapshot()` method to `DeckPatternPreviewDrawable` that copies the current preview state into a new drawable and resets `Zoom`, `PanX`, and `PanY` to default export values.

- [ ] **Step 2: Add the PNG exporter**

Create `PreviewPngExporter` with `ExportAsync(DeckPatternPreviewDrawable drawable, CancellationToken cancellationToken)` that renders the snapshot to a PNG at a stable size and writes it to `FileSystem.CacheDirectory` using `ExportFileNameBuilder.BuildPngFileName(DateTimeOffset.Now)`.

- [ ] **Step 3: Return a FileResult**

Return a `FileResult` for the created PNG so MAUI `Share.Default.RequestAsync` can share/save it on Windows and Android.

### Task 3: UI Wiring

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`

- [ ] **Step 1: Add Export PNG button**

Add an icon/text button in the preview toolbar with text `PNG` for this first version.

- [ ] **Step 2: Wire click handler**

In `MainPage.xaml.cs`, call `PreviewPngExporter.ExportAsync(patternPreviewDrawable, CancellationToken.None)` and then `Share.Default.RequestAsync`.

- [ ] **Step 3: Guard failures**

Catch exceptions and show a display alert with a short export failure message.

### Task 4: Verification

**Files:**
- No new files.

- [ ] **Step 1: Run core tests**

Run:

`dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj`

Expected: all tests pass.

- [ ] **Step 2: Build Windows**

Run:

`dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0`

Expected: build succeeds with 0 errors.

- [ ] **Step 3: Publish Android ARM64 APK**

Run:

`dotnet publish src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -c Debug -p:RuntimeIdentifier=android-arm64 -p:AndroidPackageFormat=apk`

Expected: publish succeeds and writes the signed APK.
