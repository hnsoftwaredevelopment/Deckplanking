# ResX UI Localization Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Route visible MAUI UI strings through ResX resources and refresh them when the app language setting changes.

**Architecture:** Add a small localization resource manager and markup extension in the app project. XAML binds text properties to resource keys; settings language changes update culture and notify bindings.

**Tech Stack:** .NET MAUI, ResX, XAML markup extensions, `INotifyPropertyChanged`.

---

### Task 1: Localization Infrastructure

**Files:**
- Create: `src/DeckPlanking.App/Localization/LocalizationResourceManager.cs`
- Create: `src/DeckPlanking.App/Localization/TranslateExtension.cs`
- Modify: `src/DeckPlanking.App/Settings/AppCultureManager.cs`

- [ ] Add a singleton resource manager with an indexer for resource keys.
- [ ] Notify bindings when the culture changes.

### Task 2: Resource Keys

**Files:**
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.nl.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.de.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.fr.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.es.resx`
- Modify: `src/DeckPlanking.App/Resources/Strings/AppResources.it.resx`

- [ ] Add keys for main page, settings page, alerts, and option labels.
- [ ] Translate Dutch `Trenails` as `Plankspijkers`.

### Task 3: Apply Resources

**Files:**
- Modify: `src/DeckPlanking.App/MainPage.xaml`
- Modify: `src/DeckPlanking.App/MainPage.xaml.cs`
- Modify: `src/DeckPlanking.App/SettingsPage.xaml`
- Modify: `src/DeckPlanking.App/SettingsPage.xaml.cs`
- Modify: `src/DeckPlanking.App/AppShell.xaml`

- [ ] Replace visible hardcoded strings with localization bindings.
- [ ] Update code-behind alerts and settings option display names.

### Task 4: Verification

**Files:**
- Build/test only.

- [ ] Run core tests.
- [ ] Build Windows.
- [ ] Publish Android ARM64 APK.
