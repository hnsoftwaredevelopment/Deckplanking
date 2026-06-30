# PDF Preview Export Design

## Scope

Add PDF export for the current deck pattern preview.

The PDF export is the next step after PNG export and uses the existing export storage behavior:

- Windows opens a Save As dialog.
- Android saves to Downloads and shows a confirmation message.

## User Experience

The preview toolbar gets a PDF export button next to the PNG export button.

The generated file name uses:

`deckplanking-YYYYMMDD-HHMM.pdf`

The first PDF version contains the deck pattern drawing only. It does not include input controls or a settings summary.

## Rendering

The PDF export reuses the existing preview rendering path by generating the same complete preview image used for PNG export and placing it on a single landscape PDF page.

The image should fit inside the page while preserving aspect ratio.

## Dependencies

Use Syncfusion PDF because the application already uses Syncfusion and the project has a Syncfusion license.

## Testing

Core filename generation is covered by automated tests.

App build verification covers the MAUI export wiring and PDF dependency for Windows and Android.
