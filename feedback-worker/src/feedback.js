const maxTitleLength = 120;
const maxDescriptionLength = 4000;
const maxOptionalLength = 200;

export function validateFeedbackPayload(payload) {
  const errors = [];

  if (!payload || typeof payload !== "object" || Array.isArray(payload)) {
    return { ok: false, errors: ["Feedback payload must be an object."] };
  }

  if (!["bug", "feature"].includes(payload.type)) {
    errors.push("Type must be bug or feature.");
  }

  if (!hasText(payload.title)) {
    errors.push("Title is required.");
  } else if (payload.title.trim().length > maxTitleLength) {
    errors.push(`Title must be ${maxTitleLength} characters or fewer.`);
  }

  if (!hasText(payload.description)) {
    errors.push("Description is required.");
  } else if (payload.description.trim().length > maxDescriptionLength) {
    errors.push(`Description must be ${maxDescriptionLength} characters or fewer.`);
  }

  for (const field of ["name", "contact"]) {
    if (payload[field] && String(payload[field]).trim().length > maxOptionalLength) {
      errors.push(`${capitalize(field)} must be ${maxOptionalLength} characters or fewer.`);
    }
  }

  return { ok: errors.length === 0, errors };
}

export function parseFeedbackPayload(payload) {
  return {
    type: payload.type === "feature" ? "feature" : "bug",
    title: normalizeText(payload.title),
    description: normalizeText(payload.description),
    name: normalizeOptionalText(payload.name),
    contact: normalizeOptionalText(payload.contact),
    context: {
      appVersion: normalizeOptionalText(payload.context?.appVersion),
      platform: normalizeOptionalText(payload.context?.platform),
      osVersion: normalizeOptionalText(payload.context?.osVersion),
      language: normalizeOptionalText(payload.context?.language)
    }
  };
}

export function buildGitHubIssue(payload) {
  const typeLabel = payload.type === "feature" ? "Feature" : "Bug";
  const body = [
    "## Feedback",
    "",
    `Type: ${typeLabel}`,
    "",
    "## Application",
    "",
    `App version: ${payload.context.appVersion || "Unknown"}`,
    `Platform: ${payload.context.platform || "Unknown"}`,
    `OS version: ${payload.context.osVersion || "Unknown"}`,
    `Language: ${payload.context.language || "Unknown"}`,
    ...optionalLine("Name", payload.name),
    ...optionalLine("Contact", payload.contact),
    "",
    "## Description",
    "",
    payload.description
  ].join("\n");

  return {
    title: `[${typeLabel}] ${payload.title}`,
    body
  };
}

function hasText(value) {
  return typeof value === "string" && value.trim().length > 0;
}

function normalizeText(value) {
  return String(value ?? "").trim();
}

function normalizeOptionalText(value) {
  const text = normalizeText(value);
  return text.length > 0 ? text : "";
}

function optionalLine(label, value) {
  return value ? [`${label}: ${value}`] : [];
}

function capitalize(value) {
  return value.charAt(0).toUpperCase() + value.slice(1);
}
