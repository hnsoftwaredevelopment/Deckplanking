# Start Point Help Text Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Show a dynamic help text below the Start point input explaining whether the offset is relative to the king plank or to the row next to the centerline.

**Architecture:** Keep the rule in `ScaleInputViewModel` as a localized computed property. XAML displays that property under the existing Start point field, and ResX files provide the text in all supported languages.

**Tech Stack:** .NET MAUI, CommunityToolkit.Mvvm, ResX localization.

---

### Task 1: Add Localized ViewModel Text

**Files:**
- Modify: `src/DeckPlanking.App/ViewModels/ScaleInputViewModel.cs`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.nl.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.de.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.fr.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.es.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.it.resx`

- [x] **Step 1: Add the computed property**

Add `StartPointHelpText` to `ScaleInputViewModel`:

```csharp
public string StartPointHelpText => UseKingPlank
    ? T("StartPointRelativeToKingPlank")
    : T("StartPointRelativeToCenterlineRow");
```

- [x] **Step 2: Notify when king plank changes**

In the `UseKingPlank` setter, raise `OnPropertyChanged(nameof(StartPointHelpText))` whenever the value changes.

- [x] **Step 3: Notify when language changes**

In `OnLocalizationChanged`, raise `OnPropertyChanged(nameof(StartPointHelpText))`.

- [x] **Step 4: Add resource strings**

Add these keys to all `AppResources*.resx` files:

```xml
<data name="StartPointRelativeToKingPlank" xml:space="preserve"><value>Relative to the king plank.</value></data>
<data name="StartPointRelativeToCenterlineRow" xml:space="preserve"><value>Relative to the row next to the centerline.</value></data>
```

Use natural translated values in the localized files.

### Task 2: Display The Help Text

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`

- [x] **Step 1: Add one grid row**

Extend the input grid `RowDefinitions` from 12 rows to 13 rows.

- [x] **Step 2: Add help label under Start point**

Place a smaller secondary-color label in row 11, columns 1-2:

```xml
<Label Grid.Row="11"
       Grid.Column="1"
       Grid.ColumnSpan="2"
       Text="{Binding StartPointHelpText}"
       FontSize="12"
       TextColor="{DynamicResource AppSecondaryTextColor}" />
```

- [x] **Step 3: Move Shift pattern down**

Move the Shift pattern label and picker from row 11 to row 12.

### Task 3: Verify And Mirror Documentation

**Files:**
- Update copied Markdown under `C:\Data\Obsidian\markdown\Development\HNSoftwareDevelopment\Deckplanking`

- [x] **Step 1: Build Windows app**

Run:

```powershell
dotnet build .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0 --no-restore
```

Expected: build succeeds.

- [x] **Step 2: Run core tests**

Run:

```powershell
dotnet test .\tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj --no-restore
```

Expected: all tests pass.

- [x] **Step 3: Publish Android ARM64 APK**

Run:

```powershell
dotnet publish .\src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-android -r android-arm64 -c Debug --no-restore
```

Expected: publish succeeds and updates the signed APK.

- [x] **Step 4: Mirror Markdown to Obsidian**

Copy updated `.md` files from the repository to:

```text
C:\Data\Obsidian\markdown\Development\HNSoftwareDevelopment\Deckplanking
```
