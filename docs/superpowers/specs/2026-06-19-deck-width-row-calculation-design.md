# Deck Width Row Calculation Design

## Goal

Let the user calculate the number of plank rows per side from the physical deck width and plank width, while keeping the current manual row-count input available.

## Scope

This phase adds a simple row input mode:

- manual rows per side;
- calculate rows per side from deck width and plank width.

It does not add tapered/non-rectangular deck outlines, margin planks, joggling, or beam-position checking.

## Behavior

The existing `Rows` value represents rows per side of the centerline or king plank. That interpretation stays unchanged.

When width-based mode is enabled:

- `Deck width` is the full deck width in millimeters.
- `Plank width` is the width of one deck plank in millimeters.
- `King plank width` is available when king plank is enabled and defaults to the normal plank width for older projects.
- Without king plank: `rowsPerSide = ceil(deckWidth / (2 * plankWidth))`.
- With king plank: `rowsPerSide = ceil(max(deckWidth - kingPlankWidth, 0) / (2 * plankWidth))`.
- Minimum result is `1` row per side.

When manual mode is enabled:

- The user edits `Rows` directly.
- Deck width and plank width are retained for later use, but do not drive the preview.

## Persistence

Project JSON and last-used settings store:

- row input mode;
- deck width;
- plank width;
- king plank width;
- row count.

Older project files that do not contain the new fields remain loadable. Missing width fields are treated as default values, missing king plank width uses plank width, and missing mode defaults to manual rows.

## Testing

Core tests cover:

- width-based rows without king plank;
- width-based rows with king plank;
- positive-value validation;
- project JSON round-trip including the new fields.
