using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await _dbContext.Authors.ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Author> AddAuthorAsync(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorDomain = new Author
            {
                FullName = addAuthorRequestDTO.FullName
            };
            await _dbContext.Authors.AddAsync(authorDomain);
            await _dbContext.SaveChangesAsync();
            return authorDomain;
        }

        public async Task<Author?> UpdateAuthorByIdAsync(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var existingAuthor = await _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (existingAuthor == null) return null;

            existingAuthor.FullName = authorNoIdDTO.FullName;
            await _dbContext.SaveChangesAsync();
            return existingAuthor;
        }

        public async Task<Author?> DeleteAuthorByIdAsync(int id)
        {
            var existingAuthor = await _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (existingAuthor == null) return null;

            _dbContext.Authors.Remove(existingAuthor);
            await _dbContext.SaveChangesAsync();
            return existingAuthor;
        }
    }
}