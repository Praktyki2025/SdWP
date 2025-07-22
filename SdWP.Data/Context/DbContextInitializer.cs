using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SdWP.Data.Context
{
    public class DbContextInitializer
    {
        public static ApplicationDbContext Create()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
