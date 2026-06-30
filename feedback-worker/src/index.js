import {
  buildGitHubIssue,
  parseFeedbackPayload,
  validateFeedbackPayload
} from "./feedback.js";

const githubApiVersion = "2022-11-28";

export default {
  async fetch(request, env) {
    if (request.method === "OPTIONS") {
      return new Response(null, { status: 204, headers: corsHeaders(env) });
    }

    const url = new URL(request.url);
    if (request.method === "GET" && url.pathname === "/health") {
      return json({ ok: true }, 200, env);
    }

    if (request.method !== "POST" || url.pathname !== "/feedback") {
      return json({ error: "Not found" }, 404, env);
    }

    let payload;
    try {
      payload = await request.json();
    } catch {
      return json({ error: "Request body must be valid JSON." }, 400, env);
    }

    const validation = validateFeedbackPayload(payload);
    if (!validation.ok) {
      return json({ error: "Invalid feedback.", details: validation.errors }, 400, env);
    }

    if (!env.GITHUB_TOKEN) {
      return json({ error: "Feedback endpoint is not configured." }, 500, env);
    }

    const issue = buildGitHubIssue(parseFeedbackPayload(payload));
    const result = await createGitHubIssue(issue, env);

    if (!result.ok) {
      return json({ error: "GitHub issue could not be created.", details: result.details }, 502, env);
    }

    return json({ ok: true, issueUrl: result.issueUrl }, 201, env);
  }
};

async function createGitHubIssue(issue, env) {
  const owner = env.GITHUB_OWNER || "hnsoftwaredevelopment";
  const repo = env.GITHUB_REPO || "Deckplanking";
  const response = await fetch(`https://api.github.com/repos/${owner}/${repo}/issues`, {
    method: "POST",
    headers: {
      "Accept": "application/vnd.github+json",
      "Authorization": `Bearer ${env.GITHUB_TOKEN}`,
      "Content-Type": "application/json",
      "User-Agent": "modelboat-deckplanking-feedback-worker",
      "X-GitHub-Api-Version": githubApiVersion
    },
    body: JSON.stringify(issue)
  });

  const body = await response.json().catch(() => ({}));
  if (!response.ok) {
    return {
      ok: false,
      details: body.message || `GitHub returned ${response.status}.`
    };
  }

  return {
    ok: true,
    issueUrl: body.html_url
  };
}

function json(body, status, env) {
  return new Response(JSON.stringify(body), {
    status,
    headers: {
      ...corsHeaders(env),
      "Content-Type": "application/json; charset=utf-8"
    }
  });
}

function corsHeaders(env) {
  return {
    "Access-Control-Allow-Origin": env.ALLOWED_ORIGIN || "*",
    "Access-Control-Allow-Methods": "GET,POST,OPTIONS",
    "Access-Control-Allow-Headers": "Content-Type"
  };
}
