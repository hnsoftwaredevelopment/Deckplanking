# Stern Rounding Design

## Goal

Add a light numeric stern-rounding option to complete the simple deck contour controls. The behavior mirrors bow rounding, but is only available when the selected deck shape narrows both bow and stern.

## User Experience

When the selected deck shape is "narrowed at bow and stern", the input form shows a new percentage field:

- Stern rounding

The value ranges from 0 to 100 percent:

- 0 keeps the current straight stern taper.
- Higher values soften the stern taper into a rounded shoulder.
- 100 gives the strongest stern rounding within the simple contour model.

The field is hidden for rectangular decks and bow-only narrowed decks.

## Geometry

Stern rounding uses the same sampled quadratic curve approach as bow rounding. The renderer still receives a polygon contour, so preview clipping, hit testing, PNG export, PDF export, and print all remain on the same path.

For the stern side, x=0 is the stern front and the taper ends at `SternTaperLengthPercentage`. The rounded shoulder is sampled on both upper and lower stern sides and mirrored over the deck centerline.

## Persistence

The stern rounding percentage is stored in project JSON and restored when projects are opened. Existing project files default to 0 so their appearance stays unchanged.

## Validation

Stern rounding must be between 0 and 100 percent. Invalid values use the existing validation flow.

## Out Of Scope

This feature does not add draggable handles, separate left/right control points, waterways, margin planks, or changes to plank calculation.

## Testing

Core tests cover:

- 0 percent stern rounding keeps the existing straight stern contour.
- rounded stern contours include mirrored upper and lower curve points.
- invalid stern rounding values are rejected.
- project serialization includes stern rounding.
