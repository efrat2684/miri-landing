# Miri Landing

Monorepo: **Angular client** (repo root) + **ASP.NET API** (`api/`).

## Structure

```
/
  api/                 # Server (C#) — use this as Root Directory on Render
  src/                 # Client (Angular)
  angular.json
  package.json
```

## Run locally

**API:**

```powershell
cd api
dotnet run --launch-profile http
```

**Client:**

```powershell
npm install
npm start
```

Open: http://localhost:4200

## Deploy server to Render (monorepo)

One GitHub repo is fine. In Render **Web Service** settings:

| Setting | Value |
|---------|--------|
| **Root Directory** | `api` |
| **Runtime** | .NET |
| **Build Command** | `dotnet publish -c Release -o out` |
| **Start Command** | `dotnet out/MiriLanding.Api.dll` |

Add environment variables (see `api/README.md`):

`Email__Smtp__User`, `Email__Smtp__Password`, `Email__Recipient`, etc.

After deploy, set the client production API URL in `src/environments/environment.prod.ts` to your Render URL.

## GitHub

Single repository, for example: `miri-landing`

```powershell
git remote set-url origin https://github.com/efrat2684/miri-landing.git
git push -u origin main
```
