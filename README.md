# Incident & Crisis Management Platform

A full-stack application for managing incidents and crises, featuring a .NET 10 Web API backend and a Next.js 16.3.4 frontend (Turbopack).

## Prerequisites

Ensure you have the following installed on your machine:

- **Docker Desktop** (for the database)
- **.NET 10.0 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **Node.js** (v20.9+ recommended, 20.9 is minimum for Next 16) & **npm 10+** ([Download](https://nodejs.org/))

## Getting Started

### 1. Infrastructure Setup

Start the PostgreSQL database using Docker Compose.

```bash
docker-compose up -d
```

This will spin up a Postgres instance on port **5432** with the following credentials (via `docker-compose.yml:1` + user-secrets):
- **Database:** `reconciliation`
- **User:** `postgres`
- **Password:** `masterpassword`
- **Host:** `localhost:5432`

> For local dev the Api uses **user-secrets** `ConnectionStrings:DefaultConnection=Host=localhost;Port=5432;Database=reconciliation;Username=postgres;Password=masterpassword` (`Api/Api.csproj:7` `UserSecretsId`), not `appsettings.json` `5433/darlingson`.

### 2. Backend Setup (API)

Navigate to the `Api` directory to set up the .NET backend.

```bash
cd Api
```

**Restore dependencies:**

```bash
dotnet restore
```

**Apply Database Migrations:**

Ensure the database container is running, then create the schema:

```bash
dotnet ef database update
```

**Run the API:**

```bash
dotnet run
```

The API will typically start on `http://localhost:5000` or `https://localhost:5001`. Check the console output for the exact URL. Swagger documentation should be available at `/swagger`.

### 3. Frontend Setup (Web)

Navigate to the `web` directory to set up the Next.js frontend.

```bash
cd web
```

**Install dependencies:**

```bash
npm install
```

**Run the development server:**

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the application.

**Or one-command (Api + web, DB via docker):**
```bash
npm install # at repo root installs concurrently + web deps
npm run dev # Api (.NET) + web (Next Turbopack) together
```

## Project Structure

- **Api/**: ASP.NET Core Web API (Backend)
- **web/**: Next.js + React Application (Frontend)
- **docker-compose.yml**: Database infrastructure configuration

## Configuration

### Backend
Configuration is located in `Api/appsettings.json`. The default connection string is pre-configured to match the Docker setup:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=reconciliation;Username=postgres;Password=masterpassword"
}
```
Use user-secrets for local: `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=reconciliation;Username=postgres;Password=masterpassword" --project Api`

### Frontend
Frontend configuration can be managed via environment variables (e.g., `.env.local`).

## Troubleshooting

- **Database Connection Failed:** Ensure the Docker container is running (`docker ps`) and port 5433 is not blocked.
- **Port Conflicts:** If ports 3000 (Web) or 5000/5001 (API) are in use, you may need to stop other services or change the port configurations.
