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
        return _context.Authors.ToList();
    }

    public double GetRatingPercentage(List<Review> reviews)
    {
        return _ratingService.GetRatingPercentage(reviews);
    }
}
