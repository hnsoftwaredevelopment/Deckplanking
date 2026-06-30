# Project Foundation Design

## Goal

Create the first working foundation for Modelboat Deckplanking: a .NET MAUI app for Windows and Android backed by a testable core library that reproduces the deck-planking calculations from the Excel specification.

## Scope

This first slice sets up the repository, solution structure, core calculation model, unit tests, and basic app infrastructure. It does not implement the full graphical deck editor yet. It prepares that work by keeping all calculation behavior outside MAUI.

## Architecture

The solution is split into three projects:

- `DeckPlanking.Core`: platform-independent business logic and models.
- `DeckPlanking.Core.Tests`: unit tests for scale conversion, rounding, standard patterns, and seam positions.
- `DeckPlanking.App`: .NET MAUI UI shell targeting Windows and Android.

The app references the core library, but the core library has no dependency on MAUI, Syncfusion, storage, resources, or UI concepts.

## Product Decisions

- Display name: `Modelboat Deckplanking`.
- Short name: `Deckplanking`.
- Built-in shift patterns for v1: Every 2, Every 3, Every 4, Every 5.
- The Excel custom pattern is intentionally excluded from v1 and documented as a possible future extension.
- Measurement rounding and pattern division are separate concepts.
- Imperial display rounding uses common fractional denominators.
- Metric display rounding uses decimal places.
- Pattern division comes from the selected shift pattern.
- Project persistence will use local JSON files, not SQLite.
- Syncfusion licensing is loaded from a local ignored file or environment variable so the license is never committed.
- RESX localization is prepared for Dutch, English, German, French, Spanish, and Italian.
- Theme infrastructure is prepared for Light, Dark, Blue, Saffron, and Dark Red.

## Core Calculation Model

The core uses millimeters as its canonical scale unit.

Scale settings support decimal scale `1:N` and imperial scale `X inches = 1 real foot`. Real plank length can be entered in meters or feet.

Standard phase sequences:

- Every 2: `1, 2`
- Every 3: `1, 3, 2`
- Every 4: `1, 3, 2, 4`
- Every 5: `1, 3, 5, 2, 4`

For row `i`, phase is selected cyclically. The reference phase is the phase at row 9. Seam offset is `(phase - referencePhase + startPoint) mod divisions`, with offset `0` converted to the full division count.

## Testing

Core behavior is test-first. The first tests cover decimal and imperial scale conversion, standard pattern definitions, omission of Custom in v1, display rounding separation, and seam generation for the Every 3 Excel example.
