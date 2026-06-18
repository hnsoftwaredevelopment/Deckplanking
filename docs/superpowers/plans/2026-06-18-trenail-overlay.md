# Trenail Overlay Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a phase-one trenail overlay that can be toggled on/off and draws two trenails at each internal plank end.

**Architecture:** The core project will calculate trenail points from existing `CenterlinePatternPreviewRow` data so the UI only renders prepared positions. The MAUI drawable will draw the overlay after plank rows and seam lines, controlled by one ViewModel toggle.

**Tech Stack:** .NET 10, C#, .NET MAUI Graphics, CommunityToolkit.Mvvm, xUnit.

---

## File Structure

- Create `src/DeckPlanking.Core/Preview/TrenailVerticalPlacement.cs`: enum for row-relative marker placement.
- Create `src/DeckPlanking.Core/Preview/TrenailPoint.cs`: immutable point model in deck millimeters and row index.
- Create `src/DeckPlanking.Core/Preview/TrenailOverlayBuilder.cs`: core calculation for phase-one two-pin trenails.
- Create `tests/DeckPlanking.Core.Tests/TrenailOverlayBuilderTests.cs`: unit tests for counts and positions.
- Modify `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`: add `ShowTrenails` toggle property.
- Modify `src/DeckPlanking.App/MainPage.xaml`: add compact switch near preview controls.
- Modify `src/DeckPlanking.App/MainPage.xaml.cs`: pass `ShowTrenails` to the drawable and redraw on toggle change.
- Modify `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`: calculate row rectangles and draw trenail markers.
- Modify `src/DeckPlanking.App/Resources/Strings/AppResources*.resx`: add a localized `ShowTrenails` label.

---

### Task 1: Core Trenail Model And Tests

**Files:**
- Create: `src/DeckPlanking.Core/Preview/TrenailVerticalPlacement.cs`
- Create: `src/DeckPlanking.Core/Preview/TrenailPoint.cs`
- Create: `src/DeckPlanking.Core/Preview/TrenailOverlayBuilder.cs`
- Test: `tests/DeckPlanking.Core.Tests/TrenailOverlayBuilderTests.cs`

- [ ] **Step 1: Write failing tests**

Create `tests/DeckPlanking.Core.Tests/TrenailOverlayBuilderTests.cs`:

```csharp
using DeckPlanking.Core.Patterns;
using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class TrenailOverlayBuilderTests
{
    [Fact]
    public void BuildsTwoTrenailsForEachInternalSeam()
    {
        var rows = CenterlinePatternPreviewBuilder.Build(
            plankLengthMillimeters: 140m,
            deckLengthMillimeters: 300m,
            patternKind: ShiftPatternKind.Every5,
            rowsPerSide: 1,
            startPoint: 0,
            includeKingPlank: false);

        var points = TrenailOverlayBuilder.Build(rows);
        var firstRowSeams = rows[0].SourceRow.SeamPositionsMillimeters
            .Where(position => position > 0m && position < 300m)
            .ToArray();

        Assert.Equal(firstRowSeams.Length * 2 * rows.Count, points.Count);
        Assert.Contains(points, point => point.RowIndex == 0 && point.PositionMillimeters == firstRowSeams[0] && point.VerticalPlacement == TrenailVerticalPlacement.Upper);
        Assert.Contains(points, point => point.RowIndex == 0 && point.PositionMillimeters == firstRowSeams[0] && point.VerticalPlacement == TrenailVerticalPlacement.Lower);
    }

    [Fact]
    public void ReturnsNoPointsWhenRowsAreEmpty()
    {
        var points = TrenailOverlayBuilder.Build([]);

        Assert.Empty(points);
    }

    [Fact]
    public void SkipsDeckBoundarySeams()
    {
        var row = new CenterlinePatternPreviewRow(
            PatternPreviewSide.Upper,
            new PatternPreviewRow(
                RowNumber: 1,
                Phase: 1,
                SeamOffsetSegments: 1,
                SeamPositionsMillimeters: [0m, 50m, 100m],
                SeamPositionsText: "0, 50, 100"));

        var points = TrenailOverlayBuilder.Build([row], deckLengthMillimeters: 100m);

        Assert.Equal(2, points.Count);
        Assert.All(points, point => Assert.Equal(50m, point.PositionMillimeters));
    }
}
```

- [ ] **Step 2: Run tests to verify failure**

Run:

```powershell
dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --filter TrenailOverlayBuilderTests
```

Expected: compile failure because `TrenailOverlayBuilder`, `TrenailPoint`, and `TrenailVerticalPlacement` do not exist.

- [ ] **Step 3: Add core model and builder**

