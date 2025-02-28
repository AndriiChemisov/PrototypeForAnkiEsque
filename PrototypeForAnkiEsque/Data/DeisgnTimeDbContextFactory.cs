using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
// This file is used to define the database context factory for the application. The database context factory is used to create a new instance of the database context when running EF Core commands like migrations or updates.
// The DesignTimeDbContextFactory class implements the IDesignTimeDbContextFactory interface provided by EF Core. The CreateDbContext method is used to create a new instance of the ApplicationDbContext class.
// The CreateDbContext method builds the configuration from the appsettings.json file and gets the connection string from the configuration.
// The CreateDbContext method then creates a new instance of the DbContextOptionsBuilder and configures it with the connection string.
// Finally, the CreateDbContext method returns a new instance of the ApplicationDbContext class with the configured options.
// Simple explanation: The DesignTimeDbContextFactory class is used to create a new instance of the ApplicationDbContext class when running EF Core commands like migrations or updates.
namespace PrototypeForAnkiEsque.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration (appsettings.json)
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get connection string
            var connectionString = config.GetConnectionString("DefaultConnection");

            // Create a DbContextOptionsBuilder to configure the DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            // Return the DbContext instance with options
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
