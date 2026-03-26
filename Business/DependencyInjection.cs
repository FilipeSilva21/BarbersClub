using Business.Services;
using BarbersClub.Business.Services.Interfaces;
using Business.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBarberShopService, BarberShopService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IDashboardStatsService, DashboardStatsService>();
            services.AddScoped<IUserService, UserService>();
            
            return services;
        }
    }
}