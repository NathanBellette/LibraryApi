using FluentAssertions;
using LibraryApi.Models;
using LibraryApi.Services;
using Xunit;

namespace LibraryApi.Tests.Services;

public class BookServiceTests
{
    private readonly BookService _service;

    public BookServiceTests()
    {
        _service = new BookService();
    }

    [Fact]
    public void Add_ShouldCreateBookWithUniqueId()
    {
        // Arrange
        var name = "Test Book";
        var owner = "Test Owner";
        var availability = true;

        // Act
        var result = _service.Add(name, owner, availability);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(name);
        result.Owner.Should().Be(owner);
        result.Availability.Should().Be(availability);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Add_ShouldCreateBooksWithDifferentIds()
    {
        // Arrange
        var book1 = _service.Add("Book 1", "Owner 1", true);
        var book2 = _service.Add("Book 2", "Owner 2", false);

        // Assert
        book1.Id.Should().NotBe(book2.Id);
    }

    [Fact]
    public void GetById_WithExistingId_ShouldReturnBook()
    {
        // Arrange
        var addedBook = _service.Add("Test Book", "Test Owner", true);

        // Act
        var result = _service.GetById(addedBook.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(addedBook);
    }

    [Fact]
    public void GetById_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = _service.GetById(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Delete_WithExistingId_ShouldReturnTrueAndRemoveBook()
    {
        // Arrange
        var addedBook = _service.Add("Test Book", "Test Owner", true);

        // Act
        var result = _service.Delete(addedBook.Id);

        // Assert
        result.Should().BeTrue();
        _service.GetById(addedBook.Id).Should().BeNull();
    }

    [Fact]
    public void Delete_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = _service.Delete(nonExistingId);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("clean", "Clean Code")]
    [InlineData("ROBERT", "Clean Code")]
    [InlineData("pragmatic", "The Pragmatic Programmer")]
    [InlineData("andy", "The Pragmatic Programmer")]
    [InlineData("design", "Design Patterns")]
    [InlineData("erich", "Design Patterns")]
    [InlineData("nonexistent", "")]
    public void Search_ShouldFindBooksByCaseInsensitiveNameOrOwner(string query, string expectedBookName)
    {
        // Act
        var results = _service.Search(query).ToList();

        // Assert
        if (string.IsNullOrEmpty(expectedBookName))
        {
            results.Should().BeEmpty();
        }
        else
        {
            results.Should().ContainSingle(b => b.Title == expectedBookName);
        }
    }

    [Fact]
    public void Search_WithEmptyQuery_ShouldReturnAllBooks()
    {
        // Act
        var results = _service.Search("").ToList();

        // Assert
        results.Should().HaveCount(3); // Seeded books
        results.Should().Contain(b => b.Title == "The Pragmatic Programmer");
        results.Should().Contain(b => b.Title == "Clean Code");
        results.Should().Contain(b => b.Title == "Design Patterns");
    }

    [Fact]
    public void Search_ShouldReturnResultsInOrder()
    {
        // Arrange
        _service.Add("Zebra Book", "Z Owner", true);
        _service.Add("Apple Book", "A Owner", true);

        // Act
        var results = _service.Search("").ToList();

        // Assert
        results.Should().BeInAscendingOrder(b => b.Title).And.ThenBeInAscendingOrder(b => b.Owner);
    }

    [Theory]
    [InlineData(1, 2, 2)]
    [InlineData(2, 2, 1)]
    [InlineData(1, 10, 3)]
    [InlineData(2, 10, 0)]
    public void GetPage_ShouldReturnCorrectPagination(int page, int pageSize, int expectedCount)
    {
        // Act
        var result = _service.GetPage(page, pageSize);

        // Assert
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
        result.Items.Should().HaveCount(expectedCount);
        result.TotalCount.Should().Be(3); // Seeded books
        result.TotalPages.Should().Be(1); // 3 books with pageSize >= 3
    }

    [Fact]
    public void GetPage_WithInvalidPage_ShouldDefaultToPage1()
    {
        // Act
        var result = _service.GetPage(0, 10);

        // Assert
        result.Page.Should().Be(1);
    }

    [Fact]
    public void GetPage_WithInvalidPageSize_ShouldDefaultToPageSize10()
    {
        // Act
        var result = _service.GetPage(1, 0);

        // Assert
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public void GetPage_WithLargePageSize_ShouldCapAt100()
    {
        // Act
        var result = _service.GetPage(1, 200);

        // Assert
        result.PageSize.Should().Be(100);
    }

    [Fact]
    public void GetPage_ShouldReturnResultsInOrder()
    {
        // Arrange
        _service.Add("Zebra Book", "Z Owner", true);
        _service.Add("Apple Book", "A Owner", true);

        // Act
        var result = _service.GetPage(1, 10);

        // Assert
        result.Items.Should().BeInAscendingOrder(b => b.Title).And.ThenBeInAscendingOrder(b => b.Owner);
    }

    [Fact]
    public void GetPage_ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange
        // Add 7 more books to have 10 total
        for (int i = 0; i < 7; i++)
        {
            _service.Add($"Book {i}", $"Owner {i}", true);
        }

        // Act
        var result = _service.GetPage(1, 3);

        // Assert
        result.TotalCount.Should().Be(10);
        result.TotalPages.Should().Be(4); // 10 books / 3 per page = 4 pages
    }
}
