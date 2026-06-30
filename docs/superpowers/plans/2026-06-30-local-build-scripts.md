# Local Build Scripts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add repeatable local build scripts for Windows and Android ARM64 with clean artifact output, version injection, and Windows satellite language cleanup.

**Architecture:** PowerShell scripts live under `scripts/` and publish into ignored `artifacts/` folders. A shared helper computes a `YY.MM.DD.xxx` display version and passes version properties into MSBuild without rewriting project files.

**Tech Stack:** PowerShell, .NET MAUI publish, MSBuild properties.

---

### Task 1: Build Version Helper

**Files:**
- Create: `scripts/build-common.ps1`
- Modify: `.gitignore`

- [x] Add `artifacts/` to `.gitignore`.
- [x] Add helper functions for repo path resolution, next daily build version, MSBuild version arguments, and Windows satellite directory cleanup.
- [x] Store the local counter in `artifacts/build-version-state.json`.

### Task 2: Local Publish Scripts

**Files:**
- Create: `scripts/build-windows.ps1`
- Create: `scripts/build-android-arm64.ps1`
- Create: `scripts/build-all.ps1`

- [x] Publish Windows Release to `artifacts/windows/Deckplanking-YY.MM.DD.xxx`.
- [x] Publish Android ARM64 Release APK to `artifacts/android-arm64/Deckplanking-YY.MM.DD.xxx`.
- [x] Allow `-VersionOverride` for deterministic rebuilds.
- [x] Run Windows cleanup after publish so only `en`, `nl`, `de`, `fr`, `es`, and `it` satellite language folders remain.

### Task 3: Verification

**Files:**
- No extra files.

- [x] Run core tests.
- [x] Run Windows script and verify `Deckplanking.exe` exists.
- [x] Verify Windows publish contains no unused satellite language directories.
- [x] Run Android ARM64 script and verify the signed APK exists.
