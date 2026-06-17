using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class UserService : IUserService
{
    private readonly SocialBookAppContext _context;

    public UserService(SocialBookAppContext context)
    {
        _context = context;
    }

    public User GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateFavouriteGenre(List<Genre> genres, int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateDetails(User user, int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public void CreateUser(User user)
    {
        throw new NotImplementedException();
    }

    public void AddLanguage(string language, int id)
    {
        throw new NotImplementedException();
    }
}
