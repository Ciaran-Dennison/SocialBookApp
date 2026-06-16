using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class BookService : IBookService
{
    private readonly IRatingService _ratingService;

    public BookService(IRatingService ratingService)
    {
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
