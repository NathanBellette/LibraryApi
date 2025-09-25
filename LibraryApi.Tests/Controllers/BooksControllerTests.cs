using FluentAssertions;
using LibraryApi.Contracts.Requests;
using LibraryApi.Controllers;
using LibraryApi.Models;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LibraryApi.Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new BooksController(_mockBookService.Object);
    }

    [Fact]
    public void List_WithValidParameters_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var expectedResult = new PagedResult<Book>
        {
            Items = new List<Book> { new Book(Guid.NewGuid(), "Test Book", "Test Owner", true) },
            Page = page,
            PageSize = pageSize,
            TotalCount = 1,
            TotalPages = 1
        };

        _mockBookService.Setup(s => s.GetPage(page, pageSize)).Returns(expectedResult);

        // Act
        var result = _controller.List(page, pageSize);

        // Assert
        result.Should().BeOfType<ActionResult<PagedResult<Book>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        _mockBookService.Verify(s => s.GetPage(page, pageSize), Times.Once);
    }

    [Theory]
    [InlineData(0, 10, 1, 10)]
    [InlineData(-1, 5, 1, 5)]
    [InlineData(2, 0, 2, 10)]
    [InlineData(1, 150, 1, 100)]
    public void List_WithInvalidParameters_ShouldNormalizeAndCallService(int inputPage, int inputPageSize, int expectedPage, int expectedPageSize)
    {
        // Arrange
        var expectedResult = new PagedResult<Book>
        {
            Items = new List<Book>(),
            Page = expectedPage,
            PageSize = expectedPageSize,
            TotalCount = 0,
            TotalPages = 0
        };

        _mockBookService.Setup(s => s.GetPage(expectedPage, expectedPageSize)).Returns(expectedResult);

        // Act
        _controller.List(inputPage, inputPageSize);

        // Assert
        _mockBookService.Verify(s => s.GetPage(expectedPage, expectedPageSize), Times.Once);
    }

    [Fact]
    public void Search_WithQuery_ShouldReturnOkWithSearchResults()
    {
        // Arrange
        var query = "test";
        var expectedBooks = new List<Book>
        {
            new Book(Guid.NewGuid(), "Test Book 1", "Owner 1", true),
            new Book(Guid.NewGuid(), "Test Book 2", "Owner 2", false)
        };

        _mockBookService.Setup(s => s.Search(query)).Returns(expectedBooks);

        // Act
        var result = _controller.Search(query);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<Book>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedBooks);
        _mockBookService.Verify(s => s.Search(query), Times.Once);
    }

    [Fact]
    public void Search_WithEmptyQuery_ShouldCallServiceWithEmptyString()
    {
        // Arrange
        var expectedBooks = new List<Book>();
        _mockBookService.Setup(s => s.Search("")).Returns(expectedBooks);

        // Act
        _controller.Search("");

        // Assert
        _mockBookService.Verify(s => s.Search(""), Times.Once);
    }

    [Fact]
    public void Search_WithNullQuery_ShouldCallServiceWithEmptyString()
    {
        // Arrange
        var expectedBooks = new List<Book>();
        _mockBookService.Setup(s => s.Search("")).Returns(expectedBooks);

        // Act
        _controller.Search(null!);

        // Assert
        _mockBookService.Verify(s => s.Search(""), Times.Once);
    }

    [Fact]
    public void Create_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Test Book",
            Owner = "Test Owner",
            Availability = true
        };

        var createdBook = new Book(Guid.NewGuid(), request.Title, request.Owner, request.Availability);
        _mockBookService.Setup(s => s.Add(request.Title, request.Owner, request.Availability))
                       .Returns(createdBook);

        // Act
        var result = _controller.Create(request);

        // Assert
        result.Should().BeOfType<ActionResult<Book>>();
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(BooksController.GetById));
        createdAtActionResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdBook.Id);
        createdAtActionResult.Value.Should().BeEquivalentTo(createdBook);
        _mockBookService.Verify(s => s.Add(request.Title, request.Owner, request.Availability), Times.Once);
    }

    [Theory]
    [InlineData("", "Owner")]
    [InlineData("   ", "Owner")]
    [InlineData(null!, "Owner")]
    [InlineData("Name", "")]
    [InlineData("Name", "   ")]
    [InlineData("Name", null!)]
    public void Create_WithInvalidRequest_ShouldReturnValidationProblem(string name, string owner)
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = name,
            Owner = owner,
            Availability = true
        };

        // Act
        var result = _controller.Create(request);

        // Assert
        result.Should().BeOfType<ActionResult<Book>>();
        var validationProblemResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        validationProblemResult.StatusCode.Should().Be(400);
        _mockBookService.Verify(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public void Create_ShouldTrimNameAndOwner()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "  Test Book  ",
            Owner = "  Test Owner  ",
            Availability = true
        };

        var createdBook = new Book(Guid.NewGuid(), "Test Book", "Test Owner", true);
        _mockBookService.Setup(s => s.Add("Test Book", "Test Owner", true))
                       .Returns(createdBook);

        // Act
        _controller.Create(request);

        // Assert
        _mockBookService.Verify(s => s.Add("Test Book", "Test Owner", true), Times.Once);
    }

    [Fact]
    public void GetById_WithExistingId_ShouldReturnOkWithBook()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedBook = new Book(id, "Test Book", "Test Owner", true);
        _mockBookService.Setup(s => s.GetById(id)).Returns(expectedBook);

        // Act
        var result = _controller.GetById(id);

        // Assert
        result.Should().BeOfType<ActionResult<Book>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedBook);
        _mockBookService.Verify(s => s.GetById(id), Times.Once);
    }

    [Fact]
    public void GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockBookService.Setup(s => s.GetById(id)).Returns((Book?)null);

        // Act
        var result = _controller.GetById(id);

        // Assert
        result.Should().BeOfType<ActionResult<Book>>();
        result.Result.Should().BeOfType<NotFoundResult>();
        _mockBookService.Verify(s => s.GetById(id), Times.Once);
    }

    [Fact]
    public void Delete_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockBookService.Setup(s => s.Delete(id)).Returns(true);

        // Act
        var result = _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockBookService.Verify(s => s.Delete(id), Times.Once);
    }

    [Fact]
    public void Delete_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockBookService.Setup(s => s.Delete(id)).Returns(false);

        // Act
        var result = _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockBookService.Verify(s => s.Delete(id), Times.Once);
    }
}
