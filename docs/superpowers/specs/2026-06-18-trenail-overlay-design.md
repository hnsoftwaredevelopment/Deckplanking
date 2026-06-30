# Trenail Overlay Design

## Scope

Add a first, focused trenail overlay to the deck preview. The overlay is a visual layer only: it does not change plank length, seam placement, row generation, or orientation behavior.

This first phase includes:

- A show/hide toggle in the preview controls.
- A default trenail pattern of two trenails per plank end.
- Trenail positions derived from the generated plank seams and deck bounds.
- Live redraw when the toggle or deck pattern changes.

This first phase does not include user-selectable trenail styles. Those are reserved for phase 2.

## Phase 2 Styles

The later settings panel should support three styles:

- One trenail centered at the plank end.
- One trenail alternating between the upper and lower edge per plank segment.
- Two trenails, one near the upper edge and one near the lower edge.

When shown in the UI, each style should use a small plank-end preview so the user sees the exact pin placement instead of reading a technical description.

## Core Model

The core library gets a small preview model for trenails:

- `TrenailPoint`: row index or row number, x-position in millimeters, and vertical placement within the row.
- `TrenailPatternKind`: initially only `TwoPerPlankEnd`; later expanded with the phase 2 styles.
- `TrenailOverlayBuilder`: takes the same generated centerline preview rows used by the drawing and returns trenail points for visible plank ends.

The builder should skip deck boundaries unless those are useful as plank ends in the rendered reference. For phase 1, it should place trenails around internal seam positions and avoid drawing outside the row.

## Rendering

`DeckPatternPreviewDrawable` draws trenails after planks and seam lines, so the markers remain visible. Trenails are small dark circular markers. Their size should stay readable at normal zoom and not dominate the preview when zoomed out.

The overlay must respect:

- Current zoom and pan.
- King plank on/off.
- Continuous centerline pattern behavior.
- Bow/stern orientation display.

## UI

The main page adds one compact toggle near the preview controls. The label should be localized through ResX. The current project does not yet persist settings, so the toggle is session-only in phase 1.

## Testing

Add core unit tests for:

- Trenail generation at internal seams.
- No trenails when there are no rows or no seams.
- King plank and non-king-plank row sets both produce stable counts.

The UI rendering is verified by building Windows and publishing an Android ARM64 APK after implementation.
