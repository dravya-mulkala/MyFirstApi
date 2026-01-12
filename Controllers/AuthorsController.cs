using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly BooksContext _context;

        public AuthorsController(BooksContext context)
        {
            _context = context;
        }

        // GET api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAll()
        {
            return await _context.Authors.ToListAsync();
        }

        // GET api/authors/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            return author is null ? NotFound() : author;
        }

        // POST api/authors
        [HttpPost]
        public async Task<ActionResult<Author>> Create(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        // PUT api/authors/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Author author)
        {
            if (id != author.Id) return BadRequest();

            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/authors/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author is null) return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
