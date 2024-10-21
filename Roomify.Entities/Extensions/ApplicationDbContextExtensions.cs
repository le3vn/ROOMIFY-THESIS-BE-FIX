using Microsoft.EntityFrameworkCore;
using Roomify.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationDbContextExtensions
    {
        public static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseOpenIddict();
            });
        }
    }
}
