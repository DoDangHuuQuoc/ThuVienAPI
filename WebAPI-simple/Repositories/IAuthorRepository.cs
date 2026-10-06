using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<Author> AddAuthorAsync(AddAuthorRequestDTO addAuthorRequestDTO);
        Task<Author?> UpdateAuthorByIdAsync(int id, AuthorNoIdDTO authorNoIdDTO);
        Task<Author?> DeleteAuthorByIdAsync(int id);
    }
}