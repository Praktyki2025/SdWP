using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SdWP.Data.Context
{
    public class DbContextInitializer
    {
        public static ApplicationDbContextcs Create()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<ApplicationDbContextcs>()
                .UseSqlServer(connectionString)
                .Options;

            return new ApplicationDbContextcs(options);
        }
    }
}
