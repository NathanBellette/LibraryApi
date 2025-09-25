# Library API

A RESTful ASP.NET Core Web API for managing a library's book collection. This API provides endpoints for searching, adding, deleting, and listing books with pagination support.

## Features

- **Search Books**: Find books by name or owner (case-insensitive)
- **Add Books**: Create new book entries with validation
- **Delete Books**: Remove books by ID
- **List Books**: Paginated book listing with configurable page size
- **In-Memory Storage**: Data persists during application lifetime
- **RESTful Design**: Standard HTTP methods and status codes
- **OpenAPI Documentation**: Auto-generated API documentation

## Book Model

Each book has the following properties:
- **Id**: Unique identifier (Guid)
- **Name**: Book title (required, max 200 characters)
- **Owner**: Book owner name (required, max 200 characters)
- **Availability**: Boolean indicating if book is available

## API Endpoints

### List Books (Paginated)
```
GET /api/books?page=1&pageSize=10
```
- **page**: Page number (default: 1)
- **pageSize**: Items per page (default: 10, max: 100)

### Search Books
```
GET /api/books/search?q=search_term
```
- **q**: Search query (searches both name and owner)

### Get Book by ID
```
GET /api/books/{id}
```

### Create Book
```
POST /api/books
Content-Type: application/json

{
  "name": "Book Title",
  "owner": "Owner Name",
  "availability": true
}
```

### Delete Book
```
DELETE /api/books/{id}
```

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022, VS Code, or any .NET-compatible IDE

## Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd LibraryApi
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Run the Application
```bash
dotnet run --project LibraryApi
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

### 4. View API Documentation
Once running, visit:
- **OpenAPI/Swagger**: `https://localhost:5001/openapi/v1.json` (Development only)

## Testing

This project includes comprehensive unit and integration tests using xUnit, Moq, and FluentAssertions.

### Run All Tests
```bash
dotnet test
```

### Run Tests with Verbose Output
```bash
dotnet test --verbosity normal
```

### Run Specific Test Project
```bash
dotnet test LibraryApi.Tests
```

### Run Tests by Category
```bash
# Unit tests only
dotnet test --filter Category=Unit

# Integration tests only
dotnet test --filter Category=Integration
```

### Test Coverage
The test suite includes:

#### Unit Tests
- **BookService Tests**: In-memory data operations
  - Add/delete books
  - Search functionality
  - Pagination logic
  - Thread safety

- **BooksController Tests**: API endpoint behavior
  - Request validation
  - Response formatting
  - Error handling
  - Mocked dependencies

#### Integration Tests
- **End-to-end API testing**: Full HTTP request/response cycle
- **Real application behavior**: Tests against running application
- **CRUD workflows**: Complete create, read, update, delete operations

## Project Structure

```
LibraryApi/
├── Controllers/
│   └── BooksController.cs          # REST API endpoints
├── Models/
│   ├── Book.cs                     # Domain model
│   └── PagedResult.cs             # Pagination wrapper
├── Services/
│   └── BookService.cs             # Business logic & data access
├── Contracts/
│   └── Requests/
│       └── CreateBookRequest.cs   # DTO for book creation
├── Program.cs                      # Application entry point
└── appsettings.json               # Configuration

LibraryApi.Tests/
├── Controllers/
│   └── BooksControllerTests.cs    # Controller unit tests
├── Services/
│   └── BookServiceTests.cs        # Service unit tests
├── Integration/
│   └── BooksIntegrationTests.cs   # End-to-end tests
└── TestUtilities/                 # Test helpers and utilities
```

## Example Usage

### Using curl

#### List books
```bash
curl "http://localhost:5000/api/books?page=1&pageSize=5"
```

#### Search books
```bash
curl "http://localhost:5000/api/books/search?q=clean"
```

#### Create a book
```bash
curl -X POST "http://localhost:5000/api/books" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Clean Architecture",
    "owner": "Robert Martin",
    "availability": true
  }'
```

#### Get specific book
```bash
curl "http://localhost:5000/api/books/{book-id}"
```

#### Delete a book
```bash
curl -X DELETE "http://localhost:5000/api/books/{book-id}"
```

### Using PowerShell

#### List books
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/books?page=1&pageSize=5"
```

#### Create a book
```powershell
$body = @{
    name = "Clean Architecture"
    owner = "Robert Martin"
    availability = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/books" -Method POST -Body $body -ContentType "application/json"
```

## Configuration

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` for OpenAPI documentation
- `ASPNETCORE_URLS`: Override default URLs (e.g., `http://localhost:8080`)

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Development

### Adding New Features
1. Create feature branch: `git checkout -b feature/new-feature`
2. Implement changes with tests
3. Run tests: `dotnet test`
4. Commit changes: `git commit -m "Add new feature"`
5. Push and create pull request

### Code Style
- Follow C# naming conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Maintain test coverage above 90%

## Troubleshooting

### Common Issues

#### Port Already in Use
```bash
# Find process using port 5000
lsof -i :5000

# Kill the process
kill -9 <PID>
```

#### Test Failures
```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```

#### Missing Dependencies
```bash
# Restore packages
dotnet restore

# Clear NuGet cache
dotnet nuget locals all --clear
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request
