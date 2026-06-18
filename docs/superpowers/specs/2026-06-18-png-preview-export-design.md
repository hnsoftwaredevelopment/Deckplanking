# PNG Preview Export Design

## Scope

Add the first export feature for Modelboat Deckplanking: exporting the deck pattern preview as a PNG image.

This first export step includes only PNG. PDF export and printing remain follow-up work.

## User Experience

The preview area gets an export control for PNG export. The exported image contains only the deck pattern drawing, not the input controls and not a settings summary.

The generated filename uses:

`deckplanking-YYYYMMDD-HHMM.png`

## Rendering

The export uses the same preview drawing logic as the on-screen preview, so the exported image matches the current pattern, king plank setting, orientation, zoom-independent pattern state, and trenail selection.

The exported PNG should render at a stable export size instead of capturing the current zoom/pan viewport. The first version uses a complete deck preview image.

## Platform Behavior

On Windows, the PNG is written to a user-accessible location.

On Android, the PNG is created as an app file and handed to the platform share/save flow so the user can store or share it.

## Testing

Core filename generation is covered by automated tests.

App build verification covers the MAUI export wiring for Windows and Android.
