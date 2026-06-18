using SocialBookAppDomain;

namespace SocialBookAppApplication;

public interface IBookService : IRatingService
{

    public List<Book> GetAllBooks();
    public Book? GetBookById(int id);

    public void AddBook(Book book);

    public void DeleteBook(int id);

    public List<Book> GetUserBooks(int userId);

    public void AddReview(int bookId, int userId, Review review);
}
