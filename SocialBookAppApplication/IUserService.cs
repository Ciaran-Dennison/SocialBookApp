using SocialBookAppDomain;

namespace SocialBookAppApplication;

public interface IUserService
{
    public List<User> GetAllUsers();

    public User? GetUserById(int id);

    public void UpdateFavouriteGenres(List<Genre> genres, int id);

    public void UpdateDetails(User user, int id);

    public void DeleteUser(int id);

    public void CreateUser(User user);

    public void AddLanguage(string language, int id);

    public User? GetUserByUsername(string username);
}
