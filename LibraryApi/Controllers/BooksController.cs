using LibraryApi.Models;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public ActionResult<PagedResult<Book>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 100) pageSize = 10;
        var result = bookService.GetPage(page, pageSize);
        return Ok(result);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<Book>> Search([FromQuery] string q = "")
    {
        var results = bookService.Search(q ?? string.Empty);
        return Ok(results);
    }

    [HttpPost]
    public ActionResult<Book> Create([FromBody] CreateBookRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Owner))
        {
            ModelState.AddModelError("name", "Name is required");
            ModelState.AddModelError("owner", "Owner is required");
            return ValidationProblem(ModelState);
        }
        var created = bookService.Add(request.Title.Trim(), request.Owner.Trim(), request.Availability);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<Book> GetById([FromRoute] Guid id)
    {
        var book = bookService.GetById(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var removed = bookService.Delete(id);
        return removed ? NoContent() : NotFound();
    }
}


