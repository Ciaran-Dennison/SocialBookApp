using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class AuthorService : IAuthorService
{
    private readonly IRatingService _ratingService;

    public AuthorService(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }



    public double GetRatingPercentage(List<Review> reviews)
    {
        return _ratingService.GetRatingPercentage(reviews);
    }
}
