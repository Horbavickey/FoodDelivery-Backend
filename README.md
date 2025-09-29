# FoodDelivery – Backend

Third-year backend for a Food Delivery API.

**Stack**
- .NET 8, ASP.NET Core Web API
- EF Core + PostgreSQL
- Swagger/OpenAPI

**Run (local)**
1) `cd FoodDelivery.Api`
2) `dotnet run` → open `http://localhost:<port>/swagger`

**Database (later today)**
- Set connection string in `FoodDelivery.Api/appsettings.json`
- First migration: `dotnet ef migrations add InitialCreate -p FoodDelivery.Infrastructure -s FoodDelivery.Api`
- Apply: `dotnet ef database update -p FoodDelivery.Infrastructure -s FoodDelivery.Api`

**Scope**
- Auth (email login, registration returns JWT)
- Profile (name, birthdate, address, phone)
- Menu (filters: categories, veg-only; sorting: name/price/rating)
- Cart & Orders (checkout clears cart; confirm only when In Process)
- Ratings (only if user previously ordered the dish)
