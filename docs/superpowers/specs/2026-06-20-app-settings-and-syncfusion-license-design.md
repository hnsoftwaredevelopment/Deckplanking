# App Settings and Syncfusion License Design

## Goal

Add a first settings screen for app-wide preferences and make local Syncfusion license discovery work from development-friendly locations without committing the license file.

## Settings Scope

The first settings screen stores app preferences, not project settings. It includes language, theme, and display unit system. These values are saved through MAUI Preferences so they survive app restarts on Windows and Android.

## Theme Behavior

Theme selection uses the existing theme resource dictionaries. The active dictionary is replaced at runtime and the primary page colors use dynamic resources so the visible screen can react without restarting the app.

## Language Behavior

Language selection is stored as a preference and applied to the current thread culture at startup and after changes. Full ResX coverage can grow later; this step prepares the setting and persistence.

## Syncfusion License Behavior

The license loader checks the environment variable first, then known local license filenames in the executable folder, the current working directory, the repository root, and the `docs` folder. It never logs or exposes the license value. Git ignore rules continue to keep local license files out of GitHub.
