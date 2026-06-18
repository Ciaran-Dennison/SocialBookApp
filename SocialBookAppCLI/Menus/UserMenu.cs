using InquirerCore;
using InquirerCore.Prompts;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppCLI.Menus;

public class UserMenu
{
    private readonly IUserService _userService;
    
    public UserMenu(IUserService userService)
    {
        _userService = userService;
    }

    public void Show()
    {
        while (true)
        { 
            Console.Clear();
            var userOptions = new string[] { "Create User", "Update User Details", "Delete User", "Get User", "Update Favourite Genres", "Add Language", "List All Users", "Back" };
            var userMenu = new ListInput("userMenu", "User Management", userOptions);
            var inquirer = new Inquirer(userMenu);
            inquirer.Ask();

            switch (userMenu.Answer())
            {
                case "1":
                    var usernameInput = new Input("username", "Enter username:");
                    var firstNameInput = new Input("firstName", "Enter first name:");
                    var lastNameInput = new Input("lastName", "Enter last name:");
                    var emailInput = new Input("email", "Enter email:");
                    var passwordInput = new Input("password", "Enter password:");
                    var privacyLevelInput = new ListInput("privacyLevel", "Select privacy level:", Enum.GetNames(typeof(PrivacyLevel)));

                    new Inquirer(usernameInput, firstNameInput, lastNameInput, emailInput, passwordInput, privacyLevelInput).Ask();

                    DateTime dob = DateTime.MinValue;
                    bool validDob = false;

                    while (!validDob)
                    {
                        var dobInput = new Input("dob", "Enter date of birth (dd/MM/yyyy):");
                        new Inquirer(dobInput).Ask();

                        if (DateTime.TryParseExact(dobInput.Answer(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dob))
                        {
                            validDob = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format. Please use dd/MM/yyyy.");
                        }
                    }

                    var newUser = new User
                    {
                        UserName = usernameInput.Answer(),
                        FirstName = firstNameInput.Answer(),
                        LastName = lastNameInput.Answer(),
                        Email = emailInput.Answer(),
                        Password = passwordInput.Answer(),
                        CreatedDate = DateTime.Now,
                        DateOfBirth = dob,
                        PrivacyLevel = (PrivacyLevel)(int.Parse(privacyLevelInput.Answer()) - 1)
                    };

                    try
                    {
                        _userService.CreateUser(newUser);
                        Console.WriteLine("User created successfully!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error creating user: {ex.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    break;

                case "2":
                    var updateIdInput = new Input("id", "Enter user ID to update:");
                    var newUsernameInput = new Input("username", "Enter new username:");
                    var newFirstNameInput = new Input("firstName", "Enter new first name:");
                    var newLastNameInput = new Input("lastName", "Enter new last name:");
                    var newEmailInput = new Input("email", "Enter new email:");
                    var newPasswordInput = new Input("password", "Enter new password:");
                    var newPrivacyLevelInput = new ListInput("privacyLevel", "Select new privacy level:", Enum.GetNames(typeof(PrivacyLevel)));

                    var updateInquirer = new Inquirer(updateIdInput, newUsernameInput, newFirstNameInput, newLastNameInput, newEmailInput, newPasswordInput, newPrivacyLevelInput);
                    updateInquirer.Ask();

                    var updatedUser = new User
                    {
                        UserName = newUsernameInput.Answer(),
                        FirstName = newFirstNameInput.Answer(),
                        LastName = newLastNameInput.Answer(),
                        Email = newEmailInput.Answer(),
                        Password = newPasswordInput.Answer(),
                        PrivacyLevel = (PrivacyLevel)(int.Parse(newPrivacyLevelInput.Answer()) - 1)
                    };

                    try
                    {
                        _userService.UpdateDetails(updatedUser, int.Parse(updateIdInput.Answer()));
                        Console.WriteLine("User updated successfully!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error updating user: {ex.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    break;
                case "3":
                    var deleteIdInput = new Input("id", "Enter user ID to delete:");
                    var confirmDeleteInput = new InputConfirmation("confirmDelete", "Are you sure you want to delete this user?");
                    var deleteInquirer = new Inquirer(deleteIdInput, confirmDeleteInput);
                    deleteInquirer.Ask();
                    if (confirmDeleteInput.Answer() == "y")
                    {
                        if (int.TryParse(deleteIdInput.Answer(), out int userId))
                        {
                            try
                            {
                                _userService.DeleteUser(userId);
                                Console.WriteLine("User deleted successfully!");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();

                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine($"Error deleting user: {ex.Message}");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid user ID.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("User deletion canceled.");
                    }
                    break;

                case "4":
                    var getIdInput = new Input("id", "Enter user ID:");
                    new Inquirer(getIdInput).Ask();

                    if (int.TryParse(getIdInput.Answer(), out int getId))
                    {
                        try
                        {
                            var user = _userService.GetUserById(getId);
                            if (user == null)
                            {
                                Console.WriteLine("User not found.");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                                break;
                            }
                            Console.WriteLine($"ID: {user.Id}");
                            Console.WriteLine($"Username: {user.UserName}");
                            Console.WriteLine($"Name: {user.FirstName} {user.LastName}");
                            Console.WriteLine($"Email: {user.Email}");
                            Console.WriteLine($"Date of Birth: {user.DateOfBirth.ToShortDateString()}");
                            Console.WriteLine($"Created: {user.CreatedDate.ToShortDateString()}");
                            Console.WriteLine($"Languages: {string.Join(", ", user.Languages)}");
                            Console.WriteLine($"Privacy Level: {user.PrivacyLevel}");
                            Console.WriteLine($"Favourite Genres: {string.Join(", ", user.FavouriteGenres)}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                    break;

                case "5":
                    var genreIdInput = new Input("id", "Enter user ID:");
                    new Inquirer(genreIdInput).Ask();

                    if (int.TryParse(genreIdInput.Answer(), out int genreUserId))
                    {
                        var genres = new List<Genre>();
                        bool addingGenres = true;

                        while (addingGenres)
                        {
                            var genreOptions = Enum.GetNames(typeof(Genre)).Append("Done").ToArray();
                            var genreInput = new ListInput("genre", "Select a genre (select Done when finished):", genreOptions);
                            new Inquirer(genreInput).Ask();

                            if (genreInput.Answer() == genreOptions.Length.ToString())
                            {
                                addingGenres = false;
                            }
                            else
                            {
                                var selected = (Genre)(int.Parse(genreInput.Answer()) - 1);
                                genres.Add(selected);
                            }
                        }

                        try
                        {
                            _userService.UpdateFavouriteGenres(genres, genreUserId);
                            Console.WriteLine("Favourite genres updated successfully!");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                    break;

                case "6":
                    var langIdInput = new Input("id", "Enter user ID:");
                    var langInput = new Input("language", "Enter language to add:");
                    new Inquirer(langIdInput, langInput).Ask();

                    if (int.TryParse(langIdInput.Answer(), out int langId))
                    {
                        try
                        {
                            _userService.AddLanguage(langInput.Answer(), langId);
                            Console.WriteLine("Language added successfully!");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                    break;

                case "7":
                    var users = _userService.GetAllUsers();
                    if (users.Count == 0)
                    {
                        Console.WriteLine("No users found.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    }
                    foreach (var user in users)
                    {
                        Console.WriteLine($"ID: {user.Id}, Username: {user.UserName}, Email: {user.Email}");
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;

                case "8":
                    return;
            }
        }
    }
}
