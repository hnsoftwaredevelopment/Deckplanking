# App Settings and Syncfusion License Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a first app settings screen and fix development Syncfusion license discovery.

**Architecture:** App-wide settings live in small MAUI app services that wrap Preferences, theme resource replacement, and culture application. The Settings page binds directly in code-behind for this first version to keep the feature compact.

**Tech Stack:** .NET MAUI, Shell navigation, MAUI Preferences, ResourceDictionary themes, Syncfusion licensing.

---

### Task 1: App Preference Types

**Files:**
- Create: `src/DeckPlanking.App/Settings/AppLanguageOption.cs`
- Create: `src/DeckPlanking.App/Settings/AppThemeOption.cs`
- Create: `src/DeckPlanking.App/Settings/DisplayUnitSystemOption.cs`
- Create: `src/DeckPlanking.App/Settings/AppPreferenceItem.cs`
- Create: `src/DeckPlanking.App/Settings/AppPreferencesStore.cs`

- [ ] Add option records and a Preferences-backed store for language, theme, and display unit system.
- [ ] Ensure default values are English, Light, and Metric.

### Task 2: Runtime Application

**Files:**
- Create: `src/DeckPlanking.App/Settings/AppCultureManager.cs`
- Create: `src/DeckPlanking.App/Settings/AppThemeManager.cs`
- Modify: `src/DeckPlanking.App/App.xaml`
- Modify: `src/DeckPlanking.App/App.xaml.cs`

- [ ] Apply saved culture and theme at app startup.
- [ ] Replace the active theme dictionary when settings change.

### Task 3: Settings Page

**Files:**
- Create: `src/DeckPlanking.App/SettingsPage.xaml`
- Create: `src/DeckPlanking.App/SettingsPage.xaml.cs`
- Create: `src/DeckPlanking.App/Resources/Images/settings.svg`
- Modify: `src/DeckPlanking.App/AppShell.xaml.cs`
- Modify: `src/DeckPlanking.App/MainPage.xaml`

- [ ] Add a Shell route and toolbar icon.
- [ ] Add pickers for language, theme, and display unit system.
- [ ] Save settings immediately when a picker changes.

### Task 4: Syncfusion License Discovery

**Files:**
- Modify: `src/DeckPlanking.App/Infrastructure/SyncfusionLicenseRegistration.cs`
- Modify: `.gitignore`

- [ ] Search known local license locations including repository `docs`.
- [ ] Keep local license files ignored.

### Task 5: Verification

**Files:**
- Test/build only.

- [ ] Run `dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj`.
- [ ] Run `dotnet build src\DeckPlanking.App\DeckPlanking.App.csproj -f net10.0-windows10.0.19041.0`.
- [ ] Publish Android ARM64 APK.
