using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class BookService : IBookService
{
    private readonly SocialBookAppContext _context;
    private readonly IRatingService _ratingService;

    public BookService(SocialBookAppContext context, IRatingService ratingService)
    {
        _context = context;
        _ratingService = ratingService;
    }

    public double GetRatingPercentage(List<Review> reviews)
    {
        return _ratingService.GetRatingPercentage(reviews);
    }

    public List<Book> GetAllBooks()
    {
        return _context.Books.ToList();
    }

    public Book? GetBookById(int id)
    {
        return _context.Books.FirstOrDefault(b => b.Id == id);
    }

    public void AddBook(Book book)
    {
        _context.Books.Add(book);
        _context.SaveChanges();
    }

    public void DeleteBook(int id)
    {
        var book = GetBookById(id);
        if (book == null) throw new ArgumentException("Book not found.");
        _context.Books.Remove(book);
        _context.SaveChanges();
    }

    public List<Book> GetUserBooks(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) throw new ArgumentException("User not found.");
        return _context.Books.Where(b => b.Reviews.Any(r => r.User.Id == userId)).ToList();
    }

    public void AddReview(int bookId, int userId, Review review)
    {
        var book = GetBookById(bookId);
        if (book == null) throw new ArgumentException("Book not found.");
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) throw new ArgumentException("User not found.");
        review.User = user;
        review.Book = book;
        _context.Reviews.Add(review);
        _context.SaveChanges();
    }
}
