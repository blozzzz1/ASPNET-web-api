# User Management API

ASP.NET Core Web API для управления пользователями, товарами и заказами.

## Стек

- .NET 10 / ASP.NET Core
- CQRS + MediatR
- AutoMapper
- EF Core + SQLite
- JWT в HttpOnly cookie
- CORS
- FluentValidation
- Swagger (Swashbuckle)
- xUnit

## Быстрый старт

```bash
cd UserManagementApi
dotnet run
```

- API: http://localhost:5186  
- Swagger: http://localhost:5186/swagger  

### Seed-пользователь

| Поле | Значение |
|------|----------|
| Email | `ivan.petrov@example.com` |
| Password | `Password123!` |

## Auth

JWT сохраняется в HttpOnly cookie `access_token`.

| Метод | URL | Доступ |
|-------|-----|--------|
| POST | `/api/auth/register` | анонимно |
| POST | `/api/auth/login` | анонимно |
| POST | `/api/auth/logout` | авторизован |
| GET | `/api/auth/me` | авторизован |

Пример для фронтенда:

```js
fetch("http://localhost:5186/api/auth/login", {
  method: "POST",
  credentials: "include",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    email: "ivan.petrov@example.com",
    password: "Password123!"
  })
});
```

## Основные эндпоинты

Все ниже требуют авторизации (`[Authorize]`).

- `GET/POST /api/users`, `GET/PUT/DELETE /api/users/{id}`
- `GET/POST /api/products`, `GET/PUT/DELETE /api/products/{id}`
- `GET/POST /api/orders`, `GET/DELETE /api/orders/{id}`, `PATCH /api/orders/{id}/status`

## Структура

```
UserManagementApi/
  Controllers/       # API
  Application/       # CQRS commands/queries, DTOs, AutoMapper
  Domain/Entities/   # User, Product, Order, OrderItem
  Infrastructure/    # EF, Repositories, Auth, Middleware
UserManagementApi.Tests/
```

## Конфигурация

`appsettings.json`:

- `ConnectionStrings:DefaultConnection` — SQLite (`usermanagement.auth.db`)
- `Jwt` — issuer, audience, key, cookie name
- `Cors:AllowedOrigins` — origins с `AllowCredentials` (по умолчанию `3000`, `5173`, `4200`)

Окружение `Testing` использует InMemory БД (для тестов).

## Тесты

```bash
dotnet test
```

