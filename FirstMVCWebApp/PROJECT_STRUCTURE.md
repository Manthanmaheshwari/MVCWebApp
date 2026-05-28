# FirstMVCWebApp Project Structure

## Project Organization

```
FirstMVCWebApp/
├── Controllers/          # MVC Controllers
│   └── AuthController.cs                    # Authentication related endpoints
│
├── Dto/                 # Data Transfer Objects
│   └── UserDTO.cs                          # DTO for user registration/login operations
│
├── Models/              # Domain Models
│   ├── User.cs                             # Database User entity model
│   └── ErrorViewModel.cs                   # Error handling view model
│
├── Data/                # Data Access Layer
│   └── AppDbContext.cs                     # Entity Framework DbContext
│
├── Migrations/          # EF Core Migrations
│   ├── 20260523051502_dbinit.cs
│   ├── 20260523051502_dbinit.Designer.cs
│   └── AppDbContextModelSnapshot.cs
│
├── Views/               # Razor Views
│   ├── Auth/
│   │   ├── Login.cshtml                    # Login view
│   │   └── Register.cshtml                 # Registration view (uses UserDTO)
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _ValidationScriptsPartial.cshtml
│       └── Error.cshtml
│
├── wwwroot/             # Static Files
│   ├── css/
│   ├── js/
│   └── lib/              # Client libraries (Bootstrap, jQuery, etc.)
│
├── Properties/          # Project Properties
│   └── launchSettings.json
│
├── Program.cs           # Application entry point & service configuration
├── appsettings.json     # Configuration
├── appsettings.Development.json
└── FirstMVCWebApp.csproj

```

## Folder Descriptions

### Controllers
Handles HTTP requests and returns responses. Contains action methods for authentication operations.

### Dto (Data Transfer Objects)
Contains classes used to transfer data between different layers:
- **UserDTO**: Used for registration/login operations (doesn't include database IDs)

### Models
Domain/Entity models that represent database tables and business entities:
- **User**: Represents the User table in the database
- **ErrorViewModel**: View model for error handling

### Data
Data access layer containing Entity Framework DbContext:
- **AppDbContext**: Manages database connections and entity configurations

### Views
Razor views for the presentation layer:
- Views are organized by controller (Auth folder for AuthController)
- Uses DTOs (UserDTO) for strongly-typed views

## Key Architecture Points

1. **Separation of Concerns**: DTOs are separate from domain models
2. **Models vs DTOs**:
   - Models: Database entities with IDs and additional properties
   - DTOs: Transfer objects without sensitive information
3. **Views Reference DTOs**: Register.cshtml uses `@using FirstMVCWebApp.Dto`

## Dependencies
- .NET 10
- Entity Framework Core
- ASP.NET Core MVC
- Bootstrap (UI Framework)
- jQuery & jQuery Validation

