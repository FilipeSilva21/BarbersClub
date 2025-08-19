// Arquivo: BarbersClub.Entity/DependencyInjection.cs

using BarbersClub.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.DbContext;

namespace BarbersClub.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProjectDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("BarbersClubDB")));
            
            return services;
        }
    }
}