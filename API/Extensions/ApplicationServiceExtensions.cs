using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>(m => m.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
