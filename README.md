# SkyRoute

SkyRoute is a full-stack flight search and booking application built for the developer challenge using .NET 10, Clean Architecture, Angular 18, NgRx, JWT authentication, and PostgreSQL.

The solution covers the required flight search flow, provider pricing rules, route-aware passenger validation, loading and empty states, protected booking, and a polished Angular UI. The backend is structured into Domain, Application, Infrastructure, and API projects. The frontend is a standalone Angular application with NgRx feature slices for auth, airports, search, and booking.

## Stack

- Backend: ASP.NET Core Web API, .NET 10, Entity Framework Core, PostgreSQL, JWT
- Frontend: Angular 18 standalone components, NgRx Store/Effects, SCSS
- Database: PostgreSQL 17 via Docker Compose
- Testing: xUnit, FluentAssertions, ASP.NET Core integration tests, Angular Jasmine/Karma tests

## Challenge Coverage

- Airport catalog with 6 seeded airports across Argentina, Chile, and Brazil
- Search form with origin, destination, departure date, passengers, and cabin class
- Mock provider aggregation for GlobalAir and BudgetWings
- GlobalAir pricing: base fare + 15% surcharge
- BudgetWings pricing: base fare - 10% with a minimum final price of 29.99
- Per-passenger and total pricing shown in search and booking
- Route-aware document validation:
  - Domestic routes require National ID
  - International routes require Passport Number
- Frontend-only sorting using NgRx selectors
- JWT-secured booking flow
- Booking confirmation with generated reference code

## Architecture

### Backend

- `backend/src/SkyRoute.Domain`
  - Core entities, enums, pricing strategies, route/document rules
- `backend/src/SkyRoute.Application`
  - DTOs, interfaces, use-case services, validation, exceptions
- `backend/src/SkyRoute.Infrastructure`
  - EF Core persistence, repositories, JWT/offer token services, deterministic provider mocks, seed data
- `backend/src/SkyRoute.Api`
  - Controllers, DI composition, authentication, CORS, Swagger, startup pipeline

### Frontend

- `frontend/src/app/features`
  - Search, auth, booking, and shell components
- `frontend/src/app/store`
  - NgRx state for auth, airports, search, and booking
- `frontend/src/app/core`
  - API client, storage service, auth interceptor, route guards
- `frontend/src/app/shared`
  - Shared models and utilities, including document-rule helpers and API error extraction

## Local Setup

### 1. Start PostgreSQL

From the repository root:

```powershell
docker compose up -d postgres
```

PostgreSQL listens on `localhost:5433`.

### 2. Run the API

```powershell
cd backend
dotnet run --project src/SkyRoute.Api
```

API base URL:

- `http://localhost:5279`

Swagger UI:

- `http://localhost:5279/swagger`

The API applies migrations on startup and seeds airports, providers, and the demo user automatically.

### 3. Run the Angular client

```powershell
cd frontend
npm install
npm start
```

Client URL:

- `http://localhost:4200`

## Demo Credentials

- Email: `demo@skyroute.local`
- Password: `Travel123!`

## Verified Commands

### Backend

```powershell
cd backend
dotnet test SkyRoute.sln
```

### Frontend build

```powershell
cd frontend
npm run build
```

### Frontend tests

In the current Windows environment, Edge was used as the headless browser:

```powershell
cd frontend
$env:CHROME_BIN='C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe'
npm test -- --watch=false --browsers=ChromeHeadless
```

## Notes And Tradeoffs

- Provider integrations are deterministic mocks rather than live airline APIs.
- Offer selection is protected with a signed offer token so booking does not trust client-side prices.
- Authentication uses access tokens only; refresh tokens and account recovery were intentionally kept out of scope.
- The UI is optimized for the required challenge flow rather than a multi-step enterprise booking engine.
- Backend validation remains authoritative even when the Angular client already validates inputs.

## Repository Structure

```text
SkyRoute/
├── backend/
│   ├── src/
│   └── tests/
├── frontend/
└── docker-compose.yml
```