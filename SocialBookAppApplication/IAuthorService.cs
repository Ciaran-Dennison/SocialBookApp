

using SocialBookAppDomain;

namespace SocialBookAppApplication;

public interface IAuthorService : IRatingService
{
    public List<Author> GetAllAuthors();
    
}
