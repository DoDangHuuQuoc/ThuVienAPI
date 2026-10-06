
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public List<BookWithAuthorAndPublisherDTO> GetAllBooks(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 1000)
        {
            var allBooks = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors)
                    .ThenInclude(ba => ba.Author)
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead
                        ? book.DateRead
                        : null,
                    Rate = book.IsRead
                        ? book.Rate
                        : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    DateAdded = book.DateAdded,

                    PublisherName = book.Publisher != null
                        ? book.Publisher.Name
                        : "Unknown",

                    AuthorNames = book.Book_Authors
                        .Select(n => n.Author!.FullName)
                        .ToList()
                })
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(filterOn) &&
                !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals(
                    "title",
                    StringComparison.OrdinalIgnoreCase))
                {
                    allBooks = allBooks.Where(x =>
                        x.Title!.Contains(filterQuery));
                }
            }


            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals(
                    "title",
                    StringComparison.OrdinalIgnoreCase))
                {
                    allBooks = isAscending
                        ? allBooks.OrderBy(x => x.Title)
                        : allBooks.OrderByDescending(x => x.Title);
                }
            }


            var skipResults =
                (pageNumber - 1) * pageSize;

            return allBooks
                .Skip(skipResults)
                .Take(pageSize)
                .ToList();
        }



        public BookWithAuthorAndPublisherDTO? GetBookById(int id)
        {
            var bookWithDomain = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.Id == id);

            var bookWithIdDTO = bookWithDomain
                .Select(book =>
                    new BookWithAuthorAndPublisherDTO()
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Description = book.Description,
                        IsRead = book.IsRead,
                        DateRead = book.DateRead,
                        Rate = book.Rate,
                        Genre = book.Genre,
                        CoverUrl = book.CoverUrl,
                        DateAdded = book.DateAdded,

                        PublisherName =
                            book.Publisher != null
                                ? book.Publisher.Name
                                : "Unknown",

                        AuthorNames = book.Book_Authors
                            .Select(n => n.Author!.FullName)
                            .ToList()
                    })
                .FirstOrDefault();

            return bookWithIdDTO;
        }



        public AddBookRequestDTO AddBook(
            AddBookRequestDTO addBookRequestDTO)
        {
            // Kiểm tra Publisher
            var publisherExists = _dbContext.Publishers
                .Any(p =>
                    p.Id == addBookRequestDTO.PublisherID);

            if (!publisherExists)
            {
                throw new ArgumentException(
                    $"Nhà xuất bản có ID {addBookRequestDTO.PublisherID} không tồn tại."
                );
            }

            // Lấy Author ID không trùng
            var authorIds = addBookRequestDTO.AuthorIds
                .Distinct()
                .ToList();

            // Kiểm tra Author
            var existingAuthorIds = _dbContext.Authors
                .Where(a => authorIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToList();

            var invalidAuthorIds = authorIds
                .Except(existingAuthorIds)
                .ToList();

            if (invalidAuthorIds.Any())
            {
                throw new ArgumentException(
                    $"Tác giả không tồn tại: {string.Join(", ", invalidAuthorIds)}."
                );
            }

            // Tạo Book
            var bookDomainModel = new Book
            {
                Title = addBookRequestDTO.Title
                    ?? string.Empty,

                Description = addBookRequestDTO.Description
                    ?? string.Empty,

                IsRead = addBookRequestDTO.IsRead,

                DateRead = addBookRequestDTO.DateRead,

                Rate = addBookRequestDTO.Rate,

                Genre = addBookRequestDTO.Genre
                    ?? string.Empty,

                CoverUrl = addBookRequestDTO.CoverUrl,

                DateAdded = addBookRequestDTO.DateAdded,

                PublisherID =
                    addBookRequestDTO.PublisherID
            };

            // Thêm Book
            _dbContext.Books.Add(bookDomainModel);

            // Lưu để lấy BookId
            _dbContext.SaveChanges();

            // Thêm Author
            foreach (var authorId in authorIds)
            {
                var bookAuthor = new Book_Author
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = authorId
                };

                _dbContext.Books_Authors
                    .Add(bookAuthor);
            }

            // Lưu Author
            _dbContext.SaveChanges();

            return addBookRequestDTO;
        }



        public AddBookRequestDTO? UpdateBookById(
            int id,
            AddBookRequestDTO bookDTO)
        {
            // Tìm Book
            var bookDomain = _dbContext.Books
                .FirstOrDefault(b => b.Id == id);

            if (bookDomain == null)
            {
                return null;
            }

            // Kiểm tra Publisher
            var publisherExists = _dbContext.Publishers
                .Any(p =>
                    p.Id == bookDTO.PublisherID);

            if (!publisherExists)
            {
                throw new ArgumentException(
                    $"Nhà xuất bản có ID {bookDTO.PublisherID} không tồn tại."
                );
            }

            // Lấy Author ID không trùng
            var authorIds = bookDTO.AuthorIds
                .Distinct()
                .ToList();

            // Kiểm tra Author
            var existingAuthorIds = _dbContext.Authors
                .Where(a => authorIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToList();

            var invalidAuthorIds = authorIds
                .Except(existingAuthorIds)
                .ToList();

            if (invalidAuthorIds.Any())
            {
                throw new ArgumentException(
                    $"Tác giả không tồn tại: {string.Join(", ", invalidAuthorIds)}."
                );
            }



            bookDomain.Title =
                bookDTO.Title ?? string.Empty;

            bookDomain.Description =
                bookDTO.Description ?? string.Empty;

            bookDomain.IsRead =
                bookDTO.IsRead;

            bookDomain.DateRead =
                bookDTO.DateRead;

            bookDomain.Rate =
                bookDTO.Rate;

            bookDomain.Genre =
                bookDTO.Genre ?? string.Empty;

            bookDomain.CoverUrl =
                bookDTO.CoverUrl;

            bookDomain.DateAdded =
                bookDTO.DateAdded;

            bookDomain.PublisherID =
                bookDTO.PublisherID;



            var oldAuthors = _dbContext.Books_Authors
                .Where(ba => ba.BookId == id)
                .ToList();

            _dbContext.Books_Authors
                .RemoveRange(oldAuthors);



            foreach (var authorId in authorIds)
            {
                var bookAuthor = new Book_Author
                {
                    BookId = id,
                    AuthorId = authorId
                };

                _dbContext.Books_Authors
                    .Add(bookAuthor);
            }

            // Lưu tất cả một lần
            _dbContext.SaveChanges();

            return bookDTO;
        }



        public Book? DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books
                .FirstOrDefault(b => b.Id == id);

            if (bookDomain == null)
            {
                return null;
            }

            // Xóa quan hệ Book - Author trước
            var bookAuthors = _dbContext.Books_Authors
                .Where(ba => ba.BookId == id)
                .ToList();

            _dbContext.Books_Authors
                .RemoveRange(bookAuthors);

            // Xóa Book
            _dbContext.Books.Remove(bookDomain);

            // Lưu
            _dbContext.SaveChanges();

            return bookDomain;
        }
    }
}