Create `src/DeckPlanking.Core/Preview/TrenailVerticalPlacement.cs`:

```csharp
namespace DeckPlanking.Core.Preview;

public enum TrenailVerticalPlacement
{
    Upper,
    Lower
}
```

Create `src/DeckPlanking.Core/Preview/TrenailPoint.cs`:

```csharp
namespace DeckPlanking.Core.Preview;

public sealed record TrenailPoint(
    int RowIndex,
    decimal PositionMillimeters,
    TrenailVerticalPlacement VerticalPlacement);
```

Create `src/DeckPlanking.Core/Preview/TrenailOverlayBuilder.cs`:

```csharp
namespace DeckPlanking.Core.Preview;

public static class TrenailOverlayBuilder
{
    public static IReadOnlyList<TrenailPoint> Build(
        IReadOnlyList<CenterlinePatternPreviewRow> rows,
        decimal? deckLengthMillimeters = null)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        var deckLength = deckLengthMillimeters ?? rows
            .SelectMany(row => row.SourceRow.SeamPositionsMillimeters)
            .DefaultIfEmpty(0m)
            .Max();

        var points = new List<TrenailPoint>();

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            foreach (var position in rows[rowIndex].SourceRow.SeamPositionsMillimeters)
            {
                if (position <= 0m || position >= deckLength)
                {
                    continue;
                }

                points.Add(new TrenailPoint(rowIndex, position, TrenailVerticalPlacement.Upper));
                points.Add(new TrenailPoint(rowIndex, position, TrenailVerticalPlacement.Lower));
            }
        }

        return points;
    }
}
```

- [ ] **Step 4: Run focused tests**

Run:

```powershell
dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --filter TrenailOverlayBuilderTests
```

Expected: all `TrenailOverlayBuilderTests` pass.

- [ ] **Step 5: Commit core model**

```powershell
git add src\DeckPlanking.Core\Preview\TrenailVerticalPlacement.cs src\DeckPlanking.Core\Preview\TrenailPoint.cs src\DeckPlanking.Core\Preview\TrenailOverlayBuilder.cs tests\DeckPlanking.Core.Tests\TrenailOverlayBuilderTests.cs
git commit -m "Add trenail overlay core model" -m "Calculate two trenail markers for each internal plank seam from the existing centerline preview rows." -m "Keeping this in the core project lets the MAUI renderer stay focused on drawing rather than pattern rules."
```

---

### Task 2: UI Toggle And Rendering

**Files:**
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/Graphics/DeckPatternPreviewDrawable.cs`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.nl.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.de.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.fr.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.es.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.it.resx`

- [ ] **Step 1: Add ViewModel toggle**

In `ScaleInputViewModel`, add:

```csharp
[ObservableProperty]
private bool showTrenails;
```

- [ ] **Step 2: Add preview toggle UI**

In `MainPage.xaml`, inside the preview controls `HorizontalStackLayout`, add:

```xml
<HorizontalStackLayout Spacing="6"
                       VerticalOptions="Center"
                       Margin="12,0,0,0">
    <Label Text="Trenails"
           VerticalOptions="Center"
           TextColor="{StaticResource AppPrimaryTextColor}" />
    <Switch IsToggled="{Binding ShowTrenails, Mode=TwoWay}"
            VerticalOptions="Center" />
</HorizontalStackLayout>
```

- [ ] **Step 3: Pass toggle into drawable**

In `DeckPatternPreviewDrawable`, add:

```csharp
public bool ShowTrenails { get; set; }
```

In `MainPage.xaml.cs`, update `OnViewModelPropertyChanged`:

```csharp
if (e.PropertyName is nameof(ScaleInputViewModel.ShowTrenails))
{
    UpdatePatternPreview();
}
```

In `UpdatePatternPreview`, add:

```csharp
patternPreviewDrawable.ShowTrenails = viewModel.ShowTrenails;
```

- [ ] **Step 4: Draw trenails**

In `DeckPatternPreviewDrawable`, collect row rectangles while drawing rows and draw markers after all planks/seams:

```csharp
var renderedRows = new List<(CenterlinePatternPreviewRow Row, RectF Rect)>();
DrawRows(canvas, upperRows, deckLength, drawingWidth, rowHeight, ref currentY, renderedRows);
...
renderedRows.Add((kingPlankRow, kingRect));
...
DrawRows(canvas, lowerRows, deckLength, drawingWidth, rowHeight, ref currentY, renderedRows);

if (ShowTrenails)
{
    DrawTrenails(canvas, renderedRows, deckLength);
}
```

Update `DrawRows` signature:

