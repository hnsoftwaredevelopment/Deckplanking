# Deckplanking Feedback Worker

Cloudflare Worker that receives feedback from the MAUI app and creates GitHub issues in `hnsoftwaredevelopment/Deckplanking`.

## Required Secret

Create a GitHub fine-grained token with access only to this repository and permission:

- Issues: Read and write

Store it in Cloudflare:

```powershell
cd feedback-worker
npx wrangler secret put GITHUB_TOKEN
```

## Deploy

```powershell
cd feedback-worker
npm install
npm test
npx wrangler deploy
```

The Worker exposes:

- `GET /health`
- `POST /feedback`

The app must post JSON with `type`, `title`, `description`, optional `name` and `contact`, and optional app `context`.
