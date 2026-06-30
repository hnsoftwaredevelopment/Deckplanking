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
    }
  });

  const issue = buildGitHubIssue(payload);

  assert.equal(issue.title, "[Bug] Preview is clipped");
  assert.match(issue.body, /App version: 26.06.30.001/);
  assert.match(issue.body, /Platform: Android/);
  assert.match(issue.body, /OS version: Android 15/);
  assert.match(issue.body, /Language: nl-NL/);
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
});
