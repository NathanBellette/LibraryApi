using FluentAssertions;
using LibraryApi.Contracts.Requests;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace LibraryApi.Tests.Integration;

public class BooksIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BooksIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetBooks_ShouldReturnPaginatedResults()
    {
        // Act
        var response = await _client.GetAsync("/api/books?page=1&pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResult<Book>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().BeGreaterOrEqualTo(3); // At least seeded books
    }

    [Fact]
    public async Task SearchBooks_ShouldReturnMatchingResults()
    {
        // Act
        var response = await _client.GetAsync("/api/books/search?q=clean");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var books = JsonSerializer.Deserialize<List<Book>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        books.Should().NotBeNull();
        books!.Should().Contain(b => b.Title.Contains("Clean", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateBook_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Integration Test Book",
            Owner = "Integration Test Owner",
            Availability = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var book = JsonSerializer.Deserialize<Book>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        book.Should().NotBeNull();
        book!.Title.Should().Be(request.Title);
        book.Owner.Should().Be(request.Owner);
        book.Availability.Should().Be(request.Availability);
        book.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateBook_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "", // Invalid: empty name
            Owner = "Test Owner",
            Availability = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBook_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange - First create a book
        var createRequest = new CreateBookRequest
        {
            Title = "Book to Delete",
            Owner = "Delete Owner",
            Availability = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/books/{createdBook!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBook_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/books/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBookById_WithExistingId_ShouldReturnBook()
    {
        // Arrange - First create a book
        var createRequest = new CreateBookRequest
        {
            Title = "Book to Get",
            Owner = "Get Owner",
            Availability = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();

        // Act
        var response = await _client.GetAsync($"/api/books/{createdBook!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var book = JsonSerializer.Deserialize<Book>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        book.Should().NotBeNull();
        book!.Should().BeEquivalentTo(createdBook);
    }

    [Fact]
    public async Task GetBookById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/books/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
