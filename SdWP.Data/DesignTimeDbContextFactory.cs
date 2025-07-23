using Microsoft.EntityFrameworkCore.Design;
using SdWP.Data.Context;

namespace SdWP.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            return DbContextInitializer.Create();
        }
    }
}
