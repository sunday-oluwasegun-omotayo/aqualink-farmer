![AquaLink CI](https://github.com/sunday-oluwasegun-omotayo/aqualink-farmer/actions/workflows/ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-active-brightgreen)

# AquaLink — Digital Infrastructure for Epe Division, Lagos State, Nigeria

> A platform for fish farmers, traders, and cooperative societies in Epe Division, Lagos State, Nigeria. Built to solve real economic problems with serious engineering.

---

## The Problem

Fish farmers in Epe Division — one of Nigeria's most productive aquaculture zones — operate without digital tools:

- **Price asymmetry** — middlemen know the Lagos market price. Farmers don't. This costs farmers 20–40% of their harvest value every cycle.
- **Paper ledgers** — cooperative societies managing millions of naira use paper records with no audit trail and no fraud protection.
- **No credit history** — rural traders cannot access formal finance because they have no verifiable transaction history.

AquaLink is the infrastructure layer that changes that.

---

## What's Built

### 🐟 Fish Farmer Ledger
Register pond cycles, log feed costs, record harvests, and list produce for verified traders.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/farms` | Register a new pond cycle |
| GET | `/api/farms` | List all cycles for a farmer |
| GET | `/api/farms/{id}` | Retrieve a specific cycle |
| PATCH | `/api/farms/{id}/harvest` | Record a harvest against an active cycle |

### 🤝 Cooperative Savings
Digital group savings with append-only ledger, multi-signature withdrawals, and treasurer-controlled approvals.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/cooperatives` | Create a cooperative group |
| GET | `/api/cooperatives/{id}` | Get group details with live balance |
| POST | `/api/cooperatives/{id}/members` | Add a member |
| POST | `/api/cooperatives/{id}/contributions` | Record a monthly contribution |
| POST | `/api/cooperatives/{id}/withdrawals` | Request a withdrawal |
| PATCH | `/api/cooperatives/{id}/withdrawals/{wid}/approve` | Treasurer approves withdrawal |

### 📊 Price Intelligence
Daily fish price index with a Hangfire background job that fires at 6am every morning and sends SMS alerts to registered farmers via Termii.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/prices` | Submit a price entry (FieldAgent role) |
| GET | `/api/prices/current` | Today's prices by market and commodity |

### 🔐 Authentication
JWT-based authentication with role-based access control.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/token` | Generate a JWT token with role claims |

**Roles:** `Farmer` · `FieldAgent` · `Treasurer`

---

## Architecture

AquaLink follows **Clean Architecture** with strict dependency rules across 9 projects:

### Project Structure

aqualink-farmer/
├── AquaLink.Farmer.API/              # Controllers, middleware, auth, Program.cs
├── AquaLink.Farmer.Domain/           # Entities, value objects, domain logic
├── AquaLink.Farmer.Application/      # CQRS commands/queries, interfaces, validators
├── AquaLink.Farmer.Infrastructure/   # EF Core, PostgreSQL, repositories
├── AquaLink.Farmer.Tests/            # xUnit unit and integration tests
├── AquaLink.Cooperative.Domain/      # Cooperative entities and domain rules
├── AquaLink.Cooperative.Application/ # Cooperative commands, queries, interfaces
├── AquaLink.Cooperative.Infrastructure/ # Cooperative DbContext and persistence
├── AquaLink.Prices.Domain/           # PriceIndex and FarmerAlert entities
├── AquaLink.Prices.Application/      # Price commands, queries, SMS interface
└── AquaLink.Prices.Infrastructure/   # Termii SMS, Hangfire job, DbContext


### Key Design Decisions

- **Append-only ledger** — contribution and wallet records are never updated or deleted. Enforced at the database level.
- **Aggregate root pattern** — `CooperativeGroup` owns and protects Members, Contributions, and Withdrawals. No entity can be created outside the aggregate.
- **CQRS with MediatR** — every command and query is isolated, testable, and independent.
- **Interface segregation** — `IFarmerDbContext`, `ICooperativeDbContext`, `IPricesDbContext` keep Infrastructure swappable.
- **Domain validation** — business rules live in entities with private setters and factory methods. Invalid state is impossible.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8 |
| Architecture | Clean Architecture + CQRS |
| Mediator | MediatR 14 |
| Validation | FluentValidation |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL 16 |
| Background Jobs | Hangfire 1.8 |
| SMS | Termii API |
| Auth | JWT Bearer + ASP.NET Core RBAC |
| Containerisation | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| API Docs | Swagger / OpenAPI 3.0 |

---

## Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run locally

```bash
# Clone the repo
git clone https://github.com/sunday-oluwasegun-omotayo/aqualink-farmer.git
cd aqualink-farmer

# Start PostgreSQL
docker-compose up -d

# Run the API
dotnet run --project AquaLink.Farmer.API

# Open Swagger UI
# https://localhost:7231/swagger

# Open Hangfire Dashboard
# https://localhost:7231/hangfire
```

### Get an auth token

```bash
POST /api/auth/token
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "role": "Farmer"
}
```

---

## Database Schema

Seven domain tables across three modules:

FarmCycles          — pond cycles per farmer
Contributions       — cooperative contributions (append-only)
CooperativeGroups   — cooperative group registry
Members             — cooperative membership records
WithdrawalRequests  — withdrawal requests with approval status
PriceIndexes        — fish price entries by market and commodity
FarmerAlerts        — SMS alert audit log (one per farmer per day)

---

## Contributing

AquaLink is actively seeking contributors across all disciplines.

**Open roles:**
- 🔧 Backend Developer (.NET / C#)
- 📱 Mobile Developer (Flutter — offline-first Android)
- 🌐 Frontend Developer (React / TypeScript)
- 🎨 UI/UX Designer (Figma — low-literacy, rural users)
- ☁️ DevOps Engineer (Azure, Docker, GitHub Actions)
- 🤖 ML Engineer (Python — credit scoring, price prediction)

**To contribute:**
1. Read [CONTRIBUTING.md](./CONTRIBUTING.md) for your role-specific brief
2. Fork the repo and clone locally
3. Pick an open [GitHub Issue](../../issues) labelled with your discipline
4. Comment on the issue to claim it
5. Open a PR with a linked issue and at least one test

**Commit format:** `feat:` / `fix:` / `chore:` / `docs:` / `test:`

---

## Roadmap

| Phase | Timeline | Deliverables |
|-------|----------|--------------|
| ✅ Phase 1 | Complete | Backend API — 3 modules, 13 endpoints, CI pipeline |
| 🔄 Phase 2 | Month 2–4 | Flutter mobile app, React trader portal |
| ⬜ Phase 3 | Month 4–6 | Termii live SMS, Redis caching, field agent app |
| ⬜ Phase 4 | Month 6–9 | Wallet ledger, escrow, Flutterwave, KYC tiers |
| ⬜ Phase 5 | Month 9–12 | ML credit scoring, Azure deployment, beta pilot |
| ⬜ Phase 6 | Month 12+ | Lagos State partnership, grant applications, launch |

---

## Grant & Partnership Opportunities

AquaLink is positioned for:
- **Tony Elumelu Foundation** — African entrepreneurship
- **USAID Feed the Future** — aquaculture and food security
- **GSMA Innovation Fund** — mobile-first rural digital services
- **World Bank Nigeria Digital Economy Project**
- **CBN AGSMEIS** — agricultural SME financing aggregator
- **Lagos State MSME Fund**

---

## License

MIT — open source, free to use, contribute, and extend.

---

## Built By

**Sunday Oluwasegun Omotayo**
Lagos, Nigeria
[LinkedIn](https://www.linkedin.com/in/sunday-oluwasegun-omotayo-35397a368) · [GitHub](https://github.com/sunday-oluwasegun-omotayo)

> Built in public. Every commit documented. Every lesson shared. No shortcuts.