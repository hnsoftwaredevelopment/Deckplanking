# Project Persistence Phase 1 Design

## Goal

Add a first local project save/load flow so a user can preserve the current deck-planking configuration as a JSON file and reopen it later on the same platform.

## Scope

This phase stores the current configuration only. It does not add a full project manager, rename/delete actions, cloud sync, or automatic cross-device synchronization.

The first UI will expose:

- Save as project
- Open project

The project file contains enough state to restore the current calculation and preview:

- real plank length
- length unit
- scale mode
- decimal scale
- imperial inches per foot
- deck length in millimeters
- row count
- start point
- shift pattern
- king plank state
- deck orientation
- trenail pattern

It intentionally does not store transient viewport state such as zoom and pan. Opening a project should reset the preview to the default viewport so the drawing is immediately understandable.

## File Format

Projects are saved as UTF-8 JSON with file extension `.deckplanking.json`.

The JSON model includes:

- `schemaVersion`, initially `1`
- `savedAt`, using UTC ISO-8601 format
- `settings`, containing the configuration values

Example shape:

```json
{
  "schemaVersion": 1,
  "savedAt": "2026-06-19T10:30:00Z",
  "settings": {
    "realPlankLength": 9,
    "lengthUnit": "Meters",
    "scaleMode": "Decimal",
    "decimalScale": 64,
    "imperialInchesPerFoot": 0.1666666667,
    "deckLengthMillimeters": 600,
    "rowCount": 8,
    "startPoint": 0,
    "shiftPattern": "Every5",
    "useKingPlank": false,
    "deckOrientation": "BowLeft",
    "trenailPattern": "TwoPerPlankEnd"
  }
}
```

Enums are serialized as strings so files remain readable and easier to migrate later.

## Architecture

The app gets a small project persistence boundary:

- `DeckPlankingProjectSettings`: DTO for restorable app settings.
- `DeckPlankingProjectDocument`: DTO for the complete saved file.
- `ProjectJsonSerializer`: converts project documents to/from JSON with stable options.
- `ProjectFileService`: platform-aware save/open entry point used by the page.

The view model will expose methods to capture the current settings and apply loaded settings. This keeps file IO out of calculation code and avoids coupling the core library to MAUI file pickers.

## Platform Behavior

Windows:

- Save uses a native Save As picker.
- Open uses a native file picker.
- Suggested extension is `.deckplanking.json`.

Android:

- Save writes the file to Downloads and shows the existing saved confirmation pattern.
- Open uses the MAUI file picker for JSON/project files where supported.

This mirrors the export behavior already chosen for PNG/PDF: Windows users choose the destination, Android users get a predictable Downloads location.

## Validation And Errors

When opening a file:

- Invalid JSON shows a friendly "Project could not be opened" message.
- Unsupported future schema versions are rejected with a clear message.
- Invalid values are applied through the same view model validation path used by manual input.

The app should never partially update the visible configuration if the project file cannot be parsed or has an unsupported schema version.

## Testing

Core tests will cover:

- JSON round-trip for a representative project.
- enum serialization as strings.
- rejection of unsupported schema versions.

Manual verification will cover:

- Save project on Windows.
- Open the saved project on Windows.
- Publish Android ARM64 APK.
- Confirm the new project controls are present and compile on Android.

