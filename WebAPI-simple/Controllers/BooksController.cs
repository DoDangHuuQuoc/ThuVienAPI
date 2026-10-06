using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        // 1. Lấy danh sách Sách (Filter, Sort, Pagination)
        [HttpGet("get-all-books")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            _logger.LogInformation("GetAll Book Action method was invoked");
            var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            _logger.LogInformation($"Finished GetAllBook request with data {JsonSerializer.Serialize(allBooks)}");
            return Ok(allBooks);
        }

        // 2. Lấy thông tin Sách theo ID
        [HttpGet("get-book-by-id/{id:int}")]
        [Authorize(Roles = "Read")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        // 3. Thêm Sách mới
        [HttpPost("add-book")]
        [Authorize(Roles = "Write")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = _bookRepository.AddBook(addBookRequestDTO);
            return Ok(book);
        }

        // 4. Cập nhật Sách theo ID
        [HttpPut("update-book-by-id/{id:int}")]
        [Authorize(Roles = "Write")]
        public IActionResult UpdateBookById([FromRoute] int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBook = _bookRepository.UpdateBookById(id, bookDTO);
            if (updatedBook == null)
            {
                return NotFound();
            }
            return Ok(updatedBook);
        }

        // 5. Xóa Sách theo ID
        [HttpDelete("delete-book-by-id/{id:int}")]
        [Authorize(Roles = "Write")]
        public IActionResult DeleteBookById([FromRoute] int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            if (deleteBook == null)
            {
                return NotFound();
            }
            return Ok(deleteBook);
        }
    }
}