```csharp
private static void DrawRows(
    ICanvas canvas,
    IReadOnlyList<CenterlinePatternPreviewRow> centerlineRows,
    float deckLength,
    float drawingWidth,
    float rowHeight,
    ref float currentY,
    IList<(CenterlinePatternPreviewRow Row, RectF Rect)> renderedRows)
```

Add each rendered row:

```csharp
renderedRows.Add((centerlineRows[index], plankRect));
```

Add renderer:

```csharp
private static void DrawTrenails(
    ICanvas canvas,
    IReadOnlyList<(CenterlinePatternPreviewRow Row, RectF Rect)> renderedRows,
    float deckLength)
{
    if (deckLength <= 0 || renderedRows.Count == 0)
    {
        return;
    }

    var points = TrenailOverlayBuilder.Build(renderedRows.Select(row => row.Row).ToArray(), (decimal)deckLength);

    canvas.FillColor = Color.FromArgb("#2A160B");

    foreach (var point in points)
    {
        var rowRect = renderedRows[point.RowIndex].Rect;
        var x = rowRect.Left + ((float)point.PositionMillimeters / deckLength * rowRect.Width);
        var y = point.VerticalPlacement == TrenailVerticalPlacement.Upper
            ? rowRect.Top + (rowRect.Height * 0.28f)
            : rowRect.Bottom - (rowRect.Height * 0.28f);
        var radius = Math.Clamp(rowRect.Height * 0.11f, 1.4f, 3.2f);

        canvas.FillCircle(x, y, radius);
    }
}
```

- [ ] **Step 5: Add localization keys**

Add `ShowTrenails` to each ResX file. English value:

```xml
<data name="ShowTrenails" xml:space="preserve">
  <value>Trenails</value>
</data>
```

Dutch value:

```xml
<data name="ShowTrenails" xml:space="preserve">
  <value>Trenails</value>
</data>
```

German value:

```xml
<data name="ShowTrenails" xml:space="preserve">
  <value>Holznagel</value>
</data>
```

French value:

```xml
<data name="ShowTrenails" xml:space="preserve">
  <value>Gournables</value>
</data>
```

Spanish value:

```xml
<data name="ShowTrenails" xml:space="preserve">
  <value>Clavijas</value>
</data>
```

Italian value:

```xml
<data name="ShowTrenails" xml:space="preserve">
  <value>Caviglie</value>
</data>
```

- [ ] **Step 6: Build app**

Run:

```powershell
dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0
```

Expected: build succeeds with 0 errors.

- [ ] **Step 7: Commit UI rendering**

```powershell
git add src\DeckPlanking.App\ViewModels\ScaleInputViewModel.cs src\DeckPlanking.App\MainPage.xaml src\DeckPlanking.App\MainPage.xaml.cs src\DeckPlanking.App\Graphics\DeckPatternPreviewDrawable.cs src\DeckPlanking.App\Resources\Strings\AppResources*.resx
git commit -m "Render trenail overlay in preview" -m "Add a preview toggle and draw two trenail markers at each calculated internal plank seam." -m "The toggle is intentionally session-only until project persistence is implemented."
```

---

### Task 3: Final Verification And ARM64 Test Build

**Files:**
- No source changes expected.

- [ ] **Step 1: Run full core test suite**

Run:

```powershell
dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj
```

Expected: 0 failed tests.

- [ ] **Step 2: Build Windows app**

Run:

```powershell
dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0
```

Expected: build succeeds with 0 errors.

- [ ] **Step 3: Publish Android ARM64 debug APK**

Run:

```powershell
dotnet publish src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -c Debug -p:RuntimeIdentifier=android-arm64 -p:AndroidPackageFormat=apk
```

Expected: `src\DeckPlanking.App\bin\Debug\net10.0-android\android-arm64\publish\nl.hnsoftwaredevelopment.deckplanking-Signed.apk` is produced.

- [ ] **Step 4: Inspect APK payload**

Run:

```powershell
tar -tf src\DeckPlanking.App\bin\Debug\net10.0-android\android-arm64\publish\nl.hnsoftwaredevelopment.deckplanking-Signed.apk | Select-String -Pattern "lib/arm64-v8a/lib_DeckPlanking|lib/arm64-v8a/libmonodroid|lib/arm64-v8a/libmonosgen"
```

Expected: output includes `lib_DeckPlanking.App.dll.so`, `lib_DeckPlanking.Core.dll.so`, `libmonodroid.so`, and `libmonosgen-2.0.so`.

- [ ] **Step 5: Report result**

Report:

- Commit hashes created during implementation.
- Test/build results.
- APK path for Samsung testing.
- Any remaining limitation: phase 2 style selection is not implemented yet.
