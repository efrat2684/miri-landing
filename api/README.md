# Miri Landing API

Part of the **miri-landing** monorepo (`api/` folder).  
ASP.NET Core API for the Miri Koler landing page contact form (sends email via Gmail SMTP).

## Run locally

```powershell
dotnet user-secrets set "Email:Smtp:User" "your@gmail.com" --project .
dotnet user-secrets set "Email:Smtp:Password" "your-app-password" --project .
dotnet run --launch-profile http
```

API: `http://localhost:5053`

## SMTP from Render / cloud

Some hosts block outbound **port 587** or delay it until the default 2‑minute MailKit timeout — the API now uses a **25s** SMTP timeout and clearer errors. If Gmail SMTP never connects from Render, use a provider with **HTTPS** (SendGrid, Mailgun, etc.) or open outbound SMTP on your host.

## Cloud deployment (environment variables)

Set these on the host (do not commit passwords):

- `Email__Recipient`
- `Email__FromAddress`
- `Email__Smtp__Host` (default: `smtp.gmail.com`)
- `Email__Smtp__Port` (default: `587`)
- `Email__Smtp__TimeoutMilliseconds` (optional, default `25000`)
- `Email__Smtp__User`
- `Email__Smtp__Password`

Endpoint: `POST /api/contact`
