# Miri Landing — Client (Angular)

Landing page for Miri Koler. The contact form calls the backend API.

## Run locally

Terminal 1 — API (folder `api/`, separate Git repo):

```powershell
cd api
dotnet run --launch-profile http
```

Terminal 2 — this client:

```powershell
npm install
npm start
```

Open: http://localhost:4200

Dev API URL is configured in `src/environments/environment.ts` (`http://localhost:5053/api`).

## Server repository

Backend source lives in a **separate GitHub repo** for cloud deployment (`miri-landing-api`).
