

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SocialBookAppInfrastructure;

public class SocialBookAppContextFactory : IDesignTimeDbContextFactory<SocialBookAppContext>
{
    public SocialBookAppContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SocialBookAppContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=SocialBookApp;Trusted_Connection=True;TrustServerCertificate=True");

        return new SocialBookAppContext(optionsBuilder.Options);
    }
}
