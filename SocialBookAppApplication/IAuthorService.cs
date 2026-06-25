

using SocialBookAppDomain;

namespace SocialBookAppApplication;

public interface IAuthorService : IRatingService
{
    public List<Author> GetAllAuthors();

    public Author GetAuthorById(int id);

    public List<Book> GetAuthorBooks(int id);

    public void AddAuthor(Author author);
    public void AssignBookToAuthor(int authorId, int bookId);
    public void UnassignBookFromAuthor(int authorId, int bookId);
    public void UpdateAuthor(int id, Author author);
}
