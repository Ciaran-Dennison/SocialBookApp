using Microsoft.EntityFrameworkCore;
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
        return _context.Books
            .Include(b => b.Reviews)
            .ToList();
    }

    public Book GetBookById(int id)
    {
        var book = _context.Books
            .Include(b => b.Reviews)
            .FirstOrDefault(b => b.Id == id);
        if (book == null) throw new ArgumentException("Book not found.");
        return book;
    }

    public void AddBook(Book book)
    {
        _context.Books.Add(book);
        _context.SaveChanges();
    }

    public void DeleteBook(int id)
    {
        var book = _context.Books
            .Include(b => b.Reviews)
            .FirstOrDefault(b => b.Id == id);
        if (book == null) throw new ArgumentException("Book not found.");

        _context.Reviews.RemoveRange(book.Reviews);
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
        review.CreatedDate = DateTime.Now;
        _context.Reviews.Add(review);
        _context.SaveChanges();
    }

    public void UpdateBook(int id, Book book)
    {
        var existing = _context.Books.FirstOrDefault(b => b.Id == id);
        if (existing == null) throw new ArgumentException("Book not found.");
        existing.Title = book.Title;
        existing.Blurb = book.Blurb;
        existing.Language = book.Language;
        existing.Genre = book.Genre;
        existing.Format = book.Format;
        existing.IsChildFriendly = book.IsChildFriendly;
        existing.Pages = book.Pages;
        existing.Chapters = book.Chapters;
        existing.Published = book.Published;
        _context.SaveChanges();
    }
}
