# Bow Rounding Design

## Goal

Add a light numeric bow-rounding option to the deck contour so narrowed bows look less like a straight taper and closer to a real rounded deck plan, without adding draggable Bezier handles yet.

## User Experience

When the selected deck shape is narrowed at the bow, the input form shows a new percentage field:

- Bow rounding

The value ranges from 0 to 100 percent:

- 0 keeps the current straight taper exactly.
- Higher values pull the bow taper into a softer curve.
- 100 gives the strongest rounded shoulder within the current simple contour model.

The field is hidden for rectangular decks because there is no bow narrowing to round.

## Geometry

The existing contour keeps using normalized deck coordinates where x=0 is stern and x=1 is bow.

Bow rounding is implemented as sampled quadratic curve points instead of a native drawing-only Bezier path. This keeps preview clipping, hit testing, PNG export, PDF export, and print on the same polygon-based model.

The bow curve uses enough sample points to avoid visibly coarse steps at high zoom levels. The lower bow side explicitly includes the lower bow-front point before returning toward the full-width deck side, so the lower curve mirrors the upper curve instead of cutting diagonally across the bow.

For the upper bow side:

- Start point: where the bow taper begins at full deck width.
- End point: the upper point of the narrowed bow front.
- Straight control point: midpoint between start and end.
- Rounded control point: full-width bow corner.

The selected bow rounding percentage interpolates between the straight and rounded control points. The lower bow side mirrors the upper curve over the deck centerline.

## Persistence

The bow rounding percentage is stored in project JSON and restored when projects are opened. Existing project files that do not contain the value default to 0 so they retain the previous appearance.

## Validation

Bow rounding must be between 0 and 100 percent. Invalid values use the existing validation flow and prevent broken contour geometry.

## Out Of Scope

This feature does not add drag handles, manually editable Bezier points, asymmetry, waterways, margin planks, or changes to plank calculation.

## Testing

Core tests cover:

- 0 percent rounding returns the same contour as the current straight taper.
- 100 percent rounding adds sampled bow curve points.
- invalid bow rounding values are rejected.
- project serialization round-trips the bow rounding setting.
