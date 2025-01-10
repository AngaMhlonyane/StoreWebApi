# Store Web API

Store Web API is a .NET-based backend application for managing users, products, and checkout processes. It includes features such as adding, editing, deleting products, and completing checkouts with a clear summary. This implementation uses SQLite as the database provider.

## Features

- User registration with API keys.
- Product management: Add, edit, delete, and list products.
- Checkout system:
  - Start checkout with products and quantities.
  - Calculate total costs in ZAR.
  - Complete checkout with inventory adjustments.
  - Modify or delete items in an existing checkout.
  - Ensure only one active checkout per user.
  - Handle unavailable products and invalid requests gracefully.

## Prerequisites

- .NET 6 SDK or later
- SQLite installed (or use the built-in support with `Microsoft.EntityFrameworkCore.Sqlite`)
- Visual Studio or VS Code for development

## Configuration

### 1. Clone the Repository
```bash
git clone <repository-url>
cd StoreWebApi
```

### 2. Update the Connection String

Ensure the application uses SQLite as the database. Update the `appsettings.json` file:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=CheckoutAPI.db"
}
```

This will create or use the `CheckoutAPI.db` file in the project directory.

### 3. Install Dependencies

Ensure all necessary NuGet packages are installed:

```bash
dotnet restore
```
or here is a list of packages

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Swashbuckle.AspNetCore
dotnet add package FluentValidation.AspNetCore
dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

### 4. Apply Migrations

Run the following command to create the database schema:

```bash
dotnet ef database update
```
This will apply the migrations and create the necessary tables in `CheckoutAPI.db`.

### 5. Run the Application

## Error Handling

- **Invalid API Key:** Returns `401 Unauthorized`.
- **Unavailable Product:** Returns `400 Bad Request` with a meaningful message.
- **Invalid Quantity:** Returns `400 Bad Request`.
- **Multiple Checkouts:** Returns `400 Bad Request` if a user tries to start a new checkout while one is active.

## Development Notes

- Ensure SQLite is properly configured in the connection string.
- Use `DB Browser for SQLite` to view or modify the database during development.
- All API calls require a valid `apiKey`.

