using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialBookAppApplication;
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