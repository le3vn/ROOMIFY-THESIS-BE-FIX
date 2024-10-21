using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Roomify.Entities.MigrationsDesigner
{
    internal class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=Roomify;Username=postgres;Password=HelloWorld!");
            optionsBuilder.UseOpenIddict();

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
