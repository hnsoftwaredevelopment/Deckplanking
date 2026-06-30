# Deck Contour V1 Design

## Goal

Add a simple visual deck contour to the pattern preview so the drawing can approximate a real model ship deck without turning the app into a CAD tool.

## Scope

Deck contour v1 is a preview/export feature. It changes how the deck drawing is clipped and framed, but it does not change the underlying plank pattern calculation.

The feature intentionally stays approximate. The goal is a practical visual guide for model builders, not a mathematically exact hull/deck shape.

## User Options

The app adds a deck shape option with three values:

- Rectangular
- Narrowed at bow
- Narrowed at bow and stern

For narrowed shapes, the user can enter width and taper percentages:

- Bow width percentage
- Bow taper length percentage
- Stern width percentage, visible only when the selected shape narrows the stern
- Stern taper length percentage, visible only when the selected shape narrows the stern

The values describe the deck width at the end compared with the full deck width. For example, 60 means the bow end is drawn at 60 percent of the full deck width.

The taper length values describe how much of the deck length is used for the narrowing. For example, a bow taper length of 25 means the bow narrowing starts at 75 percent of the deck length. This keeps the middle of the deck full-width and better approximates a real deck plan.

## Geometry

The contour is symmetrical over the deck centerline.

In v1, the contour is a simple polygon with a full-width middle section and local taper zones:

- Rectangular: full width from stern to bow.
- Narrowed at bow: stern and middle stay full width, the final bow section narrows symmetrically.
- Narrowed at bow and stern: the middle stays full width, with separate symmetrical taper sections at bow and stern.

The bow may be rounded or sharper in real life, and the stern is often nearly rectangular with only a small narrowing. V1 uses straight taper edges as a clear approximation.

## Rendering

The preview keeps rendering the same calculated plank rows and seams, but clips the visible drawing to the deck contour.

Rows outside the contour are hidden by clipping. Rows inside the contour keep their existing pattern, seams, trenails, ruler, orientation labels, zoom/pan behavior, and segment inspection.

The centerline and king plank remain centered and follow the existing behavior.

## Export And Print

PNG export, PDF export, and print use the same contour as the on-screen preview.

The exported drawing does not include extra settings text; the contour itself is visible in the drawing.

## Project Persistence

The selected deck shape, width percentages, and taper length percentages are stored in project files and restored when a project is opened.

Last-used settings also remember the selected contour settings.

## Validation

Width percentages must be practical positive values:

- Minimum: 10 percent
- Maximum: 100 percent

Taper length percentages must stay within a practical range:

- Minimum: 0 percent
- Maximum: 50 percent

Invalid values show the existing validation style and prevent broken preview geometry.

## Out Of Scope

Deck contour v1 does not include:

- Curved bow shapes
- Manually editable control points
- Port/starboard asymmetry
- Margin planks or waterways
- Joggling at bow or stern
- Changes to the plank calculation itself

These can be considered later if the simple contour proves useful and still feels too schematic.

## Future Direction: Bezier Bow Shape

A later version can add an advanced bow-shape mode based on a quadratic Bezier curve. The user would define:

- a start point on the deck side where narrowing begins;
- an end point at the bow front, using a finite bow width instead of a sharp point;
- a control point between start and end that changes the roundness or sharpness of the curve.

Only one side needs to be defined by the user. The opposite side is mirrored automatically over the deck centerline, keeping the deck symmetrical.

This is the right geometric model for a realistic rounded bow, but it is intentionally deferred because it adds interaction complexity on both Windows and Android: selecting and dragging handles, keeping handles readable while zooming and panning, validating impossible shapes, persisting control points, and exporting the exact same curve.

When this is implemented, it should start with clear numeric controls for start position, bow width, and curve roundness. Direct drag handles in the preview can be added after the numeric version is understandable and stable.

## Implementation Notes

The contour should live in the core model as simple settings and in the app renderer as a clipping polygon.

The renderer should keep the existing rectangle as the default path so current projects look unchanged until the user selects another shape.

Tests should cover shape settings serialization and the polygon calculation separately from the MAUI drawing code.
