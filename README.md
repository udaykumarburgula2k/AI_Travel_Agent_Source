# AI Travel Agent

ASP.NET Core Web API + React frontend for AI-powered travel itinerary generation.

## Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server
- OpenAI API key

## Database
Create an empty database named `AI_Travel_Agent`, then run the SQL script from `database/create_tables.sql`.

## Backend
```bash
cd backend/TravelAssistant.Api

dotnet restore

dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_KEY"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=AI_Travel_Agent;Trusted_Connection=True;TrustServerCertificate=True"

dotnet run
```

API Swagger:
`https://localhost:7001/swagger` or check terminal URL.

## Frontend
```bash
cd frontend/travel-assistant-client
npm install
npm run dev
```

Open:
`http://localhost:5173`

If your backend URL is different, update `frontend/travel-assistant-client/src/api.js`.
