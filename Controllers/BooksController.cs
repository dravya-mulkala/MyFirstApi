using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("api/[controller]")] // → api/books
public class BooksController : ControllerBase
{
    private readonly BooksContext _context;

    public BooksController(BooksContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll()
    {
        return await _context.Books.ToListAsync();
    }

    [HttpGet("{id:int}", Name = "GetBookById")] // GET api/books/1
    public async Task<ActionResult<Book>> GetById(int id)
    {
        var book = await _context.Books.FindAsync(id);
        return book is null ? NotFound() : book;
    }

    [HttpPost] // POST api/books
    public async Task<ActionResult<Book>> Create(Book input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
            return BadRequest("Title is required.");

        _context.Books.Add(input);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetBookById", new { id = input.BookId }, input);
    }

    [HttpPut("{id:int}")] // PUT api/books/1
    public async Task<IActionResult> Update(int id, Book input)
    {
        if (id != input.BookId)
            return BadRequest();

        _context.Entry(input).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Books.Any(b => b.BookId == id))
                return NotFound();
            throw;
        }

        return NoContent(); // 204
    }

    [HttpDelete("{id:int}")] // DELETE api/books/1
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
