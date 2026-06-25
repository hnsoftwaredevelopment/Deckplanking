# About and Feedback Design

## Goal

Add a lightweight About page and prepare an in-app feedback flow for bug reports and feature requests.

## Decisions

- Show the application logo, display name, version, platform, and OS version on an About page.
- Prefer the assembly file version for the displayed version, because the Visual Studio automatic versioning extension updates that value per build.
- Add a Feedback page where users can choose between a bug report and a feature request.
- Automatically include app version, platform, OS version, and UI language in the feedback payload.
- Do not place a GitHub token in the MAUI app. Direct GitHub issue creation will be handled later by a small backend, such as an Azure Function.

## First Implementation

The first implementation prepares and formats the feedback payload and copies it to the clipboard. Automatic sending is intentionally deferred until the backend endpoint and secret handling are in place.

## Deferred

- Azure Function endpoint for receiving feedback.
- Server-side GitHub issue creation.
- Optional project-settings attachment after explicit user consent.
