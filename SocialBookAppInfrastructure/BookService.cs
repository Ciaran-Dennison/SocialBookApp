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

    public Book GetBookById(int id)
    {
        throw new NotImplementedException();
    }
}
