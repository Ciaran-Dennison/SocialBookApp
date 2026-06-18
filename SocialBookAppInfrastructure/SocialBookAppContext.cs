
using Microsoft.EntityFrameworkCore;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class SocialBookAppContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Author> Authors { get; set; }

    public SocialBookAppContext(DbContextOptions<SocialBookAppContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Book>()
        .HasMany(b => b.Authors)
        .WithMany(a => a.Books);

    }
}
