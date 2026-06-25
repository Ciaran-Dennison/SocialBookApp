using Microsoft.EntityFrameworkCore;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class AuthorService : IAuthorService
{
    private readonly SocialBookAppContext _context;
    private readonly IRatingService _ratingService;

    public AuthorService(SocialBookAppContext context, IRatingService ratingService)
    {
        _context = context;
        _ratingService = ratingService;
    }

    public List<Author> GetAllAuthors()
    {
        return _context.Authors
            .Include(a => a.Books)
            .Include(a => a.Reviews)
            .ToList();
    }

    public Author GetAuthorById(int id)
    {
        var author = _context.Authors
            .Include(a => a.Books)
            .Include(a => a.Reviews)
            .FirstOrDefault(a => a.Id == id);
        if (author == null) throw new ArgumentException("Author not found.");
        return author;
    }

    public List<Book> GetAuthorBooks(int id)
    {
        var author = _context.Authors.FirstOrDefault(a => a.Id == id);
        if (author == null) throw new ArgumentException("Author not found.");
        return _context.Books.Where(b => b.Authors.Any(a => a.Id == id)).ToList();
    }

    public void AddAuthor(Author author)
    {
        _context.Authors.Add(author);
        _context.SaveChanges();
    }

    public void AssignBookToAuthor(int authorId, int bookId)
    {
        var author = _context.Authors
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == authorId);
        if (author == null) throw new ArgumentException("Author not found.");

        var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
        if (book == null) throw new ArgumentException("Book not found.");

        author.Books.Add(book);
        _context.SaveChanges();
    }

    public double GetRatingPercentage(List<Review> reviews)
    {
        return _ratingService.GetRatingPercentage(reviews);
    }
}
