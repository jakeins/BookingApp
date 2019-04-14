using BookingApp.Repositories;
using BookingApp.Repositories.Interfaces;
using BookingApp.Services;
using BookingApp.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BookingApp.Helpers
{
    public static class InitializeServicesExtension
    {
        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IResourcesService, ResourcesService>();
            services.AddScoped<IResourcesRepository, ResourcesRepository>();

            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IFolderRepository, FolderRepository>();

            services.AddScoped<IBookingsService, BookingsService>();
            services.AddScoped<IBookingsRepository, BookingsRepository>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ISmtpService, GoogleSmtpService>();
            services.AddScoped<IMessageService, MailMessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRuleService, RuleService>();
            services.AddScoped<IRuleRepository, RuleRepository>();

            services.AddScoped<IStatisticsService, StatisticsService>();

            services.AddScoped<IMapperService, MapperService>();
        }
    }
}
