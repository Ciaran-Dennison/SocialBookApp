using InquirerCore;
using InquirerCore.Prompts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialBookAppApplication;
using SocialBookAppCLI.Menus;
using SocialBookAppInfrastructure;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddDbContext<SocialBookAppContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IRatingService, RatingService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IBookService, BookService>();
services.AddScoped<IAuthorService, AuthorService>();
services.AddScoped<IReviewService, ReviewService>();

var serviceProvider = services.BuildServiceProvider();
var userService = serviceProvider.GetRequiredService<IUserService>();
var bookService = serviceProvider.GetRequiredService<IBookService>();
var authorService = serviceProvider.GetRequiredService<IAuthorService>();
var reviewService = serviceProvider.GetRequiredService<IReviewService>();



bool running = true;

while (running)
{
    Console.Clear();
    var options = new string[] { "Manage Users", "Manage Books", "View Authors", "View Publishers", "Exit" };
    var mainMenu = new ListInput("menu", "What would you like to do?", options);
    new Inquirer(mainMenu).Ask();

    switch (mainMenu.Answer())
    {
        case "1":
            new UserMenu(userService).Show();
            break;
        case "2":
            new BookMenu(bookService).Show();
            break;
        case "3":
            break;
        case "4":
            break;
        case "5":
            Console.WriteLine("Exiting the application...");
            Thread.Sleep(500); // Wait for 5 seconds before exiting
            running = false;
            break;
    }
}