
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialBookAppDomain;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<string> Languages { get; set; } = new List<string>();

    public string FavouriteGenresString { get; set; } = string.Empty;

    [NotMapped]
    public List<Genre> FavouriteGenres
    {
        get => FavouriteGenresString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => Enum.Parse<Genre>(g))
            .ToList();
        set => FavouriteGenresString = string.Join(",", value);
    }
    public PrivacyLevel PrivacyLevel { get; set; }
}
