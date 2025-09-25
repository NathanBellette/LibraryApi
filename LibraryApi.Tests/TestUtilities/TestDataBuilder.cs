using LibraryApi.Contracts.Requests;
using LibraryApi.Models;

namespace LibraryApi.Tests.TestUtilities;

public static class TestDataBuilder
{
    public static Book CreateBook(string name = "Test Book", string owner = "Test Owner", bool availability = true)
    {
        return new Book(Guid.NewGuid(), name, owner, availability);
    }

    public static CreateBookRequest CreateBookRequest(string name = "Test Book", string owner = "Test Owner", bool availability = true)
    {
        return new CreateBookRequest
        {
            Title = name,
            Owner = owner,
            Availability = availability
        };
    }

    public static PagedResult<Book> CreatePagedResult(List<Book> books, int page = 1, int pageSize = 10)
    {
        return new PagedResult<Book>
        {
            Items = books,
            Page = page,
            PageSize = pageSize,
            TotalCount = books.Count,
            TotalPages = (int)Math.Ceiling(books.Count / (double)pageSize)
        };
    }

    public static List<Book> CreateBookList(int count = 3)
    {
        var books = new List<Book>();
        for (int i = 0; i < count; i++)
        {
            books.Add(CreateBook($"Book {i + 1}", $"Owner {i + 1}", i % 2 == 0));
        }
        return books;
    }
}