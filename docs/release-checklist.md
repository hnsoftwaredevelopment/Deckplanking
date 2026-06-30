# Release Checklist

Use this checklist before publishing a Windows installer, Android test APK, or Google Play bundle.

## 1. Prepare

- [ ] Confirm the working tree contains only intended changes.
- [ ] Confirm `docs/SyncfusionLicense.txt` or another supported local Syncfusion license file exists for local builds.
- [ ] Confirm GitHub secret `SYNCFUSION_LICENSE` exists before using GitHub build workflows.
- [ ] Confirm the target version uses `YY.MM.DD.xxx`.

## 2. Local Verification

- [ ] Run core tests:

```powershell
dotnet test tests\DeckPlanking.Core.Tests\DeckPlanking.Core.Tests.csproj -c Release
```

- [ ] Run feedback worker tests:

```powershell
Push-Location feedback-worker
npm test
Pop-Location
```

- [ ] Build Windows artifact:

```powershell
.\scripts\build-windows.ps1
```

- [ ] Build Android ARM64 test APK:

```powershell
.\scripts\build-android-arm64.ps1
```

- [ ] Optional: build Google Play AAB:

```powershell
.\scripts\build-android-playstore.ps1
```

## 3. Windows Smoke Test

- [ ] Start `Deckplanking.exe` from `artifacts\windows\Deckplanking-YY.MM.DD.xxx`.
- [ ] Confirm the app opens without errors.
- [ ] Confirm About shows the expected version.
- [ ] Confirm only these language/resource directories remain in the Windows artifact:
  - `de`
  - `es`
  - `fr`
  - `it`
  - `nl`
  - `Microsoft.UI.Xaml`
  - `NpuDetect`
- [ ] Save and reopen a project.
- [ ] Export PNG.
- [ ] Export PDF.
- [ ] Print preview or print to PDF.
- [ ] Submit a test feedback issue if feedback changes were made.

## 4. Android Smoke Test

- [ ] Install the ARM64 APK from `artifacts\android-arm64\Deckplanking-YY.MM.DD.xxx` on a Samsung ARM64 device.
- [ ] Confirm the app starts without closing.
- [ ] Confirm About shows the expected version.
- [ ] Save and reopen a project.
- [ ] Export PNG to Downloads/share flow.
- [ ] Export PDF and confirm the Syncfusion trial watermark is not present.
- [ ] Print/share PDF.
- [ ] Submit a test feedback issue if feedback changes were made.

## 5. GitHub Actions

- [ ] Push the branch to GitHub.
- [ ] Confirm the `CI` workflow succeeds.
- [ ] Run `Build Windows` manually when a Windows artifact is needed.
- [ ] Run `Build Android` manually when Android APK/AAB artifacts are needed.
- [ ] Download GitHub artifacts and confirm they contain the same files as local builds.

Required GitHub secrets:

- `SYNCFUSION_LICENSE`

Optional Google Play signing secrets:

- `ANDROID_KEYSTORE_BASE64`
- `ANDROID_KEYSTORE_PASSWORD`
- `ANDROID_KEY_ALIAS`
- `ANDROID_KEY_PASSWORD`

## 6. Windows Installer

- [ ] Build the Windows artifact first.
- [ ] Confirm `installer\Build-Installer.ps1` points to the intended `artifacts\windows\Deckplanking-YY.MM.DD.xxx` folder.
- [ ] Confirm `installer\Installer.iss` uses the same artifact folder.
- [ ] Build the installer with Inno Setup.
- [ ] Install on a clean Windows machine or VM.
- [ ] Confirm Start Menu shortcut, taskbar icon, app icon, uninstall entry, and app launch.
- [ ] Confirm exports and feedback still work from the installed app.

## 7. Release Notes

- [ ] Summarize user-visible changes.
- [ ] Mention known limitations, such as Google Play availability if the AAB is not being published yet.
- [ ] Attach Windows installer, Windows zip artifact if needed, Android ARM64 APK, and optional Play Store AAB to the release.
