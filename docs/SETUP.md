# Setup Instructions

## Prerequisites
- .NET 8 SDK
- SQL Server
- Node.js 22+

## Backend
1. Set environment variables:
   - `ConnectionStrings__DefaultConnection`
   - `Jwt__Key`
   - `Jwt__Issuer`
   - `Jwt__Audience`
   - `Gemini__ApiKey`
2. Restore and run:
   ```bash
   dotnet restore src/backend/HouseholdInventory.sln
   dotnet ef database update --project src/backend/HouseholdInventory.Infrastructure --startup-project src/backend/HouseholdInventory.Api
   dotnet run --project src/backend/HouseholdInventory.Api
   ```

## Frontend
```bash
cd src/frontend
npm install
npm run dev
```

## Seeded users
- Admin: `admin@household.local` / `Admin123!`
- Roommate: `roommate@household.local` / `Roommate123!`
