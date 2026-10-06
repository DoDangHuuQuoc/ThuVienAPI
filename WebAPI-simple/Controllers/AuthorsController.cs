using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthorById([FromRoute] int id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var author = await _authorRepository.AddAuthorAsync(addAuthorRequestDTO);
            return Ok(author);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthorById([FromRoute] int id, [FromBody] AuthorNoIdDTO authorNoIdDTO)
        {
            var author = await _authorRepository.UpdateAuthorByIdAsync(id, authorNoIdDTO);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthorById([FromRoute] int id)
        {
            var deletedAuthor = await _authorRepository.DeleteAuthorByIdAsync(id);
            if (deletedAuthor == null)
            {
                return NotFound();
            }
            return Ok(deletedAuthor);
        }
    }
}