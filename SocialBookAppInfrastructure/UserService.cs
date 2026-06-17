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

    public User? GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public void UpdateFavouriteGenres(List<Genre> genres, int id)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        user.FavouriteGenres = genres;
        _context.SaveChanges();
    }

    public void UpdateDetails(User user, int id)
    {
        var existingUser = GetUserById(id);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }
        existingUser.UserName = user.UserName;
        existingUser.Password = user.Password;
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.PrivacyLevel = user.PrivacyLevel;
        _context.SaveChanges();
    }

    public void DeleteUser(int id)
    {
        var existingUser = GetUserById(id);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }
        _context.Users.Remove(existingUser);
        _context.SaveChanges();
    }

    public void CreateUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentException("User cannot be null");
        }
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void AddLanguage(string language, int id)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        user.Languages.Add(language);
        _context.SaveChanges();
    }
}
