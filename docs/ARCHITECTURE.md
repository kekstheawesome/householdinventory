# Architecture Overview

## Clean Architecture
- **Domain**: entities, enums, and base abstractions.
- **Application**: DTOs and service contracts.
- **Infrastructure**: EF Core, Identity, JWT, seeding, Gemini integration.
- **API**: controllers, auth, authorization policies, Swagger.
- **Frontend**: React + TypeScript dashboard consuming the API.

## Auth and Roles
- ASP.NET Core Identity manages users and password policies.
- JWT bearer tokens are issued by the backend.
- Roles: `Admin`, `Roommate`.
- Authorization policies:
  - `AdminOnly`: user/role/settings/audit management.
  - `HouseholdMember`: standard household access.
- OAuth2/OIDC extensibility is preserved by centralizing auth at Identity + JWT issuance boundaries.

## Database Schema
- `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles` from Identity.
- `InventoryItems`
  - `Id`, `Name`, `Description`, `Category`, `Quantity`, `Unit`, `MinimumStockThreshold`, `ExpiryDateUtc`, `Location`, `IsDeleted`, timestamps.
- `ShoppingListItems`
  - `Id`, `Name`, `Quantity`, `Unit`, `Notes`, `IsCompleted`, `InventoryItemId`, timestamps.
- `AuditLogs`
  - `Id`, `Action`, `EntityName`, `EntityId`, `Category`, `UserId`, `UserEmail`, `OldValuesJson`, `NewValuesJson`, `Summary`, `TimestampUtc`.

## Gemini Flow
1. User asks question via frontend.
2. Backend loads live SQL context from inventory, shopping, and audit tables.
3. Backend serializes a constrained context object.
4. Backend sends prompt + structured context to Gemini.
5. Frontend receives answer from backend only; Gemini credentials never leave the server.

## API Endpoints
- `POST /api/auth/login`
- `POST /api/auth/users`
- `GET|POST|PUT|DELETE /api/inventory`
- `POST /api/inventory/{id}/restock`
- `POST /api/inventory/{id}/consume`
- `GET /api/dashboard`
- `GET /api/audit`
- `GET|POST /api/shoppinglist`
- `POST /api/shoppinglist/{id}/complete`
- `GET /api/users`
- `POST /api/chat`
