import test from "node:test";
import assert from "node:assert/strict";

import {
  buildGitHubIssue,
  parseFeedbackPayload,
  validateFeedbackPayload
} from "../src/feedback.js";

test("validates required feedback fields", () => {
  const result = validateFeedbackPayload({
    type: "bug",
    title: "Preview is clipped",
    description: "The bow is no longer visible after changing the rounding."
  });

  assert.equal(result.ok, true);
});

test("rejects missing title and description", () => {
  const result = validateFeedbackPayload({
    type: "feature",
    title: "",
    description: ""
  });

  assert.equal(result.ok, false);
  assert.deepEqual(result.errors, ["Title is required.", "Description is required."]);
});

test("builds issue title and body with application context", () => {
  const payload = parseFeedbackPayload({
    type: "bug",
    title: "Preview is clipped",
    description: "The bow is no longer visible.",
    name: "Henk",
    contact: "henk@example.com",
    context: {
      appVersion: "26.06.30.001",
      platform: "Android",
      osVersion: "Android 15",
      language: "nl-NL"
    },
    diagnostics: {
      architecture: "Arm64",
      deviceType: "Phone",
      unitSystem: "Metric",
      theme: "Light",
      screen: "1080x2340 @ 3.0"
    }
  });

  const issue = buildGitHubIssue(payload);

  assert.equal(issue.title, "[Bug] Preview is clipped");
  assert.match(issue.body, /App version: 26.06.30.001/);
  assert.match(issue.body, /Platform: Android/);
  assert.match(issue.body, /OS version: Android 15/);
  assert.match(issue.body, /Language: nl-NL/);
  assert.match(issue.body, /## Diagnostics/);
  assert.match(issue.body, /Architecture: Arm64/);
  assert.match(issue.body, /Device type: Phone/);
  assert.match(issue.body, /Units: Metric/);
  assert.match(issue.body, /Theme: Light/);
  assert.match(issue.body, /Screen: 1080x2340 @ 3\.0/);
  assert.match(issue.body, /Name: Henk/);
  assert.match(issue.body, /Contact: henk@example.com/);
  assert.match(issue.body, /The bow is no longer visible\./);
});

test("builds feature request title", () => {
  const payload = parseFeedbackPayload({
    type: "feature",
    title: "Add exact print scale",
    description: "Print the deck at exact scale."
  });

  const issue = buildGitHubIssue(payload);

  assert.equal(issue.title, "[Feature] Add exact print scale");
  assert.doesNotMatch(issue.body, /## Diagnostics/);
});
