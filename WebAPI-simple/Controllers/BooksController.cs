
using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.CustomActionFilter;
using WebAPI_simple.Data;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;

        public BooksController(
            AppDbContext dbContext,
            IBookRepository bookRepository)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
        }

        // =========================
        // GET ALL BOOKS
        // =========================
        [HttpGet("get-all-books")]
        public IActionResult GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool isAscending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            var allBooks = _bookRepository.GetAllBooks(
                filterOn,
                filterQuery,
                sortBy,
                isAscending,
                pageNumber,
                pageSize);

            return Ok(allBooks);
        }

        // =========================
        // GET BOOK BY ID
        // =========================
        [HttpGet("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);

            if (bookWithIdDTO == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sách"
                });
            }

            return Ok(bookWithIdDTO);
        }

        // =========================
        // POST - ADD BOOK
        // =========================
        [HttpPost("add-book")]
        public IActionResult AddBook(
            [FromBody] AddBookRequestDTO bookDTO)
        {
            try
            {
                if (!ValidateAddBook(bookDTO))
                {
                    return BadRequest(ModelState);
                }

                var book = _bookRepository.AddBook(bookDTO);

                return Ok(book);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================
        // PUT - UPDATE BOOK
        // =========================
        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(
            [FromRoute] int id,
            [FromBody] AddBookRequestDTO bookDTO)
        {
            try
            {
                if (!ValidateAddBook(bookDTO))
                {
                    return BadRequest(ModelState);
                }

                var updateBook = _bookRepository.UpdateBookById(
                    id,
                    bookDTO);

                if (updateBook == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy sách"
                    });
                }

                return Ok(updateBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================
        // DELETE - DELETE BOOK
        // =========================
        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(
            [FromRoute] int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);

            if (deleteBook == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sách"
                });
            }

            return Ok(deleteBook);
        }

        // =========================
        // VALIDATE ADD / UPDATE
        // =========================
        #region Private methods

        private bool ValidateAddBook(
            AddBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO),
                    "Please add book data");

                return false;
            }

            // Kiểm tra Description
            if (string.IsNullOrEmpty(
                addBookRequestDTO.Description))
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.Description),
                    $"{nameof(addBookRequestDTO.Description)} cannot be null");
            }

            // Kiểm tra Rate từ 0 đến 5
            if (addBookRequestDTO.Rate < 0 ||
                addBookRequestDTO.Rate > 5)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.Rate),
                    $"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}

