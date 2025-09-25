using LibraryApi.Models;

namespace LibraryApi.Services;

public interface IBookService
{
    Book Add(string title, string owner, bool availability);
    bool Delete(Guid id);
    Book? GetById(Guid id);
    IEnumerable<Book> Search(string query);
    PagedResult<Book> GetPage(int page, int pageSize);
}

public class BookService : IBookService
{
    private readonly List<Book> _books = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public BookService()
    {
        // Seed sample data
        Add("The Pragmatic Programmer", "Andy", true);
        Add("Clean Code", "Robert", false);
        Add("Design Patterns", "Erich", true);
    }

    public Book Add(string title, string owner, bool availability)
    {
        var book = new Book(Guid.NewGuid(), title, owner, availability);
        _lock.EnterWriteLock();
        try
        {
            _books.Add(book);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
        return book;
    }

    public bool Delete(Guid id)
    {
        _lock.EnterWriteLock();
        try
        {
            var index = _books.FindIndex(b => b.Id == id);
            if (index >= 0)
            {
                _books.RemoveAt(index);
                return true;
            }
            return false;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public Book? GetById(Guid id)
    {
        _lock.EnterReadLock();
        try
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public IEnumerable<Book> Search(string query)
    {
        var normalized = (query ?? string.Empty).Trim().ToLowerInvariant();
        _lock.EnterReadLock();
        try
        {
            return _books
                .Where(b => b.Title.ToLowerInvariant().Contains(normalized) || b.Owner.ToLowerInvariant().Contains(normalized))
                .OrderBy(b => b.Title)
                .ThenBy(b => b.Owner)
                .ToList();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public PagedResult<Book> GetPage(int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 100) pageSize = 10;

        _lock.EnterReadLock();
        try
        {
            var total = _books.Count;
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            var items = _books
                .OrderBy(b => b.Title)
                .ThenBy(b => b.Owner)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedResult<Book>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = totalPages
            };
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}


