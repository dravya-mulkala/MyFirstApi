using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishersController : ControllerBase
{
    private readonly BooksContext _context;

    public PublishersController(BooksContext context)
    {
        _context = context;
    }

    // 🔓 Any authenticated user can view all publishers
    [Authorize]
    [HttpGet] // GET api/publishers
    public async Task<ActionResult<IEnumerable<Publisher>>> GetAll()
    {
        return await _context.Publishers.ToListAsync();
    }

    // 🔓 Any authenticated user can view a publisher by id
    [Authorize]
    [HttpGet("{id:int}", Name = "GetPublisherById")] // GET api/publishers/1
    public async Task<ActionResult<Publisher>> GetById(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        return publisher is null ? NotFound() : publisher;
    }

    // 🔒 Only Admins can create new publishers
    [Authorize(Roles = "Admin")]
    [HttpPost] // POST api/publishers
    public async Task<ActionResult<Publisher>> Create([FromBody] Publisher input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return BadRequest("Publisher name is required.");
        }

        _context.Publishers.Add(input);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetPublisherById", new { id = input.PublisherId }, input);
    }

    // 🔒 Only Admins can update an existing publisher
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")] // PUT api/publishers/1
    public async Task<IActionResult> Update(int id, [FromBody] Publisher input)
    {
        if (id != input.PublisherId)
        {
            return BadRequest("Publisher ID mismatch.");
        }

        _context.Entry(input).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Publishers.Any(e => e.PublisherId == id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // 🔒 Only Admins can delete a publisher
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")] // DELETE api/publishers/1
    public async Task<IActionResult> Delete(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher is null) return NotFound();

        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
