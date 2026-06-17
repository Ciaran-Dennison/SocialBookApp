using SocialBookAppDomain;

namespace SocialBookAppApplication;

public interface IBookService : IRatingService
{
    public Book? GetBookById(int id);
}
